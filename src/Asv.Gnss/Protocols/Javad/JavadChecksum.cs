using System;

namespace Asv.Gnss;

public enum JavadChecksumKind
{
    None,
    Binary8,
    AsciiHex8,
    Crc16,
}

public enum JavadByteOrder
{
    LittleEndian,
    BigEndian,
}

/// <summary>GREIS 4.2.01 Appendix A.1 checksums, including the final rotation.</summary>
public static class JavadChecksum
{
    public static byte Compute8(ReadOnlySpan<byte> data)
    {
        byte result = 0;
        foreach (var value in data)
        {
            result = (byte)(((result << 2) | (result >> 6)) ^ value);
        }
        return (byte)((result << 2) | (result >> 6));
    }

    public static ushort Compute16(ReadOnlySpan<byte> data, ushort initialValue = 0)
    {
        var result = initialValue;
        foreach (var value in data)
        {
            result ^= (ushort)(value << 8);
            for (var bit = 0; bit < 8; bit++)
            {
                result = (ushort)((result << 1) ^ ((result & 0x8000) == 0 ? 0 : 0x1021));
            }
        }
        return result;
    }

    internal static int Size(JavadChecksumKind kind) =>
        kind switch
        {
            JavadChecksumKind.None => 0,
            JavadChecksumKind.Binary8 => 1,
            JavadChecksumKind.AsciiHex8 or JavadChecksumKind.Crc16 => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
}
