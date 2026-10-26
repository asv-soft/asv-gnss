using Asv.IO;

namespace Asv.Gnss;

/// <summary>Endpoint-scoped access to native ComNav binary messages.</summary>
public interface IComNavBinaryMicroserviceClient : IGnssProtocolMicroserviceClient<ComNavBinaryMessageBase>
{
}

/// <summary>Receives native ComNav binary messages for one GNSS endpoint.</summary>
public sealed class ComNavBinaryMicroserviceClient : GnssProtocolMicroserviceClient<ComNavBinaryMessageBase>, IComNavBinaryMicroserviceClient
{
    /// <summary>Creates a typed receive stream using the caller-owned connection.</summary>
    /// <param name="context">The receiver connection and shared services.</param>
    /// <param name="deviceId">The exact endpoint identity of this receiver.</param>
    public ComNavBinaryMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, deviceId, ComNavProtocol.BinaryInfo)
    {
    }
}
