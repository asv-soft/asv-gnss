using System;
using System.Diagnostics.CodeAnalysis;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Shared classification and exact endpoint identity for GNSS receiver traffic.</summary>
internal static class GnssDeviceSource
{
    internal const string AddressFamily = "gnss-endpoint";

    internal static bool TryGetEndpointId(IProtocolMessage? message, [NotNullWhen(true)] out string? endpointId)
    {
        endpointId = null;
        if (message is not (NmeaMessageBase or RtcmV2MessageBase or RtcmV3MessageBase or UbxMessageBase or
            SbfMessageBase or ComNavBinaryMessageBase or ComNavAsciiMessageBase or ComNavSimpleAnswerMessageBase or
            JavadMessageBase or AsvMessageBase or GbasMessageBlockBase or RinexMessageBase))
            return false;

        var endpoint = message.GetEndpointId();
        if (string.IsNullOrWhiteSpace(endpoint)) return false;
        endpointId = endpoint;
        return true;
    }

    internal static bool Matches(IProtocolMessage? message, GnssDeviceId id) =>
        TryGetEndpointId(message, out var endpointId) &&
        string.Equals(endpointId, id.EndpointId, StringComparison.Ordinal);

    internal static DeviceAddress CreateAddress(string endpointId) => new(AddressFamily, endpointId);
}
