using System;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using R3;

namespace Asv.Gnss;


public interface IRtcmV3MicroserviceClient : IMicroserviceClient
{
    ReadOnlyReactiveProperty<DateTime?> UtcTime { get; }
    ReadOnlyReactiveProperty<RtcmV3Msm4Base> Msm4 { get; }
    ReadOnlyReactiveProperty<GeoPoint> ReferenceStationPosition { get; }
    ReadOnlyReactiveProperty<RtcmV3Message1005and1006> ReferenceStationArp { get; }
    ReadOnlyReactiveProperty<RtcmV3Message1230> GlonassBias { get; }
}

public class RtcmV3MicroserviceClient : MicroserviceClient<RtcmV3MessageBase>, IRtcmV3MicroserviceClient
{
    private readonly GnssDeviceId _deviceId;
    private readonly ReactiveProperty<DateTime?> _utcTime = new();
    private readonly ReactiveProperty<RtcmV3Msm4Base> _msm4 = new();
    private readonly ReactiveProperty<GeoPoint> _referenceStationPosition = new();
    private readonly ReactiveProperty<RtcmV3Message1005and1006> _referenceStationArp = new();
    private readonly ReactiveProperty<RtcmV3Message1230> _glonassBias = new();
    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;
    private readonly IDisposable _sub4;


    public RtcmV3MicroserviceClient(IMicroserviceContext context, GnssDeviceId deviceId)
        : base(context, $"{deviceId}.{RtcmV3Protocol.Info.Id}")
    {
        _deviceId = deviceId;

        _sub2 = context.Connection.RxFilterByType<RtcmV3Msm4Base>().Subscribe(msg =>
        {
            _utcTime.Value = RtcmV3Protocol.Gps2Utc(msg.EpochTime);
            _msm4.Value = msg;
        });
        _sub3 = context.Connection.RxFilterByType<RtcmV3Message1005and1006>().Subscribe(msg =>
        {
            _referenceStationPosition.Value = new GeoPoint(msg.Latitude, msg.Longitude, msg.Altitude);
            _referenceStationArp.Value = msg;
        });
        _sub4 = context.Connection.RxFilterByType<RtcmV3Message1230>().Subscribe(msg => _glonassBias.Value = msg);
    }

    protected override void FillMessageBeforeSent(RtcmV3MessageBase message)
    {
    }

    protected override bool FilterDeviceMessages(RtcmV3MessageBase arg)
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
            _utcTime.Dispose();
            _msm4.Dispose();
            _referenceStationPosition.Dispose();
            _referenceStationArp.Dispose();
            _glonassBias.Dispose();
            _sub2.Dispose();
            _sub3.Dispose();
            _sub4.Dispose();
        }
        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await CastAndDispose(_utcTime);
        await CastAndDispose(_msm4);
        await CastAndDispose(_referenceStationPosition);
        await CastAndDispose(_referenceStationArp);
        await CastAndDispose(_glonassBias);
        await CastAndDispose(_sub2);
        await CastAndDispose(_sub3);
        await CastAndDispose(_sub4);
        
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

    public override string TypeName => RtcmV3Protocol.Info.Id;
    
    public ReadOnlyReactiveProperty<DateTime?> UtcTime => _utcTime;
    public ReadOnlyReactiveProperty<RtcmV3Msm4Base> Msm4 => _msm4;
    public ReadOnlyReactiveProperty<GeoPoint> ReferenceStationPosition => _referenceStationPosition;
    public ReadOnlyReactiveProperty<RtcmV3Message1005and1006> ReferenceStationArp => _referenceStationArp;
    public ReadOnlyReactiveProperty<RtcmV3Message1230> GlonassBias => _glonassBias;
}

