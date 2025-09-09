using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Asv.IO;

namespace Asv.Gnss;

public class UbxGnssDevice : GnssDevice
{
    private readonly GnssDeviceId _id;
    private readonly IMicroserviceContext _context;

    public UbxGnssDevice(GnssDeviceId id, ClientDeviceConfig config, ImmutableArray<IClientDeviceExtender> extenders,
        IMicroserviceContext context) : base(id, config, extenders, context)
    {
        _id = id;
        _context = context;
    }

    
    
    protected override async IAsyncEnumerable<IMicroserviceClient> InternalCreateMicroservices(CancellationToken cancel)
    {
        yield return new NmeaMicroserviceClient(_context, _id);
        yield return new RtcmV3MicroserviceClient(_context, _id);
        yield return new UbxMicroserviceClient(_context, _id);

    }
}
