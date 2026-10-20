using System;
using System.Threading;
using System.Threading.Tasks;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class RtcmV3MicroserviceClientTest
{
    [Fact]
    public void Receive_MessagesFromTwoEndpoints_ShouldKeepAllDeviceStateSeparate()
    {
        var protocol = (IProtocolContext)Protocol.Create(_ => { });
        using var connection = new InMemoryConnection(protocol);
        var context = new MicroserviceContext(connection);
        using var first = new RtcmV3MicroserviceClient(context, new GnssDeviceId("first"));
        using var second = new RtcmV3MicroserviceClient(context, new GnssDeviceId("second"));
        var firstMessages = CreateMessages("first", 10);
        var secondMessages = CreateMessages("second", 20);

        Publish(connection, firstMessages);

        AssertState(first, firstMessages);
        Assert.Null(second.Msm4.CurrentValue);
        Assert.Null(second.ReferenceStationArp.CurrentValue);
        Assert.Null(second.GlonassBias.CurrentValue);
        Assert.Null(second.UtcTime.CurrentValue);

        Publish(connection, secondMessages);

        AssertState(first, firstMessages);
        AssertState(second, secondMessages);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("unrelated")]
    public void Receive_MissingOrUnrelatedEndpoint_ShouldIgnoreAllMessageTypes(string? endpointId)
    {
        var protocol = (IProtocolContext)Protocol.Create(_ => { });
        using var connection = new InMemoryConnection(protocol);
        using var client = new RtcmV3MicroserviceClient(new MicroserviceContext(connection), new GnssDeviceId("first"));
        var ignoredMessages = CreateMessages(endpointId, 20);

        Publish(connection, ignoredMessages);

        Assert.Null(client.Msm4.CurrentValue);
        Assert.Null(client.ReferenceStationArp.CurrentValue);
        Assert.Null(client.GlonassBias.CurrentValue);
        Assert.Null(client.UtcTime.CurrentValue);
        var ownMessages = CreateMessages("first", 10);
        Publish(connection, ownMessages);
        AssertState(client, ownMessages);

        Publish(connection, ignoredMessages);
        var unrelatedProtocol = new UbxAckAck();
        unrelatedProtocol.Tags["endpoint"] = "first";
        connection.Publish(unrelatedProtocol);

        AssertState(client, ownMessages);
    }

    private static (RtcmV3Msm4Msg1074 Msm, RtcmV3Message1005 Arp, RtcmV3Message1230 Bias) CreateMessages(string? endpointId, int value)
    {
        var msm = new RtcmV3Msm4Msg1074
        {
            EpochTime = new DateTime(2026, 10, 20, 12, value, 18, DateTimeKind.Utc)
        };
        var arp = new RtcmV3Message1005
        {
            Latitude = value,
            Longitude = value + 1,
            Altitude = value + 2,
            ReferenceStationID = (uint)value
        };
        var bias = new RtcmV3Message1230
        {
            ReferenceStationID = (uint)value,
            L1CACodePhaseBias = value
        };
        if (endpointId != null)
        {
            msm.Tags["endpoint"] = endpointId;
            arp.Tags["endpoint"] = endpointId;
            bias.Tags["endpoint"] = endpointId;
        }
        return (msm, arp, bias);
    }

    private static void Publish(InMemoryConnection connection, (RtcmV3Msm4Msg1074 Msm, RtcmV3Message1005 Arp, RtcmV3Message1230 Bias) messages)
    {
        connection.Publish(messages.Msm);
        connection.Publish(messages.Arp);
        connection.Publish(messages.Bias);
    }

    private static void AssertState(RtcmV3MicroserviceClient client, (RtcmV3Msm4Msg1074 Msm, RtcmV3Message1005 Arp, RtcmV3Message1230 Bias) messages)
    {
        Assert.Same(messages.Msm, client.Msm4.CurrentValue);
        Assert.Same(messages.Arp, client.ReferenceStationArp.CurrentValue);
        Assert.Same(messages.Bias, client.GlonassBias.CurrentValue);
        Assert.Equal(RtcmV3Protocol.Gps2Utc(messages.Msm.EpochTime), client.UtcTime.CurrentValue);
        Assert.Equal(messages.Arp.Latitude, client.ReferenceStationPosition.CurrentValue.Latitude);
        Assert.Equal(messages.Arp.Longitude, client.ReferenceStationPosition.CurrentValue.Longitude);
        Assert.Equal(messages.Arp.Altitude, client.ReferenceStationPosition.CurrentValue.Altitude);
    }

    private sealed class InMemoryConnection(IProtocolContext context) : ProtocolConnection("test", context)
    {
        public void Publish(IProtocolMessage message) => InternalPublishRxMessage(message);
        public override ValueTask Send(IProtocolMessage message, CancellationToken cancel = default) => ValueTask.CompletedTask;
    }
}
