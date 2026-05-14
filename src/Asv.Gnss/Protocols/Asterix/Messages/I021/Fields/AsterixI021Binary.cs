using System;

namespace Asv.Gnss;

internal static class AsterixI021Binary
{
    public static uint ReadUInt24(ref ReadOnlySpan<byte> buffer)
    {
        var value = (uint)((buffer[0] << 16) | (buffer[1] << 8) | buffer[2]);
        buffer = buffer[3..];
        return value;
    }

    public static int ReadInt24(ref ReadOnlySpan<byte> buffer)
    {
        var value = (int)ReadUInt24(ref buffer);
        if ((value & 0x800000) != 0)
        {
            value |= unchecked((int)0xFF000000);
        }

        return value;
    }

    public static void WriteUInt24(ref Span<byte> buffer, uint value)
    {
        buffer[0] = (byte)(value >> 16);
        buffer[1] = (byte)(value >> 8);
        buffer[2] = (byte)value;
        buffer = buffer[3..];
    }

    public static void WriteInt24(ref Span<byte> buffer, int value)
    {
        WriteUInt24(ref buffer, (uint)(value & 0x00FFFFFF));
    }

    public static int SignExtend(int value, int bits)
    {
        var shift = 32 - bits;
        return (value << shift) >> shift;
    }

    public static double ReadTimeSeconds(ref ReadOnlySpan<byte> buffer)
    {
        return ReadUInt24(ref buffer) / 128.0;
    }

    public static void WriteTimeSeconds(ref Span<byte> buffer, double seconds)
    {
        WriteUInt24(ref buffer, (uint)Math.Round(seconds * 128.0));
    }
}