using Asv.IO;

namespace Asv.Gnss;

public static class GnssDeviceMixin
{
    public static void RegisterGnssDevice(this IClientDeviceFactoryBuilder builder)
    {
        builder.Register(new GnssDeviceFactory());
    }
   
}