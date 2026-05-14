using System;

namespace Asv.Gnss;

internal static class AsterixI048Binary
{
    public static uint ReadUInt24(ref ReadOnlySpan<byte> buffer)
    {
        var value = (uint)((buffer[0] << 16) | (buffer[1] << 8) | buffer[2]);
        buffer = buffer[3..];
        return value;
    }

    public static void WriteUInt24(ref Span<byte> buffer, uint value)
    {
        buffer[0] = (byte)(value >> 16);
        buffer[1] = (byte)(value >> 8);
        buffer[2] = (byte)value;
        buffer = buffer[3..];
    }

    public static int SignExtend(int value, int bitCount)
    {
        var shift = 32 - bitCount;
        return (value << shift) >> shift;
    }
}