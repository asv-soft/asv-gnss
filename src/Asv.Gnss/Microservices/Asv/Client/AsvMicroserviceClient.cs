using Asv.IO;

namespace Asv.Gnss;

/// <summary>Endpoint-scoped access to native ASV messages.</summary>
public interface IAsvMicroserviceClient : IGnssProtocolMicroserviceClient<AsvMessageBase>
{
}

/// <summary>Receives native ASV messages for one GNSS endpoint.</summary>
public sealed class AsvMicroserviceClient : GnssProtocolMicroserviceClient<AsvMessageBase>, IAsvMicroserviceClient
{
    /// <summary>Creates a typed receive stream using the caller-owned connection.</summary>
    /// <param name="context">The receiver connection and shared services.</param>
    /// <param name="deviceId">The exact endpoint identity of this receiver.</param>
    public AsvMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, deviceId, AsvProtocol.Info)
    {
    }
}
