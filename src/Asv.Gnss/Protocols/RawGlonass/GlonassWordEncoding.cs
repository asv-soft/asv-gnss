using System;
using Asv.IO;

namespace Asv.Gnss;

internal static class GlonassWordEncoding
{
    internal static Field Field(string name, IFieldType type, string? units = null)
    {
        var builder = new Field.Builder().Name(name).Title(name).DataType(type);
        if (units is not null)
            builder.Units(units);
        return builder.Build();
    }

    internal static uint Read(ReadOnlySpan<byte> data, int position, int length)
    {
        uint value = 0;
        for (var index = position; index < position + length; index++)
            value = (value << 1) | (uint)((data[index / 8] >> (7 - index % 8)) & 1);
        return value;
    }

    internal static void Unsigned(
        Span<byte> data,
        int position,
        int length,
        ulong value,
        string name
    )
    {
        if (value >= (1UL << length))
            throw new ArgumentOutOfRangeException(
                name,
                $"The value does not fit in {length} bits."
            );
        for (var index = length - 1; index >= 0; index--)
        {
            var offset = position + length - 1 - index;
            var mask = (byte)(1 << (7 - offset % 8));
            if (((value >> index) & 1) != 0)
                data[offset / 8] |= mask;
            else
                data[offset / 8] &= (byte)~mask;
        }
    }

    internal static void ScaledUnsigned(
        Span<byte> data,
        int position,
        int length,
        double value,
        double scale,
        string name
    )
    {
        var encoded = Math.Round(value / scale, MidpointRounding.AwayFromZero);
        if (!double.IsFinite(encoded) || encoded < 0 || encoded >= (1UL << length))
            throw new ArgumentOutOfRangeException(name);
        Unsigned(data, position, length, (ulong)encoded, name);
    }

    internal static void Signed(
        Span<byte> data,
        int position,
        int length,
        double value,
        double scale,
        string name
    )
    {
        var magnitude = Math.Round(Math.Abs(value / scale), MidpointRounding.AwayFromZero);
        if (!double.IsFinite(magnitude) || magnitude >= (1UL << (length - 1)))
            throw new ArgumentOutOfRangeException(name);
        var negative = BitConverter.DoubleToInt64Bits(value) < 0;
        Unsigned(
            data,
            position,
            length,
            (ulong)magnitude | (negative ? 1UL << (length - 1) : 0),
            name
        );
    }

    internal static void Time(Span<byte> data, double seconds)
    {
        var original =
            Read(data, 12, 5) * 3600.0 + Read(data, 17, 6) * 60.0 + Read(data, 23, 1) * 30.0;
        if (seconds == original)
            return;
        if (!double.IsFinite(seconds) || seconds < 0 || seconds >= 86400 || seconds % 30 != 0)
            throw new ArgumentOutOfRangeException(nameof(GlonassWord1.TofLocalSec));
        var value = (uint)seconds;
        Unsigned(data, 12, 5, value / 3600, nameof(GlonassWord1.TofLocalSec));
        Unsigned(data, 17, 6, value % 3600 / 60, nameof(GlonassWord1.TofLocalSec));
        Unsigned(data, 23, 1, value % 60 / 30, nameof(GlonassWord1.TofLocalSec));
    }

    internal static void Accuracy(Span<byte> data, double value)
    {
        ReadOnlySpan<double> values = [1, 2, 2.5, 4, 5, 7, 10, 12, 14, 16, 32, 64, 128, 256, 512];
        var index = double.IsNaN(value) ? 15 : values.IndexOf(value);
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(GlonassWord4.Accuracy));
        Unsigned(data, 55, 4, (uint)index, nameof(GlonassWord4.Accuracy));
    }

    internal static void Inclination(Span<byte> data, double value)
    {
        var raw = Read(data, 47, 18);
        var magnitude = (raw & 0x1FFFF) * GlonassRawHelper.P2_20;
        var original = (63.0 / 180.0 + ((raw & 0x20000) != 0 ? -magnitude : magnitude)) * Math.PI;
        // The physical inclination cannot distinguish the two sign-magnitude representations of zero.
        if (value == original)
            return;
        Signed(
            data,
            47,
            18,
            value / Math.PI - 63.0 / 180.0,
            GlonassRawHelper.P2_20,
            nameof(GlonassWordEven.i)
        );
    }

    internal static void Frequency(Span<byte> data, long value)
    {
        var delta = checked(value - 1602000000);
        if (delta % 562500 != 0)
            throw new ArgumentOutOfRangeException(nameof(GlonassWordOdd.Frequency));
        var channel = delta / 562500;
        if (channel is < -7 or > 24)
            throw new ArgumentOutOfRangeException(nameof(GlonassWordOdd.Frequency));
        Unsigned(
            data,
            74,
            5,
            (ulong)(channel < 0 ? channel + 32 : channel),
            nameof(GlonassWordOdd.Frequency)
        );
    }
}
