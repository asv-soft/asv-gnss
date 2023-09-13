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
		Task Push<T>(T pkt, CancellationToken cancel) where T : ComNavAsciiCommandBase;
		Task<TPacket> Pool<TPacket, TPoolPacket>(TPoolPacket pkt, CancellationToken cancel = default)
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
		private ComNavPortEnum _configPort;

		public ComNavDevice(string connectionString, string rtcmV2ConnectionString, string cfgConnectionString) : this(
			connectionString, rtcmV2ConnectionString, cfgConnectionString, ComNavDeviceConfig.Default)
		{

		}

		public ComNavDevice(string connectionString, string rtcmV2ConnectionString, string cfgConnectionString,
			ComNavDeviceConfig config) 
			: this(
				new GnssConnection(connectionString,
					new ComNavBinaryParser().RegisterDefaultMessages(),
					new ComNavAsciiParser().RegisterDefaultMessages(),
					new Nmea0183Parser().RegisterDefaultMessages(),
					new RtcmV3Parser().RegisterDefaultMessages()),
				new GnssConnection(rtcmV2ConnectionString, 
					new RtcmV2Parser().RegisterDefaultMessages(),
					new ComNavAsciiParser().RegisterDefaultMessages()),
				new GnssConnection(cfgConnectionString,
					new ComNavAsciiParser().RegisterDefaultMessages(),
					new ComNavSimpleAnswerParser().RegisterDefaultMessages()),
				config)
		{

		}

		public ComNavDevice(IGnssConnection connection, IGnssConnection rtcmV2Connection, IGnssConnection cfgConnection, ComNavDeviceConfig config, bool disposeConnection = true)
		{
			Connection = connection;
			RtcmV2Connection = rtcmV2Connection;
			_cfgConnection = cfgConnection;
			_config = config;

			if (disposeConnection)
			{
				Disposable.AddAction(() =>
				{
					Connection?.Dispose();
					RtcmV2Connection?.Dispose();
					_cfgConnection?.Dispose();
				});
			}
		}

		public async Task Init(CancellationToken cancel)
		{
			try
			{
				using var linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel, DisposeCancel);
				linkedCancel.CancelAfter(_config.ConnectTimeoutMs);
				var tcs = new TaskCompletionSource<Unit>();
#if NETFRAMEWORK
				using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
				await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif

				using var conn = Observable.Zip(
						((IPort)Connection.Stream).State,
						((IPort)_cfgConnection.Stream).State,
						((IPort)RtcmV2Connection.Stream).State)
					.Where(_ => _.All(__ => __ == PortState.Connected)).Subscribe(_ => tcs.TrySetResult(Unit.Default));
				
				await tcs.Task.ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				if (IsDisposed) return;
				if (cancel.IsCancellationRequested)
				{
					throw;
				}
			}

			var port = await Pool<ComNavComConfigAMessage, ComNavAsciiLogCommand>(new ComNavAsciiLogCommand()
				{ Format = ComNavFormat.Ascii, Type = ComNavMessageEnum.COMCONFIG }, Connection, cancel).ConfigureAwait(false);

			var rtcmV2Port = await Pool<ComNavComConfigAMessage, ComNavAsciiLogCommand>(new ComNavAsciiLogCommand()
				{ Format = ComNavFormat.Ascii, Type = ComNavMessageEnum.COMCONFIG }, RtcmV2Connection, cancel).ConfigureAwait(false);
			
			var cfgPort = await Pool<ComNavComConfigAMessage, ComNavAsciiLogCommand>(new ComNavAsciiLogCommand()
				{ Format = ComNavFormat.Ascii, Type = ComNavMessageEnum.COMCONFIG }, _cfgConnection, cancel).ConfigureAwait(false);

			_mainPort = port.Source;
			_configPort = cfgPort.Source;
			_rtcmV2Port = rtcmV2Port.Source;
		}

		private readonly IGnssConnection _cfgConnection;
		public IGnssConnection Connection { get; }
		public IGnssConnection RtcmV2Connection { get; }


		private void SetTargetPort<T>(ref T pkt)
			where T : ComNavAsciiCommandBase
		{
			if (pkt is not ComNavAsciiLogCommand command) return;
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
					using var linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel, DisposeCancel);
					linkedCancel.CancelAfter(_config.CommandTimeoutMs);
					var tcs = new TaskCompletionSource<Unit>();
#if NETFRAMEWORK
					using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
                    await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif

					using var subscribeOk = _cfgConnection.Filter<ComNavSimpleOkMessage>().Subscribe(_ => tcs.TrySetResult(Unit.Default));
					using var subscribeTransmit = _cfgConnection.Filter<ComNavSimpleTransmitMessage>().Subscribe(_ => tcs.TrySetResult(Unit.Default));
					using var subscribeError = _cfgConnection.Filter<ComNavSimpleErrorMessage>().Subscribe(_ => tcs.TrySetException(new ComNavDeviceResponseException(_cfgConnection.Stream.Name, pkt)));

					await _cfgConnection.Send(pkt, linkedCancel.Token).ConfigureAwait(false);
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

			throw new ComNavDeviceTimeoutException(_cfgConnection.Stream.Name, pkt, _config.CommandTimeoutMs);
		}

		public Task<TPacket> Pool<TPacket, TPoolPacket>(TPoolPacket pkt, CancellationToken cancel = default)
			where TPacket : ComNavAsciiMessageBase
			where TPoolPacket : ComNavAsciiCommandBase
		{
			return Pool<TPacket, TPoolPacket>(pkt, _cfgConnection, cancel);
		}

		private async Task<TPacket> Pool<TPacket, TPoolPacket>(TPoolPacket pkt, IGnssConnection srcConnection, CancellationToken cancel = default)
			where TPacket : ComNavAsciiMessageBase
			where TPoolPacket : ComNavAsciiCommandBase
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
					using var subscribeAck = srcConnection.Filter<TPacket>().Subscribe(_ => tcs.TrySetResult(_));

					using var subscribeNak = srcConnection.Filter<ComNavSimpleErrorMessage>()
						.Subscribe(_ => tcs.TrySetException(new ComNavDeviceResponseException(srcConnection.Stream.Name, pkt)));

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

			throw new ComNavDeviceTimeoutException(srcConnection.Stream.Name, pkt, _config.CommandTimeoutMs);
		}
	}
}