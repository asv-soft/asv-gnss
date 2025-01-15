using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss
{
    public interface IComNavDevice
    {
        IGnssConnection Connection { get; }
        IGnssConnection RtcmV2Connection { get; }
        Task Push<T>(T pkt, CancellationToken cancel)
            where T : ComNavAsciiCommandBase;
        Task<TPacket> Pool<TPacket, TPoolPacket>(
            TPoolPacket pkt,
            CancellationToken cancel = default
        )
            where TPacket : ComNavAsciiMessageBase
            where TPoolPacket : ComNavAsciiCommandBase;
    }

    public class ComNavDeviceConfig
    {
        public static ComNavDeviceConfig Default = new();
        public int AttemptCount { get; set; } = 3;
        public int CommandTimeoutMs { get; set; } = 3000;

        public int ConnectTimeoutMs { get; set; } = 5000;
    }

    public class ComNavDevice : DisposableOnceWithCancel, IComNavDevice
    {
        private readonly ComNavDeviceConfig _config;
        private ComNavPortEnum _mainPort;
        private ComNavPortEnum _rtcmV2Port;

        public ComNavDevice(string connectionString)
            : this(connectionString, ComNavDeviceConfig.Default) { }

        public ComNavDevice(string connectionString, ComNavDeviceConfig config)
            : this(
                new GnssConnection(
                    connectionString,
                    new ComNavBinaryParser().RegisterDefaultMessages(),
                    new ComNavAsciiParser().RegisterDefaultMessages(),
                    new ComNavSimpleAnswerParser().RegisterDefaultMessages(),
                    new Nmea0183Parser().RegisterDefaultMessages(),
                    new RtcmV3Parser().RegisterDefaultMessages(),
                    new RtcmV2Parser().RegisterDefaultMessages()
                ),
                config
            ) { }

        public ComNavDevice(string connectionString, string rtcmV2ConnectionString)
            : this(connectionString, rtcmV2ConnectionString, ComNavDeviceConfig.Default) { }

        public ComNavDevice(
            string connectionString,
            string rtcmV2ConnectionString,
            ComNavDeviceConfig config
        )
            : this(
                new GnssConnection(
                    connectionString,
                    new ComNavBinaryParser().RegisterDefaultMessages(),
                    new ComNavAsciiParser().RegisterDefaultMessages(),
                    new ComNavSimpleAnswerParser().RegisterDefaultMessages(),
                    new Nmea0183Parser().RegisterDefaultMessages(),
                    new RtcmV3Parser().RegisterDefaultMessages()
                ),
                new GnssConnection(
                    rtcmV2ConnectionString,
                    new RtcmV2Parser().RegisterDefaultMessages(),
                    new ComNavAsciiParser().RegisterDefaultMessages()
                ),
                config
            ) { }

        public ComNavDevice(
            IGnssConnection connection,
            IGnssConnection rtcmV2Connection,
            ComNavDeviceConfig config,
            bool disposeConnection = true
        )
        {
            Connection = connection;
            RtcmV2Connection = rtcmV2Connection;
            _config = config;

            if (disposeConnection)
            {
                Disposable.AddAction(() =>
                {
                    Connection?.Dispose();
                    RtcmV2Connection?.Dispose();
                });
            }
        }

        public ComNavDevice(
            IGnssConnection connection,
            ComNavDeviceConfig config,
            bool disposeConnection = true
        )
        {
            Connection = connection;
            RtcmV2Connection = connection;
            _config = config;

            if (disposeConnection)
            {
                Disposable.AddAction(() =>
                {
                    Connection?.Dispose();
                });
            }
        }

        public async Task Init(CancellationToken cancel)
        {
            try
            {
                using var linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(
                    cancel,
                    DisposeCancel
                );
                linkedCancel.CancelAfter(_config.ConnectTimeoutMs);
                var tcs = new TaskCompletionSource<Unit>();
#if NETFRAMEWORK
                using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
                await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif

                using var conn = Observable
                    .Zip(((IPort)Connection.Stream).State, ((IPort)RtcmV2Connection.Stream).State)
                    .Where(_ => _.All(__ => __ == PortState.Connected))
                    .Subscribe(_ => tcs.TrySetResult(Unit.Default));

                await tcs.Task.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                if (IsDisposed)
                    return;
                if (cancel.IsCancellationRequested)
                {
                    throw;
                }
            }

            var port = await Pool<ComNavComConfigAMessage, ComNavAsciiLogCommand>(
                    new ComNavAsciiLogCommand()
                    {
                        Format = ComNavFormat.Ascii,
                        Type = ComNavMessageEnum.COMCONFIG,
                    },
                    Connection,
                    cancel
                )
                .ConfigureAwait(false);

            var rtcmV2Port = await Pool<ComNavComConfigAMessage, ComNavAsciiLogCommand>(
                    new ComNavAsciiLogCommand()
                    {
                        Format = ComNavFormat.Ascii,
                        Type = ComNavMessageEnum.COMCONFIG,
                    },
                    RtcmV2Connection,
                    cancel
                )
                .ConfigureAwait(false);

            _mainPort = port.Source;
            _rtcmV2Port = rtcmV2Port.Source;

            Console.WriteLine($"Main port ({Connection.Stream.Name}): '{port.Source:G}'");
            Console.WriteLine(
                $"RtcmV2 port ({RtcmV2Connection.Stream.Name}): '{rtcmV2Port.Source:G}'"
            );
        }

        public IGnssConnection Connection { get; }
        public IGnssConnection RtcmV2Connection { get; }

        private void SetTargetPort<T>(ref T pkt)
            where T : ComNavAsciiCommandBase
        {
            if (pkt is not ComNavAsciiLogCommand command)
                return;
            command.PortName = command.Type.IsRtcmV2LogCommand()
                ? $"{_rtcmV2Port:G}"
                : $"{_mainPort:G}";
        }

        public async Task Push<T>(T pkt, CancellationToken cancel = default)
            where T : ComNavAsciiCommandBase
        {
            SetTargetPort(ref pkt);
            byte currentAttempt = 0;
            while (currentAttempt < _config.AttemptCount)
            {
                ++currentAttempt;
                try
                {
                    using var linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(
                        cancel,
                        DisposeCancel
                    );
                    linkedCancel.CancelAfter(_config.CommandTimeoutMs);
                    var tcs = new TaskCompletionSource<Unit>();
#if NETFRAMEWORK
                    using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
                    await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif

                    using var subscribeOk = Connection
                        .Filter<ComNavSimpleOkMessage>()
                        .Subscribe(_ => tcs.TrySetResult(Unit.Default));
                    using var subscribeTransmit = Connection
                        .Filter<ComNavSimpleTransmitMessage>()
                        .Subscribe(_ => tcs.TrySetResult(Unit.Default));
                    using var subscribeError = Connection
                        .Filter<ComNavSimpleErrorMessage>()
                        .Subscribe(_ =>
                            tcs.TrySetException(
                                new ComNavDeviceResponseException(Connection.Stream.Name, pkt)
                            )
                        );

                    await Connection.Send(pkt, linkedCancel.Token).ConfigureAwait(false);
                    await tcs.Task.ConfigureAwait(false);
                    return;
                }
                catch (TaskCanceledException)
                {
                    if (IsDisposed)
                        return;
                    if (cancel.IsCancellationRequested)
                    {
                        throw;
                    }
                }
            }

            throw new ComNavDeviceTimeoutException(
                Connection.Stream.Name,
                pkt,
                _config.CommandTimeoutMs
            );
        }

        public Task<TPacket> Pool<TPacket, TPoolPacket>(
            TPoolPacket pkt,
            CancellationToken cancel = default
        )
            where TPacket : ComNavAsciiMessageBase
            where TPoolPacket : ComNavAsciiCommandBase
        {
            return Pool<TPacket, TPoolPacket>(pkt, Connection, cancel);
        }

        private async Task<TPacket> Pool<TPacket, TPoolPacket>(
            TPoolPacket pkt,
            IGnssConnection srcConnection,
            CancellationToken cancel = default
        )
            where TPacket : ComNavAsciiMessageBase
            where TPoolPacket : ComNavAsciiCommandBase
        {
            byte currentAttempt = 0;
            while (currentAttempt < _config.AttemptCount)
            {
                ++currentAttempt;
                try
                {
                    using var linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(
                        cancel,
                        DisposeCancel
                    );
                    linkedCancel.CancelAfter(_config.CommandTimeoutMs);
                    var tcs = new TaskCompletionSource<TPacket>();
#if NETFRAMEWORK
                    using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
                    await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif
                    using var subscribeAck = srcConnection
                        .Filter<TPacket>()
                        .Subscribe(_ => tcs.TrySetResult(_));

                    using var subscribeNak = srcConnection
                        .Filter<ComNavSimpleErrorMessage>()
                        .Subscribe(_ =>
                            tcs.TrySetException(
                                new ComNavDeviceResponseException(srcConnection.Stream.Name, pkt)
                            )
                        );

                    await srcConnection.Send(pkt, linkedCancel.Token).ConfigureAwait(false);
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

            throw new ComNavDeviceTimeoutException(
                srcConnection.Stream.Name,
                pkt,
                _config.CommandTimeoutMs
            );
        }
    }
}
