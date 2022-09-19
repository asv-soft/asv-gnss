using System;

namespace Asv.Gnss
{
    public class UbxBinaryParser : GnssMessageParserBase<UbxMessageBase, ushort>
    {
        public const string GnssProtocolId = "UBX";
        public override string ProtocolId => GnssProtocolId;

        public const int MaxPacketSize = 1024 * 8;
        private readonly byte[] _buffer = new byte[MaxPacketSize];
        private int _bufferIndex = 0;

        private State _state = State.Sync1;
        private int _payloadLength = 0;
        private int _payloadReadBytes = 0;

       

        private enum State
        {
            Sync1,
            Sync2,
            IdAndLength,
            Payload,
            Crc1,
            Crc2
        }

        public override bool Read(byte data)
        {
            switch (_state)
            {
                case State.Sync1:
                    if (data != UbxHelper.SyncByte1) break;
                    _bufferIndex = 0;
                    _buffer[_bufferIndex++] = UbxHelper.SyncByte1;
                    _state = State.Sync2;
                    break;
                case State.Sync2:
                    if (data != UbxHelper.SyncByte2)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.IdAndLength;
                        _buffer[_bufferIndex++] = UbxHelper.SyncByte2;
                    }
                    break;
                case State.IdAndLength:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == 6)
                    {
                        _payloadLength = _buffer[4] | (_buffer[5] << 8);
                        _state = State.Payload;
                        _payloadReadBytes = 0;
                        // reset on oversize packet
                        if (_payloadLength > _buffer.Length)
                        {
                            // buffer oversize
                            Reset();
                        }
                    }
                    break;
                case State.Payload:
                    // read payload
                    if (_payloadReadBytes < _payloadLength)
                    {
                        _buffer[_payloadReadBytes + 6] = data;
                        _payloadReadBytes++;

                        if (_payloadReadBytes == _payloadLength)
                            _state = State.Crc1;
                    }
                    break;
                case State.Crc1:
                    _buffer[_payloadReadBytes + 6] = data;
                    _state = State.Crc2;
                    break;
                case State.Crc2:
                    _buffer[_payloadReadBytes + 6 + 1] = data;
                    var originalCrc = UbxCrc16.Calc(new ReadOnlySpan<byte>(_buffer,2,_payloadLength + 4 /*ID + Length*/));
                    var sourceCrc = new[] { _buffer[_payloadReadBytes + 6], data };

                    if (originalCrc.Crc1 == sourceCrc[0] && originalCrc.Crc2 == sourceCrc[1])
                    {
                        var msgId = UbxHelper.ReadMessageId(_buffer);
                        var span = new ReadOnlySpan<byte>(_buffer,0, _payloadReadBytes + 8);
                        ParsePacket(msgId,ref span);
                        return true;
                    }
                    else
                    {
                        PublishWhenCrcError();
                    }
                    Reset();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        public override void Reset()
        {
            _state = State.Sync1;
        }

        

    }
}