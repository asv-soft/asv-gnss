using Asv.IO;

namespace Asv.Gnss;

/// <summary>Endpoint-scoped access to native Javad GREIS messages.</summary>
public interface IJavadMicroserviceClient : IGnssProtocolMicroserviceClient<JavadMessageBase>
{
}

/// <summary>Receives native Javad GREIS messages for one GNSS endpoint.</summary>
public sealed class JavadMicroserviceClient : GnssProtocolMicroserviceClient<JavadMessageBase>, IJavadMicroserviceClient
{
    /// <summary>Creates a typed receive stream using the caller-owned connection.</summary>
    /// <param name="context">The receiver connection and shared services.</param>
    /// <param name="deviceId">The exact endpoint identity of this receiver.</param>
    public JavadMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, deviceId, JavadProtocol.Info)
    {
    }
}
