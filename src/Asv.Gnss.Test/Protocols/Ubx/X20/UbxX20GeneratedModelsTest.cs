using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxX20GeneratedModelsTest
{
    [Fact]
    public void Registry_All163ManufacturerVariants_HaveDistinctTypedModelsAnd101WireIdentifiers()
    {
        var entries = UbxX20Protocol.Messages;
        var concrete = typeof(UbxX20MessageBase).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(UbxX20MessageBase).IsAssignableFrom(type)).ToArray();

        Assert.Equal(163, entries.Count);
        Assert.Equal(101, entries.Select(entry => entry.Id).Distinct().Count());
        var manifest = string.Concat(entries.Select(entry => $"{entry.Id:X4} {entry.Name} {entry.Variant} {entry.Direction}\n")
            .Order(StringComparer.Ordinal));
        Assert.Equal("c1950d0ae5bd1a4e95ea8b46eab8e930a0e4664573cdff4a75eaa86294001352",
            Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(manifest))));
        Assert.Equal(concrete.OrderBy(type => type.Name), entries.Select(entry => entry.MessageType).OrderBy(type => type.Name));
        Assert.Equal(163, entries.Select(entry => (entry.Id, entry.Variant)).Distinct().Count());
        foreach (var entry in entries)
        {
            var model = entry.Create();
            Assert.Equal(entry.MessageType, model.GetType());
            Assert.Equal(entry.Id, model.Id);
            Assert.Equal(entry.Name, model.Name);
            Assert.Equal(entry.Variant, model.Variant);
            Assert.Equal(entry.MessageType, UbxX20Protocol.CreateMessage(entry.Id, entry.Variant).GetType());
            Assert.False(string.IsNullOrWhiteSpace(entry.Direction));
        }
    }

    [Fact]
    public void Serialize_EveryVariant_DefaultDiscriminatorsSelectExactModelAndRoundTrip()
    {
        var factory = new UbxX20MessageFactory();
        foreach (var entry in UbxX20Protocol.Messages)
        {
            var source = entry.Create();
            var wire = Serialize(source);
            var selected = factory.Create(source.Id, wire.AsSpan(6, wire.Length - 8));
            Assert.NotNull(selected);
            Assert.Equal(source.GetType(), selected.GetType());
            ReadOnlySpan<byte> input = wire;
            selected.Deserialize(ref input);
            Assert.True(input.IsEmpty);
            Assert.Equal(wire, Serialize(selected));
        }
    }

    [Fact]
    public void Randomize_EveryVariant_PreservesDiscriminatorsAndRoundTripsTypedPayload()
    {
        var factory = new UbxX20MessageFactory();
        var random = new Random(124578);
        var changed = 0;
        foreach (var entry in UbxX20Protocol.Messages)
        {
            var source = entry.Create();
            var before = Serialize(source);
            source.Randomize(random);
            var wire = Serialize(source);
            if (!before.SequenceEqual(wire))
                changed++;
            var decoded = factory.Create(source.Id, wire.AsSpan(6, wire.Length - 8));
            Assert.NotNull(decoded);
            Assert.Equal(source.GetType(), decoded.GetType());
            ReadOnlySpan<byte> input = wire;
            decoded.Deserialize(ref input);
            Assert.True(input.IsEmpty);
            Assert.Equal(wire, Serialize(decoded));
        }
        Assert.True(changed > 100);
    }

    [Fact]
    public void Accept_RepeatedTraversalOfAll163Models_DoesNotAllocate()
    {
        var models = UbxX20Protocol.Messages.Select(entry => entry.Create()).ToArray();
        var random = new Random(125);
        var visitor = new NoOpVisitor();
        foreach (var model in models)
        {
            model.Randomize(random);
            for (var iteration = 0; iteration < 100; iteration++)
                model.Accept(visitor);
            var before = GC.GetAllocatedBytesForCurrentThread();
            for (var iteration = 0; iteration < 100; iteration++)
                model.Accept(visitor);
            Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
        }
        GC.KeepAlive(models);
    }

    [Fact]
    public void Accept_EveryCanonicalFieldAndNestedRecord_HasTypedMetadataAndPreservesWireBytes()
    {
        var records = new HashSet<Type>();
        foreach (var entry in UbxX20Protocol.Messages)
        {
            var value = entry.Create();
            var before = Serialize(value);
            AssertMetadata(value.GetType(), records);
            var visitor = new RootFieldsVisitor();
            value.Accept(visitor);
            Assert.Equal(Metadata(value.GetType()).Fields.Select(field => field.Name).Order(), visitor.Fields.Order());
            Assert.Equal(before, Serialize(value));
        }
        Assert.NotEmpty(records);
        foreach (var type in records.ToArray())
        {
            var value = Assert.IsAssignableFrom<IVisitable>(Activator.CreateInstance(type));
            AssertMetadata(type, records);
            var visitor = new RootFieldsVisitor();
            value.Accept(visitor);
            Assert.Equal(Metadata(type).Fields.Select(field => field.Name).Order(), visitor.Fields.Order());
        }
    }

    [Fact]
    public void Deserialize_NavPvtIndependentBytes_PreservesNativeScalesReservedBytesAndUnknownFlagBits()
    {
        var payload = new byte[92];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, 345678901);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(4), 2026);
        payload[6] = 10;
        payload[7] = 22;
        payload[20] = 3;
        payload[21] = 0xA3;
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(24), 602345678);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(28), -123456789);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(76), 123);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(78), 0xBEE0);
        payload[80] = 0xAA;
        payload[81] = 0x55;
        payload[82] = 0xFF;
        payload[83] = 0x80;
        var wire = Frame(0x0107, payload);
        var message = Decode<UbxX20NavPvtData1>(wire);

        Assert.Equal(345678901u, message.ITOW);
        Assert.Equal(602345678, message.Lon);
        Assert.Equal(-123456789, message.Lat);
        Assert.Equal((ushort)123, message.PDOP);
        Assert.Equal(new byte[] { 0xAA, 0x55, 0xFF, 0x80 }, message.Reserved0);
        Assert.Equal(wire, Serialize(message));
        var lonField = Metadata(message.GetType())[nameof(message.Lon)]!;
        Assert.Contains("1e-7", lonField.GetDescription());
        Assert.Equal("deg", lonField.GetUnits());

        message.Flags3AuthTime = 0;
        Assert.Equal((ushort)(0xBEE0 & ~0x2000), message.Flags3);
        message.Flags3AuthTime = 1;
        Assert.Equal((ushort)0xBEE0, message.Flags3);
    }

    [Fact]
    public void Accept_ScalarAndRepeatedRecordEdits_UpdateBackingStorageDuringCallback()
    {
        var message = new UbxX20RxmRawxData1
        {
            RcvTow = 123.5,
            Meas = [new() { PrMes = 20_000_000.25, CpMes = -100.5, DoMes = 12.25f, GnssId = 0, SvId = 7 }],
        };
        var storage = message.Meas;
        var visitor = new DirectEditVisitor(message);

        message.Accept(visitor);

        Assert.Equal(2, visitor.Edits);
        Assert.Same(storage, message.Meas);
        Assert.Equal((byte)1, message.NumMeas);
        var decoded = Decode<UbxX20RxmRawxData1>(Serialize(message));
        Assert.Equal(456.75, decoded.RcvTow);
        Assert.Equal(20_000_001.5, Assert.Single(decoded.Meas).PrMes);
        Assert.Equal(-100.5, decoded.Meas[0].CpMes);
        Assert.Equal(12.25f, decoded.Meas[0].DoMes);
    }

    [Fact]
    public void Deserialize_RawxIndependentRepeatedBytes_UsesCountAndFloatingPointWireLayout()
    {
        var payload = new byte[48];
        BinaryPrimitives.WriteDoubleLittleEndian(payload, 123456.25);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(8), 2400);
        payload[10] = 18;
        payload[11] = 1;
        payload[12] = 0x83;
        payload[13] = 1;
        payload[14] = 0xA1;
        payload[15] = 0xB2;
        BinaryPrimitives.WriteDoubleLittleEndian(payload.AsSpan(16), 22_000_000.125);
        BinaryPrimitives.WriteDoubleLittleEndian(payload.AsSpan(24), -15.5);
        BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(32), -123.25f);
        payload[36] = 6;
        payload[37] = 14;
        payload[38] = 2;
        payload[39] = 9;
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(40), 1234);
        payload[42] = 45;
        payload[46] = 0xCF;
        payload[47] = 0xEE;
        var wire = Frame(0x0215, payload);

        var message = Decode<UbxX20RxmRawxData1>(wire);

        Assert.Equal(123456.25, message.RcvTow);
        Assert.Equal((ushort)2400, message.Week);
        var measurement = Assert.Single(message.Meas);
        Assert.Equal(22_000_000.125, measurement.PrMes);
        Assert.Equal(-15.5, measurement.CpMes);
        Assert.Equal(-123.25f, measurement.DoMes);
        Assert.Equal((byte)6, measurement.GnssId);
        Assert.Equal((ushort)1234, measurement.Locktime);
        Assert.Equal((byte)0xCF, measurement.TrkStat);
        Assert.Equal(wire, Serialize(message));
    }

    [Fact]
    public void Deserialize_RawxNonFiniteFloatingPointPayloads_PreservesNaNPayloadBitsAndSignedZero()
    {
        var payload = new byte[48];
        BinaryPrimitives.WriteUInt64LittleEndian(payload, 0x8000000000000000);
        payload[11] = 1;
        payload[13] = 1;
        BinaryPrimitives.WriteUInt64LittleEndian(payload.AsSpan(16), 0x7FF8000000001234);
        BinaryPrimitives.WriteUInt64LittleEndian(payload.AsSpan(24), 0xFFF0000000000000);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(32), 0x7FC01234);
        var wire = Frame(0x0215, payload);

        var message = Decode<UbxX20RxmRawxData1>(wire);
        message.Accept(new NoOpVisitor());

        Assert.Equal(unchecked((long)0x8000000000000000), BitConverter.DoubleToInt64Bits(message.RcvTow));
        Assert.True(double.IsNaN(message.Meas[0].PrMes));
        Assert.True(double.IsNegativeInfinity(message.Meas[0].CpMes));
        Assert.True(float.IsNaN(message.Meas[0].DoMes));
        Assert.Equal(wire, Serialize(message));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Deserialize_CfgCfgOptionalDeviceMask_PreservesPresenceAndEveryBit(bool present)
    {
        var payload = new byte[present ? 13 : 12];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, 0xDEADBEEF);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(4), 0x12345678);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), 0xFEDCBA98);
        if (present)
            payload[12] = 0xF7;
        var wire = Frame(0x0609, payload);

        var message = Decode<UbxX20CfgCfgData1>(wire);

        Assert.Equal(present, message.Group1 is not null);
        if (present)
            Assert.Equal((byte)0xF7, message.Group1!.DeviceMask);
        Assert.Equal(0xDEADBEEFu, message.ClearMask);
        Assert.Equal(wire, Serialize(message));
    }

    [Fact]
    public void Deserialize_ReusedModel_ClearsAbsentOptionalGroupAndEmptyRepeatedRecords()
    {
        var factory = new UbxX20MessageFactory();
        var cfgPayload = new byte[13];
        cfgPayload[12] = 0xA5;
        var cfg = Assert.IsType<UbxX20CfgCfgData1>(factory.Create(0x0609, cfgPayload));
        ReadInto(cfg, Frame(0x0609, cfgPayload));
        Assert.Equal((byte)0xA5, cfg.Group1!.DeviceMask);
        ReadInto(cfg, Frame(0x0609, new byte[12]));
        Assert.Null(cfg.Group1);
        Assert.Equal(20, cfg.GetByteSize());

        var rawPayload = new byte[48];
        rawPayload[11] = 1;
        rawPayload[13] = 1;
        BinaryPrimitives.WriteDoubleLittleEndian(rawPayload.AsSpan(16), 20_000_000.25);
        var raw = Assert.IsType<UbxX20RxmRawxData1>(factory.Create(0x0215, rawPayload));
        ReadInto(raw, Frame(0x0215, rawPayload));
        Assert.Equal(20_000_000.25, Assert.Single(raw.Meas).PrMes);
        var oldStorage = raw.Meas;
        var emptyPayload = new byte[16];
        emptyPayload[13] = 1;
        ReadInto(raw, Frame(0x0215, emptyPayload));
        Assert.Empty(raw.Meas);
        Assert.NotSame(oldStorage, raw.Meas);
        Assert.Equal((byte)0, raw.NumMeas);
        Assert.Equal(24, raw.GetByteSize());
        return;

        static void ReadInto(UbxX20MessageBase message, byte[] wire)
        {
            ReadOnlySpan<byte> input = wire;
            message.Deserialize(ref input);
            Assert.True(input.IsEmpty);
        }
    }

    [Fact]
    public void Deserialize_InfAsciiBytes_PreservesNonAsciiAndNulInsteadOfNormalizingPayload()
    {
        var wire = Frame(0x0404, [0x41, 0x00, 0xFF, 0x5A]);
        var message = Decode<UbxX20InfDebugData0>(wire);

        Assert.Equal(new byte[] { 0x41, 0x00, 0xFF, 0x5A }, message.Str);
        Assert.Equal("A", message.StrAscii);
        Assert.Equal(wire, Serialize(message));
    }

    [Fact]
    public void Constructors_VariantIdentifiersAreNotAssumedToBeWireVersionValues()
    {
        var eph = new UbxX20MgaGpsEph();
        Assert.Equal((byte)1, eph.Type);
        Assert.Equal((byte)0, eph.Version);
        var raw = new UbxX20RxmRawxData1();
        Assert.Equal((byte)1, raw.Version);
        var osnma = new UbxX20SecOsnmaData2();
        Assert.Equal((byte)3, osnma.Version);
        var system = new UbxX20MonSysData0();
        Assert.Equal((byte)1, system.MsgVer);
        Assert.Equal((byte)2, new UbxX20UpdSosAck().Cmd);
    }

    [Fact]
    public void DirectConcreteVariant_WrongVersionTypeOrCommand_IsRejectedOnReadAndWrite()
    {
        Assert.Throws<FormatException>(() => Decode<UbxX20SecSigData1>(Frame(0x2709, [3, 0, 0, 0])));
        var eph = new byte[68];
        eph[0] = 2;
        Assert.Throws<FormatException>(() => Decode<UbxX20MgaGpsEph>(Frame(0x1300, eph)));
        Assert.Throws<FormatException>(() => Decode<UbxX20UpdSosAck>(Frame(0x0914, [3, 0, 0, 0, 0, 0, 0, 0])));
        Assert.Throws<InvalidOperationException>(() => new UbxX20SecSigData1 { Version = 99 }.GetByteSize());
        Assert.Throws<InvalidOperationException>(() => new UbxX20MgaGpsEph { Type = 2 }.GetByteSize());
        Assert.Throws<InvalidOperationException>(() => new UbxX20UpdSosAck { Cmd = 3 }.GetByteSize());
    }

    [Fact]
    public void Deserialize_OsnmaPackedSatelliteCount_DecodesRecordsWithoutLosingOtherHeaderBits()
    {
        var payload = new byte[36];
        payload[0] = 3;
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(24), 0xC1234502);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(28), 0xA345);
        payload[30] = 12;
        payload[31] = 0xA5;
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(32), 0xC789);
        payload[34] = 24;
        payload[35] = 0x5A;
        var wire = Frame(0x270A, payload);

        var message = Decode<UbxX20SecOsnmaData2>(wire);

        Assert.Equal(0xC1234502u, message.GeneralAndTiming);
        Assert.Equal(2u, message.GeneralAndTimingAuthSVs);
        Assert.Equal(2, message.AuthPerSV.Length);
        Assert.Equal((byte)12, message.AuthPerSV[0].SvId);
        Assert.Equal((byte)24, message.AuthPerSV[1].SvId);
        Assert.Equal((ushort)0xA345, message.AuthPerSV[0].Bitfield1);
        Assert.Equal(wire, Serialize(message));
        message.AuthPerSV = [message.AuthPerSV[0]];
        Assert.Throws<InvalidOperationException>(() => message.GetByteSize());
        message.GeneralAndTimingAuthSVs = 1;
        Assert.Equal(0xC1234501u, message.GeneralAndTiming);
        Assert.Equal((byte)12, Assert.Single(Decode<UbxX20SecOsnmaData2>(Serialize(message)).AuthPerSV).SvId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(504)]
    public void Deserialize_PmpIndependentUserData_UsesDeclaredLengthIncluding504ByteBoundary(int count)
    {
        var payload = new byte[24 + count];
        payload[0] = 1;
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(2), (ushort)count);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(4), 123456);
        for (var index = 0; index < count; index++)
            payload[24 + index] = (byte)(index + 1);
        var wire = Frame(0x0272, payload);

        var message = Decode<UbxX20RxmPmpData2>(wire);

        Assert.Equal((ushort)count, message.NumBytesUserData);
        Assert.Equal(payload[24..], message.UserData);
        Assert.Equal(123456u, message.TimeTag);
        Assert.Equal(wire, Serialize(message));
        message.UserData = [0xA1, 0xB2];
        Assert.Equal((ushort)2, message.NumBytesUserData);
        Assert.Equal(new byte[] { 0xA1, 0xB2 }, Decode<UbxX20RxmPmpData2>(Serialize(message)).UserData);
    }

    [Fact]
    public void Deserialize_PmpMissingExtraOrOver504UserBytes_AreRejected()
    {
        var missing = new byte[24];
        missing[0] = 1;
        missing[2] = 1;
        Assert.Throws<FormatException>(() => Decode<UbxX20RxmPmpData2>(Frame(0x0272, missing)));
        missing[2] = 0;
        Assert.Throws<FormatException>(() => Decode<UbxX20RxmPmpData2>(Frame(0x0272, missing.Concat(new byte[1]).ToArray())));
        var oversized = new byte[529];
        oversized[0] = 1;
        BinaryPrimitives.WriteUInt16LittleEndian(oversized.AsSpan(2), 505);
        Assert.Throws<FormatException>(() => Decode<UbxX20RxmPmpData2>(Frame(0x0272, oversized)));
        Assert.Throws<InvalidOperationException>(() => new UbxX20RxmPmpData2 { UserData = new byte[505] }.GetByteSize());
    }

    [Fact]
    public void Deserialize_SpartnIndependentTwoKeyPayload_UsesEachDeclaredKeyLength()
    {
        var payload = new byte[25];
        payload[0] = 1;
        payload[1] = 2;
        payload[4] = 0xAA;
        payload[5] = 2;
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(6), 2400);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), 123456);
        payload[12] = 0xBB;
        payload[13] = 3;
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(14), 2401);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(16), 654321);
        new byte[] { 0xA1, 0xA2, 0xB1, 0xB2, 0xB3 }.CopyTo(payload, 20);
        var wire = Frame(0x0236, payload);

        var message = Decode<UbxX20RxmSpartnkeyData0>(wire);

        Assert.Equal((byte)2, message.NumKeys);
        Assert.Equal(new byte[] { 2, 3 }, message.KeysDescription.Select(key => key.KeyLengthBytes));
        Assert.Equal(new ushort[] { 2400, 2401 }, message.KeysDescription.Select(key => key.ValidFromWno));
        Assert.Equal(new byte[] { 0xA1, 0xA2, 0xB1, 0xB2, 0xB3 }, message.Key);
        Assert.Equal(wire, Serialize(message));
        var empty = Decode<UbxX20RxmSpartnkeyData0>(Frame(0x0236, [1, 0, 0, 0]));
        Assert.Empty(empty.KeysDescription);
        Assert.Empty(empty.Key);
    }

    [Fact]
    public void Deserialize_SpartnWrongConcatenatedLengthOrMoreThanTwoKeys_AreRejected()
    {
        var missing = new byte[13];
        missing[0] = 1;
        missing[1] = 1;
        missing[5] = 2;
        Assert.Throws<FormatException>(() => Decode<UbxX20RxmSpartnkeyData0>(Frame(0x0236, missing)));
        Assert.Throws<FormatException>(() => Decode<UbxX20RxmSpartnkeyData0>(Frame(0x0236, [1, 0, 0, 0, 0xAA])));
        var three = new byte[28];
        three[0] = 1;
        three[1] = 3;
        Assert.Throws<FormatException>(() => Decode<UbxX20RxmSpartnkeyData0>(Frame(0x0236, three)));
        Assert.Throws<InvalidOperationException>(() => new UbxX20RxmSpartnkeyData0
        {
            KeysDescription = [new() { KeyLengthBytes = 2 }], Key = [0xAA],
        }.GetByteSize());
        Assert.Throws<InvalidOperationException>(() => new UbxX20RxmSpartnkeyData0
        {
            KeysDescription = [new(), new(), new()],
        }.GetByteSize());
    }

    [Fact]
    public void Deserialize_TruncatedMisalignedAndCountMismatchedPayloads_AreRejected()
    {
        Assert.ThrowsAny<Exception>(() => Decode<UbxX20NavPvtData1>(Frame(0x0107, new byte[91])));
        Assert.Throws<FormatException>(() => Decode<UbxX20NavPvtData1>(Frame(0x0107, new byte[93])));
        var raw = new byte[16];
        raw[11] = 1;
        raw[13] = 1;
        Assert.Throws<FormatException>(() => Decode<UbxX20RxmRawxData1>(Frame(0x0215, raw)));
        raw[11] = 0;
        Assert.Throws<FormatException>(() => Decode<UbxX20RxmRawxData1>(Frame(0x0215, raw.Concat(new byte[32]).ToArray())));
        Assert.Throws<FormatException>(() => Decode<UbxX20MonVerData0>(Frame(0x0A04, new byte[41])));
        Assert.Throws<FormatException>(() => Decode<UbxX20CfgCfgData1>(Frame(0x0609, new byte[14])));
    }

    [Fact]
    public void Serialize_InvalidFixedArrayOversizedPayloadAndOverflowingCount_AreRejected()
    {
        Assert.Throws<InvalidOperationException>(() => new UbxX20NavPvtData1 { Reserved0 = new byte[3] }.GetByteSize());
        Assert.Throws<InvalidOperationException>(() => new UbxX20InfDebugData0 { Str = new byte[65536] }.GetByteSize());
        Assert.Throws<OverflowException>(() => new UbxX20RxmRawxData1 { Meas = new UbxX20RxmRawxData1MeasRecord[256] }.GetByteSize());
    }

    [Fact]
    public void Accept_DerivedCountEdit_IsRejectedAndLeavesTheSourceArrayIntact()
    {
        var message = new UbxX20RxmRawxData1 { Meas = [new()] };
        var storage = message.Meas;
        Assert.Throws<InvalidOperationException>(() => message.Accept(new CountEditVisitor()));
        Assert.Same(storage, message.Meas);
        Assert.Equal((byte)1, message.NumMeas);
        Assert.Equal(true, Metadata(message.GetType())[nameof(message.NumMeas)]!.Metadata["readOnly"]);
    }

    private static T Decode<T>(byte[] wire) where T : UbxX20MessageBase, new()
    {
        var message = new T();
        ReadOnlySpan<byte> input = wire;
        message.Deserialize(ref input);
        Assert.True(input.IsEmpty);
        return message;
    }

    private static byte[] Serialize(UbxX20MessageBase value)
    {
        var result = new byte[value.GetByteSize()];
        var output = result.AsSpan();
        value.Serialize(ref output);
        Assert.True(output.IsEmpty);
        return result;
    }

    private static byte[] Frame(ushort id, byte[] payload)
    {
        var result = new byte[payload.Length + 8];
        result[0] = 0xB5;
        result[1] = 0x62;
        result[2] = (byte)(id >> 8);
        result[3] = (byte)id;
        BinaryPrimitives.WriteUInt16LittleEndian(result.AsSpan(4), checked((ushort)payload.Length));
        payload.CopyTo(result, 6);
        byte a = 0, b = 0;
        for (var index = 2; index < result.Length - 2; index++)
        {
            a = unchecked((byte)(a + result[index]));
            b = unchecked((byte)(b + a));
        }
        result[^2] = a;
        result[^1] = b;
        return result;
    }

    private static StructType Metadata(Type type) => Assert.IsType<StructType>(
        type.GetProperty("StructType", BindingFlags.Public | BindingFlags.Static)!.GetValue(null));

    private static void AssertMetadata(Type type, HashSet<Type> records)
    {
        var metadata = Metadata(type);
        var expected = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(property => type.GetField("_" + char.ToLowerInvariant(property.Name[0]) + property.Name[1..], BindingFlags.NonPublic | BindingFlags.Instance) is not null
                || metadata.Fields.Any(field => field.Name == property.Name && field.Metadata.ContainsKey("readOnly"))).ToArray();
        Assert.Equal(expected.Select(property => property.Name).Order(), metadata.Fields.Select(field => field.Name).Order());
        foreach (var field in metadata.Fields)
        {
            Assert.NotNull(field.DataType);
            Assert.False(string.IsNullOrWhiteSpace(field.GetDescription()));
        }
        foreach (var property in expected)
        {
            var nested = property.PropertyType.IsArray ? property.PropertyType.GetElementType()! : property.PropertyType;
            if (typeof(IVisitable).IsAssignableFrom(nested) && records.Add(nested))
                AssertMetadata(nested, records);
        }
    }

    private class NoOpVisitor : UInt8Type.IVisitor, Int8Type.IVisitor, UInt16Type.IVisitor,
        Int16Type.IVisitor, UInt32Type.IVisitor, Int32Type.IVisitor, FloatType.IVisitor,
        DoubleType.IVisitor, StructType.IVisitor, OptionalStructType.IVisitor, ArrayType.IVisitor, ListType.IVisitor
    {
        protected int Depth;
        protected virtual void Seen(Field field) { }
        public void VisitUnknown(Field field, IFieldType type) => Seen(field);
        public virtual void Visit(Field field, UInt8Type type, ref byte value) => Seen(field);
        public void Visit(Field field, Int8Type type, ref sbyte value) => Seen(field);
        public void Visit(Field field, UInt16Type type, ref ushort value) => Seen(field);
        public void Visit(Field field, Int16Type type, ref short value) => Seen(field);
        public void Visit(Field field, UInt32Type type, ref uint value) => Seen(field);
        public void Visit(Field field, Int32Type type, ref int value) => Seen(field);
        public void Visit(Field field, FloatType type, ref float value) => Seen(field);
        public virtual void Visit(Field field, DoubleType type, ref double value) => Seen(field);
        public void BeginStruct(Field field, StructType type) { Seen(field); Depth++; }
        public void EndStruct() => Depth--;
        public void BeginOptionalStruct(Field field, OptionalStructType type, bool isPresent, out bool createNew)
        { Seen(field); Depth++; createNew = false; }
        public void EndOptionalStruct(bool isPresent) => Depth--;
        public void BeginArray(Field field, ArrayType type) { Seen(field); Depth++; }
        public void EndArray() => Depth--;
        public void BeginList(Field field, ListType type, ref uint size) { Seen(field); Depth++; }
        public void EndList() => Depth--;
    }

    private sealed class RootFieldsVisitor : NoOpVisitor
    {
        public HashSet<string> Fields { get; } = [];
        protected override void Seen(Field field)
        {
            if (Depth == 0)
                Assert.True(Fields.Add(field.Name));
        }
    }

    private sealed class DirectEditVisitor(UbxX20RxmRawxData1 message) : NoOpVisitor
    {
        public int Edits { get; private set; }
        public override void Visit(Field field, DoubleType type, ref double value)
        {
            if (field.Name == nameof(message.RcvTow))
            {
                value = 456.75;
                Assert.Equal(value, message.RcvTow);
                Edits++;
            }
            else if (field.Name == nameof(UbxX20RxmRawxData1MeasRecord.PrMes))
            {
                value = 20_000_001.5;
                Assert.Equal(value, message.Meas[0].PrMes);
                Edits++;
            }
        }
    }

    private sealed class CountEditVisitor : NoOpVisitor
    {
        public override void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name == nameof(UbxX20RxmRawxData1.NumMeas))
                value++;
        }
    }
}
