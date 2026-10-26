using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class GnssProtocolMicroserviceClientTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task Nmea_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new NmeaMessageGga { Latitude = 12.5 };
        return AssertRouting<NmeaMessageBase, NmeaMessageGga>((context, id) => new NmeaMicroserviceClient(context, id),
            () => new NmeaMessageGga(), message, new NmeaMessageGll(), received => { Assert.Equal(12.5, received.Latitude); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task RtcmV2_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new RtcmV2Message14 { LeapSeconds = 18 };
        return AssertRouting<RtcmV2MessageBase, RtcmV2Message14>((context, id) => new RtcmV2MicroserviceClient(context, id),
            () => new RtcmV2Message14(), message, new RtcmV2Message1(), received => { Assert.Equal(18, received.LeapSeconds); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task RtcmV3_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new RtcmV3Message1005 { Latitude = 12.5 };
        return AssertRouting<RtcmV3MessageBase, RtcmV3Message1005>((context, id) => new RtcmV3MicroserviceClient(context, id),
            () => new RtcmV3Message1005(), message, new RtcmV3Message1230(), received => { Assert.Equal(12.5, received.Latitude); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task Ubx_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new UbxAckAck { AckClassId = 6, AckSubclassId = 1 };
        return AssertRouting<UbxMessageBase, UbxAckAck>((context, id) => new UbxMicroserviceClient(context, id),
            () => new UbxAckAck(), message, new UbxAckNak(), received => { Assert.Equal(6, received.AckClassId); Assert.Equal(1, received.AckSubclassId); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task Sbf_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new SbfPacketPvtGeodeticRev2 { Latitude = 12.5 };
        return AssertRouting<SbfMessageBase, SbfPacketPvtGeodeticRev2>((context, id) => new SbfMicroserviceClient(context, id),
            () => new SbfPacketPvtGeodeticRev2(), message, new SbfPacketDOP(), received => { Assert.Equal(12.5, received.Latitude); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task ComNavBinary_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new ComNavBinaryBestPosPacket { Latitude = 12.5 };
        return AssertRouting<ComNavBinaryMessageBase, ComNavBinaryBestPosPacket>((context, id) => new ComNavBinaryMicroserviceClient(context, id),
            () => new ComNavBinaryBestPosPacket(), message, new ComNavBinaryPsrPosPacket(), received => { Assert.Equal(12.5, received.Latitude); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task ComNavAscii_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new ComNavPsrPosAMessage { Latitude = 12.5 };
        return AssertRouting<ComNavAsciiMessageBase, ComNavPsrPosAMessage>((context, id) => new ComNavAsciiMicroserviceClient(context, id),
            () => new ComNavPsrPosAMessage(), message, new ComNavLogListAMessage(), received => { Assert.Equal(12.5, received.Latitude); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task ComNavSimpleAnswer_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new ComNavSimpleOkMessage { Body = "native answer" };
        return AssertRouting<ComNavSimpleAnswerMessageBase, ComNavSimpleOkMessage>((context, id) => new ComNavSimpleAnswerMicroserviceClient(context, id),
            () => new ComNavSimpleOkMessage(), message, new ComNavSimpleErrorMessage(), received => { Assert.Equal("native answer", received.Body); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task Javad_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new JavadReceiverTimeMessage { TimeOfDay = 12345 };
        return AssertRouting<JavadMessageBase, JavadReceiverTimeMessage>((context, id) => new JavadMicroserviceClient(context, id),
            () => new JavadReceiverTimeMessage(), message, new JavadEpochTimeMessage(), received => { Assert.Equal(12345u, received.TimeOfDay); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task Asv_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new AsvMessageHeartBeat { DeviceType = AsvDeviceType.GbasServer };
        return AssertRouting<AsvMessageBase, AsvMessageHeartBeat>((context, id) => new AsvMicroserviceClient(context, id),
            () => new AsvMessageHeartBeat(), message, new AsvMessagePvtGeo(), received => { Assert.Equal(AsvDeviceType.GbasServer, received.DeviceType); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task Gbas_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new MessageBlockType1 { ModifiedZCount = 12.5 };
        return AssertRouting<GbasMessageBlockBase, MessageBlockType1>((context, id) => new GbasMicroserviceClient(context, id),
            () => new MessageBlockType1(), message, new MessageBlockType2(), received => { Assert.Equal(12.5, received.ModifiedZCount); }, disposeAsync);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public Task Rinex_RoutesNativeMessagesAndDisposesIndependently(bool disposeAsync)
    {
        var message = new RinexObservationEpochMessage { EpochFlag = 6 };
        return AssertRouting<RinexMessageBase, RinexObservationEpochMessage>((context, id) => new RinexMicroserviceClient(context, id),
            () => new RinexObservationEpochMessage(), message, new RinexNavigationMessage(), received => { Assert.Equal(6, received.EpochFlag); }, disposeAsync);
    }

    [Fact]
    public void EveryProtocol_HasDistinctServiceIdentity()
    {
        using var connection = new InMemoryConnection((IProtocolContext)Protocol.Create(_ => { }));
        var context = new MicroserviceContext(connection);
        var id = new GnssDeviceId("receiver");
        IMicroserviceClient[] services =
        [
            new NmeaMicroserviceClient(context, id),
            new RtcmV2MicroserviceClient(context, id),
            new RtcmV3MicroserviceClient(context, id),
            new UbxMicroserviceClient(context, id),
            new SbfMicroserviceClient(context, id),
            new ComNavBinaryMicroserviceClient(context, id),
            new ComNavAsciiMicroserviceClient(context, id),
            new ComNavSimpleAnswerMicroserviceClient(context, id),
            new JavadMicroserviceClient(context, id),
            new AsvMicroserviceClient(context, id),
            new GbasMicroserviceClient(context, id),
            new RinexMicroserviceClient(context, id)
        ];
        try
        {
            Assert.Equal(12, services.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count());
            Assert.Equal(12, services.Select(x => x.TypeName).Distinct(StringComparer.Ordinal).Count());
        }
        finally
        {
            foreach (var service in services) service.Dispose();
        }
    }

    [Fact]
    public void UbxService_IncludesX20NativeMessages()
    {
        using var connection = new InMemoryConnection((IProtocolContext)Protocol.Create(_ => { }));
        using var client = new UbxMicroserviceClient(new MicroserviceContext(connection), new GnssDeviceId("receiver"));
        var received = new List<UbxX20NavClockData0>();
        using var subscription = client.Filter<UbxX20NavClockData0>().Subscribe(received.Add);
        var message = new UbxX20NavClockData0();
        message.Tags["endpoint"] = "receiver";
        connection.Publish(message);
        Assert.Same(message, Assert.Single(received));
    }

    [Fact]
    public void Constructor_RejectsMissingArguments()
    {
        using var connection = new InMemoryConnection((IProtocolContext)Protocol.Create(_ => { }));
        var context = new MicroserviceContext(connection);
        Assert.Throws<ArgumentNullException>(() => new NmeaMicroserviceClient(context, null!));
        Assert.Throws<ArgumentNullException>(() => new NmeaMicroserviceClient(null!, new GnssDeviceId("receiver")));
    }

    [Fact]
    public void LegacySubclass_CanOverrideEndpointFilter()
    {
        using var connection = new InMemoryConnection((IProtocolContext)Protocol.Create(_ => { }));
        using var client = new LegacyNmeaClient(new MicroserviceContext(connection), new GnssDeviceId("receiver"));
        var received = new List<NmeaMessageBase>();
        using var subscription = client.OnMessage.Subscribe(received.Add);
        var rejected = new NmeaMessageGll();
        rejected.Tags["endpoint"] = "receiver";
        connection.Publish(rejected);
        var accepted = new NmeaMessageGga();
        accepted.Tags["endpoint"] = "receiver";
        connection.Publish(accepted);
        var foreign = new NmeaMessageGga();
        foreign.Tags["endpoint"] = "other";
        connection.Publish(foreign);
        Assert.Same(accepted, Assert.Single(received));
    }

    private sealed class LegacyNmeaClient(IMicroserviceContext context, GnssDeviceId id)
        : NmeaMicroserviceClient(context, id)
    {
        protected override bool FilterDeviceMessages(NmeaMessageBase message) =>
            base.FilterDeviceMessages(message) && message is NmeaMessageGga;
    }

    private static async Task AssertRouting<TBase, TMessage>(
        Func<IMicroserviceContext, GnssDeviceId, GnssProtocolMicroserviceClient<TBase>> create,
        Func<TMessage> createMessage, TMessage own, TBase otherSubtype, Action<TMessage> assertNative,
        bool disposeAsync)
        where TBase : class, IProtocolMessage
        where TMessage : class, TBase
    {
        using var connection = new InMemoryConnection((IProtocolContext)Protocol.Create(_ => { }));
        var context = new MicroserviceContext(connection);
        using var first = create(context, new GnssDeviceId("receiver"));
        using var second = create(context, new GnssDeviceId("other"));
        await first.Init();
        await second.Init();
        var all = new List<TBase>();
        var typed = new List<TMessage>();
        var secondMessages = new List<TBase>();
        using var allSubscription = first.OnMessage.Subscribe(all.Add);
        using var typedSubscription = first.Filter<TMessage>().Subscribe(typed.Add);
        using var secondSubscription = second.OnMessage.Subscribe(secondMessages.Add);

        foreach (var endpoint in new string?[] { null, "", " ", "other", "Receiver", " receiver", "receiver " })
        {
            var ignored = createMessage();
            if (endpoint != null) ignored.Tags["endpoint"] = endpoint;
            connection.Publish(ignored);
        }
        IProtocolMessage unrelated = own is NmeaMessageBase ? new UbxAckAck() : new NmeaMessageGll();
        unrelated.Tags["endpoint"] = "receiver";
        connection.Publish(unrelated);
        Assert.Empty(all);
        Assert.Empty(typed);
        Assert.Single(secondMessages);

        own.Tags["endpoint"] = "receiver";
        connection.Publish(own);
        otherSubtype.Tags["endpoint"] = "receiver";
        connection.Publish(otherSubtype);
        Assert.Equal(2, all.Count);
        Assert.Same(own, all[0]);
        Assert.Same(otherSubtype, all[1]);
        assertNative(Assert.Single(typed));
        Assert.Same(own, typed[0]);

        if (disposeAsync) await first.DisposeAsync();
        else first.Dispose();
        var afterDisposal = createMessage();
        afterDisposal.Tags["endpoint"] = "receiver";
        connection.Publish(afterDisposal);
        Assert.Equal(2, all.Count);
        Assert.Single(typed);
        Assert.False(connection.IsDisposed);
        var stillActive = createMessage();
        stillActive.Tags["endpoint"] = "other";
        connection.Publish(stillActive);
        Assert.Equal(2, secondMessages.Count);
    }

    private sealed class InMemoryConnection(IProtocolContext context) : ProtocolConnection("test", context)
    {
        public void Publish(IProtocolMessage message) => InternalPublishRxMessage(message);
        public override ValueTask Send(IProtocolMessage message, CancellationToken cancel = default) => ValueTask.CompletedTask;
    }
}
