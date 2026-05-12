using System;
using Asv.IO;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Gnss;

public class AsvMessageParser : ProtocolParser<AsvMessageBase, ushort>
{
    public const string GnssProtocolId = "Asv";
    public const ushort MaxMessageSize = AsvProtocol.MaxPacketSize;
    public const byte Sync1 = AsvProtocol.SyncByte1;
    public const byte Sync2 = AsvProtocol.SyncByte2;

    private readonly byte[] _buffer = new byte[AsvProtocol.MaxPacketSize];
    private readonly ILogger<AsvMessageParser> _logger;
    private State _state = State.Sync1;
    private int _bufferIndex;
    private int _packetSize;

    public AsvMessageParser(
        IProtocolMessageFactory<AsvMessageBase, ushort> messageFactory,
        IProtocolContext context,
        IStatisticHandler? statisticHandler) : base(messageFactory, context, statisticHandler)
    {
        _logger = context.LoggerFactory.CreateLogger<AsvMessageParser>();
    }

    public override bool Push(byte data)
    {
        switch (_state)
        {
            case State.Sync1:
                if (data != AsvProtocol.SyncByte1)
                {
                    return false;
                }

                _bufferIndex = 0;
                _buffer[_bufferIndex++] = data;
                _state = State.Sync2;
                break;
            case State.Sync2:
                if (data != AsvProtocol.SyncByte2)
                {
                    Reset();
                    return false;
                }

                _buffer[_bufferIndex++] = data;
                _state = State.PayloadLength;
                break;
            case State.PayloadLength:
                _buffer[_bufferIndex++] = data;
                if (_bufferIndex == 4)
                {
                    var payloadLength = AsvProtocol.ReadPayloadLength(_buffer);
                    _packetSize = payloadLength + AsvProtocol.HeaderSize + AsvProtocol.CrcSize;
                    if (payloadLength > AsvProtocol.MaxPayloadSize || _packetSize > _buffer.Length)
                    {
                        Reset();
                        return false;
                    }

                    _state = State.Message;
                }

                break;
            case State.Message:
                _buffer[_bufferIndex++] = data;
                if (_bufferIndex == _packetSize)
                {
                    var msgId = AsvProtocol.ReadMessageId(_buffer);
                    var spanMsg = new ReadOnlySpan<byte>(_buffer, 0, _packetSize);
                    try
                    {
                        InternalParsePacket(msgId, ref spanMsg);
                        Reset();
                        return true;
                    }
                    catch (ProtocolParserException ex)
                    {
                        _logger.ZLogTrace($"{ex.Message}[AsvMessage{msgId}]");
                        InternalOnError(ex);
                        Reset();
                        return false;
                    }
                    catch (Exception ex)
                    {
                        _logger.ZLogTrace($"{ex.Message}[AsvMessage{msgId}]");
                        InternalOnError(new ProtocolParserException(Info, "Parser ", ex));
                        Reset();
                        return false;
                    }
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    public override void Reset()
    {
        _bufferIndex = 0;
        _packetSize = 0;
        _state = State.Sync1;
    }

    public override ProtocolInfo Info => AsvProtocol.Info;

    private enum State
    {
        Sync1,
        Sync2,
        PayloadLength,
        Message
    }
}
