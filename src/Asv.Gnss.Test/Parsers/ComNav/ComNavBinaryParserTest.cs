using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class ComNavBinaryParserTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(27)]
    public void Push_InvalidHeaderLengthThenValidFrame_ShouldDiscardHeaderAndRecover(byte headerLength)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);

        Assert.Equal(0, Push(parser, [0xAA, 0x44, 0x12, headerLength]));
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, CreateFrame([1, 2, 3])));

        Assert.Equal(new byte[] { 1, 2, 3 }, Assert.IsType<PayloadMessage>(Assert.Single(messages)).Payload);
    }

    [Theory]
    [InlineData(10209)]
    [InlineData(65535)]
    public void Push_OversizedPayloadThenValidFrame_ShouldDiscardHeaderAndRecover(ushort payloadLength)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var header = CreateFrame([]).AsSpan(0, 28).ToArray();
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(8), payloadLength);

        Assert.Equal(0, Push(parser, header));
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, CreateFrame([4, 5])));

        Assert.Equal(new byte[] { 4, 5 }, Assert.IsType<PayloadMessage>(Assert.Single(messages)).Payload);
    }

    [Fact]
    public void Push_MaximumFrameThenValidFrame_ShouldPublishBothPayloads()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var payload = new byte[ComNavBinaryParser.MaxPacketSize - 28 - 4];
        Array.Fill(payload, (byte)0x5A);
        var frame = CreateFrame(payload);

        Assert.Equal(ComNavBinaryParser.MaxPacketSize, frame.Length);
        Assert.Equal(1, Push(parser, frame));
        Assert.Equal(1, Push(parser, CreateFrame([0x11])));

        Assert.Equal(2, messages.Count);
        Assert.Equal(payload, Assert.IsType<PayloadMessage>(messages[0]).Payload);
        Assert.Equal(new byte[] { 0x11 }, Assert.IsType<PayloadMessage>(messages[1]).Payload);
    }

    [Fact]
    public void Push_ExtendedHeader_ShouldReadPayloadAfterAppendedFields()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);

        Assert.Equal(1, Push(parser, CreateFrame([7, 8, 9], 32)));

        Assert.Equal(new byte[] { 7, 8, 9 }, Assert.IsType<PayloadMessage>(Assert.Single(messages)).Payload);
    }

    [Fact]
    public void Push_BadCrcThenValidFrame_ShouldPublishOnlyValidPayload()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var corrupted = CreateFrame([1]);
        corrupted[^1] ^= 0xFF;

        Assert.Equal(0, Push(parser, corrupted));
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, CreateFrame([2])));

        Assert.Equal(new byte[] { 2 }, Assert.IsType<PayloadMessage>(Assert.Single(messages)).Payload);
    }

    [Fact]
    public void Reset_TruncatedFrame_ShouldAllowNextValidPayload()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Assert.Equal(0, Push(parser, CreateFrame([1, 2]).AsSpan(0, 29)));
        Assert.Empty(messages);

        parser.Reset();

        Assert.Equal(1, Push(parser, CreateFrame([3, 4])));
        Assert.Equal(new byte[] { 3, 4 }, Assert.IsType<PayloadMessage>(Assert.Single(messages)).Payload);
    }

    private static IProtocolParser CreateParser()
    {
        return Protocol.Create(builder => builder.Protocols.RegisterComNavBinaryProtocol(factory =>
        {
            factory.Clear();
            factory.Add<PayloadMessage>();
        })).CreateParser(ComNavProtocol.BinaryInfo.Id);
    }

    private static byte[] CreateFrame(ReadOnlySpan<byte> payload, byte headerLength = 28)
    {
        var frame = new byte[headerLength + payload.Length + 4];
        frame[0] = 0xAA;
        frame[1] = 0x44;
        frame[2] = 0x12;
        frame[3] = headerLength;
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(4), PayloadMessage.PacketId);
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(8), checked((ushort)payload.Length));
        payload.CopyTo(frame.AsSpan(headerLength));
        uint crc = 0;
        foreach (var data in frame.AsSpan(0, frame.Length - 4))
        {
            crc ^= data;
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
            }
        }
        BinaryPrimitives.WriteUInt32LittleEndian(frame.AsSpan(frame.Length - 4), crc);
        return frame;
    }

    private static int Push(IProtocolParser parser, ReadOnlySpan<byte> frame)
    {
        var completeFrames = 0;
        foreach (var data in frame)
        {
            if (parser.Push(data))
            {
                completeFrames++;
            }
        }
        return completeFrames;
    }

    public sealed class PayloadMessage : ComNavBinaryMessageBase
    {
        public const ushort PacketId = 65000;
        public override ushort MessageId => PacketId;
        public override string Name => "Parser test payload";
        public byte[] Payload { get; private set; } = [];

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            Payload = buffer[..MessageLength].ToArray();
            buffer = buffer[MessageLength..];
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer) => throw new NotSupportedException();
        protected override int InternalGetContentByteSize() => Payload.Length;
    }
}
