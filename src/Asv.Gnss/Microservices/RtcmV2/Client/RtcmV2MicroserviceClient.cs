using Asv.IO;

namespace Asv.Gnss;

/// <summary>Endpoint-scoped access to native RTCM v2 messages.</summary>
public interface IRtcmV2MicroserviceClient : IGnssProtocolMicroserviceClient<RtcmV2MessageBase>
{
}

/// <summary>Receives native RTCM v2 messages for one GNSS endpoint.</summary>
public sealed class RtcmV2MicroserviceClient : GnssProtocolMicroserviceClient<RtcmV2MessageBase>, IRtcmV2MicroserviceClient
{
    /// <summary>Creates a typed receive stream using the caller-owned connection.</summary>
    /// <param name="context">The receiver connection and shared services.</param>
    /// <param name="deviceId">The exact endpoint identity of this receiver.</param>
    public RtcmV2MicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, deviceId, RtcmV2Protocol.Info)
    {
    }
}
