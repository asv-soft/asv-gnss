using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class JavadBinaryMessageTest
{
    private static IProtocolMessageFactory<JavadMessageBase, string> CreateFactory()
    {
        var builder = new ProtocolMessageFactoryBuilder<JavadMessageBase, string>(
            JavadProtocol.Info
        );
        JavadBinaryMessageFactory.Register(builder);
        return builder.Build();
    }

    [Fact]
    public void BinaryFactory_RegistersAllNativeAndLegacyIdsAndRoundTripsDefaults()
    {
        var factory = CreateFactory();
        var ids = factory.GetSupportedIds().ToArray();
        Assert.Equal(271, ids.Length);
        Assert.Equal(271, ids.Distinct(StringComparer.Ordinal).Count());
        Assert.Contains("sj", ids);
        Assert.Contains("SI", ids);
        Assert.Contains("rM", ids);
        Assert.Contains("GD", ids);
        Assert.Contains("gd", ids);
        foreach (var id in ids)
        {
            foreach (var order in new[] { JavadByteOrder.LittleEndian, JavadByteOrder.BigEndian })
            {
                var original = factory.Create(id)!;
                original.ByteOrder = order;
                var bytes = Serialize(original);
                var decoded = factory.Create(id)!;
                decoded.ByteOrder = order;
                ReadOnlySpan<byte> input = bytes;
                decoded.Deserialize(ref input);
                Assert.True(input.IsEmpty, id);
                Assert.Equal(original.GetType(), decoded.GetType());
                Assert.Equal(bytes, Serialize(decoded));
            }
        }
    }

    [Fact]
    public void GeneratedBinaryModels_RoundTripPopulatedFieldsAndVariableArrays()
    {
        var factory = CreateFactory();
        var custom = new HashSet<string>(StringComparer.Ordinal)
        {
            "JP",
            "MF",
            "AI",
            "IQ",
            "CN",
            "sp",
            "sP",
            "Sp",
            "LH",
            "rM",
            "rT",
            "GD",
            "QD",
            "LD",
        };
        foreach (var id in factory.GetSupportedIds().Where(id => !custom.Contains(id)))
        {
            foreach (var order in new[] { JavadByteOrder.LittleEndian, JavadByteOrder.BigEndian })
            {
                var original = factory.Create(id)!;
                Populate(original);
                original.ByteOrder = order;
                var decoded = factory.Create(id)!;
                decoded.ByteOrder = order;
                Deserialize(decoded, Serialize(original));
                AssertFieldsEqual(original, decoded);
            }
        }
    }

    private static IEnumerable<PropertyInfo> PayloadProperties(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property =>
                property.CanWrite && property.Name is not ("ByteOrder" or "ExtensionData")
            );

    private static void Populate(object value)
    {
        foreach (var property in PayloadProperties(value.GetType()))
        {
            var type = property.PropertyType;
            if (Nullable.GetUnderlyingType(type) is not null)
                continue;
            if (type == typeof(string))
                property.SetValue(value, "K");
            else if (type == typeof(char))
                property.SetValue(value, 'k');
            else if (type == typeof(float))
                property.SetValue(value, -12.5f);
            else if (type == typeof(double))
                property.SetValue(value, 1234.125);
            else if (type.IsPrimitive)
                property.SetValue(value, Convert.ChangeType(7, type));
            else if (type.IsArray)
            {
                var elementType = type.GetElementType()!;
                var current = (Array)property.GetValue(value)!;
                var array = Array.CreateInstance(
                    elementType,
                    current.Length == 0 ? 2 : current.Length
                );
                for (var index = 0; index < array.Length; index++)
                {
                    var item = elementType.IsPrimitive
                        ? Convert.ChangeType(index + 11, elementType)
                        : Activator.CreateInstance(elementType)!;
                    if (!elementType.IsPrimitive)
                        Populate(item);
                    array.SetValue(item, index);
                }
                property.SetValue(value, array);
            }
            else if (property.GetValue(value) is { } nested)
                Populate(nested);
        }
    }

    private static void AssertFieldsEqual(object expected, object actual)
    {
        foreach (var property in PayloadProperties(expected.GetType()))
        {
            var left = property.GetValue(expected);
            var right = property.GetValue(actual);
            if (left is string text)
            {
                Assert.Equal(text.TrimEnd(' '), ((string)right!).TrimEnd(' '));
            }
            else if (left is Array array)
            {
                var other = Assert.IsAssignableFrom<Array>(right);
                Assert.Equal(array.Length, other.Length);
                for (var index = 0; index < array.Length; index++)
                {
                    if (array.GetType().GetElementType()!.IsPrimitive)
                        Assert.Equal(array.GetValue(index), other.GetValue(index));
                    else
                        AssertFieldsEqual(array.GetValue(index)!, other.GetValue(index)!);
                }
            }
            else if (left is null || property.PropertyType.IsValueType)
                Assert.Equal(left, right);
            else
            {
                Assert.IsType(left.GetType(), right);
                AssertFieldsEqual(left, right!);
            }
        }
    }

    [Fact]
    public void ReceiverTimeAndDate_DecodeIndependentWireValuesInBothByteOrders()
    {
        var time = Decode<JavadReceiverTimeMessage>("~~", Convert.FromHexString("78563412"));
        Assert.Equal(0x12345678u, time.TimeOfDay);
        Assert.Equal(Convert.FromHexString("78563412"), Payload(time));
        var date = Decode<JavadReceiverDateMessage>(
            "RD",
            Convert.FromHexString("07EA0A1602"),
            JavadByteOrder.BigEndian
        );
        Assert.Equal((ushort)2026, date.Year);
        Assert.Equal((byte)10, date.Month);
        Assert.Equal((byte)22, date.Day);
        Assert.Equal((byte)2, date.ReferenceTimeScale);
        Assert.Equal(Convert.FromHexString("07EA0A1602"), Payload(date));
    }

    [Fact]
    public void CartesianPosition_PreservesRawUnitsAndFutureExtensionBeforeChecksum()
    {
        var payload = Convert.FromHexString(
            "000000000000F03F00000000000000C000000000000008400000003F04A5B6"
        );
        var message = Decode<JavadCartesianPositionMessage>("PO", payload);
        Assert.Equal(1, message.X);
        Assert.Equal(-2, message.Y);
        Assert.Equal(3, message.Z);
        Assert.Equal(0.5f, message.PositionSigma);
        Assert.Equal((byte)4, message.SolutionType);
        Assert.Equal(new byte[] { 0xA5, 0xB6 }, message.ExtensionData);
        Assert.Equal(payload, Payload(message));
    }

    // Complete, unmodified frames from RTKLIB test/data/rcvraw/javad_20110115.jps.
    // File offsets: DP=3380, NU=3513, WE=3873, EN=4007. Lengths include checksum.
    public static TheoryData<string, string, string, int> HistoricalFixedFrames =>
        new()
        {
            {
                "DP",
                "44503030457B44213F3FC9803F76AF083F011A",
                nameof(JavadDilutionOfPrecisionMessage.Edop),
                4
            },
            {
                "NU",
                "4E5530313900000000006089BE0000CCB4000015BE000000B900045704DA",
                nameof(JavadGlonassUtcGpsParametersMessage.Nt),
                2
            },
            {
                "WE",
                "57453034378181A406402100007B14AE277ED57EC1295C8FA27FCB794134333333B3F0D84014AE87BF999997BFEE7C5FC051491DB817B751386F1203B900004C33000030ADD10A08005202C7",
                nameof(JavadSbasEphemerisMessage.Flags),
                2
            },
            {
                "EN",
                "454E30393101D0090800145C01809706000018520200005CB30000000000D0CB2F5C89D33A809706005C01000060DF3C4BB54000000040EBE64A3F0000000E37B2E33F000000385343C83F00000082C3F5D33F000000BA1563B73F00288730008CEBB00000042D00304C430000A6C1008085B50090E1360000C032000040B20000000000000000000019420000DEBE0000733C000247",
                nameof(JavadGalileoEphemerisMessage.DAf0),
                4
            },
        };

    [Theory]
    [MemberData(nameof(HistoricalFixedFrames))]
    public void HistoricalFixedFrames_PreserveWireBytesAndResetTrailingFieldsWhenReused(
        string id,
        string frameHex,
        string trailingField,
        int fieldSize
    )
    {
        var historical = Convert.FromHexString(frameHex);
        var message = CreateFactory().Create(id)!;
        var valueProperty = message.GetType().GetProperty(trailingField)!;
        var presenceProperty = message.GetType().GetProperty("Has" + trailingField)!;
        var presentValue = Convert.ChangeType(7, valueProperty.PropertyType);
        Assert.True((bool)presenceProperty.GetValue(message)!);
        Assert.Equal(historical.Length + fieldSize, message.GetByteSize());
        valueProperty.SetValue(message, presentValue);
        var modern = Serialize(message);
        var decoded = CreateFactory().Create(id)!;
        Deserialize(decoded, modern);
        Assert.True((bool)presenceProperty.GetValue(decoded)!);
        Assert.Equal(presentValue, valueProperty.GetValue(decoded));

        Deserialize(decoded, historical);

        Assert.False((bool)presenceProperty.GetValue(decoded)!);
        Assert.Equal(Activator.CreateInstance(valueProperty.PropertyType), valueProperty.GetValue(decoded));
        Assert.Empty(decoded.ExtensionData);
        Assert.Equal(historical, Serialize(decoded));
        switch (decoded)
        {
            case JavadDilutionOfPrecisionMessage dops:
                Assert.Equal((byte)1, dops.SolutionType);
                break;
            case JavadGlonassUtcGpsParametersMessage utc:
                Assert.Equal((short)1111, utc.Dn);
                break;
            case JavadSbasEphemerisMessage sbas:
                Assert.Equal((byte)129, sbas.WaasPrn);
                Assert.Equal((ushort)594, sbas.Wn);
                break;
            case JavadGalileoEphemerisMessage galileo:
                Assert.Equal((byte)1, galileo.RequiredData.Sv);
                Assert.Equal((byte)2, galileo.NavType);
                break;
        }

        Deserialize(decoded, modern);

        Assert.True((bool)presenceProperty.GetValue(decoded)!);
        Assert.Equal(presentValue, valueProperty.GetValue(decoded));
        Assert.Equal(modern, Serialize(decoded));
        decoded.ExtensionData = [0xA1, 0xB2, 0xC3];
        var future = Serialize(decoded);
        Deserialize(message, future);
        Assert.Equal(decoded.ExtensionData, message.ExtensionData);
        Assert.Equal(future, Serialize(message));
    }

    [Theory]
    [MemberData(nameof(HistoricalFixedFrames))]
    public void HistoricalFixedFrames_RejectTruncatedRequiredOrPartiallyPresentTrailingFields(
        string id,
        string frameHex,
        string trailingField,
        int fieldSize
    )
    {
        var historical = Convert.FromHexString(frameHex);
        var payload = historical[5..^1];
        var message = CreateFactory().Create(id)!;
        var truncated = Frame(id, payload[..^1], JavadChecksumKind.Binary8);
        Assert.Throws<FormatException>(() => Deserialize(message, truncated));
        for (var length = 1; length < fieldSize; length++)
        {
            var partial = Frame(id, payload.Concat(new byte[length]).ToArray(), JavadChecksumKind.Binary8);
            Assert.Throws<FormatException>(() => Deserialize(message, partial));
        }

        Deserialize(message, historical);
        Assert.False((bool)message.GetType().GetProperty("Has" + trailingField)!.GetValue(message)!);
        message.ExtensionData = [0xA1];
        Assert.Throws<InvalidOperationException>(() => message.GetByteSize());
    }

    [Fact]
    public void SatelliteArrays_DecodeIndexPairsSignedMeasurementsAndMissingSentinels()
    {
        var indices = Decode<JavadExtendedSatelliteIndicesMessage>(
            "SX",
            Convert.FromHexString("010302F90806")
        );
        Assert.Equal(3, indices.Satellites.Length);
        Assert.Equal((byte)2, indices.Satellites[1].SystemId);
        Assert.Equal((byte)249, indices.Satellites[1].SatelliteId);
        var integers = Decode<JavadIntegerPseudoRangesr1Message>(
            "r1",
            Convert.FromHexString("FFFFFF7F85FFFFFF")
        );
        Assert.Equal(new[] { int.MaxValue, -123 }, integers.Spr);
        var ranges = Decode<JavadPseudoRangesRCMessage>(
            "RC",
            Convert.FromHexString("000000000000F83F000000000000F87F")
        );
        Assert.Equal(1.5, ranges.Pr[0]);
        Assert.True(double.IsNaN(ranges.Pr[1]));
        Assert.Equal(Convert.FromHexString("000000000000F83F000000000000F87F"), Payload(ranges));
    }

    [Theory]
    [InlineData("x0")]
    [InlineData("y0")]
    public void Scintillation_UsesFloatArrayDespitePrintedSizeErratum(string id)
    {
        var model = CreateFactory().Create(id)!;
        var bytes = Frame(id, Convert.FromHexString("0000003F0000803F"), model.ChecksumKind);
        ReadOnlySpan<byte> span = bytes;
        model.Deserialize(ref span);
        var values = model switch
        {
            JavadScintillationS4Payload s4 => s4.S4,
            JavadScintillationSigmaPhiPayload phi => phi.SigmaPhi,
            _ => throw new InvalidOperationException(),
        };
        Assert.Equal(new[] { 0.5f, 1f }, values);
        Assert.Equal(14, model.GetByteSize());
        Assert.Equal(bytes, Serialize(model));
    }

    [Theory]
    [InlineData(0, 123)]
    [InlineData(2, 168)]
    [InlineData(8, 160)]
    public void GpsEphemeris_DecodesOptionalSignalSelectedCorrectionLayouts(
        byte signal,
        int bodySize
    )
    {
        var payload = Build(writer =>
        {
            var required = new byte[122];
            required[0] = 21;
            writer.Write(required);
            if (signal == 0)
                return;
            writer.Write(signal);
            writer.Write(1234);
            writer.Write(-2345);
            writer.Write(0.25);
            writer.Write(0.5f);
            writer.Write(new byte[] { 1, 2, 3, 4 });
            writer.Write(1.5f);
            writer.Write(2.5f);
            if (signal == 2)
            {
                writer.Write(3.5f);
                writer.Write(4.5f);
            }
            writer.Write(5.5f);
        });
        var message = Decode<JavadGpsEphemerisMessage>("GE", payload);
        Assert.Equal((byte)21, message.RequiredData.Sv);
        Assert.Equal(bodySize + 5, message.GetByteSize());
        if (signal == 0)
        {
            Assert.Null(message.OptionalData);
        }
        else
        {
            var optional = Assert.IsType<JavadGpsEphemerisOptionalData>(message.OptionalData);
            Assert.Equal(1234, optional.LTope);
            Assert.Equal(-2345, optional.LTopc);
            Assert.Equal(5.5f, optional.DAf0);
            if (signal == 2)
            {
                Assert.Equal(4.5f, optional.Cnav!.FIscL5Q5);
                Assert.Null(optional.Cnav2);
            }
            else
            {
                Assert.Equal(2.5f, optional.Cnav2!.FIscL1CD);
                Assert.Null(optional.Cnav);
            }
        }
        Assert.Equal(payload, Payload(message));
    }

    [Theory]
    [InlineData(1, 134)]
    [InlineData(8, 152)]
    [InlineData(64, 160)]
    public void BeiDouEphemeris_DecodesLegacyModernAndB2bVariants(byte signal, int bodySize)
    {
        var payload = Build(writer =>
        {
            writer.Write(new byte[122]);
            writer.Write(1.5f);
            writer.Write(signal);
            writer.Write(2.5f);
            if (signal == 1)
            {
                writer.Write((byte)17);
                writer.Write((byte)19);
            }
            else
            {
                writer.Write(3.5);
                writer.Write(4.5f);
                writer.Write(5.5f);
                writer.Write(6.5f);
                if (signal == 64)
                {
                    writer.Write(7.5f);
                    writer.Write(8.5f);
                }
            }
        });
        var message = Decode<JavadBeiDouEphemerisMessage>("CN", payload);
        Assert.Equal(bodySize + 5, message.GetByteSize());
        Assert.Equal(1.5f, message.Tgd2);
        if (signal == 1)
        {
            Assert.Equal((byte)19, message.LegacyData!.AODE);
            Assert.Null(message.ModernData);
        }
        else
        {
            Assert.Equal(3.5, message.ModernData!.ADot);
            Assert.Equal(signal == 64 ? 8.5f : (float?)null, message.ModernData.Tgd_B2Bpilot);
            Assert.Null(message.LegacyData);
        }
        Assert.Equal(payload, Payload(message));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IqSamples_DecodesInterleavingSelectedByIsQ(bool quadrature)
    {
        var payload = quadrature
            ? Convert.FromHexString("010304010100FEFF0300FCFF")
            : Convert.FromHexString("0103040001000300");
        var message = Decode<JavadIqSamplesMessage>("IQ", payload);
        Assert.Equal((byte)3, message.Satellite.SatelliteId);
        Assert.Equal(2, message.Samples.Length);
        Assert.Equal((short)1, message.Samples[0].I);
        Assert.Equal((short)3, message.Samples[1].I);
        Assert.Equal(quadrature ? (short)-4 : (short?)null, message.Samples[1].Q);
        Assert.Equal(payload, Payload(message));
    }

    [Fact]
    public void Spectrum_DecodesBlocksAndAgcArraysInWireOrder()
    {
        var basic = Decode<JavadSpectrumMessage>(
            "sp",
            Convert.FromHexString("FEFF640002020100020003000400")
        );
        Assert.Equal((short)-2, basic.CurrFrq);
        Assert.Equal(new short[] { 1, 2 }, basic.Blocks[0].Values);
        Assert.Equal(new short[] { 3, 4 }, basic.Blocks[1].Values);
        var bytes = Convert.FromHexString("FEFF64000202010002000A0B1415030004000C0D1617");
        var extended = Decode<JavadExtendedSpectrumMessage>("sP", bytes);
        Assert.Equal(new short[] { 3, 4 }, extended.Blocks[1].Values);
        Assert.Equal(new byte[] { 12, 13 }, extended.Blocks[1].AgcMinimum);
        Assert.Equal(new byte[] { 22, 23 }, extended.Blocks[1].AgcMaximum);
        Assert.Equal(bytes, Payload(extended));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SingleSpectrum_PreservesOptionalMetadataAndUsesBothDimensions(bool metadata)
    {
        var payload = Build(writer =>
        {
            writer.Write(4);
            writer.Write((byte)2);
            writer.Write((byte)2);
            writer.Write((ushort)2);
            writer.Write(
                new short[] { 11, 12, 13, 14 }
                    .SelectMany(BitConverter.GetBytes)
                    .ToArray()
            );
            if (!metadata)
                return;
            writer.Write(1575.42f);
            writer.Write(0.5f);
            writer.Write(32);
            writer.Write(Encoding.ASCII.GetBytes("GPS L1\0\0"));
            writer.Write(123456);
            writer.Write(Encoding.ASCII.GetBytes("LNA     "));
            writer.Write((byte)'a');
        });
        var message = Decode<JavadSingleSpectrumMessage>("Sp", payload);
        Assert.Equal(new short[] { 11, 12, 13, 14 }, message.Values);
        Assert.Equal(metadata ? 33 : 0, message.GetByteSize() - 22);
        if (metadata)
        {
            Assert.Equal(32, message.SpectrumInfo!.Points);
            Assert.Equal('a', message.SpectrumInfo.Antenna);
        }
        else
            Assert.Null(message.SpectrumInfo);
        Assert.Equal(payload, Payload(message));
    }

    [Fact]
    public void LoggingHistory_DecodesPaddingAndPerSatelliteHistoryWithoutChecksum()
    {
        var payload = Convert.FromHexString("016102002100E80300000700000078563412EFCDAB90");
        var message = Decode<JavadLoggingHistoryMessage>("LH", payload);
        Assert.Equal((ushort)33, message.BitsCount);
        Assert.Equal(new byte[] { 7 }, message.Uids);
        Assert.Equal(new byte[] { 0, 0, 0 }, message.Pad);
        Assert.Equal(new[] { 0x12345678u, 0x90ABCDEFu }, message.Hist);
        Assert.Equal(JavadChecksumKind.None, message.ChecksumKind);
        Assert.Equal(payload, Payload(message));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void LegacyMeasurements_DecodesVersionedSlotsAndPreservesRecordExtensions(int version)
    {
        var knownSize =
            version == 0 ? 14
            : version == 1 ? 10
            : 6;
        var slotSize = knownSize + 1;
        var flags = (ushort)(
            (version << 13) | (1 << 8) | (1 << 5) | (slotSize - (version < 2 ? 10 : 6))
        );
        var payload = Build(writer =>
        {
            writer.Write((ushort)9);
            writer.Write((ushort)0x1234);
            writer.Write(123456u);
            writer.Write((short)-8);
            writer.Write(flags);
            writer.Write(456789u);
            writer.Write((byte)3);
            writer.Write((byte)1);
            writer.Write((byte)0xA1);
            for (var i = 0; i < 2; i++)
            {
                writer.Write((short)(-10 - i));
                writer.Write((uint)(0x10203040 + i));
                if (version < 2)
                {
                    writer.Write((ushort)0xABCD);
                    writer.Write((ushort)0x1234);
                }
                if (version == 0)
                    writer.Write(0x55667788u);
                writer.Write((byte)0xB2);
            }
        });
        var message = Decode<JavadLegacyRawMeasurementsMessage>("rM", payload);
        Assert.Equal(version, message.Version);
        Assert.Equal(7, message.HeaderSize);
        Assert.Equal(slotSize, message.SlotSize);
        var satellite = Assert.Single(message.Satellites);
        Assert.Equal(new byte[] { 0xA1 }, satellite.HeaderExtension);
        Assert.Equal(2, satellite.Slots.Length);
        Assert.Equal((short)-11, satellite.Slots[1].StatusOrDeltaRange);
        Assert.Equal(version < 2 ? (ushort)0xABCD : (ushort?)null, satellite.Slots[1].Flags);
        Assert.Equal(version == 0 ? 0x55667788u : (uint?)null, satellite.Slots[1].Word2);
        Assert.Equal(new byte[] { 0xB2 }, satellite.Slots[1].ExtensionData);
        Assert.Equal(payload, Payload(message));
    }

    [Fact]
    public void LegacyNavigation_DecodesExplicitRecordSizeAndPreservesUnknownRecordTail()
    {
        var payload = Build(writer =>
        {
            writer.Write((byte)44);
            writer.Write((sbyte)17);
            writer.Write((byte)2);
            for (var i = 0; i < 10; i++)
                writer.Write((uint)(0x12345600 + i));
            writer.Write((byte)0xA1);
            writer.Write((byte)0xB2);
        });
        var message = Decode<JavadLegacyGpsNavigationGDMessage>("GD", payload);
        var satellite = Assert.Single(message.Satellites);
        Assert.Equal((sbyte)17, satellite.Prn);
        Assert.Equal(0x12345609u, satellite.Data[9]);
        Assert.Equal(new byte[] { 0xA1, 0xB2 }, satellite.ExtensionData);
        Assert.Equal(payload, Payload(message));
    }

    [Theory]
    [InlineData("SX", "010201")]
    [InlineData("RC", "01020304050607")]
    [InlineData("IQ", "010200020100")]
    [InlineData("gd", "010000000000FF00")]
    [InlineData("GD", "00")]
    [InlineData("rT", "0000000000")]
    [InlineData("AI", "0100000000000000000000")]
    public void BinaryPayload_RejectsTruncatedArraysInvalidCountsAndInvalidSelectors(
        string id,
        string hex
    )
    {
        var model = CreateFactory().Create(id)!;
        var frame = Frame(id, Convert.FromHexString(hex), model.ChecksumKind);
        Assert.Throws<FormatException>(() => Deserialize(model, frame));
    }

    [Theory]
    [InlineData("XX01000")]
    [InlineData("JP0X000")]
    [InlineData("JP01X00")]
    [InlineData("JP01002")]
    public void MessagesFormat_RejectsInvalidIdentifierVersionOrByteOrder(string payload)
    {
        var frame = Frame("MF", Encoding.ASCII.GetBytes(payload), JavadChecksumKind.AsciiHex8);
        Assert.Throws<FormatException>(() => Deserialize(new JavadMessagesFormatMessage(), frame));
    }

    [Fact]
    public void PayloadSerialization_RejectsArrayDimensionsAndInconsistentUnionFields()
    {
        Assert.Throws<ArgumentException>(() =>
            new JavadAttitudeBaselinesMessage { Bl0 = [1, 2] }.GetByteSize()
        );
        Assert.Throws<InvalidOperationException>(() =>
            new JavadIqSamplesMessage { IsQ = 1, Samples = [new() { I = 1 }] }.GetByteSize()
        );
        Assert.Throws<InvalidOperationException>(() =>
            new JavadBeiDouEphemerisMessage { NavType = 8 }.GetByteSize()
        );
        Assert.Throws<ArgumentException>(() =>
            new JavadSpectrumMessage { SpectrumCount = 2, BlockCount = 1 }.GetByteSize()
        );
        Assert.Throws<InvalidOperationException>(() =>
            new JavadGpsEphemerisMessage
            {
                OptionalData = new() { NavType = 8, Cnav = new() },
            }.GetByteSize()
        );
    }

    private static T Decode<T>(
        string id,
        byte[] payload,
        JavadByteOrder order = JavadByteOrder.LittleEndian
    )
        where T : JavadMessageBase, new()
    {
        var message = new T { ByteOrder = order };
        var bytes = Frame(id, payload, message.ChecksumKind, order);
        Deserialize(message, bytes);
        return message;
    }

    private static void Deserialize(JavadMessageBase message, byte[] frame)
    {
        ReadOnlySpan<byte> input = frame;
        message.Deserialize(ref input);
        Assert.True(input.IsEmpty);
    }

    private static byte[] Payload(JavadMessageBase message)
    {
        var frame = Serialize(message);
        var checksum = message.ChecksumKind switch
        {
            JavadChecksumKind.None => 0,
            JavadChecksumKind.Binary8 => 1,
            _ => 2,
        };
        return frame[5..(frame.Length - checksum)];
    }

    private static byte[] Serialize(JavadMessageBase message)
    {
        var bytes = new byte[message.GetByteSize()];
        Span<byte> destination = bytes;
        message.Serialize(ref destination);
        Assert.True(destination.IsEmpty);
        return bytes;
    }

    private static byte[] Build(Action<BinaryWriter> write)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.ASCII, true);
        write(writer);
        return stream.ToArray();
    }

    private static byte[] Frame(
        string id,
        byte[] payload,
        JavadChecksumKind kind,
        JavadByteOrder order = JavadByteOrder.LittleEndian
    )
    {
        var checksumSize = kind switch
        {
            JavadChecksumKind.None => 0,
            JavadChecksumKind.Binary8 => 1,
            _ => 2,
        };
        var bytes = Encoding
            .ASCII.GetBytes(id + (payload.Length + checksumSize).ToString("X3"))
            .Concat(payload)
            .Concat(new byte[checksumSize])
            .ToArray();
        if (kind == JavadChecksumKind.None)
            return bytes;
        var data = bytes.AsSpan(0, bytes.Length - checksumSize);
        if (kind == JavadChecksumKind.Crc16)
        {
            ushort value = 0;
            foreach (var item in data)
            {
                value ^= (ushort)(item << 8);
                for (var bit = 0; bit < 8; bit++)
                    value = (ushort)((value & 0x8000) != 0 ? (value << 1) ^ 0x1021 : value << 1);
            }
            bytes[^2] = order == JavadByteOrder.LittleEndian ? (byte)value : (byte)(value >> 8);
            bytes[^1] = order == JavadByteOrder.LittleEndian ? (byte)(value >> 8) : (byte)value;
        }
        else
        {
            byte value = 0;
            foreach (var item in data)
                value = (byte)(((value << 2) | (value >> 6)) ^ item);
            value = (byte)((value << 2) | (value >> 6));
            if (kind == JavadChecksumKind.Binary8)
                bytes[^1] = value;
            else
                Encoding.ASCII.GetBytes(value.ToString("X2"), bytes.AsSpan(bytes.Length - 2));
        }
        return bytes;
    }
}
