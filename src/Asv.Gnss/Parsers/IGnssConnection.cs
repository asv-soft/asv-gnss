using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss
{
    public interface IGnssConnection:IDisposable
    {
        IDataStream Stream { get; }
        IRxValue<int> StatisticRxBytes { get; }
        IRxValue<int> StatisticTxBytes { get; }
        IEnumerable<IGnssMessageParser> Parsers { get; }
        IObservable<GnssParserException> OnError { get; }
        IObservable<IGnssMessageBase> OnMessage { get; }
        IObservable<IGnssMessageBase> OnTxMessage { get; }
        Task<bool> Send(IGnssMessageBase msg, CancellationToken cancel);
        
    }


    public static class GnssFactory
    {
        public static IGnssConnection CreateDefault(string connectionString)
        {
            return new GnssConnection(connectionString,
                new AsvMessageParser().RegisterDefaultMessages(),
                new ComNavBinaryParser().RegisterDefaultMessages(),
                new Nmea0183Parser().RegisterDefaultMessages(),
                new RtcmV2Parser().RegisterDefaultMessages(),
                new RtcmV3Parser().RegisterDefaultMessages(),
                new UbxBinaryParser().RegisterDefaultMessages(),
                new SbfBinaryParser().RegisterDefaultMessages());
        }

        public static IObservable<TMsg> Filter<TMsg,TMsgId>(this IObservable<GnssMessageBase<TMsgId>> src) 
            where TMsg :GnssMessageBase<TMsgId>, new()
        {
            var msg = new TMsg();
            var id = msg.MessageId;
            return src.Where(_ => _.MessageId.Equals(id) && _ is TMsg).Cast<TMsg>();
        }
        public static IObservable<TMsg> Filter<TMsg>(this IGnssConnection src)
        {
            return src.OnMessage.Where(_ => _ is TMsg).Cast<TMsg>();
        }
        
        public static IObservable<RtcmV3RawMessage> GetRtcmV3RawMessages(this IGnssConnection src)
        {
            return ((RtcmV3Parser)src.Parsers.First(_ => _ is RtcmV3Parser)).OnRawMessage;
        }

        public static IObservable<TMsg> FilterWithTag<TMsg>(this IGnssConnection src, Action<TMsg> setTagCallback)
        {
            return src.OnMessage.Where(_ => _ is TMsg).Cast<TMsg>().Do(setTagCallback);
        }
        public static IObservable<TMsg> FilterWithTag<TMsg>(this IGnssConnection src, object tag)
            where TMsg : IGnssMessageBase
        {
            return src.OnMessage.Where(_ => _ is TMsg).Cast<TMsg>().Do(_ => _.Tag = tag);
        }

        public static IObservable<TMsg> Filter<TMsg>(this IGnssConnection src, Func<TMsg, bool> filter)
        {
            return src.OnMessage.Where(_ => _ is TMsg).Cast<TMsg>().Where(filter);
        }
    }
}