using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class UbxX20ProtocolTest
{
    [Fact]
    public void RegisterX20_All163Variants_DispatchesTheirExactTypedModels()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterUbxX20Protocol());
        using var parser = protocol.CreateParser(UbxProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var messageSubscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });

        foreach (var descriptor in UbxX20Protocol.Messages)
        {
            var frame = UbxTestHelper.Serialize(descriptor.Create());
            foreach (var value in frame)
            {
                parser.Push(value);
            }
        }

        Assert.Empty(errors);
        Assert.Equal(163, messages.Count);
        Assert.Equal(UbxX20Protocol.Messages.Select(message => message.MessageType), messages.Select(message => message.GetType()));
        Assert.Equal(UbxX20Protocol.Messages.Select(message => message.Variant),
            messages.Cast<UbxX20MessageBase>().Select(message => message.Variant));
    }

    [Fact]
    public void RegisterLegacy_ExistingIdsKeepTheirTypesAndNewIdsUseX20Models()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterUbxProtocol());
        using var parser = protocol.CreateParser(UbxProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var messageSubscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        byte[] stream =
        [
            .. Convert.FromHexString("B562050102000608163F"),
            .. UbxTestHelper.CreateFrame(0x06, 0x08, [0xE8, 0x03, 1, 0, 1, 0]),
            .. UbxTestHelper.CreateFrame(0x01, 0x61, [4, 3, 2, 1]),
        ];

        foreach (var value in stream)
        {
            parser.Push(value);
        }

        Assert.Empty(errors);
        Assert.Equal(3, messages.Count);
        Assert.Equal((byte)8, Assert.IsType<UbxAckAck>(messages[0]).AckSubclassId);
        Assert.IsType<UbxCfgRate>(messages[1]);
        Assert.Equal(0x01020304u, Assert.IsType<UbxX20NavEoeData0>(messages[2]).ITOW);
    }

    [Fact]
    public void RegisterLegacy_ConfigureRemove_ExcludesLegacyAndX20IdsWithoutFallback()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterUbxProtocol(factory =>
        {
            Assert.True(factory.Remove(0x0501));
            Assert.True(factory.Remove(0x0161));
        }));
        using var parser = protocol.CreateParser(UbxProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var messageSubscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        byte[] stream =
        [
            .. Convert.FromHexString("B562050102000608163F"),
            .. UbxTestHelper.CreateFrame(0x01, 0x61, [4, 3, 2, 1]),
            .. UbxTestHelper.CreateFrame(0x06, 0x08, [0xE8, 0x03, 1, 0, 1, 0]),
        ];

        foreach (var value in stream)
            parser.Push(value);

        Assert.IsType<UbxCfgRate>(Assert.Single(messages));
        Assert.Equal(2, errors.Count);
        Assert.All(errors, error => Assert.IsType<ProtocolParserUnknownMessageException>(error));
    }

    [Fact]
    public void RegisterLegacy_ConfigureClearAndCustomModel_PreservesTheExactConfiguredSet()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterUbxProtocol(factory =>
        {
            factory.Clear();
            factory.Add<CustomEoe>();
        }));
        using var parser = protocol.CreateParser(UbxProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var messageSubscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        byte[] stream =
        [
            .. Convert.FromHexString("B562050102000608163F"),
            .. UbxTestHelper.CreateFrame(0x01, 0x61, [4, 3, 2, 1]),
            .. UbxTestHelper.CreateFrame(0x01, 0x01, new byte[20]),
        ];

        foreach (var value in stream)
            parser.Push(value);

        Assert.Equal(0x01020304u, Assert.IsType<CustomEoe>(Assert.Single(messages)).Value);
        Assert.Equal(2, errors.Count);
        Assert.All(errors, error => Assert.IsType<ProtocolParserUnknownMessageException>(error));
    }

    [Fact]
    public void Push_TwoParsersShareOneFactory_KeepTheirVariantSelectionIndependent()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterUbxX20Protocol());
        using var first = protocol.CreateParser(UbxProtocol.Info.Id);
        using var second = protocol.CreateParser(UbxProtocol.Info.Id);
        var firstMessages = new List<IProtocolMessage>();
        var secondMessages = new List<IProtocolMessage>();
        using var firstSubscription = first.OnMessage.Subscribe(firstMessages.Add);
        using var secondSubscription = second.OnMessage.Subscribe(secondMessages.Add);
        var poll = UbxTestHelper.CreateFrame(0x0A, 0x04, []);
        var payload = new byte[40];
        Encoding.ASCII.GetBytes("X20 HPG 2.11").CopyTo(payload, 0);
        Encoding.ASCII.GetBytes("ZED-X20P").CopyTo(payload, 30);
        var response = UbxTestHelper.CreateFrame(0x0A, 0x04, payload);

        for (var index = 0; index < response.Length; index++)
        {
            if (index < poll.Length)
            {
                first.Push(poll[index]);
            }
            second.Push(response[index]);
        }

        Assert.IsType<UbxX20MonVerPoll0>(Assert.Single(firstMessages));
        var decoded = Assert.IsType<UbxX20MonVerData0>(Assert.Single(secondMessages));
        Assert.Equal(response, UbxTestHelper.Serialize(decoded));
        Assert.NotEqual(firstMessages[0].GetType(), secondMessages[0].GetType());
    }

    [Fact]
    public void Push_BadChecksumAndUnsupportedVersion_RejectBothAndRecoverAtNextFrame()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterUbxX20Protocol());
        using var parser = protocol.CreateParser(UbxProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var messageSubscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        var damaged = UbxTestHelper.CreateFrame(0x01, 0x61, [1, 2, 3, 4]);
        damaged[^1] ^= 0xFF;
        byte[] bytes =
        [
            .. damaged,
            .. UbxTestHelper.CreateFrame(0x27, 0x03, [0x7F, 0, 0, 0]),
            .. UbxTestHelper.CreateFrame(0x01, 0x61, [4, 3, 2, 1]),
        ];

        foreach (var value in bytes)
        {
            parser.Push(value);
        }

        Assert.Equal(0x01020304u, Assert.IsType<UbxX20NavEoeData0>(Assert.Single(messages)).ITOW);
        Assert.Equal(2, errors.Count);
        Assert.Contains(errors, error => error is ProtocolDeserializeMessageException);
        Assert.Contains(errors, error => error is ProtocolParserUnknownMessageException);
    }

    [Fact]
    public void Push_X20MaximumPayload_UsesBoundedBufferAndPreservesFollowingMessage()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterUbxX20Protocol());
        using var parser = protocol.CreateParser(UbxProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var messageSubscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        var payload = new byte[ushort.MaxValue];
        payload.AsSpan(12).Fill(0xA5);
        var maximum = UbxTestHelper.CreateFrame(0x13, 0x80, payload);
        var following = UbxTestHelper.CreateFrame(0x01, 0x61, [1, 0, 0, 0]);

        foreach (var value in maximum.Concat(following))
        {
            parser.Push(value);
        }

        Assert.Empty(errors);
        Assert.Equal(2, messages.Count);
        var database = Assert.IsType<UbxX20MgaDbdData0>(messages[0]);
        Assert.Equal(UbxX20Protocol.MaxPacketSize, database.GetByteSize());
        Assert.Equal(maximum, UbxTestHelper.Serialize(database));
        Assert.Equal(1u, Assert.IsType<UbxX20NavEoeData0>(messages[1]).ITOW);
        Assert.Equal(8192, UbxMessageParser.MaxPacketSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(17)]
    [InlineData(257)]
    public void Receive_MixedNmeaAndX20Frames_PreservesOrderThroughVirtualPort(int chunkSize)
    {
        var protocol = Protocol.Create(builder =>
        {
            builder.Protocols.RegisterUbxX20Protocol();
            builder.Protocols.RegisterNmeaProtocol();
        });
        using var port = new VirtualPort("x20-mixed", (IProtocolContext)protocol, static _ => true, new Statistic());
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var messageSubscription = port.OnRxMessage.Subscribe(messages.Add);
        using var errorSubscription = port.OnRxError.Subscribe(errors.Add);
        byte[] bytes =
        [
            .. UbxTestHelper.CreateFrame(0x01, 0x61, [4, 3, 2, 1]),
            .. Encoding.ASCII.GetBytes("$GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M,,*47\r\n"),
            .. Convert.FromHexString("B562050102000608163F"),
        ];

        for (var offset = 0; offset < bytes.Length; offset += chunkSize)
        {
            port.Rx.OnNext(bytes.AsSpan(offset, Math.Min(chunkSize, bytes.Length - offset)).ToArray());
        }

        Assert.Empty(errors);
        Assert.Equal(3, messages.Count);
        Assert.Equal(0x01020304u, Assert.IsType<UbxX20NavEoeData0>(messages[0]).ITOW);
        Assert.Equal(48.1173, Assert.IsType<NmeaMessageGga>(messages[1]).Latitude, 7);
        Assert.Equal((byte)6, Assert.IsType<UbxX20AckAckData0>(messages[2]).ClsID);
    }

    private sealed class CustomEoe : UbxMessageBase
    {
        public CustomEoe() { }
        public uint Value { get; private set; }
        public override byte Class => 0x01;
        public override byte SubClass => 0x61;
        public override string Name => "CUSTOM-EOE";
        protected override int GetContentByteSize() => 4;
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            Value = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
            buffer = buffer[4..];
        }
        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, Value);
            buffer = buffer[4..];
        }
        public override void Randomize(Random random) => Value = (uint)random.Next();
    }
}
