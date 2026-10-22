using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Asv.Gnss;

public class JavadDtpTest
{
    // Independent Appendix A.2 vectors: four-byte payloads, with CRC carried across blocks.
    private const string First = "02000061626364D5C803";
    private const string Second = "02010065666768019403";
    private const string Last = "040200494A0000E51503";

    [Fact]
    public void Codec_UsesLittleEndianSequenceAndCumulativeChecksum()
    {
        var first = JavadDtpCodec.Encode(
            JavadDtpBlock.Ordinal(0, "abcd"u8.ToArray()),
            4,
            0,
            out var crc
        );
        Assert.Equal(First, Convert.ToHexString(first));
        var second = JavadDtpCodec.Encode(
            JavadDtpBlock.Ordinal(1, "efgh"u8.ToArray()),
            4,
            crc,
            out crc
        );
        Assert.Equal(Second, Convert.ToHexString(second));
        var last = JavadDtpCodec.Encode(JavadDtpBlock.Last("IJ"u8.ToArray()), 4, crc, out crc);
        Assert.Equal(Last, Convert.ToHexString(last));
        Assert.NotEqual(
            JavadDtpCodec.ContinueChecksum(0, second.AsSpan(0, 7)),
            BitConverter.ToUInt16(second, 7)
        );
        ReadOnlySpan<byte> input = first.Concat(new byte[] { 0xFE }).ToArray();
        var decoded = JavadDtpCodec.Decode(ref input, 4, 0, out var decodedCrc);
        Assert.Equal("abcd"u8.ToArray(), decoded.Data);
        Assert.Equal(new byte[] { 0xFE }, input.ToArray());
        Assert.Equal(0xC8D5, decodedCrc);
    }

    [Fact]
    public void Receiver_FragmentationRetryAndDuplicatesDoNotAdvanceChecksumTwice()
    {
        var receiver = new JavadDtpReceiver(4);
        Assert.Equal(JavadDtpCodec.NegativeAcknowledge, receiver.InitialRequest);
        var first = Convert.FromHexString(First);
        foreach (var value in first[..5])
            Assert.Null(receiver.Push(value));
        Assert.Equal(JavadDtpCodec.NegativeAcknowledge, receiver.RequestRetry());
        var corrupted = (byte[])first.Clone();
        corrupted[4] ^= 1;
        var rejected = Feed(receiver, corrupted);
        Assert.Null(rejected.Block);
        Assert.Equal(JavadDtpCodec.NegativeAcknowledge, rejected.Request);
        Assert.Equal(0, receiver.Checksum);
        var accepted = Feed(receiver, first);
        Assert.Equal(JavadDtpCodec.Acknowledge, accepted.Request);
        Assert.Equal("abcd"u8.ToArray(), accepted.Block!.Data);
        var crc = receiver.Checksum;
        var duplicate = Feed(receiver, first);
        Assert.True(duplicate.IsDuplicate);
        Assert.Null(duplicate.Request);
        Assert.Equal(crc, receiver.Checksum);
        Feed(receiver, Convert.FromHexString(Second));
        Assert.True(Feed(receiver, first).IsDuplicate);
        var last = Feed(receiver, Convert.FromHexString(Last));
        Assert.True(last.IsComplete);
        Assert.Null(last.Request);
        Assert.Equal("IJ"u8.ToArray(), last.Block!.Data);
        Assert.True(receiver.IsComplete);
    }

    [Fact]
    public void Transmitter_RetransmitsIdenticalBytesAndAdvancesOnlyOnAck()
    {
        var sender = new JavadDtpTransmitter(4);
        Assert.Equal(
            JavadDtpRequestAction.PrepareNextBlock,
            sender.ProcessRequest(JavadDtpCodec.NegativeAcknowledge)
        );
        Assert.Equal(First, Convert.ToHexString(sender.Prepare("abcd"u8).Span));
        Assert.Equal(
            JavadDtpRequestAction.SendCurrentBlock,
            sender.ProcessRequest(JavadDtpCodec.NegativeAcknowledge)
        );
        Assert.Equal(First, Convert.ToHexString(sender.CurrentBlock.Span));
        Assert.Equal(0, sender.Checksum);
        Assert.Equal(
            JavadDtpRequestAction.PrepareNextBlock,
            sender.ProcessRequest(JavadDtpCodec.Acknowledge)
        );
        Assert.Equal(1, sender.NextBlockNumber);
        Assert.Equal(Second, Convert.ToHexString(sender.Prepare("efgh"u8).Span));
        sender.ProcessRequest(JavadDtpCodec.Acknowledge);
        Assert.Equal(Last, Convert.ToHexString(sender.Prepare("IJ"u8, true).Span));
        Assert.True(sender.IsLastBlock);
        Assert.Equal(
            JavadDtpRequestAction.SendCurrentBlock,
            sender.ProcessRequest(JavadDtpCodec.NegativeAcknowledge)
        );
        Assert.Equal(
            JavadDtpRequestAction.Finished,
            sender.ProcessRequest(JavadDtpCodec.Acknowledge)
        );
        Assert.Equal(
            JavadDtpRequestAction.Ignore,
            sender.ProcessRequest(JavadDtpCodec.NegativeAcknowledge)
        );
    }

