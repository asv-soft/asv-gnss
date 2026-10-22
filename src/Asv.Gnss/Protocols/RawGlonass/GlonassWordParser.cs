using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Scans decoded MSB-first navigation bits for complete eighty-five-bit GLONASS strings.
/// There is no unique preamble: use this parser only for a dedicated GLONASS navigation stream.
/// </summary>
public sealed class GlonassWordParser : ProtocolParser<GlonassWordBase, byte>
{
    private static readonly UInt128 StringMask =
        ((UInt128)1 << RawGlonassProtocol.StringBitSize) - 1;
    private UInt128 _bits;
    private int _count;

    public GlonassWordParser(
        IProtocolMessageFactory<GlonassWordBase, byte> messageFactory,
        IProtocolContext context,
        IStatisticHandler? statisticHandler
    )
        : base(messageFactory, context, statisticHandler) { }

    public override ProtocolInfo Info => RawGlonassProtocol.Info;

    public override bool Push(byte data)
    {
        // Dispatchers reset all parsers after true; preserve a following partial string in this byte.
        var completedAtByteBoundary = false;
        Span<byte> packet = stackalloc byte[RawGlonassProtocol.PacketSize];
        for (var position = 7; position >= 0; position--)
        {
            _bits = ((_bits << 1) | (uint)((data >> position) & 1)) & StringMask;
            if (_count < RawGlonassProtocol.StringBitSize)
                _count++;
            if (_count < RawGlonassProtocol.StringBitSize)
                continue;
            var id = (byte)(_bits >> 80);
            if (id is < 1 or > 15 || GlonassWordChecksum.Compute(_bits) != (byte)_bits)
                continue;
            var value = _bits;
            for (var index = packet.Length - 1; index >= 0; index--)
            {
                packet[index] = (byte)value;
                value >>= 8;
            }
            ReadOnlySpan<byte> read = packet;
            InternalParsePacket(id, ref read);
            Reset();
            completedAtByteBoundary = position == 0;
        }
        return completedAtByteBoundary;
    }

    public override void Reset()
    {
        _bits = 0;
        _count = 0;
    }
}
