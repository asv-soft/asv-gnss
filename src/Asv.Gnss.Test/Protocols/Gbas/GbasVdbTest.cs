using System;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class GbasVdbTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(11)]
    [InlineData(101)]
    public void Serialize_VdbBurst_MatchesLegacyScramblingAndFec(byte type)
    {
        var expected = Convert.FromHexString(GbasLegacyFixtures.Burst(type));
        Assert.Equal(expected, GbasTestHelper.Burst(GbasTestHelper.CreateBlock(type)));
        var application = GbasMessage.DecodeApplicationData(expected, out var slot);
        Assert.Equal(SlotIdEnum.C, slot);
        Assert.Equal(Convert.FromHexString(GbasLegacyFixtures.Block(type)), application);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public void Serialize_NonByteAlignedBurst_PreservesPrefixAndDecodes(int offset)
    {
        var original = new GbasMessage { SsId = SlotIdEnum.H };
        original.ApplicationData.Add(GbasTestHelper.CreateBlock(3));
        var buffer = new byte[(original.GetBitSize() + offset + 7) / 8];
        Array.Fill(buffer, (byte)0xFF);
        var writePosition = offset;
        original.Serialize(buffer, ref writePosition);
        Assert.Equal(original.GetBitSize() + offset, writePosition);
        Assert.Equal((byte)(0xFF << (8 - offset)), (byte)(buffer[0] & (0xFF << (8 - offset))));
        var copy = new GbasMessage();
        var readPosition = offset;
        copy.Deserialize(buffer, ref readPosition);
        Assert.Equal(writePosition, readPosition);
        Assert.Equal(SlotIdEnum.H, copy.SsId);
        Assert.Equal("UUEE", Assert.IsType<MessageBlockType3>(Assert.Single(copy.ApplicationData)).GbasId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void DecodeApplicationData_CorruptedSymbols_CorrectsUpToThreeSymbols(int count)
    {
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Burst(3));
        // Application symbols start at bit 73; perturb distinct symbols away from byte boundaries.
        for (var i = 0; i < count; i++) bytes[10 + i] ^= 0x20;
        var application = GbasMessage.DecodeApplicationData(bytes, out var slot);
        Assert.Equal(SlotIdEnum.C, slot);
        Assert.Equal(Convert.FromHexString(GbasLegacyFixtures.Block(3)), application);
    }

    [Fact]
    public void TrainingSequenceErrorCorrection_EachSingleBitError_RecoversSlotAndLength()
    {
        const byte slot = 5;
        const uint length = 632;
        var bits = new byte[4];
        var position = 0;
        SpanBitHelper.SetBitUReverse(bits, ref position, 3, slot);
        SpanBitHelper.SetBitUReverse(bits, ref position, 17, length);
        SpanBitHelper.SetBitUReverse(bits, ref position, 5, GbasMessage.GetTrainingSequenceFec(slot, length));
        for (var bit = 0; bit < 25; bit++)
        {
            var corrupted = (byte[])bits.Clone();
            corrupted[bit / 8] ^= (byte)(1 << (7 - bit % 8));
            Assert.True(GbasMessage.TrainingSequenceErrorCorrection(corrupted, 0, out var actualSlot,
                out var actualLength, out _, out var corrected));
            Assert.True(corrected);
            Assert.Equal(slot, actualSlot);
            Assert.Equal(length, actualLength);
        }
    }

    [Theory]
    [InlineData(40)]
    [InlineData(49)]
    [InlineData(1832)]
    public void GetFrameBitSize_ValidTrainingFecButInvalidLength_RejectsBurst(uint length)
    {
        var bytes = new byte[10];
        var position = 0;
        SpanBitHelper.SetBitUReverse(bytes, ref position, 24, (uint)(GbasMessage.Sync & 0xFFFFFF));
        SpanBitHelper.SetBitUReverse(bytes, ref position, 24, (uint)(GbasMessage.Sync >> 24));
        SpanBitHelper.SetBitUReverse(bytes, ref position, 3, 2);
        SpanBitHelper.SetBitUReverse(bytes, ref position, 17, length);
        SpanBitHelper.SetBitUReverse(bytes, ref position, 5, GbasMessage.GetTrainingSequenceFec(2, length));
        var start = 0;
        new GbasMessage().Scramble(bytes, ref start, 73);
        var error = Assert.Throws<FormatException>(() => GbasMessage.GetFrameBitSize(bytes));
        Assert.Contains("length", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DecodeApplicationData_EachTrainingBitError_RecoversBurstHeader()
    {
        var original = Convert.FromHexString(GbasLegacyFixtures.Burst(3));
        var expectedBits = GbasMessage.GetFrameBitSize(original);
        var expectedBlock = Convert.FromHexString(GbasLegacyFixtures.Block(3));
        for (var bit = 48; bit < 73; bit++)
        {
            var bytes = (byte[])original.Clone();
            bytes[bit / 8] ^= (byte)(1 << (7 - bit % 8));
            Assert.Equal(expectedBits, GbasMessage.GetFrameBitSize(bytes));
            var decoded = GbasMessage.DecodeApplicationData(bytes, out var slot);
            Assert.Equal(SlotIdEnum.C, slot);
            Assert.Equal(expectedBlock, decoded);
        }
    }

    [Fact]
    public void Serialize_MaximumApplicationSize_Encodes222BytesAndRejects223()
    {
        var block = new MessageBlockType3 { GbasId = "UUEE" };
        for (var i = 0; i < 212; i++) block.Filter.Add((byte)i);
        var bytes = GbasTestHelper.Burst(block);
        var application = GbasMessage.DecodeApplicationData(bytes, out var slot);
        Assert.Equal(SlotIdEnum.C, slot);
        Assert.Equal(222, application.Length);
        Assert.Equal(GbasTestHelper.Serialize(block), application);
        block.Filter.Add(0xAA);
        Assert.Throws<ArgumentException>(() => GbasTestHelper.Burst(block));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(9)]
    [InlineData(20)]
    [InlineData(28)]
    public void DecodeApplicationData_TruncatedBurst_RejectsIncompleteFrame(int length)
    {
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Burst(3));
        Assert.Throws<FormatException>(() => GbasMessage.DecodeApplicationData(bytes.AsSpan(0, length), out _));
    }

    [Fact]
    public void Deserialize_ReusedBurst_ReplacesApplicationData()
    {
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Burst(3));
        var burst = new GbasMessage();
        for (var i = 0; i < 2; i++)
        {
            var position = 0;
            burst.Deserialize(bytes, ref position);
            Assert.IsType<MessageBlockType3>(Assert.Single(burst.ApplicationData));
            Assert.Equal(burst.GetBitSize(), position);
        }
    }
}
