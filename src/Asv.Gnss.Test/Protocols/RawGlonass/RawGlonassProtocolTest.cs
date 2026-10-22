using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class RawGlonassProtocolTest
{
    // String 1 is the independently recorded UBX-RXM-SFRBX fixture used by UbxRxmSfrbxTest.
    // The other fixed vectors retain its payload and use independently calculated ICD 5.1 section 4.7 check bits.
    public static TheoryData<byte, string> Strings =>
        new()
        {
            { 1, "01003896A022064406BEA0" },
            { 2, "02003896A022064406BE21" },
            { 3, "03003896A022064406BEF1" },
            { 4, "04003896A022064406BE22" },
            { 5, "05003896A022064406BEF2" },
            { 6, "06003896A022064406BE73" },
            { 7, "07003896A022064406BEA3" },
            { 8, "08003896A022064406BEA3" },
            { 9, "09003896A022064406BE73" },
            { 10, "0A003896A022064406BEF2" },
            { 11, "0B003896A022064406BE22" },
            { 12, "0C003896A022064406BEF1" },
            { 13, "0D003896A022064406BE21" },
            { 14, "0E003896A022064406BEA0" },
            { 15, "0F003896A022064406BE70" },
        };

    [Theory]
    [MemberData(nameof(Strings))]
    public void Registration_AllFifteenStrings_ParseTypedModelsAndPreserveNativeBits(
        byte id,
        string hex
    )
    {
        var data = Convert.FromHexString(hex);
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        Assert.Equal(1, Push(parser, data));
        var message = Assert.IsAssignableFrom<GlonassWordBase>(Assert.Single(messages));
        Assert.Equal($"GlonassWord{id}", message.GetType().Name);
        Assert.Equal(id, message.Id);
        Assert.Equal(id.ToString(), message.GetIdAsString());
        Assert.Equal(RawGlonassProtocol.Info, message.Protocol);
        Assert.Equal(11, message.GetByteSize());
        Assert.Equal(data, Serialize(message));
        Assert.Empty(errors);
        Assert.True(GlonassWordChecksum.IsValid(data));
        Assert.Equal(data[^1], GlonassWordChecksum.Compute(data));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public void Push_ContiguousNativeStringsAtEveryRelevantBitOffset_PreservesFollowingString(
        int prefixBits
    )
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var stream = Pack(
            prefixBits,
            "01003896A022064406BEA0",
            "02003896A022064406BE21",
            "03003896A022064406BEF1"
        );
        foreach (var value in stream)
        {
            // Asv.IO dispatchers reset parsers after a reported byte-boundary completion.
            if (parser.Push(value))
                parser.Reset();
        }
        Assert.Equal(new byte[] { 1, 2, 3 }, messages.Cast<GlonassWordBase>().Select(m => m.Id));
        Assert.Equal(
            Convert.FromHexString("03003896A022064406BEF1"),
            Serialize((GlonassWordBase)messages[2])
        );
    }

    [Fact]
    public void Push_PartialResetAndCorruption_RecoversAtFollowingValidString()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var good = Convert.FromHexString("01003896A022064406BEA0");
        Push(parser, good.AsSpan(0, 7));
        Assert.Empty(messages);
        parser.Reset();
        var corrupt = good.ToArray();
        corrupt[^1] ^= 1;
        Push(parser, corrupt);
        Assert.Empty(messages);
        Push(parser, good);
        var message = Assert.IsType<GlonassWord1>(Assert.Single(messages));
        Assert.Equal(good, Serialize(message));
    }

    [Fact]
    public void Deserialize_AllSingleBitErrorsAreRejectedWithoutCorrection_ButLegacyReceiverDecodeRemainsPermissive()
    {
        var original = Convert.FromHexString("01003896A022064406BEA0");
        for (var bit = 3; bit < 88; bit++)
        {
            var corrupt = original.ToArray();
            corrupt[bit / 8] ^= (byte)(1 << (7 - bit % 8));
            Assert.False(GlonassWordChecksum.IsValid(corrupt));
            if (corrupt[0] is < 1 or > 15)
                Assert.Throws<FormatException>(() => Deserialize(new GlonassWord1(), corrupt));
            else
                Assert.Throws<ProtocolParserCrcException>(() =>
                    Deserialize(new GlonassWord1(), corrupt)
                );
        }
        var checksumCorrupt = original.ToArray();
        checksumCorrupt[^1] ^= 1;
        var legacy = new GlonassWord1();
        legacy.Deserialize(checksumCorrupt);
        Assert.Equal(checksumCorrupt, legacy.RawData);
        Assert.Equal(original, Serialize(legacy));
        var receiver = Assert.IsType<GlonassWord1>(
            GlonassWordFactory.Create([0x0801C4B5, 0x01103220, 0x35F50000])
        );
        Assert.Equal(original, receiver.RawData);
        Assert.Equal(original, Serialize(receiver));
    }

    [Fact]
    public void Deserialize_ConsumesExactlyOneFrameAndRejectsPaddingIdleIdLengthAndWrongConcreteType()
    {
        var good = Convert.FromHexString("01003896A022064406BEA0");
        ReadOnlySpan<byte> data = [.. good, 0xDE, 0xAD];
        var word = new GlonassWord1();
        word.Deserialize(ref data);
        Assert.Equal(new byte[] { 0xDE, 0xAD }, data.ToArray());
        Assert.Throws<FormatException>(() => Deserialize(new GlonassWord1(), good[..10]));
        foreach (var firstByte in new byte[] { 0, 0x11, 0x21, 0x81 })
        {
            var invalid = good.ToArray();
            invalid[0] = firstByte;
            Assert.Throws<FormatException>(() => Deserialize(new GlonassWord1(), invalid));
        }
        Assert.Throws<FormatException>(() =>
            Deserialize(new GlonassWord8(), Convert.FromHexString("06003896A022064406BE73"))
        );
        Assert.Throws<FormatException>(() =>
            Deserialize(new GlonassWord9(), Convert.FromHexString("07003896A022064406BEA3"))
        );
    }

    [Fact]
    public void Deserialize_SharedEvenAndOddModelsRemainReusableThroughProtocolInterface()
    {
        var even = new GlonassWordEven();
        Deserialize(even, Convert.FromHexString("06003896A022064406BE73"));
        Assert.Equal(6, even.Id);
        Deserialize(even, Convert.FromHexString("0E003896A022064406BEA0"));
        Assert.Equal(14, even.Id);
        Assert.Equal(Convert.FromHexString("0E003896A022064406BEA0"), Serialize(even));
        var odd = new GlonassWordOdd();
        Deserialize(odd, Convert.FromHexString("07003896A022064406BEA3"));
        Assert.Equal(7, odd.Id);
        Deserialize(odd, Convert.FromHexString("0F003896A022064406BE70"));
        Assert.Equal(15, odd.Id);
        Assert.Equal(Convert.FromHexString("0F003896A022064406BE70"), Serialize(odd));
    }

    [Fact]
    public void StringOne_P1UsesIcdBits77And78_AndTypedEditsRecomputeHamming()
    {
        var message = new GlonassWord1();
        Deserialize(message, Convert.FromHexString("01203896A022064406BE6D"));
        Assert.Equal(2, message.P1);
        Assert.Equal(1680, message.TofLocalSec);
        Assert.Equal(Convert.FromHexString("01203896A022064406BE6D"), Serialize(message));
        message.PositionX = -12500;
        Assert.Equal(Convert.FromHexString("01203896A02204006400F4"), Serialize(message));
    }

    [Theory]
    [InlineData(1, "0100008000008400000069")]
    [InlineData(3, "03400080000084000000F6")]
    [InlineData(4, "0480000200000000000025")]
    [InlineData(5, "050010000000004000005A")]
    [InlineData(6, "06008020000100000000DF")]
    [InlineData(7, "0780000000040000100027")]
    public void Serialize_SignMagnitudeNegativeZeroAndReservedBits_RoundTripExactly(
        byte id,
        string hex
    )
    {
        var data = Convert.FromHexString(hex);
        var message = CreateWord(id);
        Deserialize(message, data);
        Assert.Equal(data, Serialize(message));
    }

    [Theory]
    [MemberData(nameof(Strings))]
    public void Accept_AllStringModelsVisitEveryModeledPropertyWithItsWireCompatibleType(
        byte id,
        string hex
    )
    {
        var message = CreateWord(id);
        Deserialize(message, Convert.FromHexString(hex));
        var collector = new FieldCollector();
        message.Accept(collector);
        string[] expected = id switch
        {
            1 => ["P1", "TofLocalSec", "VelocityX", "AccelerationX", "PositionX"],
            2 => ["Health", "P2", "Tb", "VelocityY", "AccelerationY", "PositionY"],
            3 => ["P3", "GamN", "P", "ln", "VelocityZ", "AccelerationZ", "PositionZ"],
            4 => ["TauN", "DelayTauN", "Age", "P4", "Accuracy", "Nt", "Slot", "M"],
            5 => ["Na", "TauC", "N4", "TauGPS", "ln"],
            14 => ["FrameNumber", "С", "M", "Prn", "DelaT2", "LOmega", "i", "e"],
            15 => ["FrameNumber", "omega", "ToaSec", "DeltaT", "DeltaDT", "Frequency", "ln"],
            _ when id % 2 == 0 => ["С", "M", "Prn", "DelaT2", "LOmega", "i", "e"],
            _ => ["omega", "ToaSec", "DeltaT", "DeltaDT", "Frequency", "ln"],
        };
        Assert.Equal(
            expected.OrderBy(x => x),
            collector.Fields.Select(f => f.Name).OrderBy(x => x)
        );
        foreach (var field in collector.Fields)
        {
            var type = message.GetType().GetProperty(field.Name)!.PropertyType;
            Assert.Equal(
                type == typeof(double) ? typeof(DoubleType)
                    : type == typeof(long) ? typeof(Int64Type)
                    : type == typeof(ushort) ? typeof(UInt16Type)
                    : typeof(UInt8Type),
                field.DataType.GetType()
            );
        }
        Assert.Equal(Convert.FromHexString(hex), Serialize(message));
    }

    [Fact]
    public void Accept_VisitsBackingReferencesAndEditedValuesDetermineSerializedBits()
    {
        var message = new GlonassWord1();
        Deserialize(message, Convert.FromHexString("01003896A022064406BEA0"));
        var editor = new PositionEditor(message);
        message.Accept(editor);
        Assert.True(editor.VisitedP1);
        Assert.True(editor.VisitedPosition);
        Assert.Equal(Convert.FromHexString("01203896A02204006400F4"), Serialize(message));
    }

    [Fact]
    public void Serialize_RejectsUnrepresentableFieldsWithoutWritingPartialOutput()
    {
        var message = new GlonassWord1 { P1 = 4 };
        var bytes = Enumerable.Repeat((byte)0xCC, 15).ToArray();
        Assert.Throws<ArgumentOutOfRangeException>(() => SerializeInto(message, bytes));
        Assert.All(bytes, value => Assert.Equal(0xCC, value));
        message.P1 = 0;
        message.PositionX = double.PositiveInfinity;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        message.PositionX = 0;
        message.TofLocalSec = 1;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        Assert.Throws<ArgumentException>(() => SerializeInto(new GlonassWord1(), new byte[10]));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Serialize(new GlonassWord4 { Accuracy = 3 })
        );
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Serialize(new GlonassWord7 { Frequency = 1602000001 })
        );
    }

    [Fact]
    public void ConstructorDefaults_RemainCompatibleAndExplicitPhysicalValuesEnableNewMessageSerialization()
    {
        var ephemeris = new GlonassWord4();
        var even = new GlonassWord6();
        var odd = new GlonassWord7();
        Assert.Equal(0, ephemeris.Accuracy);
        Assert.Equal(0, even.i);
        Assert.Equal(0, odd.Frequency);
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(ephemeris));
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(even));
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(odd));
        ephemeris.Accuracy = 1;
        even.i = 63.0 / 180.0 * Math.PI;
        odd.Frequency = 1602000000;
        Assert.Equal(Convert.FromHexString("0400000000000000000052"), Serialize(ephemeris));
        Assert.Equal(Convert.FromHexString("0600000000000000000003"), Serialize(even));
        Assert.Equal(Convert.FromHexString("07000000000000000000D3"), Serialize(odd));
    }

    [Fact]
    public void ToString_PreservesLegacyJsonPropertiesWithoutProtocolInfrastructure()
    {
        var json = Newtonsoft.Json.Linq.JObject.Parse(new GlonassWord1().ToString());
        Assert.NotNull(json["WordId"]);
        Assert.NotNull(json["RawData"]);
        Assert.NotNull(json["PositionX"]);
        Assert.Null(json["Id"]);
        Assert.Null(json["Name"]);
        Assert.Null(json["Protocol"]);
        Assert.Null(json["Tags"]);
    }

    private static GlonassWordBase CreateWord(byte id) =>
        id switch
        {
            1 => new GlonassWord1(),
            2 => new GlonassWord2(),
            3 => new GlonassWord3(),
            4 => new GlonassWord4(),
            5 => new GlonassWord5(),
            6 => new GlonassWord6(),
            7 => new GlonassWord7(),
            8 => new GlonassWord8(),
            9 => new GlonassWord9(),
            10 => new GlonassWord10(),
            11 => new GlonassWord11(),
            12 => new GlonassWord12(),
            13 => new GlonassWord13(),
            14 => new GlonassWord14(),
            15 => new GlonassWord15(),
            _ => throw new ArgumentOutOfRangeException(nameof(id)),
        };

    private static void Deserialize(GlonassWordBase message, byte[] bytes)
    {
        ReadOnlySpan<byte> data = bytes;
        message.Deserialize(ref data);
        Assert.True(data.IsEmpty);
    }

    private static byte[] Serialize(GlonassWordBase message)
    {
        var bytes = new byte[message.GetByteSize()];
        SerializeInto(message, bytes);
        return bytes;
    }

    private static void SerializeInto(GlonassWordBase message, byte[] bytes)
    {
        Span<byte> data = bytes;
        message.Serialize(ref data);
    }

    private static IProtocolParser CreateParser() =>
        Protocol
            .Create(builder => builder.Protocols.RegisterRawGlonassProtocol())
            .CreateParser(RawGlonassProtocol.Info.Id);

    private static int Push(IProtocolParser parser, ReadOnlySpan<byte> bytes)
    {
        var count = 0;
        foreach (var value in bytes)
            if (parser.Push(value))
                count++;
        return count;
    }

    private static byte[] Pack(int prefixBits, params string[] frames)
    {
        var bytes = new byte[(prefixBits + frames.Length * 85 + 7) / 8];
        for (var index = 0; index < prefixBits; index++)
            bytes[index / 8] |= (byte)(1 << (7 - index % 8));
        var position = prefixBits;
        foreach (var frame in frames)
        {
            var source = Convert.FromHexString(frame);
            for (var index = 3; index < 88; index++, position++)
                if ((source[index / 8] & (1 << (7 - index % 8))) != 0)
                    bytes[position / 8] |= (byte)(1 << (7 - position % 8));
        }
        return bytes;
    }

    private sealed class FieldCollector : IVisitor
    {
        public List<Field> Fields { get; } = [];

        public void VisitUnknown(Field field, IFieldType type) => Fields.Add(field);
    }

    private sealed class PositionEditor(GlonassWord1 message)
        : UInt8Type.IVisitor,
            DoubleType.IVisitor
    {
        public bool VisitedP1 { get; private set; }
        public bool VisitedPosition { get; private set; }

        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name != nameof(GlonassWord1.P1))
                return;
            value = 2;
            Assert.Equal(value, message.P1);
            VisitedP1 = true;
        }

        public void Visit(Field field, DoubleType type, ref double value)
        {
            if (field.Name != nameof(GlonassWord1.PositionX))
                return;
            value = -12500;
            Assert.Equal(value, message.PositionX);
            VisitedPosition = true;
        }
    }
}
