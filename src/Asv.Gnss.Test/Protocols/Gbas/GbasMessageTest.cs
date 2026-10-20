using System;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class GbasMessageTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(11)]
    [InlineData(101)]
    public void Deserialize_LegacyApplicationBlock_PreservesBytesAndHeader(byte type)
    {
        var expected = Convert.FromHexString(GbasLegacyFixtures.Block(type));
        var original = GbasTestHelper.CreateBlock(type);
        Assert.Equal(expected, GbasTestHelper.Serialize(original));
        var copy = GbasMessageBlockFactory.CreateGbasMessageBlock(type);
        ReadOnlySpan<byte> read = [.. expected, 0x12, 0x34];
        copy.Deserialize(ref read);
        Assert.Equal(new byte[] { 0x12, 0x34 }, read.ToArray());
        Assert.Equal(type, copy.MessageTypeId);
        Assert.Equal("UUEE", copy.GbasId);
        Assert.Equal(MessageBlockIdEnum.Normal, copy.MessageBlockId);
        Assert.Equal(expected, GbasTestHelper.Serialize(copy));
    }

    [Fact]
    public void Deserialize_Type1_PreservesSignedCorrectionsAndUnavailableSentinels()
    {
        var copy = Read<MessageBlockType1>(1);
        Assert.Equal(123.4, copy.ModifiedZCount, 7);
        Assert.Equal(AdditionalMessageEnum.FirstOfTwo, copy.AdditionalMessageFlag);
        Assert.Equal(0x1234, copy.EphemerisCrc);
        Assert.Null(copy.SourceAvailabilityDuration);
        var measurement = Assert.Single(copy.Measurements);
        Assert.Equal(7, measurement.RangingSourceId);
        Assert.Equal(23, measurement.Iod);
        Assert.Equal(-123.45, measurement.Prc, 7);
        Assert.Equal(3.456, measurement.Rrc, 7);
        Assert.Equal(1.24, measurement.SigmaPrGnd, 7);
        Assert.Equal(-6.35, measurement.B[0], 7);
        Assert.True(double.IsNaN(measurement.B[1]));
        Assert.Equal(0, measurement.B[2]);
        Assert.Equal(6.35, measurement.B[3], 7);
    }

    [Fact]
    public void Deserialize_Type2_PreservesCoordinatesAndEveryAdditionalBlock()
    {
        var copy = Read<MessageBlockType2>(2);
        Assert.Equal(55.75, copy.Latitude, 7);
        Assert.Equal(-37.5, copy.Longitude, 7);
        Assert.Equal(-123.45, copy.EllipsoidHeight, 7);
        Assert.Equal(-12.25, copy.LocalMagneticVariation, 7);
        Assert.Equal(397, copy.RefractivityIndex);
        Assert.Equal(8500, copy.ScaleHeight);
        Assert.Equal(4, copy.AdditionalData.Count);
        var first = Assert.IsType<AdditionalDataBlock1>(copy.AdditionalData[0]);
        Assert.Equal(17, first.RSDS);
        Assert.Equal(200u, first.DMax);
        Assert.Equal(4.5, first.Kmd_e_GLONASS, 7);
        var second = Assert.IsType<AdditionalDataBlock2>(copy.AdditionalData[1]);
        Assert.Equal(23456, second.ChannelNumber);
        Assert.Equal(-1.4, second.DeltaLatitude, 7);
        Assert.Equal(2.6, second.DeltaLongitude, 7);
        var third = Assert.IsType<AdditionalDataBlock3>(copy.AdditionalData[2]);
        Assert.Equal(1.7, third.Y_EIG, 7);
        Assert.Equal(0.4, third.M_EIG, 7);
        var fourth = Assert.IsType<AdditionalDataBlock4>(copy.AdditionalData[3]);
        Assert.Equal(new[] { SlotIdEnum.A, SlotIdEnum.D, SlotIdEnum.H }, fourth.SlotGroupDefinition);
    }

    [Fact]
    public void Deserialize_Type4_PreservesFasCoordinatesUnitsAndAlertLimits()
    {
        var fas = Assert.Single(Read<MessageBlockType4>(4).FasDataSet);
        Assert.Equal("UUEE", fas.AirportId);
        Assert.Equal("G24A", fas.ReferencePathIdentifier);
        Assert.Equal(24, fas.RunwayNumber);
        Assert.Equal(RunwayLetterEnum.L, fas.RunwayLetter);
        Assert.Equal(42, fas.RPDS);
        Assert.Equal(55.75, fas.LtpFtpLatitude, 7);
        Assert.Equal(-37.5, fas.LtpFtpLongitude, 7);
        Assert.Equal(-0.001, fas.DeltaFPapLatitude, 7);
        Assert.Equal(0.002, fas.DeltaFPapLongitude, 7);
        Assert.Equal(TCHUnitsEnum.M, fas.TCHUnits);
        Assert.Equal(15.25, fas.TCH, 7);
        Assert.Equal(3, fas.GlidePathAngle, 7);
        Assert.Equal(106.25, fas.CourseWidth, 7);
        Assert.Equal(80, fas.DeltaLengthOffset, 7);
        Assert.Equal(10, fas.FASVAL, 7);
        Assert.Equal(40, fas.FASLAL, 7);
    }

    [Fact]
    public void Deserialize_Type5_PreservesGlobalAndApproachSpecificAvailability()
    {
        var copy = Read<MessageBlockType5>(5);
        Assert.Equal(987.6, copy.ModifiedZCount, 7);
        var global = Assert.Single(copy.ImpactedSources);
        Assert.Equal(8, global.RangingSourceId);
        Assert.Equal(SourceAvailabilitySenseEnum.WillStart, global.SourceAvailabilitySense);
        Assert.Equal(1270, global.SourceAvailabilityDuration);
        var approach = Assert.Single(copy.ObstructedApproaches);
        Assert.Equal(42, approach.RPDS);
        var source = Assert.Single(approach.ImpactedSources);
        Assert.Equal(9, source.RangingSourceId);
        Assert.Equal(SourceAvailabilitySenseEnum.WillCease, source.SourceAvailabilitySense);
        Assert.Equal(30, source.SourceAvailabilityDuration);
    }

    [Fact]
    public void Deserialize_Type11_PreservesBoundaryCorrectionsAndBothSigmas()
    {
        var copy = Read<MessageBlockType11>(11);
        Assert.Equal(543.2, copy.ModifiedZCount, 7);
        Assert.Equal(AdditionalMessageEnum.SecondOfTwo, copy.AdditionalMessageFlag);
        var measurement = Assert.Single(copy.Measurements);
        Assert.Equal(12, measurement.RangingSourceId);
        Assert.Equal(-327.67, measurement.Prc30, 7);
        Assert.Equal(32.767, measurement.Rrc30, 7);
        Assert.Equal(1.2, measurement.SigmaPrGndD, 7);
        Assert.Equal(2.4, measurement.SigmaPrGndD30, 7);
    }

    [Fact]
    public void Deserialize_Type101_PreservesDifferentSigmaScaleAndOptionalB()
    {
        var copy = Read<MessageBlockType101>(101);
        Assert.Equal(1199.9, copy.ModifiedZCount, 7);
        Assert.Equal(0xBEEF, copy.EphemerisCrc);
        Assert.Equal((ushort)2540, copy.SourceAvailabilityDuration);
        var measurement = Assert.Single(copy.Measurements);
        Assert.Equal(327.67, measurement.Prc, 7);
        Assert.Equal(-32.767, measurement.Rrc, 7);
        Assert.Equal(50.8, measurement.SigmaPrGnd, 7);
        Assert.True(measurement.B.HasValue);
        Assert.True(double.IsNaN(measurement.B.Value));
    }

    [Fact]
    public void Deserialize_Type101WithoutB_ConsumesSevenByteMeasurement()
    {
        var origin = new MessageBlockType101 { GbasId = "UUEE", MessageBlockId = MessageBlockIdEnum.Normal };
        origin.Measurements.Add(new MeasurementBlockMsg101 { RangingSourceId = 3, Prc = -1.23, SigmaPrGnd = 2.4 });
        var bytes = GbasTestHelper.Serialize(origin);
        var copy = new MessageBlockType101();
        ReadOnlySpan<byte> read = bytes;
        copy.Deserialize(ref read);
        Assert.Empty(read.ToArray());
        Assert.Equal(25, bytes.Length);
        var measurement = Assert.Single(copy.Measurements);
        Assert.Null(measurement.B);
        Assert.Equal(-1.23, measurement.Prc, 7);
        Assert.Equal(2.4, measurement.SigmaPrGnd, 7);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(11)]
    [InlineData(101)]
    public void Deserialize_ReusedBlock_ReplacesCollections(byte type)
    {
        var copy = GbasMessageBlockFactory.CreateGbasMessageBlock(type);
        var expected = Convert.FromHexString(GbasLegacyFixtures.Block(type));
        for (var i = 0; i < 2; i++)
        {
            ReadOnlySpan<byte> read = expected;
            copy.Deserialize(ref read);
            Assert.Empty(read.ToArray());
            Assert.Equal(expected, GbasTestHelper.Serialize(copy));
        }
    }

    [Theory]
    [InlineData(6)]
    [InlineData(9)]
    [InlineData(12)]
    public void Deserialize_CorruptedBlock_RejectsCrc(int index)
    {
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Block(3));
        bytes[index] ^= 0x01;
        Assert.Throws<ProtocolParserCrcException>(() => DeserializeType3(bytes));
    }

    [Fact]
    public void Deserialize_InvalidBlockMarker_RejectsIdentifier()
    {
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Block(3));
        bytes[0] ^= 0x01;
        var error = Assert.Throws<FormatException>(() => DeserializeType3(bytes));
        Assert.Contains("identifier", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(9)]
    [InlineData(12)]
    public void Deserialize_TruncatedBlock_RejectsIncompleteData(int size)
    {
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Block(3));
        Assert.Throws<FormatException>(() => DeserializeType3(bytes[..size]));
    }

    [Fact]
    public void Deserialize_FasWithInvalidInnerCrc_RejectsData()
    {
        var fas = Assert.Single(((MessageBlockType4)GbasTestHelper.CreateBlock(4)).FasDataSet);
        var bytes = new byte[fas.GetByteSize()];
        var position = 0;
        fas.Serialize(bytes, ref position);
        bytes[10] ^= 0x20;
        Assert.Throws<FormatException>(() =>
        {
            var p = 0;
            new FasDataSet().Deserialize(bytes, ref p);
        });
    }

    [Fact]
    public void Deserialize_ValidCrcWithUnexpectedTrailingPayload_RejectsDeclaredLength()
    {
        var original = Convert.FromHexString(GbasLegacyFixtures.Block(11));
        var malformed = new byte[original.Length + 1];
        original.AsSpan(0, original.Length - 4).CopyTo(malformed);
        var position = 40;
        SpanBitHelper.SetBitUReverse(malformed, ref position, 8, (uint)malformed.Length);
        position = (malformed.Length - 4) * 8;
        var crc = GbasMessageBlockBase.Crc32QCalc(malformed, 0, position);
        SpanBitHelper.SetBitUReverse(malformed, ref position, 32, crc);
        var error = Assert.Throws<FormatException>(() =>
        {
            ReadOnlySpan<byte> read = malformed;
            new MessageBlockType11().Deserialize(ref read);
        });
        Assert.Contains("length", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("TOOLONG")]
    [InlineData("abcd")]
    [InlineData("АБВГ")]
    public void Serialize_UnsupportedStationIdentifier_RejectsValue(string identifier)
    {
        var block = GbasTestHelper.CreateBlock(3);
        block.GbasId = identifier;
        Assert.Throws<ArgumentException>(() => GbasTestHelper.Serialize(block));
    }

    private static T Read<T>(byte type) where T : GbasMessageBlockBase, new()
    {
        ReadOnlySpan<byte> read = Convert.FromHexString(GbasLegacyFixtures.Block(type));
        var result = new T();
        result.Deserialize(ref read);
        Assert.Empty(read.ToArray());
        return result;
    }

    private static void DeserializeType3(byte[] bytes)
    {
        ReadOnlySpan<byte> read = bytes;
        new MessageBlockType3().Deserialize(ref read);
    }
}
