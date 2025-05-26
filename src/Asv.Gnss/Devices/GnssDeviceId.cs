using Asv.IO;

namespace Asv.Gnss;

public class GnssDeviceId : DeviceId
{
    public const string GnssDeviceClass = "Gnss receiver";
    
    public GnssDeviceId(string endpointId) : base(GnssDeviceClass)
    {
        EndpointId = endpointId;
    }

    public string EndpointId { get; }
    public override string AsString()
    {
        return $"{DeviceClass}:{EndpointId}";
    }
}