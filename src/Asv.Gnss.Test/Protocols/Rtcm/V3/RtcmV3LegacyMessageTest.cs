using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Asv.Gnss;

public class RtcmV3LegacyMessageTest
{
    private static readonly IReadOnlyDictionary<ushort, Func<RtcmV3MessageBase>> MessageFactory =
        new Dictionary<ushort, Func<RtcmV3MessageBase>>
        {
            [1004] = () => new RtcmV3Message1004(),
            [1005] = () => new RtcmV3Message1005(),
            [1006] = () => new RtcmV3Message1006(),
            [1007] = () => new RtcmV3Message1007(),
            [1008] = () => new RtcmV3Message1008(),
            [1012] = () => new RtcmV3Message1012(),
            [1013] = () => new RtcmV3Message1013(),
            [1019] = () => new RtcmV3Message1019(),
            [1020] = () => new RtcmV3Message1020(),
            [1029] = () => new RtcmV3Message1029(),
            [1030] = () => new RtcmV3Message1030(),
            [1031] = () => new RtcmV3Message1031(),
            [1032] = () => new RtcmV3Message1032(),
            [1033] = () => new RtcmV3Message1033(),
            [1042] = () => new RtcmV3Message1042(),
            [1046] = () => new RtcmV3Message1046(),
            [1073] = () => new RtcmV3Msm3Msg1073(),
            [1074] = () => new RtcmV3Msm4Msg1074(),
            [1075] = () => new RtcmV3Msm5Msg1075(),
            [1077] = () => new RtcmV3Msm7Msg1077(),
            [1083] = () => new RtcmV3Msm3Msg1083(),
            [1084] = () => new RtcmV3Msm4Msg1084(),
            [1085] = () => new RtcmV3Msm5Msg1085(),
            [1087] = () => new RtcmV3Msm7Msg1087(),
            [1093] = () => new RtcmV3Msm3Msg1093(),
            [1094] = () => new RtcmV3Msm4Msg1094(),
            [1095] = () => new RtcmV3Msm5Msg1095(),
            [1097] = () => new RtcmV3Msm7Msg1097(),
            [1104] = () => new RtcmV3Msm4Msg1104(),
            [1107] = () => new RtcmV3Msm7Msg1107(),
            [1114] = () => new RtcmV3Msm4Msg1114(),
            [1117] = () => new RtcmV3Msm7Msg1117(),
            [1123] = () => new RtcmV3Msm3Msg1123(),
            [1124] = () => new RtcmV3Msm4Msg1124(),
            [1125] = () => new RtcmV3Msm5Msg1125(),
            [1127] = () => new RtcmV3Msm7Msg1127(),
            [1230] = () => new RtcmV3Message1230(),
            [4094] = () => new RtcmV3Msg4094(),
        };

    [Theory]
    [MemberData(nameof(LegacyResourceData))]
    public void Serialize_RecordedFrames_PreservesPayload(string resourceName, IReadOnlyDictionary<ushort, int> expectedCounts)
    {
        var data = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", resourceName));
        var checkedCount = 0;
        foreach (var frame in EnumerateRtcm3Frames(data))
        {
            var message = DeserializeFrame(frame);
            var serialized = new byte[message.GetByteSize()];
            Array.Fill(serialized, (byte)0xFF);
            Span<byte> output = serialized;
            message.Serialize(ref output);
            Assert.Empty(output.ToArray());
            Assert.True(frame.SequenceEqual(serialized), $"Message {message.Id}, frame {checkedCount}: expected {Convert.ToHexString(frame)}, actual {Convert.ToHexString(serialized)}");
            DeserializeFrame(serialized);
            checkedCount++;
        }
        Assert.Equal(expectedCounts.Values.Sum(), checkedCount);
    }

    [Theory]
    [InlineData(1073, Msm1073Frame)]
    [InlineData(1083, Msm1083Frame)]
    [InlineData(1093, Msm1093Frame)]
    [InlineData(1123, Msm1123Frame)]
    public void Serialize_Msm3Frame_PreservesPayload(ushort expectedId, string hexFrame)
    {
        var frame = Hex(hexFrame);
        var message = DeserializeFrame(frame);
        var serialized = new byte[message.GetByteSize()];
        Span<byte> output = serialized;
        message.Serialize(ref output);
        Assert.Equal(expectedId, message.Id);
        Assert.Equal(frame, serialized);
    }

