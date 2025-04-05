using System;
using System.Runtime.CompilerServices;
using Asv.Common;

namespace Asv.Gnss;

public readonly record struct NmeaHexFormat(string Format, int MinSize)
{
    public string Format { get; } = Format;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetByteSize(int? value)
    {
        return value == null ? 0 : Math.Max(MinSize, value.Value.CountHexDigits());
    }
    
    public static NmeaHexFormat HexX = new("X", 1);
    public static NmeaHexFormat HexX1 = new("X1", 1);
    public static NmeaHexFormat HexX2 = new("X2", 2);
}