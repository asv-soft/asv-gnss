using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>A single endpoint-scoped receiver with separate clients for its GNSS protocol families.</summary>
public class GnssDevice : ClientDevice<GnssDeviceId>
{
    private readonly GnssDeviceId _id;

    /// <summary>Creates an uninitialized receiver; the explorer initializes its services after discovery.</summary>
    public GnssDevice(GnssDeviceId id, ClientDeviceConfig config, ImmutableArray<IClientDeviceExtender> extenders, IMicroserviceContext context)
        : base(id, config, extenders, context)
    {
        _id = id;
        ManualLinkIndicator = new TimeBasedLinkIndicator(TimeSpan.FromSeconds(10), timeProvider: context.TimeProvider);
    }

    /// <inheritdoc />
    protected override async IAsyncEnumerable<IMicroserviceClient> InternalCreateMicroservices([EnumeratorCancellation] CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        yield return new NmeaMicroserviceClient(Context, _id);
        yield return new RtcmV2MicroserviceClient(Context, _id);
        yield return new RtcmV3MicroserviceClient(Context, _id);
        yield return new UbxMicroserviceClient(Context, _id);
        yield return new SbfMicroserviceClient(Context, _id);
        yield return new ComNavBinaryMicroserviceClient(Context, _id);
        yield return new ComNavAsciiMicroserviceClient(Context, _id);
        yield return new ComNavSimpleAnswerMicroserviceClient(Context, _id);
        yield return new JavadMicroserviceClient(Context, _id);
        yield return new AsvMicroserviceClient(Context, _id);
        yield return new GbasMicroserviceClient(Context, _id);
        yield return new RinexMicroserviceClient(Context, _id);
    }

    /// <inheritdoc />
    public override ILinkIndicator Link => ManualLinkIndicator;
    internal TimeBasedLinkIndicator ManualLinkIndicator { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing) ManualLinkIndicator.Dispose();
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await ManualLinkIndicator.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }
}
