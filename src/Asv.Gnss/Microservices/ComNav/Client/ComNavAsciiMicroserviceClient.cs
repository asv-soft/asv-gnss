using Asv.IO;

namespace Asv.Gnss;

/// <summary>Endpoint-scoped access to native ComNav ASCII log messages.</summary>
public interface IComNavAsciiMicroserviceClient : IGnssProtocolMicroserviceClient<ComNavAsciiMessageBase>
{
}

/// <summary>Receives native ComNav ASCII log messages for one GNSS endpoint.</summary>
public sealed class ComNavAsciiMicroserviceClient : GnssProtocolMicroserviceClient<ComNavAsciiMessageBase>, IComNavAsciiMicroserviceClient
{
    /// <summary>Creates a typed receive stream using the caller-owned connection.</summary>
    /// <param name="context">The receiver connection and shared services.</param>
    /// <param name="deviceId">The exact endpoint identity of this receiver.</param>
    public ComNavAsciiMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, deviceId, ComNavProtocol.AsciiInfo)
    {
    }
}
