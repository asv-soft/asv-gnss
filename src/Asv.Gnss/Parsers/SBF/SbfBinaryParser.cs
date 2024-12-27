using System;

namespace Asv.Gnss
{
    public class SbfBinaryParser : GnssMessageParserBase<SbfMessageBase, ushort>
    {
        public static string GnssProtocolId => "SBF";
        public const int MaxPacketSize = 8192;
        private State _state;
        private readonly byte[] _buffer = new byte[MaxPacketSize];
        private int _bufferIndex = 0;
        private ushort _crc;
        private ushort _msgId;
        private ushort _length;

        public override string ProtocolId => GnssProtocolId;

        private enum State
        {
            Sync1,
            Sync2,
            CrcAndIdAndLength,
            Message,
        }

        public override bool Read(byte data)
        {
            switch (_state)
            {
                case State.Sync1:
                    if (data != 0x24)
                    {
                        return false;
                    }

                    _bufferIndex = 0;
                    _buffer[_bufferIndex++] = 0x24;
                    _state = State.Sync2;
                    break;
                case State.Sync2:
                    if (data != 0x40)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.CrcAndIdAndLength;
                        _buffer[_bufferIndex++] = 0x40;
                    }

                    break;
                case State.CrcAndIdAndLength:
                    if (_bufferIndex >= _buffer.Length)
                    {
                        _state = State.Sync1;
                        return false;
                    }

                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == 8)
                    {
                        _crc = BitConverter.ToUInt16(_buffer, 2);
                        _msgId = BitConverter.ToUInt16(_buffer, 4);
                        _length = BitConverter.ToUInt16(_buffer, 6);
                        _state = State.Message;
                    }

                    break;
                case State.Message:
                    if (_bufferIndex >= _buffer.Length)
                    {
                        _state = State.Sync1;
                        return false;
                    }

                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == _length)
                    {
                        var calculatedHash = SbfCrc16.checksum(_buffer, 4, _length - 4);
                        if (calculatedHash == _crc)
                        {
                            var span = new ReadOnlySpan<byte>(_buffer, 0, _length);
                            ParsePacket(_msgId, ref span, true);
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
