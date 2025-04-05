using System.Collections.Immutable;
using Asv.IO;

namespace Asv.Gnss;

public class GnssDeviceFactory : IClientDeviceFactory
{
    public bool TryIdentify(IProtocolMessage message, out DeviceId? deviceId)
    {
        if (message is not NmeaMessageBase nmeaMessage) // TODO: add support for other protocols
        {
            deviceId = null;
            return false;
        }
        var endpointId = message.GetEndpointId();
        if (endpointId is null)
        {
            deviceId = null;
            return false;
        }

        deviceId = new GnssDeviceId(endpointId);
        return true;
    }

    public void UpdateDevice(IClientDevice device, IProtocolMessage message)
    {
        if (device is GnssDevice gnssDevice)
        {
            gnssDevice.ManualLinkIndicator.Upgrade();
        }
    }

    public IClientDevice CreateDevice(IProtocolMessage message, DeviceId deviceId, IMicroserviceContext context,
        ImmutableArray<IClientDeviceExtender> extenders)
    {
        return new GnssDevice((GnssDeviceId)deviceId, new ClientDeviceConfig(), extenders, context);
    }

    public int Order { get; } = 0;

    public override string ToString()
    {
        return nameof(GnssDeviceFactory);
    }
}