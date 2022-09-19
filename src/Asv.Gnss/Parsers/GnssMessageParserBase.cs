using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using Asv.Common;

namespace Asv.Gnss
{
    public abstract class GnssMessageParserBase : DisposableOnce, IGnssMessageParser
    {
        protected readonly Subject<GnssParserException> OnErrorSubject = new();
        protected readonly Subject<IGnssMessageBase> OnMessageSubject = new();

        public abstract int StatisticInputBytes { get; }
        public abstract string ProtocolId { get; }

        protected GnssMessageParserBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Debug.Assert(ProtocolId != null, nameof(ProtocolId) + " != null");
        }

        public abstract bool Read(byte data);
        public abstract void Reset();

        protected void PublishWhenCrcError()
        {
            InternalOnError(new GnssCrcErrorException(ProtocolId));
        }

        protected void PublishWhenReadNotAllDataWhenDeserializePacket(string messageIdOrName)
        {
            InternalOnError(new GnssReadNotAllDataWhenDeserializePacketErrorException(ProtocolId,messageIdOrName));
            //not all data readed
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        protected void InternalOnError(GnssParserException ex)
        {
            OnErrorSubject.OnNext(ex);
        }

        public IObservable<GnssParserException> OnError => OnErrorSubject;
        public IObservable<IGnssMessageBase> OnMessage => OnMessageSubject;

        protected override void InternalDisposeOnce()
        {
            OnErrorSubject.OnCompleted();
            OnErrorSubject.Dispose();
            OnMessageSubject.OnCompleted();
            OnMessageSubject.Dispose();
        }
    }


    public abstract class GnssMessageParserBase<TMessage, TMsgId> : GnssMessageParserBase
        where TMessage : GnssMessageBase<TMsgId>
    {
        private readonly Dictionary<TMsgId, Func<TMessage>> _factory = new();
        private int _readBytes;

        public void Register(Func<TMessage> factory)
        {
            var pkt = factory();
            _factory.Add(pkt.MessageId, factory);
        }

        public override int StatisticInputBytes => _readBytes;

        protected void InternalOnMessage(TMessage message)
        {
            OnMessageSubject.OnNext(message);
        }

        protected void ParsePacket(TMsgId id, ref ReadOnlySpan<byte> data, bool ignoreReadNotAllData = false)
        {
            if (_factory.TryGetValue(id, out var factory) == false)
            {
                InternalOnError(new GnssUnknownMessageException(ProtocolId,id.ToString()));
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
                InternalOnError(new GnssDeserializeMessageException(ProtocolId, id.ToString(), message.Name, e));
                return;
            }
            
            try
            {
                InternalOnMessage(message);
            }
            catch (Exception e)
            {
                InternalOnError(new GnssPublishMessageException(ProtocolId, id.ToString(),message.Name, e));
            }

            if (ignoreReadNotAllData == false && data.IsEmpty == false)
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