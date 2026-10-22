using System;
using Asv.IO;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Gnss
{
    /// <summary>
    /// Streaming parser for Septentrio Binary Format blocks.
    /// </summary>
    public class SbfBinaryParser : ProtocolParser<SbfMessageBase, ushort>
    {
        public const string GnssProtocolId = "SBF";
        public const int MaxPacketSize = 8192;

        private readonly byte[] _buffer = new byte[MaxPacketSize];
        private readonly ILogger<SbfBinaryParser> _logger;
        private State _state;
        private int _bufferIndex;
        private ushort _crc;
        private ushort _msgId;
        private ushort _length;

        public SbfBinaryParser(
            IProtocolMessageFactory<SbfMessageBase, ushort> messageFactory,
            IProtocolContext context,
            IStatisticHandler? statisticHandler) : base(messageFactory, context, statisticHandler)
        {
            _logger = context.LoggerFactory.CreateLogger<SbfBinaryParser>();
        }

        public override ProtocolInfo Info => SbfProtocol.Info;

        public override bool Push(byte data)
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
                        Reset();
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
                        Reset();
                        return false;
                    }

                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == 8)
                    {
                        _crc = BitConverter.ToUInt16(_buffer, 2);
                        _msgId = BitConverter.ToUInt16(_buffer, 4);
                        _length = BitConverter.ToUInt16(_buffer, 6);
                        if (_length < 8 || _length > MaxPacketSize || _length % 4 != 0)
                        {
                            Reset();
                            return false;
                        }

                        _state = State.Message;
                    }

                    break;
                case State.Message:
                    if (_bufferIndex >= _buffer.Length)
                    {
                        Reset();
                        return false;
                    }

                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == _length)
                    {
                        var calculatedHash = SbfCrc16.checksum(_buffer, 4, _length - 4);
                        if (calculatedHash == _crc)
                        {
                            var span = new ReadOnlySpan<byte>(_buffer, 0, _length);
                            try
                            {
                                InternalParsePacket(_msgId, ref span);
                            }
                            catch (Exception ex)
                            {
                                _logger.ZLogTrace($"{ex.Message}[SbfMessage{_msgId}]");
                                InternalOnError(new ProtocolParserException(Info, "Parser ", ex));
                                Reset();
                                return false;
                            }

                            Reset();
                            return true;
                        }

                        InternalOnError(new ProtocolParserException(Info, "CRC error"));
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
            _bufferIndex = 0;
            _crc = 0;
            _msgId = 0;
            _length = 0;
        }

        private enum State
        {
            Sync1,
            Sync2,
            CrcAndIdAndLength,
            Message
        }
    }
}
