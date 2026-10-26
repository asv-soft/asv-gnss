using System;
using Asv.IO;
using R3;

namespace Asv.Gnss;

/// <summary>A typed receive stream for one GNSS protocol on one receiver endpoint.</summary>
/// <typeparam name="TMessage">The protocol's native message base type.</typeparam>
public interface IGnssProtocolMicroserviceClient<TMessage> : IMicroserviceClient
    where TMessage : IProtocolMessage
{
    /// <summary>Gets native messages received from this service's endpoint only.</summary>
    Observable<TMessage> OnMessage { get; }

    /// <summary>Receives a native message subtype without losing the endpoint filter.</summary>
    /// <typeparam name="T">The native message subtype to observe.</typeparam>
    Observable<T> Filter<T>() where T : TMessage;
}

/// <summary>Shares endpoint-scoped routing between native GNSS protocol microservices.</summary>
/// <typeparam name="TMessage">The protocol's native message base type.</typeparam>
public abstract class GnssProtocolMicroserviceClient<TMessage> : MicroserviceClient<TMessage>,
    IGnssProtocolMicroserviceClient<TMessage>
    where TMessage : IProtocolMessage
{
    private readonly GnssDeviceId _deviceId;
    private readonly ProtocolInfo _protocol;

    /// <summary>Creates an independent message stream for a protocol and endpoint identity.</summary>
    /// <param name="context">The caller-owned connection and shared services.</param>
    /// <param name="deviceId">The endpoint identity, compared exactly and without normalization.</param>
    /// <param name="protocol">The protocol identity used in this service's name and ID.</param>
    protected GnssProtocolMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId,
        ProtocolInfo protocol) : base(context, CreateId(deviceId, protocol))
    {
        _protocol = protocol;
        _deviceId = deviceId;
    }

    /// <inheritdoc />
    public override string TypeName => _protocol.Id;

    /// <inheritdoc />
    public Observable<TMessage> OnMessage => InternalFilteredDeviceMessages;

    /// <inheritdoc />
    public Observable<T> Filter<T>() where T : TMessage => InternalFilter<T>();

    /// <inheritdoc />
    protected override bool FilterDeviceMessages(TMessage message)
    {
        // MicroserviceBase subscribes before this constructor has assigned its endpoint identity.
        return _deviceId != null &&
            string.Equals(message.GetEndpointId(), _deviceId.EndpointId, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    protected override void FillMessageBeforeSent(TMessage message)
    {
    }

    private static string CreateId(GnssDeviceId deviceId, ProtocolInfo protocol)
    {
        ArgumentNullException.ThrowIfNull(deviceId);
        ArgumentNullException.ThrowIfNull(protocol);
        if (string.IsNullOrWhiteSpace(deviceId.EndpointId))
            throw new ArgumentException("A GNSS microservice requires a nonblank endpoint identity.", nameof(deviceId));
        if (string.IsNullOrWhiteSpace(protocol.Id))
            throw new ArgumentException("A GNSS microservice requires a protocol identity.", nameof(protocol));
        return $"{deviceId}.{protocol.Id}";
    }
}
