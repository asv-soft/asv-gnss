using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class RawGpsPageCoverageTest
{
    public static IEnumerable<object[]> ScheduledPages()
    {
        // IS-GPS-200N Table 20-V: cycle position is distinct from the transmitted SV/page ID.
        byte[] fourth =
        [
            57,
            25,
            26,
            27,
            28,
            57,
            29,
            30,
            31,
            32,
            57,
            62,
            52,
            53,
            54,
            57,
            55,
            56,
            58,
            59,
            57,
            60,
            61,
            62,
            63,
        ];
        for (var index = 0; index < 25; index++)
        {
            yield return [4, index + 1, fourth[index]];
            yield return [5, index + 1, (byte)(index < 24 ? index + 1 : 51)];
        }
    }

    [Theory]
    [MemberData(nameof(ScheduledPages))]
    public void Deserialize_AllFiftyScheduledPages_ExposesIdentityAndRetainsEveryBit(
        int subframe,
        int position,
        byte pageId
    )
    {
        var source = Source((byte)subframe, pageId);
        var frame = EncodeIndependent(source, 3);
        GpsSubframeBase message = subframe == 4 ? new GpsSubframe4() : new GpsSubFrame5();
        Deserialize(message, frame);
        Assert.Equal((byte)subframe, message.Id);
        Assert.Equal(frame, Serialize(message));
        Assert.False(string.IsNullOrEmpty(message.ToString()));
        if (message is GpsSubframe4 fourth)
        {
            var expected = position switch
            {
                2 or 3 or 4 or 5 or 7 or 8 or 9 or 10 => GpsSubframe4.Subframe4Type.Almanac,
                13 => GpsSubframe4.Subframe4Type.Nmct,
                17 => GpsSubframe4.Subframe4Type.SpecialMessage,
                18 => GpsSubframe4.Subframe4Type.IonoAndUtcData,
                25 => GpsSubframe4.Subframe4Type.HealthAndConfigurations,
                _ => GpsSubframe4.Subframe4Type.None,
            };
            Assert.Equal(expected, fourth.Type);
            Assert.Equal(1, fourth.DataId);
            Assert.Equal(pageId, fourth.PageId);
            if (expected == GpsSubframe4.Subframe4Type.None)
                Assert.Equal(source[7..], fourth.RawPageData);
            else
                Assert.Null(fourth.RawPageData);
        }
        else
        {
            var fifth = Assert.IsType<GpsSubFrame5>(message);
            Assert.Equal(
                position == 25
                    ? GpsSubFrame5.Subframe5Type.ToaAndHealth
                    : GpsSubFrame5.Subframe5Type.Almanac,
                fifth.Type
            );
            Assert.Equal(1, fifth.DataId);
            Assert.Equal(pageId, fifth.PageId);
            Assert.Null(fifth.RawPageData);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Deserialize_Nmct_SignedSlotsAndAvailabilityRetainWireValues(byte availability)
    {
        var source = Source(4, 52);
        int[] expected = Enumerable
            .Range(0, 30)
            .Select(index =>
                index
                % 4 switch
                {
                    0 => -32,
                    1 => -31,
                    2 => 0,
                    _ => 31,
                }
            )
            .ToArray();
        Set(source, 56, 2, availability);
        for (var index = 0; index < 30; index++)
            Set(source, 58 + index * 6, 6, unchecked((uint)expected[index]));
        var frame = EncodeIndependent(source, 2);
        var message = new GpsSubframe4();
        Deserialize(message, frame);
        Assert.Equal(GpsSubframe4.Subframe4Type.Nmct, message.Type);
        Assert.Equal(availability, message.NmctAvailabilityIndicator);
        Assert.Equal(expected, message.NmctEstimatedRangeDeviations);
        Assert.Equal(frame, Serialize(message));
        message.Accept(new PageEditor());
        var edited = AssertParity(Serialize(message), 2);
        Assert.Equal(1u, Read(edited, 56, 2));
        Assert.Equal(32u, Read(edited, 232, 6));
        Assert.Equal((uint)(expected[28] & 63), Read(edited, 226, 6));
        Assert.Equal(-32, message.NmctEstimatedRangeDeviations[29]);
    }

    [Fact]
    public void Deserialize_SpecialMessage_PreservesDegreeByteAndVisitorEditsBothEnds()
    {
        var source = Source(4, 55);
        byte[] characters =
        [
            71,
            80,
            83,
            32,
            84,
            69,
            83,
            84,
            32,
            49,
            50,
            58,
            51,
            52,
            32,
            0xF8,
            43,
            45,
            47,
            46,
            39,
            34,
        ];
        characters.CopyTo(source, 7);
        var frame = EncodeIndependent(source, 1);
        var message = new GpsSubframe4();
        Deserialize(message, frame);
        Assert.Equal(GpsSubframe4.Subframe4Type.SpecialMessage, message.Type);
        Assert.Equal(characters, message.SpecialMessageData);
        Assert.Equal(frame, Serialize(message));
        message.Accept(new PageEditor());
        var edited = AssertParity(Serialize(message), 1);
        Assert.Equal((byte)'A', edited[7]);
        Assert.Equal((byte)'Z', edited[28]);
        Assert.Equal(0xF8, edited[22]);
        Assert.Equal(source[29] & 0xFC, edited[29] & 0xFC);
    }

    [Fact]
    public void Deserialize_UtcFutureLeapFields_VisitsSignedOffsetWeekAndDay()
    {
        var source = Source(4, 56);
        Set(source, 200, 8, 255);
        Set(source, 208, 8, 7);
        Set(source, 216, 8, 128);
        var frame = EncodeIndependent(source, 0);
        var message = new GpsSubframe4();
        Deserialize(message, frame);
        Assert.Equal(255, message.UtcGps.WN_LSF);
        Assert.Equal(7, message.UtcGps.DN);
        Assert.Equal(-128, message.UtcGps.DeltaT_LSF);
        Assert.Equal(frame, Serialize(message));
        message.Accept(new PageEditor());
        var edited = AssertParity(Serialize(message), 0);
        Assert.Equal(0u, Read(edited, 200, 8));
        Assert.Equal(1u, Read(edited, 208, 8));
        Assert.Equal(127u, Read(edited, 216, 8));
        Assert.Equal(Read(source, 192, 8), Read(edited, 192, 8));
    }

    [Fact]
    public void PageIdentity_ChangesLayoutWithoutLosingEditsAndClearsInactiveFields()
    {
        var message = new GpsSubframe4 { PageId = 55 };
        Assert.Equal(GpsSubframe4.Subframe4Type.SpecialMessage, message.Type);
        message.SpecialMessageData[0] = 0xA7;
        message.SpecialMessageData[21] = 0x65;
        message.PageId = 56;
        Assert.Equal(GpsSubframe4.Subframe4Type.IonoAndUtcData, message.Type);
        Assert.Null(message.SpecialMessageData);
        Assert.Equal(0xA7u, Read(AssertParity(Serialize(message), 0), 56, 8));
        // The last special-message character becomes a reserved UTC byte; retain the edited value.
        Assert.Equal(0x65u, Read(AssertParity(Serialize(message), 0), 224, 8));
        message.PageId = 57;
        Assert.Equal(GpsSubframe4.Subframe4Type.None, message.Type);
        Assert.Null(message.UtcGps);
        Assert.Equal(0xA7, message.RawPageData![0]);
        message.Accept(new PageEditor());
        Assert.Equal(0xE1u, Read(AssertParity(Serialize(message), 0), 56, 8));
        message.PageId = 52;
        Assert.Equal(GpsSubframe4.Subframe4Type.Nmct, message.Type);
        Assert.Null(message.RawPageData);
        Assert.Equal(3, message.NmctAvailabilityIndicator);
        message.DataId = 3;
        Assert.Equal(GpsSubframe4.Subframe4Type.None, message.Type);
        Assert.Null(message.NmctEstimatedRangeDeviations);
        message.DataId = 1;
        Assert.Equal(GpsSubframe4.Subframe4Type.Nmct, message.Type);
        Assert.Equal(3, message.NmctAvailabilityIndicator);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    public void ReservedPage_RawPayloadAndIdentityVisitorEditsAreSerialized(int subframe)
    {
        GpsSubframeBase model = subframe == 4 ? new GpsSubframe4() : new GpsSubFrame5();
        Deserialize(model, EncodeIndependent(Source((byte)subframe, 0), 0));
        model.Accept(new IdentityEditor());
        var source = AssertParity(Serialize(model), 0);
        Assert.Equal(2u, Read(source, 48, 2));
        Assert.Equal(37u, Read(source, 50, 6));
        Assert.Equal(0xE1u, Read(source, 56, 8));
        if (model is GpsSubframe4 fourth)
        {
            Assert.Equal(GpsSubframe4.Subframe4Type.None, fourth.Type);
            fourth.DataId = 1;
            fourth.PageId = 25;
            Assert.Equal(GpsSubframe4.Subframe4Type.Almanac, fourth.Type);
            Assert.Equal(25, fourth.SatelliteNumber);
            Assert.Null(fourth.RawPageData);
        }
        else
        {
            var fifth = Assert.IsType<GpsSubFrame5>(model);
            fifth.DataId = 1;
            fifth.PageId = 1;
            Assert.Equal(GpsSubFrame5.Subframe5Type.Almanac, fifth.Type);
            fifth.Af1 = 8 * Math.Pow(2, -38);
            fifth.Type = GpsSubFrame5.Subframe5Type.None;
            Assert.Throws<InvalidOperationException>(() => Serialize(fifth));
            fifth.PageId = 37;
            Assert.Equal(8u, Read(AssertParity(Serialize(fifth), 0), 224, 11));
            fifth.PageId = 51;
            Assert.Equal(GpsSubFrame5.Subframe5Type.ToaAndHealth, fifth.Type);
            Assert.Equal(24, fifth.Health.Length);
        }
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    public void Accept_TypeEdit_IsImmediatelyVisibleAndRetainsPayloadWhenSelectingRawPage(
        int subframe
    )
    {
        GpsSubframeBase model = subframe == 4 ? new GpsSubframe4() : new GpsSubFrame5();
        Deserialize(
            model,
            EncodeIndependent(Source((byte)subframe, (byte)(subframe == 4 ? 55 : 1)), 3)
        );
        if (model is GpsSubframe4 fourth)
            fourth.SpecialMessageData[0] = 0xBA;
        else
            ((GpsSubFrame5)model).Af1 = 8 * Math.Pow(2, -38);
        var editor = new TypeEditor(model);
        model.Accept(editor);
        Assert.True(editor.ImmediatelyVisible);
        Assert.Throws<InvalidOperationException>(() => Serialize(model));
        if (model is GpsSubframe4 fourthRaw)
        {
            fourthRaw.PageId = 37;
            Assert.Equal(0xBAu, Read(AssertParity(Serialize(model), 3), 56, 8));
        }
        else
        {
            ((GpsSubFrame5)model).PageId = 37;
            Assert.Equal(8u, Read(AssertParity(Serialize(model), 3), 224, 11));
        }
    }

    [Theory]
    [InlineData(4, 0, 52)]
    [InlineData(4, 2, 55)]
    [InlineData(4, 3, 56)]
    [InlineData(5, 0, 1)]
    [InlineData(5, 2, 51)]
    [InlineData(5, 3, 24)]
    public void Deserialize_UnknownDataId_DoesNotInterpretNominallyKnownPage(
        int subframe,
        byte dataId,
        byte pageId
    )
    {
        var source = Source((byte)subframe, pageId);
        Set(source, 48, 2, dataId);
        var frame = EncodeIndependent(source, 3);
        GpsSubframeBase model = subframe == 4 ? new GpsSubframe4() : new GpsSubFrame5();
        Deserialize(model, frame);
        Assert.Equal(frame, Serialize(model));
        if (model is GpsSubframe4 fourth)
        {
            Assert.Equal(GpsSubframe4.Subframe4Type.None, fourth.Type);
            Assert.Equal(dataId, fourth.DataId);
            Assert.Equal(pageId, fourth.PageId);
            Assert.Equal(source[7..], fourth.RawPageData);
            Assert.Null(fourth.NmctEstimatedRangeDeviations);
            Assert.Null(fourth.SpecialMessageData);
            Assert.Null(fourth.UtcGps);
        }
        else
        {
            var fifth = Assert.IsType<GpsSubFrame5>(model);
            Assert.Equal(GpsSubFrame5.Subframe5Type.None, fifth.Type);
            Assert.Equal(dataId, fifth.DataId);
            Assert.Equal(pageId, fifth.PageId);
            Assert.Equal(source[7..], fifth.RawPageData);
            Assert.Null(fifth.Health);
        }
    }

    [Fact]
    public void PageIdentity_RawReservedEditsSurviveTypedLayoutAndKeepCommonState()
    {
        var source = Source(4, 37);
        var message = new GpsSubframe4();
        Deserialize(message, EncodeIndependent(source, 3));
        message.TOW1_5Epoh = 54321;
        message.RawPageData![22] = 0xA7;
        message.PageId = 55;
        Assert.Equal(3, message.PreviousWordParity);
        Assert.Equal(54321u, message.TOW1_5Epoh);
        var frame = Serialize(message);
        var payload = AssertParity(frame, 3);
        Assert.Equal(0xA4, payload[29] & 0xFC);
        Assert.Equal(0u, Read(frame, 4 + 30 + 28, 2));
        Assert.Equal(0u, Read(frame, 4 + 270 + 28, 2));
        message.SpecialMessageData[0] = 0x9A;
        message.PageId = 57;
        Assert.Equal(0x9A, message.RawPageData![0]);
        Assert.Equal(0xA4, message.RawPageData[22] & 0xFC);
        Assert.Equal(3, message.PreviousWordParity);
        Assert.Equal(54321u, message.TOW1_5Epoh);
    }

    [Fact]
    public void Deserialize_ReusedPageModel_DropsInactivePayloadsWithoutDroppingReservedBytes()
    {
        var message = new GpsSubframe4();
        foreach (var id in new byte[] { 52, 55, 56, 57, 25, 63, 52 })
        {
            var source = Source(4, id);
            message.Deserialize(source);
            Assert.Equal(id, message.PageId);
            Assert.Equal(id == 52, message.NmctEstimatedRangeDeviations is not null);
            Assert.Equal(id == 55, message.SpecialMessageData is not null);
            Assert.Equal(id == 56, message.UtcGps is not null);
            Assert.Equal(id == 57, message.RawPageData is not null);
            Assert.Equal(source, AssertParity(Serialize(message), 0));
        }
    }

    [Fact]
    public void Type_AssignmentRetainsLegacyPermissivenessForIncompleteAndInvalidModels()
    {
        var fourth = new GpsSubframe4 { Type = GpsSubframe4.Subframe4Type.IonoAndUtcData };
        fourth.Type = GpsSubframe4.Subframe4Type.None;
        Assert.Equal(GpsSubframe4.Subframe4Type.None, fourth.Type);
        Assert.Throws<InvalidOperationException>(() => Serialize(fourth));
        fourth = new GpsSubframe4 { SatelliteNumber = 300 };
        fourth.Type = GpsSubframe4.Subframe4Type.HealthAndConfigurations;
        Assert.Equal(GpsSubframe4.Subframe4Type.HealthAndConfigurations, fourth.Type);
        Assert.Throws<ArgumentException>(() => Serialize(fourth));
        fourth = new GpsSubframe4 { PageId = 52 };
        fourth.NmctEstimatedRangeDeviations[0] = 99;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(fourth));
        Assert.Throws<ArgumentOutOfRangeException>(() => fourth.PageId = 55);
        fourth.Type = GpsSubframe4.Subframe4Type.None;
        Assert.Equal(GpsSubframe4.Subframe4Type.None, fourth.Type);
        Assert.Throws<InvalidOperationException>(() => Serialize(fourth));
        var fifth = new GpsSubFrame5 { SatelliteNumber = 300 };
        fifth.Type = GpsSubFrame5.Subframe5Type.ToaAndHealth;
        Assert.Equal(GpsSubFrame5.Subframe5Type.ToaAndHealth, fifth.Type);
        fifth.Type = GpsSubFrame5.Subframe5Type.None;
        Assert.Equal(GpsSubFrame5.Subframe5Type.None, fifth.Type);
        Assert.Throws<InvalidOperationException>(() => Serialize(fifth));
    }

    [Fact]
    public void Serialize_NewPageFields_RejectsWrongLengthsAndOutOfRangeWireValues()
    {
        var message = new GpsSubframe4 { PageId = 52 };
        Assert.Throws<ArgumentOutOfRangeException>(() => message.DataId = 4);
        Assert.Throws<ArgumentOutOfRangeException>(() => message.PageId = 64);
        Assert.Throws<InvalidOperationException>(() => message.RawPageData = new byte[23]);
        message.NmctAvailabilityIndicator = 4;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        message.NmctAvailabilityIndicator = 0;
        message.NmctEstimatedRangeDeviations[29] = -33;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        message.NmctEstimatedRangeDeviations[29] = 32;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        message.NmctEstimatedRangeDeviations = new int[29];
        Assert.Throws<ArgumentException>(() => Serialize(message));
        message = new GpsSubframe4 { PageId = 55 };
        message.SpecialMessageData = new byte[21];
        Assert.Throws<ArgumentException>(() => Serialize(message));
        message = new GpsSubframe4 { PageId = 56 };
        message.UtcGps.WN_LSF = 256;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        message.UtcGps.WN_LSF = 255;
        message.UtcGps.DN = -1;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        message.UtcGps.DN = 1;
        message.UtcGps.DeltaT_LSF = 128;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        message = new GpsSubframe4 { PageId = 57 };
        Assert.Throws<ArgumentException>(() => message.RawPageData = new byte[22]);
        var fifth = new GpsSubFrame5 { PageId = 37 };
        Assert.Throws<ArgumentOutOfRangeException>(() => fifth.DataId = 4);
        Assert.Throws<ArgumentOutOfRangeException>(() => fifth.PageId = 64);
        Assert.Throws<ArgumentException>(() => fifth.RawPageData = new byte[24]);
    }

    private class PageEditor
        : ArrayType.IVisitor,
            UInt8Type.IVisitor,
            Int32Type.IVisitor,
            OptionalStructType.IVisitor
    {
        protected int Index;

        public void VisitUnknown(Field field, IFieldType type) { }

        public void BeginArray(Field field, ArrayType type) => Index = 0;

        public void EndArray() { }

        public void BeginOptionalStruct(
            Field field,
            OptionalStructType type,
            bool isPresent,
            out bool createNew
        ) => createNew = false;

        public void EndOptionalStruct(bool isPresent) { }

        public virtual void Visit(Field field, UInt8Type type, ref byte value)
        {
            switch (field.Name)
            {
                case nameof(GpsSubframe4.NmctAvailabilityIndicator):
                    value = 1;
                    break;
                case nameof(GpsSubframe4.SpecialMessageData):
                    if (Index == 0)
                        value = (byte)'A';
                    if (Index == 21)
                        value = (byte)'Z';
                    Index++;
                    break;
                case nameof(GpsSubframe4.RawPageData):
                    if (Index++ == 0)
                        value = 0xE1;
                    break;
            }
        }

        public void Visit(Field field, Int32Type type, ref int value)
        {
            switch (field.Name)
            {
                case nameof(GpsSubframe4.NmctEstimatedRangeDeviations):
                    if (Index++ == 29)
                        value = -32;
                    break;
                case nameof(GpsSubframe4.UtcGpsRelationship.WN_LSF):
                    value = 0;
                    break;
                case nameof(GpsSubframe4.UtcGpsRelationship.DN):
                    value = 1;
                    break;
                case nameof(GpsSubframe4.UtcGpsRelationship.DeltaT_LSF):
                    value = 127;
                    break;
            }
        }
    }

    private sealed class TypeEditor(GpsSubframeBase model) : Int32Type.IVisitor
    {
        public bool ImmediatelyVisible { get; private set; }

        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, Int32Type type, ref int value)
        {
            if (field.Name != nameof(GpsSubframe4.Type))
                return;
            value =
                model is GpsSubframe4
                    ? (int)GpsSubframe4.Subframe4Type.None
                    : (int)GpsSubFrame5.Subframe5Type.None;
            ImmediatelyVisible = model is GpsSubframe4 fourth
                ? fourth.Type == GpsSubframe4.Subframe4Type.None
                : ((GpsSubFrame5)model).Type == GpsSubFrame5.Subframe5Type.None;
        }
    }

    private sealed class IdentityEditor : PageEditor
    {
        public override void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name == nameof(GpsSubframe4.DataId))
                value = 2;
            else if (field.Name == nameof(GpsSubframe4.PageId))
                value = 37;
            else
                base.Visit(field, type, ref value);
        }
    }

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
}