    [Fact]
    public void LastBlocks_RepresentEmptyFilesErrorsAndAbortWithoutInventingData()
    {
        var empty = JavadDtpCodec.Encode(JavadDtpBlock.Last([]), 4, 0, out _);
        Assert.Equal("04000000000000C6C103", Convert.ToHexString(empty));
        Assert.Empty(Feed(new(4), empty).Block!.Data);
        var error = JavadDtpCodec.Encode(JavadDtpBlock.Failed("bad"), 4, 0, out _);
        Assert.Equal("04FFFF62616400D91003", Convert.ToHexString(error));
        var failed = Feed(new(4), error);
        Assert.True(failed.IsComplete);
        Assert.Equal("bad", failed.Block!.Error);
        Assert.Empty(failed.Block.Data);
        var abort = Feed(new(4), [(byte)'#']);
        Assert.True(abort.IsComplete);
        Assert.Equal(JavadDtpBlockType.Abort, abort.Block!.Type);
        var sender = new JavadDtpTransmitter(4);
        Assert.Equal(JavadDtpRequestAction.Aborted, sender.ProcessRequest((byte)'!'));
        Assert.True(sender.IsAborted);
    }

    [Fact]
    public void OutOfSequenceValidBlock_AbortsInsteadOfAcceptingLostData()
    {
        var bytes = JavadDtpCodec.Encode(JavadDtpBlock.Ordinal(2, "abcd"u8.ToArray()), 4, 0, out _);
        var receiver = new JavadDtpReceiver(4);
        var result = Feed(receiver, bytes);
        Assert.Equal((byte)'#', result.Request);
        Assert.True(result.IsComplete);
        Assert.Null(result.Block);
        Assert.Equal(0, receiver.Checksum);
    }

    [Fact]
    public void Codec_RejectsTruncationCorruptChecksumAndPaddingWithoutConsumingInput()
    {
        var frame = Convert.FromHexString(First);
        Assert.Throws<FormatException>(() => ReadInvalid(frame[..^1]));
        frame[4] ^= 1;
        Assert.Throws<FormatException>(() => ReadInvalid(frame));
        var nonzeroPadding = new byte[] { 4, 1, 0, (byte)'a', 1, 0, 0, 0, 0, 3 };
        var crc = JavadDtpCodec.ContinueChecksum(0, nonzeroPadding.AsSpan(0, 7));
        nonzeroPadding[7] = (byte)crc;
        nonzeroPadding[8] = (byte)(crc >> 8);
        Assert.Throws<FormatException>(() => ReadInvalid(nonzeroPadding));
        Assert.Throws<ArgumentException>(() =>
            JavadDtpCodec.Encode(JavadDtpBlock.Ordinal(0, [1]), 4, 0, out _)
        );
        Assert.Throws<ArgumentOutOfRangeException>(() => new JavadDtpReceiver(16385));
    }

    [Fact]
    public void SenderAndReceiver_WrapUnsignedSequenceNumbersWithoutLosingCrcState()
    {
        var sender = new JavadDtpTransmitter(1);
        var receiver = new JavadDtpReceiver(1);
        for (var i = 0; i < 65537; i++)
        {
            var frame = sender.Prepare([(byte)i]);
            var received = Feed(receiver, frame.ToArray());
            Assert.Equal(JavadDtpCodec.Acknowledge, received.Request);
            Assert.Equal(
                JavadDtpRequestAction.PrepareNextBlock,
                sender.ProcessRequest(received.Request!.Value)
            );
        }
        Assert.Equal(1, sender.NextBlockNumber);
        Assert.Equal(1, receiver.ExpectedBlockNumber);
        Assert.Equal(sender.Checksum, receiver.Checksum);
    }

    [Fact]
    public void SenderAndReceiver_FinishAfterLastWriteWithoutWaitingForFinalAck()
    {
        var sender = new JavadDtpTransmitter(4);
        var receiver = new JavadDtpReceiver(4);
        Assert.Equal(
            JavadDtpRequestAction.PrepareNextBlock,
            sender.ProcessRequest(receiver.InitialRequest)
        );
        var first = sender.Prepare("abcd"u8);
        var result = Feed(receiver, first.ToArray());
        sender.MarkCurrentBlockSent();
        Assert.False(sender.IsComplete);
        Assert.Equal(
            JavadDtpRequestAction.PrepareNextBlock,
            sender.ProcessRequest(result.Request!.Value)
        );
        var last = sender.Prepare("e"u8, true);
        result = Feed(receiver, last.ToArray());
        sender.MarkCurrentBlockSent();
        Assert.Null(result.Request);
        Assert.True(result.IsComplete);
        Assert.True(sender.IsComplete);
        Assert.Equal(sender.Checksum, receiver.Checksum);
        Assert.Equal(JavadDtpRequestAction.Ignore, sender.ProcessRequest(0));
    }

    private static JavadDtpReceiveResult Feed(JavadDtpReceiver receiver, byte[] data)
    {
        var results = new List<JavadDtpReceiveResult>();
        foreach (var value in data)
            if (receiver.Push(value) is { } result)
                results.Add(result);
        return Assert.Single(results);
    }

    private static void ReadInvalid(byte[] bytes)
    {
        ReadOnlySpan<byte> buffer = bytes;
        try
        {
            JavadDtpCodec.Decode(ref buffer, 4, 0, out _);
        }
        catch
        {
            Assert.Equal(bytes.Length, buffer.Length);
            throw;
        }
    }
}
