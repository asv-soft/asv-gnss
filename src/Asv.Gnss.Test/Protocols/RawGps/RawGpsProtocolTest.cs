using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class RawGpsProtocolTest
{
    private const string Fixture =
        "08B0164F226AEA68E4FC070731985F1D6FC17140DD7FD283B83C3FC37D7ADF3364E6661260CC";

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void CommonHeader_IndependentBitsRoundTripAndVisitorEditsPreserveReservedBit(byte id)
    {
        for (var flags = 0; flags < 8; flags++)
        {
            var source = Source(id, id == 4 ? (byte)56 : (byte)51);
            Set(source, 8, 14, 0x2A65);
            Set(source, 22, 1, (uint)flags & 1);
            Set(source, 23, 1, 1);
            Set(source, 41, 1, (uint)(flags >> 1) & 1);
            Set(source, 42, 1, (uint)(flags >> 2) & 1);
            var frame = EncodeIndependent(source, 3);
            var model = Model(id);
            Deserialize(model, frame);
            Assert.Equal(0x2A65, model.TelemetryMessage);
            Assert.Equal((flags & 1) != 0, model.IntegrityStatusFlag);
            Assert.Equal((flags & 2) != 0, model.AlertFlag);
            Assert.Equal((flags & 4) != 0, model.AntiSpoofFlag);
            Assert.Equal(frame, Serialize(model));

            var editor = new HeaderEditor(model);
            model.Accept(editor);
            Assert.Equal(4, editor.ImmediatelyVisibleEdits);
            var edited = AssertParity(Serialize(model), 3);
            Assert.Equal(0x1357u, Read(edited, 8, 14));
            Assert.Equal((uint)(~flags) & 1, Read(edited, 22, 1));
            Assert.Equal((uint)(~flags >> 1) & 1, Read(edited, 41, 1));
            Assert.Equal((uint)(~flags >> 2) & 1, Read(edited, 42, 1));
            for (var bit = 0; bit < 240; bit++)
                if (bit is not (>= 8 and <= 22 or 41 or 42 or 46 or 47 or 238 or 239))
                    Assert.Equal(Read(source, bit, 1), Read(edited, bit, 1));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(31)]
    public void Aodo_WireUnitsIncludingInvalidNmctSentinel_RoundTripAndVisitorEdit(byte aodo)
    {
        var source = Source(2);
        Set(source, 233, 5, aodo);
        var frame = EncodeIndependent(source, 2);
        var model = new GpsSubframe2();
        Deserialize(model, frame);
        Assert.Equal(aodo, model.Aodo);
        Assert.Equal(frame, Serialize(model));

        var changed = (byte)((aodo + 7) % 32);
        var editor = new AodoEditor(model, changed);
        model.Accept(editor);
        Assert.True(editor.ImmediatelyVisibleEdit);
        var edited = AssertParity(Serialize(model), 2);
        Assert.Equal((uint)changed, Read(edited, 233, 5));
        for (var bit = 0; bit < 240; bit++)
            if (bit is not (46 or 47 or >= 233 and <= 239))
                Assert.Equal(Read(source, bit, 1), Read(edited, bit, 1));
        var reread = new GpsSubframe2();
        Deserialize(reread, Serialize(model));
        Assert.Equal(changed, reread.Aodo);
        Assert.Equal(model.Fit, reread.Fit);
        Assert.Equal(model.ToeSec, reread.ToeSec);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Serialize_HeaderAndAodoOverflow_DoesNotWriteOrAdvanceBuffer(bool header)
    {
        var model = new GpsSubframe2();
        model.Deserialize(Source(2));
        if (header)
            model.TelemetryMessage = 0x4000;
        else
            model.Aodo = 32;
        var destination = Enumerable.Repeat((byte)0xA5, 38).ToArray();
        Span<byte> output = destination;
        try
        {
            model.Serialize(ref output);
            Assert.Fail("A value wider than the field must be rejected.");
        }
        catch (ArgumentOutOfRangeException)
        {
            Assert.Equal(38, output.Length);
            Assert.Equal(Enumerable.Repeat((byte)0xA5, 38), destination);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void LegacyFactory_AllSubframes_RetainsHeaderFieldsWithoutRequiringParity(byte id)
    {
        var source = Source(id, id == 4 ? (byte)56 : (byte)51);
        Set(source, 8, 14, 0x3FFF);
        Set(source, 22, 1, 1);
        Set(source, 41, 2, 3);
        var words = Enumerable
            .Range(0, 10)
            .Select(index => Read(source, index * 24, 24) << 6)
            .ToArray();
        var message = GpsSubFrameFactory.Create(words);
        Assert.Equal(id, message.SubframeId);
        Assert.Equal(0x3FFF, message.TelemetryMessage);
        Assert.True(message.IntegrityStatusFlag);
        Assert.True(message.AlertFlag);
        Assert.True(message.AntiSpoofFlag);
        Assert.Equal(source, AssertParity(Serialize(message), 0));
    }

    [Fact]
    public void LegacySubclass_OnlyOriginalAbstractMemberRequired_SupportsHeaderSerialization()
    {
        var model = new LegacySubclass();
        var source = Source(1);
        model.Deserialize(source);
        Assert.Equal(source, AssertParity(Serialize(model), 0));
        model.TelemetryMessage = 7;
        Assert.Equal(7u, Read(AssertParity(Serialize(model), 0), 8, 14));
    }

    [Theory]
    [InlineData(0, Fixture)]
    [InlineData(1, "074FE9BAA26AEAE4E4FC070731985F1D6FC17140DD7FD283B83C3FC37D7ADF3364E6661260CC")]
    [InlineData(2, "08B016455D951548E4FC070731985F1D6FC17140DD7FD283B83C3FC37D7ADF3364E6661260CC")]
    [InlineData(3, "074FE9B0DD9515C4E4FC070731985F1D6FC17140DD7FD283B83C3FC37D7ADF3364E6661260CC")]
    public void Deserialize_IndependentParityFixtures_InfersPreviousStateAndRoundTrips(
        byte previous,
        string hex
    )
    {
        var bytes = Convert.FromHexString(hex);
        var message = new GpsSubframe2();
        ReadOnlySpan<byte> input = [.. bytes, 0xA5];
        message.Deserialize(ref input);
        Assert.Equal(new byte[] { 0xA5 }, input.ToArray());
        Assert.Equal(previous, message.PreviousWordParity);
        Assert.Equal(70487u, message.TOW1_5Epoh);
        Assert.Equal(2, message.Id);
        Assert.Equal(RawGpsProtocol.Info, message.Protocol);
        Assert.Equal(bytes, Serialize(message));
        AssertParity(bytes, previous);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 25)]
    [InlineData(4, 56)]
    [InlineData(4, 63)]
    [InlineData(4, 37)]
    [InlineData(5, 1)]
    [InlineData(5, 51)]
    [InlineData(5, 37)]
    public void RoundTrip_AllSubframesAndPages_PreservesEveryNativeBitAndVisitorInspection(
        byte id,
        byte page
    )
    {
        var source = Source(id, page);
        if (id == 1)
            Set(source, 160, 8, 128); // Legacy missing Tgd sentinel.
        var frame = EncodeIndependent(source, 3);
        var message = Model(id);
        Deserialize(message, frame);
        var first = new Collector();
        var second = new Collector();
        message.Accept(first);
        message.Accept(second);
        message.Accept(new PrintValueVisitor(new StringBuilder(), false));
        Assert.Equal(frame, Serialize(message));
        Assert.Equal(
            ExpectedFields(id, page).OrderBy(name => name, StringComparer.Ordinal),
            first.Fields.Select(field => field.Name).OrderBy(name => name, StringComparer.Ordinal)
        );
        Assert.Equal(
            first.Fields.Select(field => field.Name),
            second.Fields.Select(field => field.Name)
        );
        for (var index = 0; index < first.Fields.Count; index++)
            Assert.Same(first.Fields[index], second.Fields[index]);
        Assert.Contains(first.Fields, field => field.Name == nameof(GpsSubframeBase.TOW1_5Epoh));
        Assert.Throws<ArgumentNullException>(() => message.Accept(null!));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Serialize_TypedEdits_RecomputesAllParityAndNonInformationBits(byte id)
    {
        var source = Source(
            id,
            id == 4 ? (byte)56
                : id == 5 ? (byte)51
                : (byte)0
        );
        var message = Model(id);
        Deserialize(message, EncodeIndependent(source, 3));
        message.TOW1_5Epoh = 65432;
        switch (message)
        {
            case GpsSubframe1 value:
                value.WeekNumber = 825;
                break;
            case GpsSubframe2 value:
                value.iode = 207;
                break;
            case GpsSubframe3 value:
                value.iode = 207;
                break;
            case GpsSubframe4 value:
                value.Iono.Alpha0 = -17 * GpsRawHelper.P2_30;
                break;
            case GpsSubFrame5 value:
                value.Health[8] = 34;
                break;
        }
        var result = Serialize(message);
        var decoded = AssertParity(result, 3);
        Assert.Equal(65432u, Read(decoded, 24, 17));
        Assert.Equal(0u, Read(result, 4 + 30 + 28, 2));
        Assert.Equal(0u, Read(result, 4 + 270 + 28, 2));
        Assert.Equal(source.AsSpan(1, 2).ToArray(), decoded.AsSpan(1, 2).ToArray());
        switch (id)
        {
            case 1:
                Assert.Equal(825u, Read(decoded, 48, 10));
                break;
            case 2:
                Assert.Equal(207u, Read(decoded, 48, 8));
                break;
            case 3:
                Assert.Equal(207u, Read(decoded, 216, 8));
                break;
            case 4:
                Assert.Equal(239u, Read(decoded, 56, 8));
                break;
            case 5:
                Assert.Equal(34u, Read(decoded, 120, 6));
                break;
        }
        var reread = Model(id);
        Deserialize(reread, result);
        Assert.Equal(result, Serialize(reread));
    }

    [Fact]
    public void Accept_TypedReferenceEdits_AreVisibleDuringCallbackAndOnWire()
    {
        var message = new GpsSubframe1();
        Deserialize(message, EncodeIndependent(Source(1), 0));
        message.Accept(new WeekEditor(message));
        Assert.Equal(777u, Read(AssertParity(Serialize(message), 0), 48, 10));
    }

    [Fact]
    public void Deserialize_CorruptedParityDataPaddingOrTruncation_IsRejectedWithoutConsumingInput()
    {
        var fixture = Convert.FromHexString(Fixture);
        for (var bit = 0; bit < 304; bit++)
        {
            var broken = (byte[])fixture.Clone();
            broken[bit / 8] ^= (byte)(1 << (7 - bit % 8));
            Assert.Throws<ProtocolParserCrcException>(() =>
                Deserialize(new GpsSubframe2(), broken)
            );
        }
        Assert.Throws<FormatException>(() => Deserialize(new GpsSubframe2(), fixture[..^1]));
        var message = new GpsSubframe2();
        ReadOnlySpan<byte> input = fixture[..^1];
        try
        {
            message.Deserialize(ref input);
            Assert.Fail("The partial subframe must fail.");
        }
        catch (FormatException)
        {
            Assert.Equal(37, input.Length);
        }
    }

    [Fact]
    public void Parser_ContiguousSubframesAcrossByteBoundary_PublishesBothAndReportsOnlyByteAlignedCompletion()
    {
        var first = Convert.FromHexString(Fixture);
        var second = EncodeIndependent(Source(3), 0);
        var stream = Pack(first, second);
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        for (var index = 0; index < stream.Length; index++)
        {
            var completed = parser.Push(stream[index]);
            Assert.Equal(index == stream.Length - 1, completed);
            if (index == 37)
                Assert.Single(messages);
        }
        Assert.Collection(
            messages,
            message => Assert.Equal(70487u, Assert.IsType<GpsSubframe2>(message).TOW1_5Epoh),
            message => Assert.Equal(12345u, Assert.IsType<GpsSubframe3>(message).TOW1_5Epoh)
        );
    }

    [Fact]
    public void Parser_NoiseCorruptionTruncationAndReset_RecoversAtAnyBitOffset()
    {
        var valid = Convert.FromHexString(Fixture);
        for (var offset = 0; offset < 8; offset++)
        {
            using var parser = CreateParser();
            var messages = new List<IProtocolMessage>();
            using var subscription = parser.OnMessage.Subscribe(messages.Add);
            var broken = (byte[])valid.Clone();
            broken[20] ^= 1;
            foreach (var value in broken)
                parser.Push(value);
            Assert.Empty(messages);
            foreach (var value in valid[..20])
                parser.Push(value);
            parser.Reset();
            foreach (var value in valid[20..])
                parser.Push(value);
            Assert.Empty(messages);
            var stream = PackAtOffset(valid, offset);
            foreach (var value in stream)
                parser.Push(value);
            Assert.Equal(70487u, Assert.IsType<GpsSubframe2>(Assert.Single(messages)).TOW1_5Epoh);
        }
    }

    [Fact]
    public void LegacyFactory_ParityFreeReceiverWords_RemainsCompatibleAndCanSerializeNativeParity()
    {
        var source = Source(2);
        var words = new uint[10];
        for (var index = 0; index < 10; index++)
            words[index] = Read(source, index * 24, 24) << 6;
        var message = Assert.IsType<GpsSubframe2>(GpsSubFrameFactory.Create(words));
        Assert.Equal(12345u, message.TOW1_5Epoh);
        Assert.Equal(source, AssertParity(Serialize(message), 0));
        var legacy = new GpsSubframe2();
        legacy.Deserialize(source);
        Assert.Equal(Serialize(message), Serialize(legacy));
    }

    [Fact]
    public void Deserialize_ReusedPageModels_ClearInactiveFieldsAndRetainUnknownPages()
    {
        var fourth = new GpsSubframe4();
        fourth.Deserialize(Source(4, 56));
        Assert.NotNull(fourth.Iono);
        fourth.Deserialize(Source(4, 63));
        Assert.Null(fourth.Iono);
        Assert.Null(fourth.UtcGps);
        Assert.Equal(8, fourth.Health.Length);
        fourth.Deserialize(Source(4, 37));
        Assert.Equal(GpsSubframe4.Subframe4Type.None, fourth.Type);
        Assert.Null(fourth.Health);
        Assert.Null(fourth.SvConfig);
        var fifth = new GpsSubFrame5();
        fifth.Deserialize(Source(5, 1));
        Assert.Equal(1, fifth.SatelliteNumber);
        fifth.Deserialize(Source(5, 51));
        Assert.Equal(0, fifth.SatelliteNumber);
        Assert.Equal(24, fifth.Health.Length);
        fifth.Deserialize(Source(5, 37));
        Assert.Null(fifth.Health);
        Assert.Equal(GpsSubFrame5.Subframe5Type.None, fifth.Type);
    }

    [Fact]
    public void Serialize_AlmanacClockEdit_PreservesLegacyDecodedConvention()
    {
        var message = new GpsSubFrame5();
        message.Deserialize(Source(5, 1));
        foreach (var units in new[] { -32, -1, 0, 200, 279 })
        {
            message.Af0 = units * GpsRawHelper.P2_20;
            var reread = new GpsSubFrame5();
            Deserialize(reread, Serialize(message));
            Assert.Equal(message.Af0, reread.Af0);
        }
        message.Af0 = 280 * GpsRawHelper.P2_20;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
    }

    [Fact]
    public void Serialize_PageTypeChanges_UpdatesKnownPageAndRejectsAmbiguousNoneTransition()
    {
        var fourth = new GpsSubframe4();
        fourth.Deserialize(Source(4, 56));
        fourth.Type = GpsSubframe4.Subframe4Type.HealthAndConfigurations;
        fourth.SvConfig = Enumerable.Range(0, 32).Select(index => index % 16).ToArray();
        fourth.Health = Enumerable.Range(0, 8).ToArray();
        var fourthRead = new GpsSubframe4();
        Deserialize(fourthRead, Serialize(fourth));
        Assert.Equal(GpsSubframe4.Subframe4Type.HealthAndConfigurations, fourthRead.Type);
        Assert.Equal(fourth.SvConfig, fourthRead.SvConfig);
        Assert.Equal(fourth.Health, fourthRead.Health);
        fourth.Type = GpsSubframe4.Subframe4Type.None;
        Assert.Throws<InvalidOperationException>(() => Serialize(fourth));

        var fifth = new GpsSubFrame5();
        fifth.Deserialize(Source(5, 1));
        fifth.Type = GpsSubFrame5.Subframe5Type.ToaAndHealth;
        fifth.Health = Enumerable.Range(0, 24).ToArray();
        fifth.Week = 200;
        fifth.ToaSec = 40960;
        var fifthRead = new GpsSubFrame5();
        Deserialize(fifthRead, Serialize(fifth));
        Assert.Equal(GpsSubFrame5.Subframe5Type.ToaAndHealth, fifthRead.Type);
        Assert.Equal(200, fifthRead.Week);
        Assert.Equal(40960, fifthRead.ToaSec);
        Assert.Equal(fifth.Health, fifthRead.Health);
        fifth.Type = GpsSubFrame5.Subframe5Type.None;
        Assert.Throws<InvalidOperationException>(() => Serialize(fifth));
    }

    [Fact]
    public void Accept_NestedIonosphereAndHealthElements_ExposesAllFieldsAndEditsBackingStorage()
    {
        var fourth = new GpsSubframe4();
        fourth.Deserialize(Source(4, 56));
        var ionosphereFields = new Collector();
        fourth.Iono.Accept(ionosphereFields);
        Assert.Equal(
            new[]
            {
                "Alpha0",
                "Alpha1",
                "Alpha2",
                "Alpha3",
                "Betta0",
                "Betta1",
                "Betta2",
                "Betta3",
            },
            ionosphereFields.Fields.Select(field => field.Name)
        );
        var utcFields = new Collector();
        fourth.UtcGps.Accept(utcFields);
        Assert.Equal(
            new[] { "A0", "A1", "Tot", "WNt", "DeltaT_LS", "WN_LSF", "DN", "DeltaT_LSF" },
            utcFields.Fields.Select(field => field.Name)
        );
        fourth.Accept(new IonosphereEditor(fourth));
        Assert.Equal(9u, Read(AssertParity(Serialize(fourth), 0), 56, 8));

        var fifth = new GpsSubFrame5();
        fifth.Deserialize(Source(5, 51));
        fifth.Accept(new HealthEditor(fifth));
        var source = AssertParity(Serialize(fifth), 0);
        for (var index = 0; index < 24; index++)
            Assert.Equal((uint)(index + 1), Read(source, 72 + index * 6, 6));
    }

    private static string[] ExpectedFields(byte id, byte page)
    {
        var body = id switch
        {
            1 =>
                "WeekNumber Code SatelliteAccuracy SatelliteHealth Flag TocSec Af2 Af1 Af0 iodc Tgd",
            2 => "iode Crs deln M0 Cuc e Cus A ToeSec Fit Aodo",
            3 => "Cic OMG0 Cis i0 Crc omg OMGd iode idot",
            4 when page is >= 25 and <= 32 =>
                "Type SatelliteNumber A e i0 OMG0 omg M0 OMGd ToaSec Af0 Af1 Health",
            4 when page == 56 => "Type Iono UtcGps",
            4 when page == 52 => "Type NmctAvailabilityIndicator NmctEstimatedRangeDeviations",
            4 when page == 55 => "Type SpecialMessageData",
            4 when page == 63 => "Type SvConfig Health",
            5 when page is >= 1 and <= 24 =>
                "Type SatelliteNumber A e i0 OMG0 omg M0 OMGd ToaSec Af0 Af1 Health",
            5 when page == 51 => "Type ToaSec Week Health",
            _ => "Type RawPageData",
        };
        if (id is 4 or 5)
            body = "DataId PageId " + body;
        return (
            "TelemetryMessage IntegrityStatusFlag AlertFlag AntiSpoofFlag TOW1_5Epoh PreviousWordParity "
            + body
        ).Split(' ');
    }

    private static IProtocolParser CreateParser() =>
        Protocol
            .Create(builder => builder.Protocols.RegisterRawGpsProtocol())
            .CreateParser(RawGpsProtocol.Info.Id);

    private static GpsSubframeBase Model(byte id) =>
        id switch
        {
            1 => new GpsSubframe1(),
            2 => new GpsSubframe2(),
            3 => new GpsSubframe3(),
            4 => new GpsSubframe4(),
            5 => new GpsSubFrame5(),
            _ => throw new ArgumentOutOfRangeException(nameof(id)),
        };

    private static byte[] Serialize(GpsSubframeBase message)
    {
        var result = new byte[message.GetByteSize() + 1];
        result[^1] = 0xA5;
        Span<byte> output = result;
        message.Serialize(ref output);
        Assert.Equal(new byte[] { 0xA5 }, output.ToArray());
        return result[..^1];
    }

    private static void Deserialize(GpsSubframeBase message, byte[] frame)
    {
        ReadOnlySpan<byte> input = frame;
        try
        {
            message.Deserialize(ref input);
        }
        catch
        {
            Assert.Equal(frame.Length, input.Length);
            throw;
        }
        Assert.Empty(input.ToArray());
    }

    private static byte[] Source(byte id, byte page = 0)
    {
        var data = Enumerable.Range(0, 30).Select(index => (byte)(index * 17 + 53)).ToArray();
        data[0] = 0x8B;
        Set(data, 24, 17, 12345);
        Set(data, 43, 3, id);
        if (id >= 4)
        {
            Set(data, 48, 2, 1);
            Set(data, 50, 6, page);
        }
        return data;
    }

    // Independent transcription of IS-GPS-200N Table 20-XIV, with explicit one-based source-bit indices.
    private static readonly int[][] ParityIndices =
    [
        [1, 2, 3, 5, 6, 10, 11, 12, 13, 14, 17, 18, 20, 23],
        [2, 3, 4, 6, 7, 11, 12, 13, 14, 15, 18, 19, 21, 24],
        [1, 3, 4, 5, 7, 8, 12, 13, 14, 15, 16, 19, 20, 22],
        [2, 4, 5, 6, 8, 9, 13, 14, 15, 16, 17, 20, 21, 23],
        [1, 3, 5, 6, 7, 9, 10, 14, 15, 16, 17, 18, 21, 22, 24],
        [3, 5, 6, 8, 9, 10, 11, 13, 15, 19, 22, 23, 24],
    ];

    private static uint Parity(uint data, byte previous)
    {
        uint result = 0;
        for (var index = 0; index < 6; index++)
        {
            var sum = (uint)(previous >> (index is 0 or 2 or 5 ? 1 : 0)) & 1;
            foreach (var position in ParityIndices[index])
                sum ^= (data >> (24 - position)) & 1;
            result = (result << 1) | sum;
        }
        return result;
    }

    private static byte[] EncodeIndependent(byte[] source, byte previous)
    {
        var output = new byte[38];
        for (var index = 0; index < 10; index++)
        {
            var data = Read(source, index * 24, 24);
            var parity = Parity(data, previous);
            Set(
                output,
                4 + index * 30,
                30,
                ((data ^ ((previous & 1) != 0 ? 0xFFFFFFu : 0)) << 6) | parity
            );
            previous = (byte)(parity & 3);
        }
        return output;
    }

    private static byte[] AssertParity(byte[] frame, byte previous)
    {
        var result = new byte[30];
        for (var index = 0; index < 10; index++)
        {
            var word = Read(frame, 4 + index * 30, 30);
            var data = (word >> 6) ^ ((previous & 1) != 0 ? 0xFFFFFFu : 0);
            Assert.Equal(Parity(data, previous), word & 63);
            Set(result, index * 24, 24, data);
            previous = (byte)(word & 3);
        }
        return result;
    }

    private static byte[] Pack(params byte[][] frames)
    {
        var result = new byte[(frames.Length * 300 + 7) / 8];
        for (var index = 0; index < frames.Length; index++)
        for (var bit = 0; bit < 300; bit++)
            Set(result, index * 300 + bit, 1, Read(frames[index], 4 + bit, 1));
        return result;
    }

    private static byte[] PackAtOffset(byte[] frame, int offset)
    {
        var output = new byte[(300 + offset + 7) / 8];
        for (var bit = 0; bit < 300; bit++)
            Set(output, offset + bit, 1, Read(frame, 4 + bit, 1));
        return output;
    }

    private static uint Read(byte[] data, int start, int count)
    {
        uint result = 0;
        for (var index = 0; index < count; index++)
            result =
                (result << 1)
                | (uint)((data[(start + index) / 8] >> (7 - (start + index) % 8)) & 1);
        return result;
    }

    private static void Set(byte[] data, int start, int count, uint value)
    {
        for (var index = 0; index < count; index++)
        {
            var position = start + index;
            var mask = 1 << (7 - position % 8);
            data[position / 8] = (byte)(
                (data[position / 8] & ~mask)
                | (((value >> (count - index - 1)) & 1) != 0 ? mask : 0)
            );
        }
    }

    private sealed class Collector : IVisitor
    {
        public List<Field> Fields { get; } = [];

        public void VisitUnknown(Field field, IFieldType type) => Fields.Add(field);
    }

    private sealed class LegacySubclass : GpsSubframeBase
    {
        public override byte SubframeId => 1;
    }

    private sealed class HeaderEditor(GpsSubframeBase model)
        : UInt16Type.IVisitor,
            BoolType.IVisitor
    {
        public int ImmediatelyVisibleEdits { get; private set; }

        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt16Type type, ref ushort value)
        {
            if (field.Name != nameof(GpsSubframeBase.TelemetryMessage))
                return;
            value = 0x1357;
            if (model.TelemetryMessage == value)
                ImmediatelyVisibleEdits++;
        }

        public void Visit(Field field, BoolType type, ref bool value)
        {
            value = !value;
            var visible = field.Name switch
            {
                nameof(GpsSubframeBase.IntegrityStatusFlag) => model.IntegrityStatusFlag,
                nameof(GpsSubframeBase.AlertFlag) => model.AlertFlag,
                nameof(GpsSubframeBase.AntiSpoofFlag) => model.AntiSpoofFlag,
                _ => throw new InvalidOperationException(field.Name),
            };
            if (visible == value)
                ImmediatelyVisibleEdits++;
        }
    }

    private sealed class AodoEditor(GpsSubframe2 model, byte changed) : UInt8Type.IVisitor
    {
        public bool ImmediatelyVisibleEdit { get; private set; }

        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name != nameof(GpsSubframe2.Aodo))
                return;
            value = changed;
            ImmediatelyVisibleEdit = model.Aodo == changed;
        }
    }

    private sealed class IonosphereEditor(GpsSubframe4 message)
        : OptionalStructType.IVisitor,
            DoubleType.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }

        public void BeginOptionalStruct(
            Field field,
            OptionalStructType type,
            bool isPresent,
            out bool createNew
        ) => createNew = false;

        public void EndOptionalStruct(bool isPresent) { }

        public void Visit(Field field, DoubleType type, ref double value)
        {
            if (field.Name != nameof(GpsSubframe4.Ionospheric.Alpha0))
                return;
            value = 9 * GpsRawHelper.P2_30;
            Assert.Equal(value, message.Iono.Alpha0);
        }
    }

    private sealed class HealthEditor(GpsSubFrame5 message) : ArrayType.IVisitor, Int32Type.IVisitor
    {
        private int _index;

        public void VisitUnknown(Field field, IFieldType type) { }

        public void BeginArray(Field field, ArrayType type) => _index = 0;

        public void EndArray() { }

        public void Visit(Field field, Int32Type type, ref int value)
        {
            if (field.Name != nameof(GpsSubFrame5.Health))
                return;
            value = _index + 1;
            Assert.Equal(value, message.Health[_index]);
            _index++;
        }
    }

    private sealed class WeekEditor(GpsSubframe1 message) : Int32Type.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, Int32Type type, ref int value)
        {
            if (field.Name != nameof(GpsSubframe1.WeekNumber))
                return;
            value = 777;
            Assert.Equal(777, message.WeekNumber);
        }
    }
}
