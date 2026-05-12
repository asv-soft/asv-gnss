using System;
using Asv.IO;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Gnss;

public class RtcmV2Parser : ProtocolParser<RtcmV2MessageBase, ushort>
{
    public const string GnssProtocolId = RtcmV2Protocol.GnssProtocolId;
    public const byte SyncByte = RtcmV2Protocol.SyncByte;

    private readonly byte[] _buffer = new byte[RtcmV2Protocol.MaxMessageSize];
    private readonly ILogger<RtcmV2Parser> _logger;
    private uint _word;
    private int _readBytes;
    private int _readBits;
    private int _messageLength;

    public RtcmV2Parser(
        IProtocolMessageFactory<RtcmV2MessageBase, ushort> messageFactory,
        IProtocolContext context,
        IStatisticHandler? statisticHandler) : base(messageFactory, context, statisticHandler)
    {
        _logger = context.LoggerFactory.CreateLogger<RtcmV2Parser>();
    }

    public override bool Push(byte data)
    {
        if ((data & 0xC0) != 0x40)
        {
            return false;
        }

        for (var i = 0; i < 6; i++, data >>= 1)
        {
            _word = (uint)((_word << 1) + (data & 1));

            if (_readBytes == 0)
            {
                var preamble = (byte)(_word >> 22);
                if ((_word & 0x40000000) != 0)
                {
                    preamble ^= 0xFF;
                }

                if (preamble != SyncByte)
                {
                    continue;
                }

                if (!DecodeWord(_word, _buffer, 0))
                {
                    continue;
                }

                _readBytes = 3;
                _readBits = 0;
                continue;
            }

            if (++_readBits < 30)
            {
                continue;
            }

            _readBits = 0;
            if (!DecodeWord(_word, _buffer, _readBytes))
            {
                InternalOnError(new ProtocolParserException(Info, "RTCMv2 parity error"));
                _readBytes = 0;
                _word &= 0x3;
                continue;
            }

            _readBytes += 3;
            if (_readBytes == 6)
            {
                _messageLength = (_buffer[5] >> 3) * 3 + 6;
            }

            if (_readBytes < _messageLength)
            {
                continue;
            }

            _readBytes = 0;
            _word &= 0x3;
            var bitIndex = 8;
            var msgType = (ushort)SpanBitHelper.GetBitU(_buffer, ref bitIndex, 6);
            var span = new ReadOnlySpan<byte>(_buffer, 0, _messageLength);
            try
            {
                InternalParsePacket(msgType, ref span);
                Reset();
                return true;
            }
            catch (ProtocolParserException ex)
            {
                _logger.ZLogTrace($"{ex.Message}[RtcmV2Message{msgType}]");
                InternalOnError(ex);
                Reset();
                return false;
            }
            catch (Exception ex)
            {
                _logger.ZLogTrace($"{ex.Message}[RtcmV2Message{msgType}]");
                InternalOnError(new ProtocolParserException(Info, "Parser ", ex));
                Reset();
                return false;
            }
        }

        return false;
    }

    private static bool DecodeWord(uint word, byte[] data, int offset)
    {
        uint[] hamming = [0xBB1F3480, 0x5D8F9A40, 0xAEC7CD00, 0x5763E680, 0x6BB1F340, 0x8B7A89C0];
        uint parity = 0;

        if ((word & 0x40000000) != 0)
        {
            word ^= 0x3FFFFFC0;
        }

        for (var i = 0; i < 6; i++)
        {
            parity <<= 1;
            for (var w = (word & hamming[i]) >> 6; w != 0; w >>= 1)
            {
                parity ^= w & 0x1;
            }
        }

        if (parity != (word & 0x3F))
        {
            return false;
        }

        for (var i = 0; i < 3; i++)
        {
            data[i + offset] = (byte)(word >> (22 - i * 8));
        }

        return true;
    }

    public override void Reset()
    {
        _readBytes = 0;
        _readBits = 0;
        _messageLength = 0;
    }

    public override ProtocolInfo Info => RtcmV2Protocol.Info;
}
