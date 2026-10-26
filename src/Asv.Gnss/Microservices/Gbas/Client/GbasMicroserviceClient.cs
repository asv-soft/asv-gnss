using Asv.IO;

namespace Asv.Gnss;

/// <summary>Endpoint-scoped access to native GBAS application block messages.</summary>
public interface IGbasMicroserviceClient : IGnssProtocolMicroserviceClient<GbasMessageBlockBase>
{
}

/// <summary>Receives native GBAS application block messages for one GNSS endpoint.</summary>
public sealed class GbasMicroserviceClient : GnssProtocolMicroserviceClient<GbasMessageBlockBase>, IGbasMicroserviceClient
{
    /// <summary>Creates a typed receive stream using the caller-owned connection.</summary>
    /// <param name="context">The receiver connection and shared services.</param>
    /// <param name="deviceId">The exact endpoint identity of this receiver.</param>
    public GbasMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, deviceId, GbasProtocol.Info)
    {
    }
}
