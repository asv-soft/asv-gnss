using System;

namespace Asv.Gnss
{
    public interface IGnssMessageParser:IDisposable
    {
        int StatisticInputBytes { get; }
        string ProtocolId { get; }
        bool Read(byte data);
        void Reset();
        IObservable<GnssParserException> OnError { get; }
        IObservable<IGnssMessageBase> OnMessage { get; }
        
        
    }
}