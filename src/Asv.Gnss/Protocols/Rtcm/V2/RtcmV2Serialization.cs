using System;
using Asv.IO;

namespace Asv.Gnss;

internal static class RtcmV2Serialization
{
    public static void WriteUnsigned(Span<byte> buffer, ref int bitIndex, int bits, double value, double scale = 1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(bits, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(bits, 32);
        var raw = Quantize(value, scale);
        var maximum = bits == 32 ? uint.MaxValue : (1L << bits) - 1;
        if (value < 0 || raw < 0 || raw > maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"The value does not fit an unsigned {bits}-bit field.");
        }

        SpanBitHelper.SetBitU(buffer, ref bitIndex, bits, (uint)raw);
    }

    public static void WriteSigned(Span<byte> buffer, ref int bitIndex, int bits, double value, double scale = 1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(bits, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(bits, 32);
        var raw = Quantize(value, scale);
        var limit = 1L << (bits - 1);
        if (raw < -limit || raw >= limit)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"The value does not fit a signed {bits}-bit field.");
        }

        SpanBitHelper.SetBitS(buffer, ref bitIndex, bits, (int)raw);
    }

    private static double Quantize(double value, double scale)
    {
        if (!double.IsFinite(value) || !double.IsFinite(scale) || scale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "A finite value and a finite positive scale are required.");
        }

        var raw = Math.Round(value / scale, MidpointRounding.AwayFromZero);
        if (!double.IsFinite(raw))
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        return raw;
    }
}
