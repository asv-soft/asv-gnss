using System;
using System.Collections.Generic;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class UbxMessageParserTest
{
    [Fact]
    public void Push_ZeroLengthPollThenAck_ShouldPublishBothMessages()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);

        Assert.Equal(1, Push(parser, UbxTestHelper.CreateFrame(0x0A, 0x04, [])));
        Assert.Equal(1, Push(parser, AckFrame(0x06, 0x01)));

        Assert.Equal(2, messages.Count);
        Assert.IsType<UbxMonVerPool>(messages[0]);
        AssertAck(messages[1], 0x06, 0x01);
    }

    [Fact]
    public void Push_MaximumFrameThenAck_ShouldPublishBothMessages()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var payload = new byte[UbxMessageParser.MaxPacketSize - 8];
        Array.Fill(payload, (byte)'A');
        var frame = UbxTestHelper.CreateFrame(0x04, 0x04, payload);

        Assert.Equal(UbxMessageParser.MaxPacketSize, frame.Length);
        Assert.Equal(1, Push(parser, frame));
        Assert.Equal(1, Push(parser, AckFrame(0x06, 0x08)));

        Assert.Equal(2, messages.Count);
        Assert.Equal(new string('A', payload.Length), Assert.IsType<UbxInfDebug>(messages[0]).Message);
        AssertAck(messages[1], 0x06, 0x08);
    }

    [Theory]
    [InlineData(8185)]
    [InlineData(8186)]
    [InlineData(8187)]
    [InlineData(8188)]
    [InlineData(8189)]
    [InlineData(8190)]
    [InlineData(8191)]
    [InlineData(8192)]
    [InlineData(65535)]
    public void Push_OversizedFrameThenAck_ShouldDiscardOversizedFrameAndRecover(int payloadLength)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var frame = UbxTestHelper.CreateFrame(0x04, 0x04, new byte[payloadLength]);

        Assert.Equal(0, Push(parser, frame));
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, AckFrame(0x06, 0x24)));

        AssertAck(Assert.Single(messages), 0x06, 0x24);
    }

    [Fact]
    public void Push_FragmentedAndConcatenatedFrames_ShouldPublishCompleteAcksInOrder()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var first = AckFrame(0x06, 0x01);

        Assert.Equal(0, Push(parser, first.AsSpan(0, 7)));
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, first.AsSpan(7)));
        AssertAck(Assert.Single(messages), 0x06, 0x01);
        Assert.Equal(2, Push(parser, [.. AckFrame(0x06, 0x08), .. AckFrame(0x06, 0x24)]));

        Assert.Equal(3, messages.Count);
        AssertAck(messages[1], 0x06, 0x08);
        AssertAck(messages[2], 0x06, 0x24);
    }

    [Fact]
    public void Push_BadCrcThenValidAck_ShouldPublishOnlyValidAck()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var corrupted = AckFrame(0x06, 0x01);
        corrupted[^1] ^= 0xFF;

        Push(parser, corrupted);
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, AckFrame(0x06, 0x08)));

        AssertAck(Assert.Single(messages), 0x06, 0x08);
    }

    [Fact]
    public void Reset_TruncatedFrame_ShouldAllowNextValidAck()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Assert.Equal(0, Push(parser, AckFrame(0x06, 0x01).AsSpan(0, 7)));
        Assert.Empty(messages);

        parser.Reset();

        Assert.Equal(1, Push(parser, AckFrame(0x06, 0x24)));
        AssertAck(Assert.Single(messages), 0x06, 0x24);
    }

    private static IProtocolParser CreateParser()
    {
        return Protocol.Create(builder => builder.Protocols.RegisterUbxProtocol(factory =>
        {
            factory.Clear();
            factory.Add<UbxAckAck>().Add<UbxMonVerPool>().Add<UbxInfDebug>();
        })).CreateParser(UbxProtocol.Info.Id);
    }

    private static byte[] AckFrame(byte messageClass, byte messageId) =>
        UbxTestHelper.CreateFrame(0x05, 0x01, [messageClass, messageId]);

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

    private static void AssertAck(IProtocolMessage message, byte messageClass, byte messageId)
    {
        var ack = Assert.IsType<UbxAckAck>(message);
        Assert.Equal(messageClass, ack.AckClassId);
        Assert.Equal(messageId, ack.AckSubclassId);
    }
}