    [Fact]
    public void MessageFactory_ShouldContainV163Rtcm3Messages()
    {
        var expected = new ushort[]
        {
            1004, 1005, 1006, 1007, 1008, 1012, 1013, 1019, 1020, 1029,
            1030, 1031, 1032, 1033, 1042, 1046, 1073, 1074, 1075, 1077,
            1083, 1084, 1085, 1087, 1093, 1094, 1095, 1097, 1104, 1107,
            1114, 1117, 1123, 1124, 1125, 1127, 1230, 4094
        };

        Assert.Empty(expected.Except(MessageFactory.Keys));
    }

    [Theory]
    [MemberData(nameof(LegacyResourceData))]
    public void LegacyV163Resources_ShouldDeserializeKnownRtcm3Messages(string resourceName, IReadOnlyDictionary<ushort, int> expectedCounts)
    {
        var actualCounts = DeserializeResource(resourceName);

        Assert.Equal(expectedCounts.Sum(x => x.Value), actualCounts.Sum(x => x.Value));
        Assert.Empty(expectedCounts.Except(actualCounts));
        Assert.Empty(actualCounts.Except(expectedCounts));
    }

    [Theory]
    [InlineData(1073, Msm1073Frame)]
    [InlineData(1083, Msm1083Frame)]
    [InlineData(1093, Msm1093Frame)]
    [InlineData(1123, Msm1123Frame)]
    public void LegacyV163Msm3Frames_ShouldDeserialize(ushort expectedId, string hexFrame)
    {
        var message = DeserializeFrame(Hex(hexFrame));

        Assert.Equal(expectedId, message.Id);
    }

    [Fact]
    public void LegacyV163Msm1073Frame_ShouldPreserveDecodedProperties()
    {
        var message = Assert.IsType<RtcmV3Msm3Msg1073>(DeserializeFrame(Hex(Msm1073Frame)));

        Assert.Equal((uint)144187000, message.EpochTimeTow);
        Assert.Equal(1, message.MultipleMessageBit);
        Assert.Equal(8, message.Satellites.Length);
        Assert.Equal(27, message.Satellites.Sum(s => s.Signals.Length));
        Assert.Equal(5, message.Satellites[0].SatellitePrn);
        Assert.Equal("L1C", message.Satellites[0].Signals[0].RinexCode);
    }

    public static IEnumerable<object[]> LegacyResourceData()
    {
        yield return
        [
            "testglo.rtcm3",
            new Dictionary<ushort, int>
            {
                [1004] = 186,
                [1005] = 19,
                [1012] = 186,
                [1019] = 19,
                [1020] = 19,
            }
        ];
        yield return
        [
            "fw206mrtk_rtcm",
            new Dictionary<ushort, int>
            {
                [1006] = 69,
                [1008] = 69,
                [1075] = 1400,
                [1085] = 700,
                [1095] = 700,
                [1125] = 2058,
            }
        ];
        yield return
        [
            "imu_rtcm",
            new Dictionary<ushort, int>
            {
                [1006] = 13,
                [1008] = 13,
                [1019] = 9,
                [1020] = 19,
                [1033] = 13,
                [1042] = 18,
                [1046] = 6,
                [1074] = 134,
                [1084] = 134,
                [1094] = 134,
                [1124] = 268,
            }
        ];
        yield return
        [
            "GMSD7_20121014.rtcm3",
            new Dictionary<ushort, int>
            {
                [1007] = 28,
                [1008] = 28,
                [1019] = 15,
                [1020] = 16,
                [1033] = 28,
                [1077] = 257,
                [1087] = 257,
                [1117] = 257,
                [1127] = 257,
            }
        ];
    }

