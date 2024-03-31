using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    public class AsvMessageParser : GnssMessageParserBase<AsvMessageBase, ushort>
    {
        public const string GnssProtocolId = "Asv";
        public const ushort HeaderSize = 10;
        public const ushort CrcSize = sizeof(ushort); /*CRC16*/
        public const ushort DataSize = 1012; /*DATA*/
        public const ushort MaxMessageSize = DataSize + HeaderSize + CrcSize/*CRC16*/;
        public const byte Sync1 = 0xAA;
        public const byte Sync2 = 0x44;

        public override string ProtocolId => GnssProtocolId;

        private readonly byte[] _buffer = new byte[MaxMessageSize];
        private State _state;
        private int _bufferIndex;
        private int _stopIndex;

        public override bool Read(byte data)
        {
            switch (_state)
            {
                case State.Sync1:
                    if (data != Sync1) return false;
                    _bufferIndex = 0;
                    _buffer[_bufferIndex++] = Sync1;
                    _state = State.Sync2;
                    break;
                case State.Sync2:
                    if (data != Sync2)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.MessageLength;
                        _buffer[_bufferIndex++] = Sync2;
                    }
                    break;
                case State.MessageLength:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == 4)
                    {
                        var length = (int)GetBitU(_buffer, 2 * 8, 16);
                        var length1 = BitConverter.ToUInt16(_buffer, 2);
                        var length2 = (int)GetBitUBE(_buffer, 2 * 8, 16);
                        _stopIndex = length + 12; // 10 header + 2 crc = 12
                        // _stopIndex = BitConverter.ToUInt16(_buffer, 2) + 12; // 10 header + 2 crc = 12
                        _state = _stopIndex >= _buffer.Length ? State.Sync1 : State.Message;
                    }
                    break;
                case State.Message:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == _stopIndex)
                    {
                        var crc = (int)GetBitU(_buffer, (_stopIndex - 2) * 8, 16);
                        var crc1 = BitConverter.ToUInt16(_buffer, _stopIndex - 2);
                        var crc2 = (int)GetBitUBE(_buffer, (_stopIndex - 2) * 8, 16);
                        var calcCrc = AsvCrc16.Calc(_buffer, 0, _stopIndex - 2);
                        if (calcCrc != crc)
                        {
                            PublishWhenCrcError();
                            Reset();
                        }
                        else
                        {
                            var msgId = (int)GetBitU(_buffer, 8 * 8, 16);
                            var msgId1 = BitConverter.ToUInt16(_buffer, 8);
                            var msgId2 = (int)GetBitUBE(_buffer, 8 * 8, 16);
                            var span = new ReadOnlySpan<byte>(_buffer, 0, _stopIndex);
                            ParsePacket((ushort)msgId, ref span);
                            Reset();
                            return true;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        
        private uint GetBitU(IReadOnlyList<byte> buff, int pos, int len)
        {
            uint bits=0;
            for (var i = pos; i < pos + len; i++)
            {
                bits = (uint)((bits << 1) + ((buff[i / 8] >> (7 - i % 8)) & 1u));
            }
            return bits;
        }
        
        
        private uint GetBitUBE(IReadOnlyList<byte> buff, int pos, int len)
        {
            uint bits=0;
            var buffPos = pos + len - 1;
            for (var i = pos; i < pos + len; i++, buffPos--)
            {
                bits = (uint)((bits << 1) + ((buff[buffPos / 8] >> (7 - i % 8)) & 1u));
            }
            return bits;
        }
        
        private void SetBitU(IList<byte> buff, int pos, int len, uint data)
        {
            var mask = 1u << (len - 1);
            if (len is <= 0 or > 32) return;
            var buffPos = pos + len - 1;
            for (var i = pos; i < pos + len; i++, mask >>= 1, buffPos--)
            {
                if ((data & mask) != 0) buff[buffPos / 8] |= (byte)(1u << (7 - i % 8));
                else buff[buffPos / 8] &= (byte)~(1u << (7 - i % 8));
            }
        }
        
        public override void Reset()
        {
            _state = State.Sync1;
        }

        private enum State
        {
            Sync1,
            Sync2,
            MessageLength,
            Message,
        }
    }
}