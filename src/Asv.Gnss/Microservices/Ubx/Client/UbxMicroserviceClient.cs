using System;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using Microsoft.Extensions.Logging;
using R3;
using ZLogger;

namespace Asv.Gnss;

public interface IUbxMicroserviceClient : IMicroserviceClient
{
    Task Push<T>(T packet, int timeoutMs = 3000, int attemptCount = 5, CancellationToken cancel = default)
        where T : UbxMessageBase, new();

    Task<TPacket?> Pool<TPacket, TPoolPacket>(TPoolPacket packet, int timeoutMs = 3000, int attemptCount = 5,
        CancellationToken cancel = default)
        where TPacket : UbxMessageBase
        where TPoolPacket : UbxMessageBase, new();
    ReadOnlyReactiveProperty<DateTime?> UtcTime { get; }
    ReadOnlyReactiveProperty<GeoPoint> PositionMsl { get; }
    ReadOnlyReactiveProperty<GeoPoint> PositionGeoid { get; }
    ReadOnlyReactiveProperty<UbxGnssFixType?> GnssQuality { get; }
    ReadOnlyReactiveProperty<double> PositionDop { get; }
    ReadOnlyReactiveProperty<double> HorizontalDop { get; }
    ReadOnlyReactiveProperty<double> VerticalDop { get; }
    ReadOnlyReactiveProperty<int?> NavSatCount { get; }
    ReadOnlyReactiveProperty<bool> GnssFixOk { get; }
}


public class UbxMicroserviceClient : MicroserviceClient<UbxMessageBase>, IUbxMicroserviceClient
{
    private readonly GnssDeviceId _deviceId;
    private readonly ReactiveProperty<DateTime?> _dateTimeUtc = new();
    private readonly ReactiveProperty<GeoPoint> _positionMsl = new();
    private readonly ReactiveProperty<GeoPoint> _positionGeoid = new();
    private readonly ReactiveProperty<UbxGnssFixType?> _gnssQuality = new();
    private readonly ReactiveProperty<double> _positionDop = new();
    private readonly ReactiveProperty<double> _horizontalDop = new();
    private readonly ReactiveProperty<double> _verticalDop = new();
    private readonly ReactiveProperty<int?> _navSatCount = new();
    private readonly ReactiveProperty<bool> _gnssFixOk = new();
    private readonly IDisposable _sub2;
    private readonly IDisposable _sab3;
    private readonly ILogger _logger;

    public UbxMicroserviceClient(IMicroserviceContext context, GnssDeviceId id) : base(context, $"{id}.{UbxProtocol.Info.Id}")
    {
        _logger = context.LoggerFactory.CreateLogger($"{id}.{UbxProtocol.Info.Id}");
        _deviceId = id;
        _sub2 = InternalFilter<UbxNavPvt>().Subscribe(msg =>
        {
            _dateTimeUtc.Value = msg.UtcTime;
            _positionMsl.Value = new GeoPoint(msg.Latitude, msg.Longitude, msg.AltMsl);
            _positionGeoid.Value = new GeoPoint(msg.Latitude, msg.Longitude, msg.AltElipsoid);
            _gnssQuality.Value = msg.FixType;
            _gnssFixOk.Value = msg.GnssFixOK;
            _positionDop.Value = msg.PositionDOP;
            _navSatCount.Value = msg.NumberOfSatellites;
        });

        _sab3 = InternalFilter<UbxNavDop>().Subscribe(msg =>
        {
            _positionDop.Value = msg.pDOP;
            _horizontalDop.Value = msg.hDOP;
            _verticalDop.Value = msg.vDOP;
        });
    }

    
    public async Task Push<TPoolPacket>(TPoolPacket packet, int timeoutMs = 3000, int attemptCount = 5, CancellationToken cancel = default)
        where TPoolPacket : UbxMessageBase, new()
    {
        FilterDelegate<(UbxAckAck?, UbxAckNak?), UbxMessageBase> filter = Filter;
        var result = await InternalCall(packet, filter, attemptCount, null, timeoutMs, cancel).ConfigureAwait(false);
        if (result.Item2 != null)
        {
            throw new NotSupportedException($"[{Context.Connection.Id}] Error pushing {packet.Name}");
        }

        bool Filter(UbxMessageBase inputPacket, out (UbxAckAck?, UbxAckNak?) resultPacket)
        {
            switch (inputPacket)
            {
                case UbxAckAck ackAck
                    when ackAck.AckClassId == packet.Class &&
                         ackAck.AckSubclassId == packet.SubClass:
                    resultPacket = (ackAck, null);
                    return true;
                case UbxAckNak ackNak
                    when ackNak.AckClassId == packet.Class
                         && ackNak.AckSubclassId == packet.SubClass:
                    resultPacket = (null, ackNak);
                    return true;
                default:
                    resultPacket = (null, null);
                    return false;
            }
        }
    }

