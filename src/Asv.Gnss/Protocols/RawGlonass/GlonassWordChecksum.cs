using System;
using System.Numerics;

namespace Asv.Gnss;

/// <summary>GLONASS ICD edition 5.1, section 4.7, Hamming check bits for a canonical eleven-byte string.</summary>
public static class GlonassWordChecksum
{
    // Bit n of the ICD occupies bit n-1 of the integer. Masks contain data bits only.
    private static readonly UInt128[] DataMasks =
    [
        ((UInt128)0xAAAAB << 64) | 0x55555556AAAD5B00UL,
        ((UInt128)0xCCCCD << 64) | 0x9999999B33366D00UL,
        ((UInt128)0x10F0F1 << 64) | 0xE1E1E1E3C3C78E00UL,
        ((UInt128)0xFF01 << 64) | 0xFE01FE03FC07F000UL,
        ((UInt128)0x1F0001 << 64) | 0xFFFE0003FFF80000UL,
        ((UInt128)0x1 << 64) | 0xFFFFFFFC00000000UL,
        ((UInt128)0x1FFFFE << 64),
    ];

    /// <summary>Computes the eight check bits, ignoring the existing check byte.</summary>
    public static byte Compute(ReadOnlySpan<byte> data)
    {
        RequireCanonicalSize(data);
        return Compute(Read(data));
    }

    /// <summary>Checks all Hamming equations without correcting received bits.</summary>
    public static bool IsValid(ReadOnlySpan<byte> data) =>
        data.Length == RawGlonassProtocol.PacketSize
        && (data[0] & 0xE0) == 0
        && Compute(Read(data)) == data[^1];

    internal static byte Compute(UInt128 value)
    {
        value &= ~(UInt128)0xFF;
        byte result = 0;
        for (var index = 0; index < DataMasks.Length; index++)
            result |= (byte)(Parity(value & DataMasks[index]) << index);
        return (byte)(result | ((Parity(value) ^ (BitOperations.PopCount((uint)result) & 1)) << 7));
    }

    private static int Parity(UInt128 value) =>
        (BitOperations.PopCount((ulong)value) + BitOperations.PopCount((ulong)(value >> 64))) & 1;

    private static UInt128 Read(ReadOnlySpan<byte> data)
    {
        UInt128 value = 0;
        foreach (var item in data)
            value = (value << 8) | item;
        return value;
    }

    private static void RequireCanonicalSize(ReadOnlySpan<byte> data)
    {
        if (data.Length != RawGlonassProtocol.PacketSize || (data[0] & 0xE0) != 0)
            throw new ArgumentException(
                "A GLONASS string requires eleven bytes with three leading zero padding bits.",
                nameof(data)
            );
    }
}
