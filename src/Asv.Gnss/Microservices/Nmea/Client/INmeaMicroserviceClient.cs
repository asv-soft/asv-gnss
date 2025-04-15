using System;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using R3;

namespace Asv.Gnss;

public interface INmeaMicroserviceClient : IMicroserviceClient
{
    ReadOnlyReactiveProperty<TimeOnly?> PositionTimeUtc { get; }
    ReadOnlyReactiveProperty<GeoPoint> PositionMsl { get; }
    ReadOnlyReactiveProperty<GeoPoint> PositionGeoid { get; }
    ReadOnlyReactiveProperty<NmeaGpsQuality?> GpsQuality { get; }
    ReadOnlyReactiveProperty<double> PositionHdop { get; }
    ReadOnlyReactiveProperty<int?> NavSatCount { get; }
    ReadOnlyReactiveProperty<GeoPoint> Position2D { get; }
    ReadOnlyReactiveProperty<NmeaPositioningSystemMode?> PositionMode { get; }
    ReadOnlyReactiveProperty<NmeaDataStatus?> PositionStatus { get; }
    
}

public class NmeaMicroserviceClient : MicroserviceClient<NmeaMessageBase>, INmeaMicroserviceClient
{
    private readonly GnssDeviceId _deviceId;
    private readonly ReactiveProperty<GeoPoint> _positionMsl = new();
    private readonly ReactiveProperty<GeoPoint> _positionGeoid = new();
    private readonly ReactiveProperty<NmeaGpsQuality?> _gpsQuality = new();
    private readonly ReactiveProperty<TimeOnly?> _positionTimeUtc = new();
    private readonly ReactiveProperty<int?> _navSatCount = new();
    private readonly ReactiveProperty<double> _positionHdop = new();
    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;
    private readonly ReactiveProperty<GeoPoint> _position = new();
    private readonly ReactiveProperty<NmeaPositioningSystemMode?> _positionMode = new();
    private readonly ReactiveProperty<NmeaDataStatus?> _positionStatus = new();

    public NmeaMicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId) 
        : base(context, $"{deviceId}.{NmeaProtocol.Info.Id}")
    {
        _deviceId = deviceId;
        _sub2 = InternalFilter<NmeaMessageGga>().Subscribe(x =>
        {
            _positionMsl.Value = new GeoPoint(x.Latitude,x.Longitude,x.AntennaAltitudeMsl);
            _positionGeoid.Value = new GeoPoint(x.Latitude,x.Longitude,x.AntennaAltitudeMsl + x.GeoidalSeparation);
            _gpsQuality.Value = x.GpsQuality;
            _positionTimeUtc.Value = x.Time;
            _positionHdop.Value = x.HorizontalDilutionPrecision;
            _navSatCount.Value = x.NumberOfSatellites;
        });
        _sub3 = InternalFilter<NmeaMessageGll>().Subscribe(x =>
        {
            _position.Value = new GeoPoint(x.Latitude, x.Longitude, double.NaN);
            _positionMode.Value = x.PositioningMode;
            _positionStatus.Value = x.Status;
        });


    }

    public ReadOnlyReactiveProperty<TimeOnly?> PositionTimeUtc => _positionTimeUtc;
    public ReadOnlyReactiveProperty<GeoPoint> PositionMsl => _positionMsl;
    public ReadOnlyReactiveProperty<GeoPoint> PositionGeoid => _positionGeoid;
    public ReadOnlyReactiveProperty<NmeaGpsQuality?> GpsQuality => _gpsQuality;
    public ReadOnlyReactiveProperty<double> PositionHdop => _positionHdop;
    public ReadOnlyReactiveProperty<int?> NavSatCount => _navSatCount;
    public ReadOnlyReactiveProperty<GeoPoint> Position2D => _position;
    public ReadOnlyReactiveProperty<NmeaPositioningSystemMode?> PositionMode => _positionMode;
    public ReadOnlyReactiveProperty<NmeaDataStatus?> PositionStatus => _positionStatus;
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _positionMsl.Dispose();
            _positionGeoid.Dispose();
            _gpsQuality.Dispose();
            _positionTimeUtc.Dispose();
            _navSatCount.Dispose();
            _positionHdop.Dispose();
            _sub2.Dispose();
            _sub3.Dispose();
            _position.Dispose();
            _positionMode.Dispose();
            _positionStatus.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await CastAndDispose(_positionMsl);
        await CastAndDispose(_positionGeoid);
        await CastAndDispose(_gpsQuality);
        await CastAndDispose(_positionTimeUtc);
        await CastAndDispose(_navSatCount);
        await CastAndDispose(_positionHdop);
        await CastAndDispose(_sub2);
        await CastAndDispose(_sub3);
        await CastAndDispose(_position);
        await CastAndDispose(_positionMode);
        await CastAndDispose(_positionStatus);

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