using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using Microsoft.Extensions.Time.Testing;
using R3;
using Xunit;

namespace Asv.Gnss.Test;

public class GnssDeviceLifecycleTest
{
    [Fact]
    public void Create_AcceptedFirstMessage_ConnectsWithoutInitializingAndPassesExtenders()
    {
        using var fixture = new DeviceFixture();
        var extender = new CapturingExtender();
        var message = Tagged(new NmeaMessageGga(), "first");
        var id = new GnssDeviceId("first");
        using var device = Assert.IsType<GnssDevice>(fixture.Factory.CreateDevice(message, id, fixture.Context, [extender]));

        Assert.Same(id, device.Id);
        Assert.Equal(ClientDeviceState.Uninitialized, device.State.CurrentValue);
        Assert.Empty(device.Microservices);
        Assert.Equal(LinkState.Connected, device.Link.State.CurrentValue);
        Assert.Null(extender.Identity);

        device.Initialize();
        Assert.Equal(ClientDeviceState.Complete, device.State.CurrentValue);
        Assert.Same(id, extender.Identity);
        Assert.Equal(12, extender.ServiceCount);
        Assert.Equal(12, device.Microservices.Length);
        Assert.All(device.Microservices, service => Assert.True(service.IsInit));
        Assert.Equal(12, device.Microservices.Select(service => service.TypeName).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(ExpectedProtocols().Order(StringComparer.Ordinal),
            device.Microservices.Select(service => service.TypeName).Order(StringComparer.Ordinal));
        device.Initialize();
        Assert.Equal(1, extender.Calls);
    }

    [Fact]
    public void Create_UnsupportedMessageOrMismatchedIdentity_RejectsBeforeDeviceCreation()
    {
        using var fixture = new DeviceFixture();
        Assert.ThrowsAny<ArgumentException>(() => fixture.Factory.CreateDevice(
            Tagged(new NmeaMessageGga(), "first"), new GnssDeviceId("other"), fixture.Context, []));
        Assert.ThrowsAny<ArgumentException>(() => fixture.Factory.CreateDevice(
            Tagged(new AsterixMessageI002(), "first"), new GnssDeviceId("first"), fixture.Context, []));
        Assert.ThrowsAny<ArgumentException>(() => fixture.Factory.CreateDevice(
            new NmeaMessageGga(), new GnssDeviceId("first"), fixture.Context, []));
        Assert.ThrowsAny<ArgumentException>(() => fixture.Factory.CreateDevice(
            Tagged(new NmeaMessageGga(), "first"), new ForeignId(), fixture.Context, []));
    }

    [Fact]
    public void Update_WrongEndpointOrUnsupportedMessage_DoesNotRenewLink()
    {
        using var fixture = new DeviceFixture();
        using var device = fixture.Create("first");
        fixture.Time.Advance(TimeSpan.FromSeconds(9));
        Assert.ThrowsAny<ArgumentException>(() => fixture.Factory.UpdateDevice(device, Tagged(new NmeaMessageGga(), "other")));
        Assert.ThrowsAny<ArgumentException>(() => fixture.Factory.UpdateDevice(device, Tagged(new AsterixMessageI002(), "first")));
        fixture.Time.Advance(TimeSpan.FromSeconds(12));
        Assert.Equal(LinkState.Downgrade, device.Link.State.CurrentValue);
        fixture.Factory.UpdateDevice(device, Tagged(new RtcmV2Message1(), "first"));
        Assert.Equal(LinkState.Connected, device.Link.State.CurrentValue);
        Assert.True(fixture.Connection.CanPublish);
    }

    [Fact]
    public void Update_ForeignDevice_RejectsItInsteadOfSilentlyIgnoring()
    {
        using var fixture = new DeviceFixture();
        using var device = new ForeignDevice();
        Assert.ThrowsAny<ArgumentException>(() => fixture.Factory.UpdateDevice(device, Tagged(new NmeaMessageGga(), "first")));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Dispose_InitializedDevice_ReleasesOwnedLinkAndServicesButLeavesConnectionOpen(bool asyncDispose)
    {
        using var fixture = new DeviceFixture();
        var device = fixture.Create("first");
        device.Initialize();
        var link = Assert.IsType<TimeBasedLinkIndicator>(device.Link);
        var service = device.GetRequiredMicroservice<IRtcmV3MicroserviceClient>();
        var original = Tagged(new RtcmV3Message1005 { Latitude = 1, Longitude = 2, Altitude = 3 }, "first");
        fixture.Connection.Publish(original);
        Assert.Same(original, service.ReferenceStationArp.CurrentValue);
        var services = device.Microservices;
        if (asyncDispose) await device.DisposeAsync();
        else device.Dispose();
        Assert.True(link.IsDisposed);
        Assert.Empty(device.Microservices);
        fixture.Time.Advance(TimeSpan.FromMinutes(1));
        fixture.Connection.Publish(Tagged(new RtcmV3Message1005 { Latitude = 9 }, "first"));
        Assert.Same(original, service.ReferenceStationArp.CurrentValue);
        Assert.True(fixture.Connection.CanPublish);
        Assert.All(services, item => Assert.True(((Asv.Common.AsyncDisposableOnce)item).IsDisposed));
    }

    internal static T Tagged<T>(T message, string endpoint) where T : IProtocolMessage
    {
        message.Tags["endpoint"] = endpoint;
        return message;
    }

    internal static string[] ExpectedProtocols() =>
    [
        NmeaProtocol.Info.Id, RtcmV2Protocol.Info.Id, RtcmV3Protocol.Info.Id, UbxProtocol.Info.Id,
        SbfProtocol.Info.Id, ComNavProtocol.BinaryInfo.Id, ComNavProtocol.AsciiInfo.Id,
        ComNavProtocol.SimpleAnswerInfo.Id, JavadProtocol.Info.Id, AsvProtocol.Info.Id,
        GbasProtocol.Info.Id, RinexProtocol.Info.Id
    ];

    private sealed class CapturingExtender : IClientDeviceExtender
    {
        internal DeviceId? Identity;
        internal int ServiceCount;
        internal int Calls;
        public Task Extend(DeviceId deviceId, ImmutableArray<IMicroserviceClient>.Builder services, CancellationToken cancel)
        {
            Identity = deviceId;
            ServiceCount = services.Count;
            Calls++;
            return Task.CompletedTask;
        }
    }

    private sealed class ForeignId() : DeviceId("foreign")
    {
        public override string AsString() => "foreign";
    }

    private sealed class ForeignDevice : IClientDevice
    {
        private readonly ReactiveProperty<string?> _name = new();
        private readonly ReactiveProperty<ClientDeviceState> _state = new();
        private readonly TimeBasedLinkIndicator _link = new(TimeSpan.FromHours(1));
        public DeviceId Id { get; } = new ForeignId();
        public ReadOnlyReactiveProperty<string?> Name => _name;
        public ReadOnlyReactiveProperty<ClientDeviceState> State => _state;
        public ILinkIndicator Link => _link;
        public ImmutableArray<IMicroserviceClient> Microservices => [];
        public void Initialize() { }
        public void Dispose() { _link.Dispose(); _name.Dispose(); _state.Dispose(); }
        public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
    }
}

internal sealed class DeviceFixture : IDisposable
{
    internal FakeTimeProvider Time { get; } = new();
    internal GnssTestConnection Connection { get; }
    internal MicroserviceContext Context { get; }
    internal IClientDeviceFactory Factory { get; } = new GnssDeviceFactory();
    internal DeviceFixture()
    {
        Connection = new GnssTestConnection((IProtocolContext)Protocol.Create(_ => { }));
        Context = new MicroserviceContext(Connection, timeProvider: Time);
    }
    internal GnssDevice Create(string endpoint) => (GnssDevice)Factory.CreateDevice(
        GnssDeviceLifecycleTest.Tagged(new NmeaMessageGga(), endpoint), new GnssDeviceId(endpoint), Context, []);
    public void Dispose() => Connection.Dispose();
}

internal sealed class GnssTestConnection(IProtocolContext context) : ProtocolConnection("test", context)
{
    internal bool CanPublish => !IsDisposed;
    internal void Publish(IProtocolMessage message) => InternalPublishRxMessage(message);
    public override ValueTask Send(IProtocolMessage message, CancellationToken cancel = default) => ValueTask.CompletedTask;
}
