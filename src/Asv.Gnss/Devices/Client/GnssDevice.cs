using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss;

public class GnssDevice : ClientDevice<GnssDeviceId>
{
    private readonly GnssDeviceId _id;
    private readonly IMicroserviceContext _context;

    public GnssDevice(GnssDeviceId id, ClientDeviceConfig config, ImmutableArray<IClientDeviceExtender> extenders, IMicroserviceContext context)
        : base(id, config, extenders, context)
    {
        _id = id;
        _context = context;
        ManualLinkIndicator = new TimeBasedLinkIndicator(TimeSpan.FromSeconds(10));
    }

    protected override async IAsyncEnumerable<IMicroserviceClient> InternalCreateMicroservices([EnumeratorCancellation]CancellationToken cancel)
    {
        yield return new NmeaMicroserviceClient(_context, _id);
        yield return new RtcmV3MicroserviceClient(_context, _id);
        yield return new UbxMicroserviceClient(_context, _id);
    }

    public override ILinkIndicator Link => ManualLinkIndicator;
    internal TimeBasedLinkIndicator ManualLinkIndicator { get; }
    
}