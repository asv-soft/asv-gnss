using Asv.IO;

namespace Asv.Gnss;

/// <summary>Endpoint-scoped access to native RINEX file record messages.</summary>
public interface IRinexMicroserviceClient : IGnssProtocolMicroserviceClient<RinexMessageBase>
{
}

/// <summary>Receives native RINEX file record messages for one GNSS endpoint.</summary>
public sealed class RinexMicroserviceClient : GnssProtocolMicroserviceClient<RinexMessageBase>, IRinexMicroserviceClient
{
    /// <summary>Creates a typed receive stream using the caller-owned connection.</summary>
    /// <param name="context">The receiver connection and shared services.</param>
    /// <param name="deviceId">The exact endpoint identity of this receiver.</param>
    public RinexMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, deviceId, RinexProtocol.Info)
    {
    }
}