    public async Task<TPacket?> Pool<TPacket, TPoolPacket>(TPoolPacket packet, int timeoutMs = 3000, int attemptCount = 5, CancellationToken cancel = default) 
        where TPacket : UbxMessageBase
        where TPoolPacket : UbxMessageBase, new()
    {
        FilterDelegate<(TPacket?, UbxAckNak?), UbxMessageBase> filter = Filter;
        var result = await InternalCall(packet, filter, attemptCount, null, timeoutMs, cancel).ConfigureAwait(false);
        if (result.Item2 != null)
        {
            throw new NotSupportedException($"[{Context.Connection.Id}] Error pushing {packet.Name}");
        }
        return result.Item1;

        bool Filter(UbxMessageBase inputPacket, out (TPacket?, UbxAckNak?) resultPacket)
        {
            switch (inputPacket)
            {
                case TPacket pkt
                    when pkt.Class == packet.Class &&
                         pkt.SubClass == packet.SubClass:
                    resultPacket = (pkt, null);
                    return true;
                case UbxAckNak ackNak
                    when ackNak.AckClassId == packet.Class
                         && ackNak.AckSubclassId == packet.SubClass:
                    resultPacket = (null, ackNak);
                    return true;
                default:
                    resultPacket = (null, null);
                    return false;
            }
        }
    }

    protected override void FillMessageBeforeSent(UbxMessageBase message)
    {
    }

    protected override bool FilterDeviceMessages(UbxMessageBase arg)
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
            _dateTimeUtc.Dispose();
            _positionMsl.Dispose();
            _positionGeoid.Dispose();
            _gnssQuality.Dispose();
            _positionDop.Dispose();
            _horizontalDop.Dispose();
            _verticalDop.Dispose();
            _navSatCount.Dispose();
            _gnssFixOk.Dispose();
            _sub2.Dispose();
            _sab3.Dispose();
        }
        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await CastAndDispose(_dateTimeUtc);
        await CastAndDispose(_positionMsl);
        await CastAndDispose(_positionGeoid);
        await CastAndDispose(_gnssQuality);
        await CastAndDispose(_positionDop);
        await CastAndDispose(_horizontalDop);
        await CastAndDispose(_verticalDop);
        await CastAndDispose(_navSatCount);
        await CastAndDispose(_gnssFixOk);
        await CastAndDispose(_sub2);
        await CastAndDispose(_sab3);
        
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

    public override string TypeName => UbxProtocol.Info.Id;

    public ReadOnlyReactiveProperty<DateTime?> UtcTime => _dateTimeUtc;
    public ReadOnlyReactiveProperty<GeoPoint> PositionMsl => _positionMsl;
    public ReadOnlyReactiveProperty<GeoPoint> PositionGeoid => _positionGeoid;
    public ReadOnlyReactiveProperty<UbxGnssFixType?> GnssQuality => _gnssQuality;
    public ReadOnlyReactiveProperty<double> PositionDop => _positionDop;
    public ReadOnlyReactiveProperty<double> HorizontalDop => _horizontalDop;
    public ReadOnlyReactiveProperty<double> VerticalDop => _verticalDop;
    public ReadOnlyReactiveProperty<int?> NavSatCount => _navSatCount;
    public ReadOnlyReactiveProperty<bool> GnssFixOk => _gnssFixOk;
}
