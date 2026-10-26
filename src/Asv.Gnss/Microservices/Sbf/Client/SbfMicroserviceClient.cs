using Asv.IO;

namespace Asv.Gnss;

/// <summary>Endpoint-scoped access to native Septentrio SBF messages.</summary>
public interface ISbfMicroserviceClient : IGnssProtocolMicroserviceClient<SbfMessageBase>
{
}

/// <summary>Receives native Septentrio SBF messages for one GNSS endpoint.</summary>
public sealed class SbfMicroserviceClient : GnssProtocolMicroserviceClient<SbfMessageBase>, ISbfMicroserviceClient
{
    /// <summary>Creates a typed receive stream using the caller-owned connection.</summary>
    /// <param name="context">The receiver connection and shared services.</param>
    /// <param name="deviceId">The exact endpoint identity of this receiver.</param>
    public SbfMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, deviceId, SbfProtocol.Info)
    {
    }
}
