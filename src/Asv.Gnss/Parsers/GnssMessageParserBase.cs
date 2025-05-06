using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using Asv.Common;

namespace Asv.Gnss
{
    /// <summary>
    /// Base class for GNSS message parsers.
    /// </summary>
    public abstract class GnssMessageParserBase : DisposableOnce, IGnssMessageParser
    {
        protected readonly Subject<GnssParserException> OnErrorSubject = new();
        protected readonly Subject<IGnssMessageBase> OnMessageSubject = new();

        /// <summary>
        /// Gets the statistic input bytes.
        /// </summary>
        public abstract int StatisticInputBytes { get; }

        /// <summary>
        /// Gets the protocol ID.
        /// </summary>
        public abstract string ProtocolId { get; }

        protected GnssMessageParserBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Debug.Assert(ProtocolId != null, nameof(ProtocolId) + " != null");
        }

        /// <summary>
        /// Reads a byte of data.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <returns>A boolean indicating the success of the read operation.</returns>
        public abstract bool Read(byte data);

        /// <summary>
        /// Resets the parser.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Notifies when a CRC error occurs.
        /// </summary>
        protected void PublishWhenCrcError()
        {
            InternalOnError(new GnssCrcErrorException(ProtocolId));
        }

        /// <summary>
        /// Notifies when not all data read when deserializing packet.
        /// </summary>
        /// <param name="messageIdOrName">The message ID or name.</param>
        protected void PublishWhenReadNotAllDataWhenDeserializePacket(string messageIdOrName)
        {
            InternalOnError(
                new GnssReadNotAllDataWhenDeserializePacketErrorException(
                    ProtocolId,
                    messageIdOrName
                )
            );
        }

        /// <summary>
        /// Publishes an error to the subject.
        /// </summary>
        /// <param name="ex">The exception to publish.</param>
        protected void InternalOnError(GnssParserException ex)
        {
            OnErrorSubject.OnNext(ex);
        }

        /// <summary>
        /// Gets an observable of the error subject.
        /// </summary>
        public IObservable<GnssParserException> OnError => OnErrorSubject;

        /// <summary>
        /// Gets an observable of the message subject.
        /// </summary>
        public IObservable<IGnssMessageBase> OnMessage => OnMessageSubject;

        protected override void InternalDisposeOnce()
        {
            OnErrorSubject.OnCompleted();
            OnErrorSubject.Dispose();

            OnMessageSubject.OnCompleted();
            OnMessageSubject.Dispose();
        }
    }

    /// <summary>
    /// Represents a GNSS message parser.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <typeparam name="TMsgId">The type of the message ID.</typeparam>
    public abstract class GnssMessageParserBase<TMessage, TMsgId> : GnssMessageParserBase
        where TMessage : GnssMessageBase<TMsgId>
    {
        private readonly Dictionary<TMsgId, Func<TMessage>> _factory = new();
        private int _readBytes;

        /// <summary>
        /// Registers a factory.
        /// </summary>
        /// <param name="factory">The factory to register.</param>
        public void Register(Func<TMessage> factory)
        {
            var pkt = factory();
            _factory.Add(pkt.MessageId, factory);
        }

        /// <summary>
        /// Gets the statistic input bytes.
        /// </summary>
        public override int StatisticInputBytes => _readBytes;

        /// <summary>
        /// Notifies when a message is received.
        /// </summary>
        /// <param name="message">The received message.</param>
        protected void InternalOnMessage(TMessage message)
        {
            OnMessageSubject.OnNext(message);
        }

        /// <summary>
        /// Parses a packet.
        /// </summary>
        /// <param name="id">The ID of the packet.</param>
        /// <param name="data">The data of the packet.</param>
        /// <param name="ignoreReadNotAllData">Optional boolean that defaults to false. If true, does not check if all data was read.</param>
        protected void ParsePacket(
            TMsgId id,
            ref ReadOnlySpan<byte> data,
            bool ignoreReadNotAllData = false
        )
        {
            if (!_factory.TryGetValue(id, out var factory))
            {
                InternalOnError(new GnssUnknownMessageException(ProtocolId, id.ToString()));
                return;
            }

            var message = factory();
            try
            {
                var count = data.Length;
                message.Deserialize(ref data);
                Interlocked.Add(ref _readBytes, count - data.Length);
            }
            catch (Exception e)
            {
                InternalOnError(
                    new GnssDeserializeMessageException(ProtocolId, id.ToString(), message.Name, e)
                );
                return;
            }

            try
            {
                InternalOnMessage(message);
            }
            catch (Exception e)
            {
                InternalOnError(
                    new GnssPublishMessageException(ProtocolId, id.ToString(), message.Name, e)
                );
            }

            if (!ignoreReadNotAllData && !data.IsEmpty)
            {
                PublishWhenReadNotAllDataWhenDeserializePacket(message.Name);
            }
        }

        protected override void InternalDisposeOnce()
        {
            base.InternalDisposeOnce();

            _factory.Clear();
        }
    }
}
