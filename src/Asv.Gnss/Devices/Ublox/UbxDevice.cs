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
    /// <summary>
    /// Represents a UBX device that can send and receive UBX messages.
    /// </summary>
    public interface IUbxDevice
    {
        /// <summary>
        /// Gets the GNSS connection.
        /// </summary>
        /// <value>
        /// The GNSS connection.
        /// </value>
        IGnssConnection Connection { get; }

        /// <summary>
        /// Pushes a packet of type T to be processed asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the packet to be pushed.</typeparam>
        /// <param name="pkt">The packet to be pushed.</param>
        /// <param name="cancel">A token that can be used to cancel the push operation.</param>
        /// <returns>A Task representing the asynchronous push operation.</returns>
        /// <remarks>
        /// This method pushes a packet of type T to be processed asynchronously. The packet
        /// is processed in the background and this method returns a Task that can be used
        /// to track the progress or cancel the push operation if required.
        /// </remarks>
        Task Push<T>(T pkt, CancellationToken cancel)
            where T : UbxMessageBase;

        /// <summary>
        /// Pools a packet of type <typeparamref name="TPoolPacket"/> and converts it to a packet of type <typeparamref name="TPacket"/>.
        /// </summary>
        /// <typeparam name="TPacket">The type of the converted packet.</typeparam>
        /// <typeparam name="TPoolPacket">The type of the packet to be pooled.</typeparam>
        /// <param name="pkt">The packet to be pooled.</param>
        /// <param name="cancel">The cancellation token to cancel the pooling operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous pooling operation. The result of the task is the pooled packet of type <typeparamref name="TPacket"/>.</returns>
        /// <remarks>
        /// This method pools a packet of type <typeparamref name="TPoolPacket"/> and converts it to a packet of type <typeparamref name="TPacket"/>.
        /// The <paramref name="cancel"/> parameter is optional and can be used to cancel the pooling operation.
        /// </remarks>
        Task<TPacket> Pool<TPacket, TPoolPacket>(
            TPoolPacket pkt,
            CancellationToken cancel = default
        )
            where TPacket : UbxMessageBase
            where TPoolPacket : UbxMessageBase;
    }

    /// <summary>
    /// Represents the configuration settings for a UBX device.
    /// </summary>
    public class UbxDeviceConfig
    {
        /// <summary>
        /// The default UbxDeviceConfig instance.
        /// </summary>
        public static UbxDeviceConfig Default = new();

        /// <summary>
        /// Gets or sets the number of attempts.
        /// </summary>
        /// <value>
        /// The number of attempts.
        /// </value>
        public int AttemptCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the timeout for executing a command in milliseconds.
        /// </summary>
        /// <value>
        /// The command timeout in milliseconds.
        /// </value>
        public int CommandTimeoutMs { get; set; } = 3000;
    }

    /// <summary>
    /// Represents a UBX device that communicates with a GNSS device.
    /// </summary>
    public class UbxDevice : DisposableOnceWithCancel, IUbxDevice
    {
        /// <summary>
        /// Represents the configuration settings for a UbxDevice instance.
        /// </summary>
        private readonly UbxDeviceConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="UbxDevice"/> class with the specified connection string and default configuration.
        /// </summary>
        /// <param name="connectionString">The connection string to the device.</param>
        public UbxDevice(string connectionString)
            : this(connectionString, UbxDeviceConfig.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UbxDevice"/> class.
        /// Represents a UBX GNSS device.
        /// </summary>
        public UbxDevice(string connectionString, UbxDeviceConfig config)
            : this(GnssFactory.CreateDefault(connectionString), config) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UbxDevice"/> class.
        /// </summary>
        /// <param name="connection">The IGnssConnection object used for communication with the device.</param>
        /// <param name="config">The UbxDeviceConfig object containing the device configuration.</param>
        /// <param name="disposeConnection">A boolean value indicating whether the connection should be disposed along with the UbxDevice object.</param>
        public UbxDevice(
            IGnssConnection connection,
            UbxDeviceConfig config,
            bool disposeConnection = true
        )
        {
            Connection = connection;
            _config = config;

            if (disposeConnection)
            {
                connection.DisposeItWith(Disposable);
            }
        }

        /// <summary>
        /// Gets the GNSS connection.
        /// </summary>
        /// <value>
        /// The GNSS connection.
        /// </value>
        public IGnssConnection Connection { get; }

        /// <summary>
        /// Pushes a UBX message to the device and waits for acknowledgement or timeout.
        /// </summary>
        /// <typeparam name="T">The type of the UBX message.</typeparam>
        /// <param name="pkt">The UBX message to push.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>Returns a Task representing the asynchronous operation.</returns>
        public async Task Push<T>(T pkt, CancellationToken cancel)
            where T : UbxMessageBase
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
                    var tcs = new TaskCompletionSource<Unit>();
#if NETFRAMEWORK
                    using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#else
                    await using var c1 = linkedCancel.Token.Register(() => tcs.TrySetCanceled());
#endif

                    using var subscribeAck = Connection
                        .Filter<UbxAckAck>()
                        .Where(_ => _.AckClassId == pkt.Class && _.AckSubclassId == pkt.SubClass)
                        .Subscribe(_ => tcs.TrySetResult(Unit.Default));
                    using var subscribeNak = Connection
                        .Filter<UbxAckNak>()
                        .Where(_ => _.AckClassId == pkt.Class && _.AckSubclassId == pkt.SubClass)
                        .Subscribe(_ =>
                            tcs.TrySetException(
                                new UbxDeviceNakException(Connection.Stream.Name, pkt)
                            )
                        );

                    await Connection.Send(pkt, linkedCancel.Token).ConfigureAwait(false);
                    await tcs.Task.ConfigureAwait(false);
                    return;
                }
                catch (TaskCanceledException)
                {
                    if (IsDisposed)
                    {
                        return;
                    }

                    if (cancel.IsCancellationRequested)
                    {
                        throw;
                    }
                }
            }

            throw new UbxDeviceTimeoutException(
                Connection.Stream.Name,
                pkt,
                _config.CommandTimeoutMs
            );
        }

        /// <summary>
        /// Sends a packet and waits for a response, retrying multiple times if necessary.
        /// </summary>
        /// <typeparam name="TPacket">The type of packet to send and receive.</typeparam>
        /// <typeparam name="TPoolPacket">The type of packet used for pooling.</typeparam>
        /// <param name="pkt">The packet to send.</param>
        /// <param name="cancel">A <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result is the received packet.</returns>
        /// <exception cref="UbxDeviceTimeoutException">Thrown if the device does not respond within the specified timeout.</exception>
        /// <exception cref="UbxDeviceNakException">Thrown if a negative acknowledgment (NAK) is received from the device.</exception>
        public async Task<TPacket> Pool<TPacket, TPoolPacket>(
            TPoolPacket pkt,
            CancellationToken cancel = default
        )
            where TPacket : UbxMessageBase
            where TPoolPacket : UbxMessageBase
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
                    using var subscribeAck = Connection
                        .Filter<TPacket>()
                        .Where(_ => _.Class == pkt.Class && _.SubClass == pkt.SubClass)
                        .Subscribe(_ => tcs.TrySetResult(_));

                    using var subscribeNak = Connection
                        .Filter<UbxAckNak>()
                        .Where(_ => _.AckClassId == pkt.Class && _.AckSubclassId == pkt.SubClass)
                        .Subscribe(_ =>
                            tcs.TrySetException(
                                new UbxDeviceNakException(Connection.Stream.Name, pkt)
                            )
                        );

                    await Connection.Send(pkt, linkedCancel.Token).ConfigureAwait(false);
                    return await tcs.Task.ConfigureAwait(false);
                }
                catch (TaskCanceledException) when (!cancel.IsCancellationRequested) { }
            }

            throw new UbxDeviceTimeoutException(
                Connection.Stream.Name,
                pkt,
                _config.CommandTimeoutMs
            );
        }
    }
}
