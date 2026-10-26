using System;
using System.Collections.Generic;
using Asv.IO;
using Xunit;

namespace Asv.Gnss.Test;

public class GnssDeviceFactoryTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    public void TryIdentify_EachGnssProtocol_UsesIdenticalCanonicalSourceAndIdentityAddresses(int index)
    {
        IClientDeviceFactory factory = new GnssDeviceFactory();
        var message = GnssDeviceTestData.Messages()[index];
        message.Tags["endpoint"] = "receiver/COM1";

        Assert.True(factory.TryIdentify(message, out var identity));
        var gnss = Assert.IsType<GnssDeviceId>(identity);
        Assert.Equal("receiver/COM1", gnss.EndpointId);
        Assert.Equal("Gnss receiver:receiver/COM1", gnss.AsString());
        Assert.True(factory.TryGetSourceAddress(message, out var source));
        Assert.True(factory.TryGetDeviceAddress(gnss, out var address));
        Assert.Equal(new DeviceAddress("gnss-endpoint", "receiver/COM1"), source);
        Assert.Equal(source, address);
        Assert.Equal(ClientDeviceFactory.DefaultOrder, factory.Order);
    }

    [Fact]
    public void TryIdentify_UbxX20_UsesSameEndpointFamilyAsLegacyUbx()
    {
        IClientDeviceFactory factory = new GnssDeviceFactory();
        var legacy = GnssDeviceLifecycleTest.Tagged(new UbxAckAck(), "receiver");
        var x20 = GnssDeviceLifecycleTest.Tagged(new UbxX20AckAckData0(), "receiver");
        Assert.True(factory.TryGetSourceAddress(legacy, out var legacyAddress));
        Assert.True(factory.TryGetSourceAddress(x20, out var x20Address));
        Assert.Equal(legacyAddress, x20Address);
        Assert.True(factory.TryIdentify(x20, out var identity));
        Assert.Equal("Gnss receiver:receiver", identity!.AsString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void TryIdentify_MissingOrBlankEndpoint_RejectsAndClearsOutputs(string? endpoint)
    {
        IClientDeviceFactory factory = new GnssDeviceFactory();
        var message = new NmeaMessageGga();
        if (endpoint != null) message.Tags["endpoint"] = endpoint;
        Assert.False(factory.TryIdentify(message, out var identity));
        Assert.Null(identity);
        Assert.False(factory.TryGetSourceAddress(message, out var address));
        Assert.Equal(default, address);
    }

    [Fact]
    public void TryIdentify_NullOrUnrelatedProtocol_RejectsWithoutClaimingEndpoint()
    {
        IClientDeviceFactory factory = new GnssDeviceFactory();
        IProtocolMessage?[] ignored = [null, new AsterixMessageI002(), new GpsSubframe1(), new GlonassWord1()];
        foreach (var message in ignored)
        {
            if (message != null) message.Tags["endpoint"] = "receiver";
            Assert.False(factory.TryIdentify(message!, out var identity));
            Assert.Null(identity);
            Assert.False(factory.TryGetSourceAddress(message!, out var address));
            Assert.Equal(default, address);
        }
        Assert.False(factory.TryGetDeviceAddress(new ForeignDeviceId(), out var foreignAddress));
        Assert.Equal(default, foreignAddress);
        Assert.False(factory.TryGetDeviceAddress(null!, out var nullAddress));
        Assert.Equal(default, nullAddress);
    }

    [Fact]
    public void Address_ExactEndpointSpelling_RemainsOrdinalAndUnnormalized()
    {
        IClientDeviceFactory factory = new GnssDeviceFactory();
        Assert.True(factory.TryGetDeviceAddress(new GnssDeviceId("COM1"), out var first));
        Assert.True(factory.TryGetDeviceAddress(new GnssDeviceId("com1"), out var second));
        Assert.NotEqual(first, second);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Identity_InvalidEndpoint_RejectsAtConstruction(string? endpoint)
    {
        Assert.ThrowsAny<ArgumentException>(() => new GnssDeviceId(endpoint!));
    }

    [Fact]
    public void Identity_EndpointValue_DeterminesTypedObjectAndDictionaryEquality()
    {
        var first = new GnssDeviceId("COM1");
        var same = new GnssDeviceId("COM1");
        var other = new GnssDeviceId("com1");
        Assert.True(first == same);
        Assert.True(first != other);
        Assert.True(first.Equals(same));
        Assert.True(((object)first).Equals(same));
        Assert.Equal(first.GetHashCode(), same.GetHashCode());
        Assert.False(first.Equals(other));
        Assert.False(((object)first).Equals(other));
        Assert.True(EqualityComparer<DeviceId>.Default.Equals(first, same));
        Assert.False(EqualityComparer<DeviceId>.Default.Equals(first, other));
        Assert.Equal(2, new HashSet<DeviceId> { first, same, other }.Count);
    }

    private sealed class ForeignDeviceId() : DeviceId("foreign")
    {
        public override string AsString() => "foreign";
    }
}

internal static class GnssDeviceTestData
{
    internal static IReadOnlyList<IProtocolMessage> Messages() =>
    [
        new NmeaMessageGga(), new RtcmV2Message1(), new RtcmV3Message1005(), new UbxAckAck(),
        new SbfPacketDOP(), new ComNavBinaryBestPosPacket(), new ComNavPsrPosAMessage(),
        new ComNavSimpleOkMessage(), new JavadReceiverTimeMessage(), new AsvMessageHeartBeat(),
        new MessageBlockType3(), new RinexHeaderMessage()
    ];
}
