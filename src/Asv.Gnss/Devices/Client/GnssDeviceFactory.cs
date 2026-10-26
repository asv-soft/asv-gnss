using System;
using System.Collections.Immutable;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Discovers one logical GNSS receiver per exact endpoint identifier.</summary>
/// <remarks>Addresses identify endpoint sources rather than hardware serial numbers. Extraction is side-effect free.</remarks>
public class GnssDeviceFactory : IClientDeviceFactory
{
    /// <inheritdoc />
    public bool TryIdentify(IProtocolMessage message, out DeviceId? deviceId)
    {
        deviceId = null;
        if (!GnssDeviceSource.TryGetEndpointId(message, out var endpointId)) return false;
        deviceId = new GnssDeviceId(endpointId);
        return true;
    }

    /// <inheritdoc />
    public bool TryGetSourceAddress(IProtocolMessage message, out DeviceAddress address)
    {
        address = default;
        if (!GnssDeviceSource.TryGetEndpointId(message, out var endpointId)) return false;
        address = GnssDeviceSource.CreateAddress(endpointId);
        return true;
    }

    /// <inheritdoc />
    public bool TryGetDeviceAddress(DeviceId deviceId, out DeviceAddress address)
    {
        address = default;
        if (deviceId is not GnssDeviceId gnss || string.IsNullOrWhiteSpace(gnss.EndpointId)) return false;
        address = GnssDeviceSource.CreateAddress(gnss.EndpointId);
        return true;
    }

    /// <inheritdoc />
    public void UpdateDevice(IClientDevice device, IProtocolMessage message)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (device is not GnssDevice gnssDevice || device.Id is not GnssDeviceId id)
            throw new ArgumentException("The device must be a GNSS receiver.", nameof(device));
        if (!GnssDeviceSource.Matches(message, id))
            throw new ArgumentException("The message must belong to this GNSS receiver endpoint.", nameof(message));
        gnssDevice.ManualLinkIndicator.Upgrade();
    }

    /// <inheritdoc />
    public IClientDevice CreateDevice(IProtocolMessage message, DeviceId deviceId, IMicroserviceContext context,
        ImmutableArray<IClientDeviceExtender> extenders)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (deviceId is not GnssDeviceId id)
            throw new ArgumentException("The identity must be a GNSS receiver identity.", nameof(deviceId));
        if (!GnssDeviceSource.Matches(message, id))
            throw new ArgumentException("The message must belong to the requested GNSS receiver endpoint.", nameof(message));
        var device = new GnssDevice(id, new ClientDeviceConfig(), extenders, context);
        device.ManualLinkIndicator.Upgrade();
        return device;
    }

    /// <inheritdoc />
    public int Order => ClientDeviceFactory.DefaultOrder;

    /// <inheritdoc />
    public override string ToString() => nameof(GnssDeviceFactory);
}
