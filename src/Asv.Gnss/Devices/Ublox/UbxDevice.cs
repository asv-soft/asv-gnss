using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Gnss
{
    


    public interface IUbxDevice
    {
        IGnssConnection Connection { get; }
        Task Push<T>(T pkt, CancellationToken cancel) where T : UbxMessageBase;
        Task<TPacket> Pool<TPacket, TPoolPacket>(TPoolPacket pkt, CancellationToken cancel = default)
            where TPacket : UbxMessageBase
            where TPoolPacket : UbxMessageBase;
        
    }

    public class UbxDeviceConfig
    {
        public static UbxDeviceConfig Default = new();
        public int AttemptCount { get; set; } = 3;
        public int CommandTimeoutMs { get; set; } = 3000;
    }

    public class UbxDevice:DisposableOnceWithCancel, IUbxDevice
    {
        private readonly UbxDeviceConfig _config;

        public UbxDevice(string connectionString):this(connectionString,UbxDeviceConfig.Default)
        {
            
        }

        public UbxDevice(string connectionString, UbxDeviceConfig config) :this(GnssFactory.CreateDefault(connectionString),config)
        {
            
        }

        public UbxDevice(IGnssConnection connection, UbxDeviceConfig config, bool disposeConnection = true)
        {
            Connection = connection;
            _config = config;


            if (disposeConnection)
                connection.DisposeItWith(Disposable);
        }

        public IGnssConnection Connection { get; }

        public async Task Push<T>(T pkt, CancellationToken cancel)
            where T:UbxMessageBase
        {
            byte currentAttempt = 0;
            while (currentAttempt < _config.AttemptCount)
            {
                ++currentAttempt;
                try
                {
                    using var linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel,DisposeCancel);
                    linkedCancel.CancelAfter(_config.CommandTimeoutMs);
                    var tcs = new TaskCompletionSource<Unit>();
#if NETFRAMEWORK
                    using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
                    await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif

                    using var subscribeAck = Connection.Filter<UbxAckAck>().Where(_=>_.AckClassId == pkt.Class && _.AckSubclassId == pkt.SubClass)
                        .Subscribe(_ => tcs.TrySetResult(Unit.Default));
                    using var subscribeNak = Connection.Filter<UbxAckNak>().Where(_ => _.AckClassId == pkt.Class && _.AckSubclassId == pkt.SubClass)
                        .Subscribe(_ => tcs.TrySetException(new UbxDeviceNakException(Connection.Stream.Name,pkt)));

                    await Connection.Send(pkt, linkedCancel.Token).ConfigureAwait(false);
                    await tcs.Task.ConfigureAwait(false);
                    return;
                }
                catch (TaskCanceledException)
                {
                    if (IsDisposed) return;
                    if (cancel.IsCancellationRequested)
                    {
                        throw;
                    }
                }
            }

            throw new UbxDeviceTimeoutException(Connection.Stream.Name, pkt, _config.CommandTimeoutMs);
        }

        public async Task<TPacket> Pool<TPacket, TPoolPacket>(TPoolPacket pkt, CancellationToken cancel = default)
            where TPacket : UbxMessageBase
            where TPoolPacket:UbxMessageBase
        {
            byte currentAttempt = 0;
            while (currentAttempt < _config.AttemptCount)
            {
                ++currentAttempt;
                try
                {
                    using var linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel, DisposeCancel);
                    linkedCancel.CancelAfter(_config.CommandTimeoutMs);
                    var tcs = new TaskCompletionSource<TPacket>();
#if NETFRAMEWORK
                    using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
                    await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif
                    using var subscribeAck = Connection.Filter<TPacket>().Where(_ => _.Class == pkt.Class && _.SubClass == pkt.SubClass)
                        .Subscribe(_ => tcs.TrySetResult(_));

                    using var subscribeNak = Connection.Filter<UbxAckNak>().Where(_ => _.AckClassId == pkt.Class && _.AckSubclassId == pkt.SubClass)
                        .Subscribe(_ => tcs.TrySetException(new UbxDeviceNakException(Connection.Stream.Name, pkt)));

                    await Connection.Send(pkt, linkedCancel.Token).ConfigureAwait(false);
                    return await tcs.Task.ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    if (cancel.IsCancellationRequested)
                    {
                        throw;
                    }
                }
            }

            throw new UbxDeviceTimeoutException(Connection.Stream.Name, pkt, _config.CommandTimeoutMs);
        }

    }
}