using System;
using System.Collections.Generic;
using System.Linq;
using Asv.Common;
using Asv.IO;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace Asv.Gnss.Test;

public class GnssDeviceExplorerTest
{
    [Fact]
    public void Discovery_MixedGnssProtocolsOnOneEndpoint_UseOneDeviceAndSourceBinding()
    {
        using var fixture = new ExplorerFixture();
        var messages = GnssDeviceTestData.Messages();
        foreach (var message in messages)
        {
            GnssDeviceLifecycleTest.Tagged(message, "first");
            fixture.Connection.Publish(message);
        }

        var device = Assert.IsType<GnssDevice>(Assert.Single(fixture.Explorer.Devices).Value);
        Assert.Equal("first", Assert.IsType<GnssDeviceId>(device.Id).EndpointId);
        Assert.Equal(ClientDeviceState.Complete, device.State.CurrentValue);
        Assert.Equal(LinkState.Connected, device.Link.State.CurrentValue);
        Assert.Same(device, Assert.Single(fixture.Explorer.InitializedDevices));
        foreach (var message in messages)
        {
            Assert.True(fixture.Explorer.TryIdentify(message, out var identity));
            Assert.Equal(device.Id.AsString(), identity!.AsString());
            Assert.True(fixture.Explorer.TryGetSourceAddress(message, out var source));
            Assert.True(fixture.Explorer.TryGetDeviceAddress(identity, out var address));
            Assert.Equal(source, address);
        }
        Assert.Equal(12, device.Microservices.Length);
    }

    [Fact]
    public void Discovery_EndpointCaseAndProtocolMix_KeepSeparateReceivers()
    {
        using var fixture = new ExplorerFixture();
        var first = GnssDeviceLifecycleTest.Tagged(new NmeaMessageGga(), "COM1");
        var second = GnssDeviceLifecycleTest.Tagged(new ComNavBinaryBestPosPacket(), "com1");
        fixture.Connection.Publish(first);
        fixture.Connection.Publish(second);
        fixture.Connection.Publish(GnssDeviceLifecycleTest.Tagged(new AsvMessageHeartBeat(), "COM1"));
        Assert.Equal(2, fixture.Explorer.Devices.Count);
        Assert.True(fixture.Explorer.TryIdentify(first, out var firstId));
        Assert.True(fixture.Explorer.TryIdentify(second, out var secondId));
        Assert.Equal("Gnss receiver:COM1", firstId!.AsString());
        Assert.Equal("Gnss receiver:com1", secondId!.AsString());
        Assert.NotEqual(firstId.AsString(), secondId.AsString());
        Assert.All(fixture.Explorer.Devices, pair => Assert.Equal(12, pair.Value.Microservices.Length));
    }

    [Fact]
    public void Lookup_PureExtractionAndRepeatedAttribution_NeitherCreateNorRenewDevice()
    {
        using var fixture = new ExplorerFixture();
        var message = GnssDeviceLifecycleTest.Tagged(new NmeaMessageGga(), "first");
        Assert.True(fixture.Explorer.TryGetSourceAddress(message, out var source));
        Assert.Equal(new DeviceAddress("gnss-endpoint", "first"), source);
        Assert.True(fixture.Explorer.TryGetDeviceAddress(new GnssDeviceId("first"), out var identityAddress));
        Assert.Equal(source, identityAddress);
        Assert.False(fixture.Explorer.TryIdentify(message, out var unknown));
        Assert.Null(unknown);
        Assert.Empty(fixture.Explorer.Devices);

        fixture.Connection.Publish(message);
        var device = Assert.IsType<GnssDevice>(Assert.Single(fixture.Explorer.Devices).Value);
        fixture.Time.Advance(TimeSpan.FromMilliseconds(90));
        Assert.True(fixture.Explorer.TryIdentify(message, out _));
        Assert.True(fixture.Explorer.TryGetSourceAddress(message, out _));
        Assert.True(fixture.Explorer.TryGetDeviceAddress(device.Id, out _));
        fixture.Time.Advance(TimeSpan.FromMilliseconds(20));
        Assert.Empty(fixture.Explorer.Devices);
        Assert.Empty(fixture.Explorer.InitializedDevices);
        Assert.True(device.IsDisposed);
        Assert.False(fixture.Explorer.TryIdentify(message, out var removed));
        Assert.Null(removed);
        Assert.True(fixture.Explorer.TryGetSourceAddress(message, out _));
        fixture.Connection.Publish(message);
        Assert.NotSame(device, Assert.Single(fixture.Explorer.Devices).Value);
    }

    [Fact]
    public void Discovery_UnsupportedOrUntaggedTraffic_DoesNotCreateOrKeepReceiverAlive()
    {
        using var fixture = new ExplorerFixture();
        IProtocolMessage[] ignored =
        [new NmeaMessageGga(), new AsterixMessageI002(), new GpsSubframe1(), new GlonassWord1()];
        foreach (var message in ignored.Skip(1)) message.Tags["endpoint"] = "first";
        foreach (var message in ignored)
        {
            fixture.Connection.Publish(message);
            Assert.False(fixture.Explorer.TryIdentify(message, out _));
        }
        Assert.Empty(fixture.Explorer.Devices);
        fixture.Connection.Publish(GnssDeviceLifecycleTest.Tagged(new UbxAckAck(), "first"));
        Assert.Single(fixture.Explorer.Devices);
        fixture.Time.Advance(TimeSpan.FromMilliseconds(90));
        foreach (var message in ignored) fixture.Connection.Publish(message);
        fixture.Time.Advance(TimeSpan.FromMilliseconds(20));
        Assert.Empty(fixture.Explorer.Devices);
    }

    [Fact]
    public void Discovery_AcceptedOtherProtocol_RenewsSameEndpointInactivityTimeout()
    {
        using var fixture = new ExplorerFixture();
        fixture.Connection.Publish(GnssDeviceLifecycleTest.Tagged(new NmeaMessageGga(), "first"));
        var original = Assert.Single(fixture.Explorer.Devices).Value;
        fixture.Time.Advance(TimeSpan.FromMilliseconds(90));
        fixture.Connection.Publish(GnssDeviceLifecycleTest.Tagged(new RtcmV2Message1(), "first"));
        fixture.Time.Advance(TimeSpan.FromMilliseconds(20));
        Assert.Same(original, Assert.Single(fixture.Explorer.Devices).Value);
        fixture.Time.Advance(TimeSpan.FromMilliseconds(90));
        Assert.Empty(fixture.Explorer.Devices);
    }

    private sealed class ExplorerFixture : IDisposable
    {
        internal FakeTimeProvider Time { get; } = new();
        internal GnssTestConnection Connection { get; } = new((IProtocolContext)Protocol.Create(_ => { }));
        internal IDeviceExplorer Explorer { get; }
        internal ExplorerFixture()
        {
            Explorer = DeviceExplorer.Create(Connection, builder =>
            {
                builder.SetTimeProvider(Time);
                builder.SetConfig(new ClientDeviceBrowserConfig { DeviceTimeoutMs = 100, DeviceCheckIntervalMs = 10 });
                builder.Factories.RegisterGnssDevice();
            });
        }
        public void Dispose() { Explorer.Dispose(); Connection.Dispose(); }
    }
}
