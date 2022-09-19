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
    public class GnssConnection : DisposableOnceWithCancel, IGnssConnection
    {
        private readonly IDataStream _stream;
        private readonly IGnssMessageParser[] _parsers;
        private readonly object _sync = new();
        private readonly Subject<GnssParserException> _onErrorSubject = new();
        private readonly Subject<IGnssMessageBase> _onRxMessageSubject = new();
        private readonly Subject<IGnssMessageBase> _onTxMessageSubject = new();
        private readonly RxValue<int> _rxBytesSubject = new();
        private readonly RxValue<int> _txBytesSubject = new();

        public GnssConnection(IDataStream stream, params IGnssMessageParser[] parsers)
        {
            _stream = stream;
            _parsers = parsers;
            foreach (var parser in parsers)
            {
                parser.DisposeItWith(Disposable);
                parser.OnError.Subscribe(_onErrorSubject).DisposeItWith(Disposable);
                parser.OnMessage.Subscribe(_onRxMessageSubject).DisposeItWith(Disposable); ;
            }
            Disposable.Add(_onErrorSubject);
            Disposable.Add(_onRxMessageSubject);
            Disposable.Add(_onTxMessageSubject);
            Disposable.Add(_rxBytesSubject);
            Disposable.Add(_txBytesSubject);

            stream.Subscribe(OnByteRecv).DisposeItWith(Disposable);

            
        }

        public GnssConnection(string connectionString, params IGnssMessageParser[] parsers):this(PortFactory.Create(connectionString,true),parsers)
        {

        }

        public IDataStream Stream => _stream;
        public IRxValue<int> StatisticRxBytes => _rxBytesSubject;
        public IRxValue<int> StatisticTxBytes => _txBytesSubject;

        public IEnumerable<IGnssMessageParser> Parsers => _parsers;
        public IObservable<GnssParserException> OnError => _onErrorSubject;
        public IObservable<IGnssMessageBase> OnMessage => _onRxMessageSubject;
        public IObservable<IGnssMessageBase> OnTxMessage => _onTxMessageSubject;

        private void OnByteRecv(byte[] buffer)
        {
            lock (_sync)
            {
                _rxBytesSubject.OnNext(buffer.Length);
                foreach (var data in buffer)
                {
                    try
                    {
                        var packetFound = false;
                        foreach (var parser in _parsers)
                        {
                            if (!parser.Read(data)) continue;
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
                        _onErrorSubject.OnNext(new GnssParserException("COMMON",$"GnssConnection error: {e.Message}",e));
                        Debug.Assert(false);
                    }
                }
            }
        }

        public async Task<bool> Send(IGnssMessageBase msg, CancellationToken cancel)
        {
            _onTxMessageSubject.OnNext(msg);
            var result = await _stream.Send(msg, out var byteSended, cancel);
            _txBytesSubject.OnNext(byteSended);
            return result;
        }
    }
}