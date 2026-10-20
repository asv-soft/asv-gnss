using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Decodes byte-aligned VDB bursts and publishes their typed GBAS application blocks.
/// The input is the Data field of an ASV GbasVdbSendV2 message, without its ASV envelope.
/// </summary>
public class GbasMessageParser : ProtocolParser<GbasMessageBlockBase, byte>
{
    private static ReadOnlySpan<byte> SyncBytes => [0x09, 0xE0, 0x6E, 0x31, 0xFB, 0xE2];
    private readonly byte[] _buffer = new byte[GbasProtocol.MaxPacketSize];
    private int _index;
    private int _packetSize;

    public GbasMessageParser(
        IProtocolMessageFactory<GbasMessageBlockBase, byte> messageFactory,
        IProtocolContext context,
        IStatisticHandler? statisticHandler) : base(messageFactory, context, statisticHandler)
    {
    }

    public override ProtocolInfo Info => GbasProtocol.Info;

    public override bool Push(byte data)
    {
        if (_index < SyncBytes.Length)
        {
            if (data != SyncBytes[_index])
            {
                Reset();
                // A mismatch can also be the beginning of the next sync sequence.
                if (data == SyncBytes[0]) _buffer[_index++] = data;
                return false;
            }
            _buffer[_index++] = data;
            return false;
        }

        _buffer[_index++] = data;
        try
        {
            if (_index == 10)
            {
                _packetSize = (GbasMessage.GetFrameBitSize(_buffer.AsSpan(0, _index)) + 7) / 8;
                if (_packetSize > _buffer.Length)
                    throw new FormatException("GBAS burst exceeds the maximum packet size.");
            }
            if (_packetSize == 0 || _index < _packetSize) return false;

            var application = GbasMessage.DecodeApplicationData(_buffer.AsSpan(0, _packetSize), out _);
            // Validate all block boundaries before publishing any message from this burst.
            ValidateBlocks(application);
            ReadOnlySpan<byte> remaining = application;
            while (!remaining.IsEmpty)
            {
                var bit = 32;
                var type = (byte)SpanBitHelper.GetBitUReverse(remaining, ref bit, 8);
                var length = (int)SpanBitHelper.GetBitUReverse(remaining, ref bit, 8);
                var block = remaining[..length];
                InternalParsePacket(type, ref block);
                remaining = remaining[length..];
            }
            Reset();
            return true;
        }
        catch (ProtocolParserException ex)
        {
            InternalOnError(ex);
        }
        catch (Exception ex)
        {
            InternalOnError(new ProtocolParserException(Info, "Invalid GBAS burst.", ex));
        }
        Reset();
        return false;
    }

    private static void ValidateBlocks(ReadOnlySpan<byte> application)
    {
        while (!application.IsEmpty)
        {
            if (application.Length < 10)
                throw new FormatException("Truncated GBAS application block.");
            var bit = 0;
            var marker = (MessageBlockIdEnum)SpanBitHelper.GetBitUReverse(application, ref bit, 8);
            if (marker is not (MessageBlockIdEnum.Normal or MessageBlockIdEnum.Test))
                throw new FormatException("Invalid GBAS application block identifier.");
            bit = 40;
            var length = (int)SpanBitHelper.GetBitUReverse(application, ref bit, 8);
            if (length < 10 || length > application.Length)
                throw new FormatException("Invalid GBAS application block length.");
            var crcBit = length * 8 - 32;
            var crc = GbasMessageBlockBase.Crc32QCalc(application, 0, crcBit);
            if (crc != SpanBitHelper.GetBitUReverse(application, ref crcBit, 32))
                throw new ProtocolParserCrcException(GbasProtocol.Info);
            application = application[length..];
        }
    }

    public override void Reset()
    {
        _index = 0;
        _packetSize = 0;
    }
}
