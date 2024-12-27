using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a GNSS connection that communicates with a data stream and parses GNSS messages using provided parsers.
    /// </summary>
    public class GnssConnection : DisposableOnceWithCancel, IGnssConnection
    {
        /// <summary>
        /// Represents a private readonly data stream.
        /// </summary>
        /// <remarks>
        /// This variable holds a reference to an object implementing the IDataStream interface, which represents a data stream that can be read.
        /// The readonly modifier ensures that the reference cannot be changed once it is assigned.
        /// </remarks>
        private readonly IDataStream _stream;

        /// <summary>
        /// Array of objects implementing the IGnssMessageParser interface.
        /// </summary>
        private readonly IGnssMessageParser[] _parsers;

        /// <summary>
        /// The object used for synchronization.
        /// </summary>
        private readonly object _sync = new();

        /// <summary>
        /// Represents a subject for handling GNSS parser exceptions.
        /// </summary>
        private readonly Subject<GnssParserException> _onErrorSubject = new();

        /// <summary>
        /// The private, read-only Subject object used for publishing GNSS messages received on the Rx interface.
        /// </summary>
        private readonly Subject<IGnssMessageBase> _onRxMessageSubject = new();

        /// <summary>
        /// Represents the subject for transmitting GNSS messages.
        /// </summary>
        private readonly Subject<IGnssMessageBase> _onTxMessageSubject = new();

        /// <summary>
        /// Represents the Reactive Value for bytes subject.
        /// </summary>
        private readonly RxValue<int> _rxBytesSubject = new();

        /// <summary>
        /// Represents a subject for transmitting bytes.
        /// </summary>
        /// <remarks>
        /// This subject is used to transmit the number of bytes.
        /// </remarks>
        private readonly RxValue<int> _txBytesSubject = new();

        /// <summary>
        /// Represents a connection to a GNSS device.
        /// </summary>
        /// <param name="stream">The data stream used for communication.</param>
        /// <param name="parsers">The parsers used to parse incoming GNSS messages.</param>
        public GnssConnection(IDataStream stream, params IGnssMessageParser[] parsers)
        {
            _stream = stream;
            _parsers = parsers;
            foreach (var parser in parsers)
            {
                parser.DisposeItWith(Disposable);
                parser.OnError.Subscribe(_onErrorSubject).DisposeItWith(Disposable);
                parser.OnMessage.Subscribe(_onRxMessageSubject).DisposeItWith(Disposable);
            }

            Disposable.Add(_onErrorSubject);
            Disposable.Add(_onRxMessageSubject);
            Disposable.Add(_onTxMessageSubject);
            Disposable.Add(_rxBytesSubject);
            Disposable.Add(_txBytesSubject);

            stream.Subscribe(OnByteRecv).DisposeItWith(Disposable);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GnssConnection"/> class with the specified connection string and parsers.
        /// </summary>
        /// <param name="connectionString">The connection string to the GNSS device.</param>
        /// <param name="parsers">The array of GNSS message parsers.</param>
        public GnssConnection(string connectionString, params IGnssMessageParser[] parsers)
            : this(PortFactory.Create(connectionString, true), parsers) { }

        /// <summary>
        /// Gets the data stream.
        /// </summary>
        /// <value>
        /// The data stream.
        /// </value>
        public IDataStream Stream => _stream;

        /// <summary>
        /// Gets the statistic for received bytes.
        /// </summary>
        /// <returns>An instance of <see cref="IRxValue{T}"/> representing the received bytes.</returns>
        public IRxValue<int> StatisticRxBytes => _rxBytesSubject;

        /// <summary>
        /// Represents the statistic for transmitted bytes.
        /// </summary>
        /// <value>
        /// An instance of <see cref="IRxValue{T}"/> that provides the value of the transmitted bytes.
        /// </value>
        public IRxValue<int> StatisticTxBytes => _txBytesSubject;

        /// <summary>
        /// Gets the collection of GNSS message parsers.
        /// </summary>
        /// <value>
        /// The collection of GNSS message parsers.
        /// </value>
        public IEnumerable<IGnssMessageParser> Parsers => _parsers;

        /// <summary>
        /// Gets the observable sequence of <see cref="GnssParserException"/> that represents the errors occurred in the Gnss parser.
        /// </summary>
        /// <remarks>
        /// The <see cref="OnError"/> property returns an <see cref="IObservable{T}"/> that can be subscribed to receive notifications when errors occur during the parsing of GNSS data.
        /// </remarks>
        /// <value>
        /// An <see cref="IObservable{T}"/> that represents the errors occurred in the Gnss parser.
        /// </value>
        public IObservable<GnssParserException> OnError => _onErrorSubject;

        /// <summary>
        /// An IObservable property that represents the stream of incoming GNSS messages.
        /// </summary>
        public IObservable<IGnssMessageBase> OnMessage => _onRxMessageSubject;

        /// <summary>
        /// Gets an IObservable of IGnssMessageBase which represents the event when a message is transmitted.
        /// </summary>
        /// <remarks>
        /// Use this property to subscribe to the event when a message is transmitted.
        /// </remarks>
        public IObservable<IGnssMessageBase> OnTxMessage => _onTxMessageSubject;

        /// <summary>
        /// Method called when a byte array is received.
        /// </summary>
        /// <param name="buffer">The byte array received.</param>
        private void OnByteRecv(byte[] buffer)
        {
            lock (_sync)
            {
                _rxBytesSubject.OnNext(buffer.Length);
                IGnssMessageParser parser1;
                var index = 0;
                foreach (var data in buffer)
                {
                    index++;
                    try
                    {
                        var packetFound = false;
                        foreach (var parser in _parsers)
                        {
                            parser1 = parser;
                            if (!parser.Read(data))
                            {
                                continue;
                            }

                            packetFound = true;
                            break;
                        }

                        if (packetFound)
                        {
                            foreach (var parser in _parsers)
                            {
                                parser.Reset();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _onErrorSubject.OnNext(
                            new GnssParserException(
                                "COMMON",
                                $"GnssConnection error: {e.Message}",
                                e
                            )
                        );
                        Debug.Assert(false);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a GNSS message asynchronously.
        /// </summary>
        /// <param name="msg">The GNSS message to send.</param>
        /// <param name="cancel">A cancellation token to cancel the send operation.</param>
        /// <returns>A task representing the asynchronous operation. The task will be completed with a boolean result indicating whether the send operation was successful or not.</returns>
        public async Task<bool> Send(IGnssMessageBase msg, CancellationToken cancel)
        {
            _onTxMessageSubject.OnNext(msg);
            var result = await _stream.Send(msg, out var byteSended, cancel);
            _txBytesSubject.OnNext(byteSended);
            return result;
        }
    }
}
