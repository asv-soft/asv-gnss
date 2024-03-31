using System;

namespace Asv.Gnss
{
    public class ComNavBinaryParser : GnssMessageParserBase<ComNavBinaryMessageBase, ushort>
    {
        public const string GnssProtocolId = "ComNavBinary";
        public const int MaxPacketSize = 1024 * 10;
        public const byte FirstSyncByte = 0xAA;
        public const byte SecondSyncByte = 0x44;
        public const byte ThirdSyncByte = 0x12;


        private State _state;
        private readonly byte[] _buffer = new byte[MaxPacketSize];
        private int _bufferIndex = 0;
        private byte _headerLength;
        private ushort _messageLength;
        private int _stopMessageIndex;

        

        private enum State
        {
            Sync1,
            Sync2,
            Sync3,
            HeaderLength,
            Header,
            Message
        }

        public override string ProtocolId => GnssProtocolId;
        

        public override bool Read(byte data)
        {
            switch (_state)
            {
                case State.Sync1:
                    if (data != FirstSyncByte) return false;
                    _bufferIndex = 0;
                    _buffer[_bufferIndex++] = FirstSyncByte;
                    _state = State.Sync2;
                    break;
                case State.Sync2:
                    if (data != SecondSyncByte)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.Sync3;
                        _buffer[_bufferIndex++] = SecondSyncByte;
                    }
                    break;
                case State.Sync3:
                    if (data != ThirdSyncByte)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.HeaderLength;
                        _buffer[_bufferIndex++] = ThirdSyncByte;
                    }
                    break;
                case State.HeaderLength:
                    _headerLength = data;
                    _buffer[_bufferIndex++] = data;
                    _state = State.Header;
                    break;
                case State.Header:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == _headerLength)
                    {
                        _messageLength = BitConverter.ToUInt16(_buffer, 8);
                        _stopMessageIndex = _headerLength + _messageLength + 4 /* CRC 32 bit*/;
                        _state = State.Message;
                    }
                    break;
                case State.Message:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == _stopMessageIndex)
                    {
                        /* step back to last byte */
                        var crc32Index = _bufferIndex - 4 /* CRC32 */;
                        var calculatedHash = ComNavCrc32.Calc(_buffer, 0, crc32Index);
                        var readedHash = BitConverter.ToUInt32(_buffer, crc32Index);
                        if (calculatedHash == readedHash)
                        {
                            var msgId = BitConverter.ToUInt16(_buffer, 4);
                            var span = new ReadOnlySpan<byte>(_buffer, 0, _stopMessageIndex);
                            if (_messageLength == 5328)
                            {
                                
                            }
                            ParsePacket(msgId, ref span);
                            Reset();
                            return true;
                        }
                        PublishWhenCrcError();
                        Reset();
                    }
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