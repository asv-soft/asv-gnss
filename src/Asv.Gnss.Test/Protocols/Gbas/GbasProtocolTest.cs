using System;
using System.Collections.Generic;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class GbasProtocolTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(11)]
    [InlineData(101)]
    public void Push_LegacyVdbBurst_PublishesRegisteredTypedBlock(byte type)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Burst(type));
        Assert.Equal(1, Push(parser, bytes));
        var actual = Assert.IsAssignableFrom<GbasMessageBlockBase>(Assert.Single(messages));
        Assert.Equal(type, actual.MessageTypeId);
        Assert.Equal(GbasTestHelper.CreateBlock(type).GetType(), actual.GetType());
        Assert.Equal(Convert.FromHexString(GbasLegacyFixtures.Block(type)), GbasTestHelper.Serialize(actual));
    }

    [Fact]
    public void Push_FragmentedConcatenatedBursts_PublishesBlocksInOrder()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var first = Convert.FromHexString(GbasLegacyFixtures.Burst(3));
        Assert.Equal(0, Push(parser, first.AsSpan(0, 10)));
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, first.AsSpan(10)));
        Assert.Equal(2, Push(parser, [.. Convert.FromHexString(GbasLegacyFixtures.Burst(11)), .. first]));
        Assert.Equal(3, messages.Count);
        Assert.IsType<MessageBlockType3>(messages[0]);
        Assert.IsType<MessageBlockType11>(messages[1]);
        Assert.IsType<MessageBlockType3>(messages[2]);
    }

    [Fact]
    public void Push_BurstContainingSeveralBlocks_PublishesEveryBlock()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var bytes = GbasTestHelper.Burst(GbasTestHelper.CreateBlock(3), GbasTestHelper.CreateBlock(1));
        Assert.Equal(1, Push(parser, bytes));
        Assert.Equal(2, messages.Count);
        Assert.Equal(new byte[] { 0xAA, 0x55, 0x01 }, Assert.IsType<MessageBlockType3>(messages[0]).Filter);
        Assert.Equal(-123.45, Assert.Single(Assert.IsType<MessageBlockType1>(messages[1]).Measurements).Prc, 7);
    }

    [Fact]
    public void Push_NoiseAndPartialSync_PreservesFollowingValidBurst()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Assert.Equal(0, Push(parser, [0xFF, 0x00, 0x09, 0xE0, 0x09]));
        Assert.Equal(1, Push(parser, Convert.FromHexString(GbasLegacyFixtures.Burst(3))));
        Assert.Equal("UUEE", Assert.IsType<MessageBlockType3>(Assert.Single(messages)).GbasId);
    }

    [Fact]
    public void Reset_TruncatedBurst_ParsesNextValidBurst()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Burst(3));
        Assert.Equal(0, Push(parser, bytes.AsSpan(0, 14)));
        parser.Reset();
        Assert.Equal(1, Push(parser, bytes));
        Assert.IsType<MessageBlockType3>(Assert.Single(messages));
    }

    [Fact]
    public void Push_UncorrectableBurst_ReportsErrorAndRecovers()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        var bytes = Convert.FromHexString(GbasLegacyFixtures.Burst(3));
        for (var i = 10; i < 18; i++) bytes[i] ^= 0xFF;
        Assert.Equal(0, Push(parser, bytes));
        Assert.Empty(messages);
        Assert.NotEmpty(errors);
        Assert.Equal(1, Push(parser, Convert.FromHexString(GbasLegacyFixtures.Burst(11))));
        Assert.IsType<MessageBlockType11>(Assert.Single(messages));
    }

    [Fact]
    public void Register_CustomFactory_PublishesOnlySelectedType()
    {
        using var parser = Protocol.Create(builder => builder.Protocols.RegisterGbasProtocol(factory =>
        {
            factory.Clear();
            factory.Add<MessageBlockType3>();
        })).CreateParser(GbasProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Push(parser, GbasTestHelper.Burst(GbasTestHelper.CreateBlock(11), GbasTestHelper.CreateBlock(3)));
        Assert.IsType<MessageBlockType3>(Assert.Single(messages));
    }

    [Fact]
    public void Push_AsvVdbEnvelope_DecodesContainedGbasCorrections()
    {
        var origin = new AsvMessageGbasVdbSendV2
        {
            Slot = AsvGbasSlotMsg.SlotC,
            Data = Convert.FromHexString(GbasLegacyFixtures.Burst(1)),
        };
        var envelope = new byte[origin.GetByteSize()];
        var write = envelope.AsSpan();
        origin.Serialize(ref write);
        ReadOnlySpan<byte> read = envelope;
        var copy = new AsvMessageGbasVdbSendV2();
        copy.Deserialize(ref read);
        Assert.Empty(read.ToArray());
        Assert.Equal(AsvGbasSlotMsg.SlotC, copy.Slot);
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Assert.Equal(1, Push(parser, copy.Data));
        var block = Assert.IsType<MessageBlockType1>(Assert.Single(messages));
        Assert.Equal("UUEE", block.GbasId);
        Assert.Equal(-123.45, Assert.Single(block.Measurements).Prc, 7);
    }

    private static IProtocolParser CreateParser() =>
        Protocol.Create(builder => builder.Protocols.RegisterGbasProtocol()).CreateParser(GbasProtocol.Info.Id);

    private static int Push(IProtocolParser parser, ReadOnlySpan<byte> bytes)
    {
        var count = 0;
        foreach (var value in bytes) if (parser.Push(value)) count++;
        return count;
    }
}
