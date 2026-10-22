using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class ProtocolFileReaderTest
{
    private static readonly byte[] Ack = UbxTestHelper.CreateFrame(0x05, 0x01, [0x06, 0x08]);
    private static readonly byte[] Gga = Encoding.ASCII.GetBytes(
        "$GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M,,*47\r\n");

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(8192)]
    public async Task ReadAsync_NoiseAndMixedProtocolsAcrossChunks_PreservesMessageOrderAndValues(int chunkSize)
    {
        var asterix = ReadResource("Asterix", "cat002ed1.0.bin");
        var greis = Convert.FromHexString("7E7E30303504030201BC");
        byte[] bytes = [0xFF, 0xEE, 0x00, .. asterix, .. Ack, .. Gga, .. greis];
        using var stream = new ChunkedReadStream(bytes, chunkSize);

        var capture = await ProtocolFileReader.ReadAsync(stream, "mixed log.bin", TestContext.Current.CancellationToken);

        Assert.Equal("mixed log.bin", capture.FilePath);
        Assert.Equal(bytes.Length, capture.ByteCount);
        Assert.Equal(new long[] { 1, 2, 3, 4 }, capture.Messages.Select(message => message.Index));
        Assert.Equal(new[] { AsterixProtocol.Info.Id, UbxProtocol.Info.Id, NmeaProtocol.Info.Id, JavadProtocol.Info.Id },
            capture.Messages.Select(message => message.Message.Protocol.Id));
        var record = Assert.Single(Assert.IsType<AsterixMessageI002>(capture.Messages[0].Message));
        Assert.Equal(1, record.DataSourceIdentifier!.Sic);
        Assert.Equal(33501.4140625, record.TimeOfDay!.Seconds, 10);
        var ack = Assert.IsType<UbxAckAck>(capture.Messages[1].Message);
        Assert.Equal(0x06, ack.AckClassId);
        Assert.Equal(0x08, ack.AckSubclassId);
        var gga = Assert.IsType<NmeaMessageGga>(capture.Messages[2].Message);
        Assert.Equal(48.1173, gga.Latitude, 7);
        Assert.Equal(11.5166666667, gga.Longitude, 7);
        Assert.Equal(0x01020304U, Assert.IsType<JavadReceiverTimeMessage>(capture.Messages[3].Message).TimeOfDay);
        Assert.True(stream.CanRead);
    }

    [Fact]
    public async Task ReadAsync_EmptyStream_RegistersEveryImplementedStreamProtocol()
    {
        using var stream = new MemoryStream();

        var capture = await ProtocolFileReader.ReadAsync(stream, "empty.bin", TestContext.Current.CancellationToken);

        string[] expected =
        [
            NmeaProtocol.Info.Id, RtcmV2Protocol.Info.Id, RtcmV3Protocol.Info.Id, UbxProtocol.Info.Id,
            AsterixProtocol.Info.Id, AsvProtocol.Info.Id, GbasProtocol.Info.Id, SbfProtocol.Info.Id,
            ComNavProtocol.BinaryInfo.Id, ComNavProtocol.AsciiInfo.Id, ComNavProtocol.SimpleAnswerInfo.Id,
            JavadProtocol.Info.Id
        ];
        Assert.Equal(expected.Order(StringComparer.Ordinal), capture.Protocols);
        Assert.DoesNotContain(AdsbProtocol.Info.Id, capture.Protocols);
        Assert.Empty(capture.Messages);
        Assert.Equal(0, capture.ByteCount);
        Assert.Equal(0, capture.ErrorCount);
    }

    [Fact]
    public async Task ReadAsync_CorruptUbxThenValidAck_CollectsErrorAndContinues()
    {
        var damaged = (byte[])Ack.Clone();
        damaged[^1] ^= 0xFF;
        using var stream = new MemoryStream([.. damaged, .. Ack]);

        var capture = await ProtocolFileReader.ReadAsync(stream, "corrupt.ubx", TestContext.Current.CancellationToken);

        var ack = Assert.IsType<UbxAckAck>(Assert.Single(capture.Messages).Message);
        Assert.Equal(0x08, ack.AckSubclassId);
        Assert.True(capture.ErrorCount > 0);
    }

    [Fact]
    public async Task ReadAsync_UbxAckOnly_DoesNotInventAsterixMessageFromPayload()
    {
        using var stream = new MemoryStream(Ack);

        var capture = await ProtocolFileReader.ReadAsync(stream, "ack.ubx", TestContext.Current.CancellationToken);

        Assert.IsType<UbxAckAck>(Assert.Single(capture.Messages).Message);
        Assert.DoesNotContain(capture.Messages, message => message.Message.Protocol.Id == AsterixProtocol.Info.Id);
    }

    [Fact]
    public async Task ReadAsync_GreisReferenceFrame_DecodesIndependentChecksumVector()
    {
        using var stream = new MemoryStream(Convert.FromHexString("7E7E30303504030201BC"));

        var capture = await ProtocolFileReader.ReadAsync(stream, "reference.greis", TestContext.Current.CancellationToken);

        var message = Assert.IsType<JavadReceiverTimeMessage>(Assert.Single(capture.Messages).Message);
        Assert.Equal(0x01020304U, message.TimeOfDay);
    }

    [Fact]
    public async Task ReadAsync_ForeignProtocolBetweenGreisFrames_RetainsAnnouncedByteOrder()
    {
        byte[] bytes =
        [
            .. Convert.FromHexString("4D463030394A5030313030313842"),
            .. Ack,
            .. Convert.FromHexString("7E7E30303501020304FD")
        ];
        using var stream = new ChunkedReadStream(bytes, 1);

        var capture = await ProtocolFileReader.ReadAsync(stream, "mixed-big-endian.greis", TestContext.Current.CancellationToken);

        Assert.Equal(new[] { JavadProtocol.Info.Id, UbxProtocol.Info.Id, JavadProtocol.Info.Id },
            capture.Messages.Select(message => message.Message.Protocol.Id));
        var time = Assert.IsType<JavadReceiverTimeMessage>(capture.Messages[2].Message);
        Assert.Equal(0x01020304U, time.TimeOfDay);
        Assert.Equal(JavadByteOrder.BigEndian, time.ByteOrder);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(8192)]
    public async Task ReadAsync_GreisPayloadContainsUbx_DecodesInnerAndOuterMessages(int chunkSize)
    {
        var source = new JavadReceiverTimeMessage { TimeOfDay = 0x01020304, ExtensionData = Ack };
        using var stream = new ChunkedReadStream(Serialize(source), chunkSize);

        var capture = await ProtocolFileReader.ReadAsync(stream, "nested.ubx.greis", TestContext.Current.CancellationToken);

        Assert.Equal(new[] { UbxProtocol.Info.Id, JavadProtocol.Info.Id },
            capture.Messages.Select(message => message.Message.Protocol.Id));
        var ack = Assert.IsType<UbxAckAck>(capture.Messages[0].Message);
        Assert.Equal(0x08, ack.AckSubclassId);
        var time = Assert.IsType<JavadReceiverTimeMessage>(capture.Messages[1].Message);
        Assert.Equal(0x01020304U, time.TimeOfDay);
        Assert.Equal(Ack, time.ExtensionData);
    }

    [Fact]
    public async Task ReadAsync_CorruptGreisOuterContainsValidGreis_PublishesNestedFrameOnce()
    {
        var inner = Convert.FromHexString("7E7E30303504030201BC");
        var outer = Serialize(new JavadReceiverTimeMessage { TimeOfDay = 42, ExtensionData = inner });
        outer[^1] ^= 0xFF;
        using var stream = new ChunkedReadStream(outer, 1);

        var capture = await ProtocolFileReader.ReadAsync(stream, "corrupt-nested.greis", TestContext.Current.CancellationToken);

        var time = Assert.IsType<JavadReceiverTimeMessage>(Assert.Single(capture.Messages).Message);
        Assert.Equal(0x01020304U, time.TimeOfDay);
        Assert.Empty(time.ExtensionData);
        Assert.True(capture.ErrorCount > 0);
    }

    [Fact]
    public async Task ReadAsync_AllImplementedProtocolsInOneFile_DecodesEachInPhysicalOrder()
    {
        var references = OtherProtocolFrames().ToArray();
        var bytes = references.SelectMany(reference => (byte[])reference[0]).ToList();
        bytes.AddRange(ReadResource("Asterix", "cat002ed1.0.bin"));
        bytes.AddRange(Ack);
        bytes.AddRange(Gga);
        bytes.AddRange(Convert.FromHexString("7E7E30303504030201BC"));
        using var stream = new ChunkedReadStream(bytes.ToArray(), 11);

        var capture = await ProtocolFileReader.ReadAsync(stream, "all-protocols.bin", TestContext.Current.CancellationToken);

        var expected = references.Select(reference => (string)reference[1]).Concat(
            [AsterixProtocol.Info.Id, UbxProtocol.Info.Id, NmeaProtocol.Info.Id, JavadProtocol.Info.Id]);
        Assert.Equal(expected, capture.Messages.Select(message => message.Message.Protocol.Id));
        Assert.Equal(Enumerable.Range(1, 12).Select(index => (long)index), capture.Messages.Select(message => message.Index));
        Assert.Equal(bytes.Count, capture.ByteCount);
    }

    [Fact]
    public async Task ReadAsync_AsterixCrossesRingBoundaryAfterNoise_FindsCompleteRecord()
    {
        var bytes = Enumerable.Repeat((byte)0xFF, 65535)
            .Concat(ReadResource("Asterix", "cat002ed1.0.bin")).ToArray();
        using var stream = new ChunkedReadStream(bytes, 127);

        var capture = await ProtocolFileReader.ReadAsync(stream, "wrapped.ast", TestContext.Current.CancellationToken);

        var message = Assert.IsType<AsterixMessageI002>(Assert.Single(capture.Messages).Message);
        Assert.Equal(1, Assert.Single(message).MessageType!.MessageType);
        Assert.Equal(bytes.Length, capture.ByteCount);
    }

    [Fact]
    public async Task ReadAsync_TruncatedFrameAtEnd_ReturnsEarlierCompleteMessages()
    {
        using var stream = new MemoryStream([.. Ack, .. Ack[..7]]);

        var capture = await ProtocolFileReader.ReadAsync(stream, "truncated.ubx", TestContext.Current.CancellationToken);

        Assert.IsType<UbxAckAck>(Assert.Single(capture.Messages).Message);
        Assert.Equal(Ack.Length + 7, capture.ByteCount);
    }

    [Theory]
    [MemberData(nameof(OtherProtocolFrames))]
    public async Task ReadAsync_ProtocolReferenceFrame_UsesRegisteredCodec(
        byte[] frame, string protocolId, Type messageType)
    {
        using var stream = new ChunkedReadStream(frame, 3);

        var capture = await ProtocolFileReader.ReadAsync(stream, "reference.bin", TestContext.Current.CancellationToken);

        var message = Assert.Single(capture.Messages, item => item.Message.Protocol.Id == protocolId);
        Assert.Equal(messageType, message.Message.GetType());
        Assert.Equal(frame.Length, capture.ByteCount);
    }

    public static IEnumerable<object[]> OtherProtocolFrames()
    {
        yield return [Convert.FromHexString("664142404E4C5A4C42686743684B52405055555B"),
            RtcmV2Protocol.Info.Id, typeof(RtcmV2Message1)];
        yield return [Serialize(new RtcmV3Message1005()), RtcmV3Protocol.Info.Id, typeof(RtcmV3Message1005)];
        yield return [Convert.FromHexString("AA44070000000201000002000100000000F71B"),
            AsvProtocol.Info.Id, typeof(AsvMessageHeartBeat)];
        yield return [Convert.FromHexString(GbasLegacyFixtures.Burst(3)), GbasProtocol.Info.Id, typeof(MessageBlockType3)];
        yield return [Convert.FromHexString("24400151F20F1C00B8DD5316B30805000B0A0102150A1F0700020000"),
            SbfProtocol.Info.Id, typeof(SbfPacketQualityInd)];
        var hex = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Resources", "ComNav", "novatel-bestposb.hex"))
            .Where(line => !line.StartsWith('#') && !string.IsNullOrWhiteSpace(line));
        yield return [Convert.FromHexString(string.Concat(hex.SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)))),
            ComNavProtocol.BinaryInfo.Id, typeof(ComNavBinaryBestPosPacket)];
        var ascii = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Resources", "ComNav", "novatel-oem7-psrposa.txt"))
            .Single(line => line.StartsWith("#PSRPOSA", StringComparison.Ordinal));
        yield return [Encoding.ASCII.GetBytes(ascii + "\r\n"), ComNavProtocol.AsciiInfo.Id, typeof(ComNavPsrPosAMessage)];
        yield return [Encoding.ASCII.GetBytes("OK! command accepted\r\n"),
            ComNavProtocol.SimpleAnswerInfo.Id, typeof(ComNavSimpleOkMessage)];
    }

    [Fact]
    public async Task ReadAsync_FalseLongAsterixCandidate_DoesNotHideFollowingCompleteRecord()
    {
        using var stream = new MemoryStream([0x02, 0xFF, 0xFF, 0xFE, .. ReadResource("Asterix", "cat002ed1.0.bin")]);

        var capture = await ProtocolFileReader.ReadAsync(stream, "false-candidate.ast", TestContext.Current.CancellationToken);

        var message = Assert.IsType<AsterixMessageI002>(Assert.Single(capture.Messages).Message);
        Assert.Equal(33501.4140625, Assert.Single(message).TimeOfDay!.Seconds, 10);
    }

    [Fact]
    public async Task ReadAsync_PathWithSpaces_ReadsAndClosesFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"gnss capture {Guid.NewGuid():N}.bin");
        try
        {
            await File.WriteAllBytesAsync(path, Ack, TestContext.Current.CancellationToken);

            var capture = await ProtocolFileReader.ReadAsync(path, TestContext.Current.CancellationToken);

            Assert.Equal(path, capture.FilePath);
            Assert.IsType<UbxAckAck>(Assert.Single(capture.Messages).Message);
            using var exclusive = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ReadAsync_MissingFile_ReportsFileNotFound()
    {
        var path = Path.Combine(Path.GetTempPath(), $"gnss-missing-{Guid.NewGuid():N}.bin");

        await Assert.ThrowsAsync<FileNotFoundException>(() => ProtocolFileReader.ReadAsync(path, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ReadAsync_PreCanceledToken_StopsBeforeReading()
    {
        using var stream = new ChunkedReadStream(Ack, 1);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            ProtocolFileReader.ReadAsync(stream, "cancel.bin", new CancellationToken(true)));

        Assert.Equal(0, stream.ReadCount);
    }

    [Fact]
    public async Task ReadAsync_CancellationDuringReading_StopsAndLeavesCallerStreamOpen()
    {
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        using var stream = new ChunkedReadStream(Ack, 1, cancellation.Cancel);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            ProtocolFileReader.ReadAsync(stream, "cancel.bin", cancellation.Token));

        Assert.Equal(1, stream.ReadCount);
        Assert.True(stream.CanRead);
    }

    private static byte[] ReadResource(string directory, string file) =>
        File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", directory, file));

    private static byte[] Serialize(IProtocolMessage message)
    {
        var bytes = new byte[message.GetByteSize()];
        var span = bytes.AsSpan();
        message.Serialize(ref span);
        Assert.True(span.IsEmpty);
        return bytes;
    }

    private sealed class ChunkedReadStream(byte[] bytes, int chunkSize, Action? onRead = null) : Stream
    {
        private int _position;
        public int ReadCount { get; private set; }
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ReadCount++;
            var count = Math.Min(Math.Min(chunkSize, buffer.Length), bytes.Length - _position);
            bytes.AsMemory(_position, count).CopyTo(buffer);
            _position += count;
            onRead?.Invoke();
            return ValueTask.FromResult(count);
        }

        public override void Flush() => throw new NotSupportedException();
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
