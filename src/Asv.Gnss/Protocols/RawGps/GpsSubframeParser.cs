using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Searches an MSB-first bit stream for parity-valid 300-bit LNAV subframes.</summary>
public class GpsSubframeParser(
    IProtocolMessageFactory<GpsSubframeBase, byte> factory,
    IProtocolContext context,
    IStatisticHandler? statistics
) : ProtocolParser<GpsSubframeBase, byte>(factory, context, statistics)
{
    private readonly byte[] _window = new byte[RawGpsProtocol.PacketSize];
    private int _bits;
    public override ProtocolInfo Info => RawGpsProtocol.Info;

    public override bool Push(byte data)
    {
        var parsed = false;
        Span<byte> source = stackalloc byte[30];
        for (var bit = 7; bit >= 0; bit--)
        {
            for (var index = 0; index < _window.Length - 1; index++)
                _window[index] = (byte)((_window[index] << 1) | (_window[index + 1] >> 7));
            _window[^1] = (byte)((_window[^1] << 1) | ((data >> bit) & 1));
            _window[0] &= 15;
            _bits = Math.Min(RawGpsProtocol.SubframeBitSize, _bits + 1);
            if (_bits != RawGpsProtocol.SubframeBitSize)
                continue;
            var preamble = RawGpsCodec.Read(_window, 4, 8);
            if (preamble is not (0x8B or 0x74) || !RawGpsCodec.TryDecode(_window, source, out _))
                continue;
            ReadOnlySpan<byte> frame = _window;
            InternalParsePacket((byte)((source[5] >> 2) & 7), ref frame);
            _bits = 0;
            Array.Clear(_window);
            parsed = bit == 0;
        }
        return parsed;
    }

    public override void Reset()
    {
        _bits = 0;
        Array.Clear(_window);
    }
}
