using System;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using R3;

namespace Asv.Gnss;

public interface INmeaMicroserviceClient : IMicroserviceClient
{
    ReadOnlyReactiveProperty<GeoPoint> PositionMsl { get; }
    ReadOnlyReactiveProperty<GeoPoint> PositionEllipsoid { get; }
}

public class NmeaMicroserviceClient : MicroserviceClient<NmeaMessageBase>, INmeaMicroserviceClient
{
    private readonly GnssDeviceId _deviceId;
    private readonly ReactiveProperty<GeoPoint> _positionMsl = new();
    private readonly ReactiveProperty<GeoPoint> _positionEllipsoid = new();
    private readonly IDisposable _sub2;

    public NmeaMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId) 
        : base(context, $"{deviceId}.{NmeaProtocol.Info.Id}")
    {
        _deviceId = deviceId;
        _sub2 = InternalFilter<NmeaMessageGga>().Subscribe(x =>
        {
            _positionMsl.Value = new GeoPoint(x.Latitude,x.Longitude,x.AntennaAltitudeMsl);
            _positionEllipsoid.Value = new GeoPoint(x.Latitude,x.Longitude,x.GeoidalSeparation);
        });
        
    }
   
    
    public override string TypeName => NmeaProtocol.Info.Id;
    
    protected override void FillMessageBeforeSent(NmeaMessageBase message)
    {
        
    }

    protected override bool FilterDeviceMessages(NmeaMessageBase arg)
    {
        var endpointId = arg.GetEndpointId();
        if (endpointId == null)
        {
            return false;
        }

        return _deviceId.EndpointId == endpointId;
    }


    public ReadOnlyReactiveProperty<GeoPoint> PositionMsl => _positionMsl;

    public ReadOnlyReactiveProperty<GeoPoint> PositionEllipsoid => _positionEllipsoid;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _positionMsl.Dispose();
            _positionEllipsoid.Dispose();
            _sub2.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await CastAndDispose(_positionMsl);
        await CastAndDispose(_positionEllipsoid);
        await CastAndDispose(_sub2);

        await base.DisposeAsyncCore();

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}