    [Theory]
    [InlineData(1030, 20)]
    [InlineData(1031, 17)]
    public void Serialize_NetworkResiduals_WritesEverySatellite(ushort id, int epochBits)
    {
        var message = (RtcmV3Message1030and1031)MessageFactory[id]();
        message.ResidualsEpoch = 43210;
        message.ReferenceStationID = 123;
        message.NRefs = 3;
        message.SatelliteResiduals =
        [
            new() { SatelliteId = 4, SOc = 1.5, SOd = 0.04, SOh = 0.5, SIc = 3, SId = 0.07 },
            new() { SatelliteId = 17, SOc = 127.5, SOd = 5.11, SOh = 6.3, SIc = 511.5, SId = 10.23 },
        ];

        var frame = SerializeFrame(message);
        Assert.Equal(6 + (12 + epochBits + 24 + 98 + 7) / 8, frame.Length);
        var bit = 36 + epochBits + 12 + 7;
        Assert.Equal(2u, Asv.IO.SpanBitHelper.GetBitU(frame, ref bit, 5));
        var decoded = (RtcmV3Message1030and1031)DeserializeFrame(frame);
        Assert.Equal(2, decoded.NumberSatelliteSignals);
        Assert.Equal(4, decoded.SatelliteId);
        Assert.Equal(17, decoded.SatelliteResiduals[1].SatelliteId);
        Assert.Equal(10.23, decoded.SatelliteResiduals[1].SId, 8);
        Assert.Equal(frame, SerializeFrame(decoded));
    }

    [Fact]
    public void Serialize_SystemParameters_PreservesTimeAndAnnouncements()
    {
        var message = new RtcmV3Message1013
        {
            ReferenceStationID = 37, ModifiedJulianDay = 61000, SecondsOfDay = 86399, LeapSeconds = 18,
            SystemMessages = [new() { Id = 1004, SyncFlag = 1, TransmissionInterval = 1.5 }, new() { Id = 1019, TransmissionInterval = 60 }],
        };
        var frame = SerializeFrame(message);
        var decoded = Assert.IsType<RtcmV3Message1013>(DeserializeFrame(frame));
        Assert.Equal(86399u, decoded.SecondsOfDay);
        Assert.Equal(18, decoded.LeapSeconds);
        Assert.Equal(61000, decoded.ModifiedJulianDay);
        Assert.Equal(2, decoded.SystemMessages.Length);
        Assert.Equal(1.5, decoded.SystemMessages[0].TransmissionInterval);
        Assert.Equal(1019, decoded.SystemMessages[1].Id);
        Assert.Equal(frame, SerializeFrame(decoded));
    }

    [Fact]
    public void Serialize_UnicodeText_CountsCodePointsAndUtf8Bytes()
    {
        var message = new RtcmV3Message1029
        {
            ReferenceStationID = 42, ModifiedJulianDay = 61000, SecondsOfDay = 45678, Message = "AЯ😀",
        };
        var frame = SerializeFrame(message);
        var bit = 36 + 12 + 16 + 17;
        Assert.Equal(3u, Asv.IO.SpanBitHelper.GetBitU(frame, ref bit, 7));
        Assert.Equal(7u, Asv.IO.SpanBitHelper.GetBitU(frame, ref bit, 8));
        var decoded = Assert.IsType<RtcmV3Message1029>(DeserializeFrame(frame));
        Assert.Equal(message.Message, decoded.Message);
        Assert.Equal(message.SecondsOfDay, decoded.SecondsOfDay);
        Assert.Equal(frame, SerializeFrame(decoded));
    }

    [Theory]
    [InlineData("A", 128)]
    [InlineData("😀", 64)]
    public void Serialize_UnicodeText_RejectsFieldOverflow(string character, int count)
    {
        var message = new RtcmV3Message1029 { Message = string.Concat(Enumerable.Repeat(character, count)) };
        Assert.Throws<ArgumentOutOfRangeException>(() => SerializeFrame(message));
    }

    [Fact]
    public void Serialize_ReceiverDescription_DerivesLengthsFromStrings()
    {
        var message = new RtcmV3Message1033
        {
            ReferenceStationID = 55, AntennaDescriptor = "ANT", AntennaSetupID = 9,
            AntennaSerialNumber = "1234", ReceiverTypeDescriptor = "RX", ReceiverFirmwareVersion = "1.2", ReceiverSerialNumber = "56789",
        };
        var frame = SerializeFrame(message);
        var decoded = Assert.IsType<RtcmV3Message1033>(DeserializeFrame(frame));
        Assert.Equal("ANT", decoded.AntennaDescriptor);
        Assert.Equal(3u, decoded.DescriptorCounterN);
        Assert.Equal(4u, decoded.SerialNumberCounterM);
        Assert.Equal(2u, decoded.ReceiverTypeDescriptorCounterI);
        Assert.Equal(3u, decoded.ReceiverFirmwareVersionCounterJ);
        Assert.Equal("56789", decoded.ReceiverSerialNumber);
        Assert.Equal(frame, SerializeFrame(decoded));
    }

