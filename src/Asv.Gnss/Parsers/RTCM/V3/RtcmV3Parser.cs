using System;
using System.Reactive.Subjects;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV3Parser : GnssMessageParserBase<RtcmV3MessageBase, ushort>
    {
        public const string GnssProtocolId = "RTCMv3";
        private readonly Subject<RtcmV3RawMessage> _onRawData = new();
        private readonly byte[] _buffer = new byte[1030]; // 3 (preamb.) + 0-1023 bytes data + 3 (CRC)
        private State _state;
        private ushort _payloadReadedBytes;
        private int _payloadLength;

        private enum State
        {
            Sync,
            Preamb1,
            Preamb2,
            Payload,
            Crc2,
            Crc1,
            Crc3
        }

       
        

        public override string ProtocolId => GnssProtocolId;

        public override bool Read(byte data)
        {
            switch (_state)
            {
                case State.Sync:
                    if (data == RtcmV3Helper.SyncByte)
                    {
                        _state = State.Preamb1;
                        _buffer[0] = data;
                    }
                    break;
                case State.Preamb1:
                    _buffer[1] = data;
                    _state = State.Preamb2;
                    break;
                case State.Preamb2:
                    _buffer[2] = data;
                    _state = State.Payload;
                    _payloadLength = (ushort)BitHelper.GetBitU(_buffer, 14 /* preamble-8bit + reserved-6bit */, 10 /* length-10bit */);
                    _payloadReadedBytes = 0;
                    if (_payloadLength > _buffer.Length)
                    {
                        // buffer oversize
                        Reset();
                    }
                    break;
                case State.Payload:
                    // read payload
                    if (_payloadReadedBytes < _payloadLength)
                    {
                        _buffer[_payloadReadedBytes + 3] = data;
                        ++_payloadReadedBytes;
                    }
                    else
                    {
                        _state = State.Crc1;
                        goto case State.Crc1;
                    }
                    break;
                case State.Crc1:
                    _buffer[_payloadLength + 3] = data;
                    _state = State.Crc2;
                    break;
                case State.Crc2:
                    _buffer[_payloadLength + 3 + 1] = data;
                    _state = State.Crc3;
                    break;
                case State.Crc3:
                    _buffer[_payloadLength + 3 + 2] = data;

                    var originalCrc = RtcmV3Crc24.Calc(_buffer, _payloadLength + 3, 0);
                    var sourceCrc = BitHelper.GetBitU(_buffer, (uint)((_payloadLength + 3) * 8), 24);
                    if (originalCrc == sourceCrc)
                    {
                        var msgNumber = (ushort)BitHelper.GetBitU(_buffer, 24 /* preamble-8bit + reserved-6bit + length-10bit */, 12);
                        var span = new ReadOnlySpan<byte>(_buffer,0,_payloadLength + 6 /* preamble-8bit + reserved-6bit + length-10bit */ + 3 /*CRC24*/);
                        if (_onRawData.HasObservers)
                        {
                            _onRawData.OnNext(new RtcmV3RawMessage(msgNumber,span));
                        }
                        ParsePacket(msgNumber, ref span,true);
                        Reset();
                        return true;
                    }
                    PublishWhenCrcError();
                    Reset();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        public override void Reset()
        {
            _state = State.Sync;
        }

        public IObservable<RtcmV3RawMessage> OnRawMessage => _onRawData;

        protected override void InternalDisposeOnce()
        {
            base.InternalDisposeOnce();
            _onRawData.OnCompleted();
            _onRawData.Dispose();
        }
    }
}