    [Fact]
    public void Serialize_PhysicalStation_PreservesSigned38BitCoordinates()
    {
        var message = new RtcmV3Message1032
        {
            NonPhysicalReferenceStationID = 4095, PhysicalReferenceStationID = 7, ITRFEpochYear = 20,
            X = -1234567.8912, Y = 2345678.9123, Z = -3456789.1234,
        };
        var frame = SerializeFrame(message);
        Assert.Equal(26, frame.Length);
        var decoded = Assert.IsType<RtcmV3Message1032>(DeserializeFrame(frame));
        Assert.Equal(message.X, decoded.X, 4);
        Assert.Equal(message.Y, decoded.Y, 4);
        Assert.Equal(message.Z, decoded.Z, 4);
        Assert.Equal(frame, SerializeFrame(decoded));
    }

    [Fact]
    public void Serialize_BeidouEphemeris_PreservesNegativeClockAndUnsignedEccentricity()
    {
        var message = new RtcmV3Message1042
        {
            SatellitePrn = 1, WeekRaw = 1000, A2 = -RtcmV3EphemerisHelper.P2_66,
            A1 = -2 * RtcmV3Helper.P2_50, A0 = -3 * RtcmV3Helper.P2_33, e = 0.375,
        };
        var frame = SerializeFrame(message);
        var decoded = Assert.IsType<RtcmV3Message1042>(DeserializeFrame(frame));
        Assert.Equal(message.A2, decoded.A2);
        Assert.Equal(message.A1, decoded.A1);
        Assert.Equal(message.A0, decoded.A0);
        Assert.Equal(0.375, decoded.e, 12);
        Assert.Equal(frame, SerializeFrame(decoded));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1022)]
    public void Serialize_ProprietaryPayload_PreservesHalfByteAndBufferRemainder(int length)
    {
        var data = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();
        data[0] = 0x0B;
        var message = new RtcmV3Msg4094 { Data = data };
        var size = message.GetByteSize();
        var buffer = Enumerable.Repeat((byte)0xFF, size + 2).ToArray();
        Span<byte> span = buffer;
        message.Serialize(ref span);
        Assert.Equal(2, span.Length);
        Assert.Equal((byte)0xFF, span[0]);
        Assert.Equal(0x0B, buffer[4] & 0x0F);
        var decoded = Assert.IsType<RtcmV3Msg4094>(DeserializeFrame(buffer[..size]));
        Assert.Equal(data, decoded.Data);
    }

    [Fact]
    public void Serialize_OversizedPayload_RejectsLengthOverflow()
    {
        var message = new RtcmV3Msg4094 { Data = new byte[1023] };
        Assert.Throws<InvalidOperationException>(() => SerializeFrame(message));
    }

    [Fact]
    public void Serialize_Msm4_ZeroFineRangesRemainValid()
    {
        var message = new RtcmV3Msm4Msg1074
        {
            SatelliteIds = [1], SignalIds = [2], CellMask = [[1]], EpochTimeTow = 123456,
            RoughRangesRaw = [70 * RtcmV3Helper.RANGE_MS], PseudoRangeRaw = [0], PhaseRangeRaw = [0],
            LockRaw = [7], HalfCycleRaw = [1], CnrRaw = [45],
        };
        var frame = SerializeFrame(message);
        var bit = 24 + 12 + 157 + 1 + 18;
        Assert.Equal(0, Asv.IO.SpanBitHelper.GetBitS(frame, ref bit, 15));
        Assert.Equal(0, Asv.IO.SpanBitHelper.GetBitS(frame, ref bit, 22));
        var decoded = Assert.IsType<RtcmV3Msm4Msg1074>(DeserializeFrame(frame));
        Assert.Equal(0, decoded.PseudoRangeRaw[0]);
        Assert.Equal(0, decoded.PhaseRangeRaw[0]);
        Assert.Equal(frame, SerializeFrame(decoded));
    }

    private static byte[] SerializeFrame(RtcmV3MessageBase message)
    {
        var frame = new byte[message.GetByteSize()];
        Span<byte> span = frame;
        message.Serialize(ref span);
        Assert.Equal(0, span.Length);
        return frame;
    }

    private static Dictionary<ushort, int> DeserializeResource(string resourceName)
    {
        var data = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", resourceName));
        var counts = new Dictionary<ushort, int>();

        foreach (var frame in EnumerateRtcm3Frames(data))
        {
            var message = DeserializeFrame(frame);
            if (counts.ContainsKey(message.Id))
            {
                counts[message.Id]++;
            }
            else
            {
                counts.Add(message.Id, 1);
            }
        }

        return counts;
    }

    private static RtcmV3MessageBase DeserializeFrame(byte[] frame)
    {
        var id = GetMessageId(frame);
        Assert.True(MessageFactory.TryGetValue(id, out var factory), $"RTCM3 message {id} is not registered in test factory.");

        var message = factory();
        ReadOnlySpan<byte> span = frame;
        message.Deserialize(ref span);
        Assert.Equal(0, span.Length);
        return message;
    }

    private static IEnumerable<byte[]> EnumerateRtcm3Frames(byte[] data)
    {
        for (var i = 0; i < data.Length - 5;)
        {
            if (data[i] != RtcmV3Protocol.SyncByte)
            {
                i++;
                continue;
            }

            var payloadLength = ((data[i + 1] & 0x03) << 8) | data[i + 2];
            var frameLength = 3 + payloadLength + 3;
            if (payloadLength > 1023 || i + frameLength > data.Length)
            {
                i++;
                continue;
            }

            var frame = data[i..(i + frameLength)];
            var calculatedCrc = RtcmV3Crc24.Calc(frame, payloadLength + 3, 0);
            var sourceCrc = ((uint)frame[^3] << 16) | ((uint)frame[^2] << 8) | frame[^1];
            if (calculatedCrc != sourceCrc)
            {
                i++;
                continue;
            }

            yield return frame;
            i += frameLength;
        }
    }

    private static ushort GetMessageId(byte[] frame)
    {
        return (ushort)((frame[3] << 4) | ((frame[4] & 0xF0) >> 4));
    }

    private static byte[] Hex(string value)
    {
        var bytes = new byte[value.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
        }

        return bytes;
    }

    private const string Msm1073Frame =
        "D300B3431001226079E20020041D28400000000028204080F9FF90481F1F464878CB4C35D8962C101367270A4C4C990822104120850100834F045BB5776BBED59DABF8FCBCC27A88D367A457FA3FF17FD9A136425F05440A9C178C26FBF09B9A826E5D09B29803ABB00F040039D500E75403A3C00C057DA2C6F6906BDA24BF68243C2AB5FB199BF229DF43187D4AA9FF8813FE4AFFF8D98098140255B8094230250980937BEEEEEEEEEE6666DBC4DEEE6666600000003951BD";

    private const string Msm1083Frame =
        "D300B143B00130591EA20020418A03800000000030C000007FFE7FE7630506058FE39F7F70B86B7CD7F1BCBF7A0097F12942D3059AF384E755D693AC770E0E0EDAB6F5726CF7D9678FD71FC65BACB7BD877B239C4D36F2745BD61A2F5DF9BD7723F5CF2C129B904B15012E4F04B27FE72CEF9BB83E6AF4F9BFFBC741CF6B9B7D2A38F521A7D5CA8F5652BC7CC3F1E89BC7E46F1EBABEC61AFAF904409050DC50836E93BBB4EEEEEEEEDDCCA8EEEDEE98600000000B679C";

    private const string Msm1093Frame =
        "D3006C445001226079E20020000800A140000000040080807FFFD76ADAF55998B19262B1C7004D14999941A106E209447C9D443ABC792BCC6F914F405E2B0178B4C3E2D6504B7321314304C562084E281FB9007EF685D9F01CDB0073665F94177E551FF95817777777777777770000BA1F50";

    private const string Msm1123Frame =
        "D300B3463001225F9F200020000008168846000020000090F7AFFFF84ECD65E940A4A5280B5FF1E263D0BE51760C39987B70906119EA27D2645588B2114C82843BA87834EEBBDAEAFE3E220C2577BBF207E6D5CC378E7843B86BD0C7A58343360CAF07C1FF4F0CEDBC1E2CEDE553E977DFBF5341658B05781C14F520364A3ED15CFC28E7EE49BFB946FBF133F0AB57C21C2EFADEBC8AD1F3551FCC15CF1BEAC12A5D03AB3157777B72FB97BBBBBBBBBBBBAA40000000040DC3";
}
