using System;
using System.Runtime.CompilerServices;
using Asv.Common;

namespace Asv.Gnss;

public readonly record struct NmeaIntFormat(string Format, int MinSize)
{
    public string Format { get; } = Format;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetByteSize(int? value)
    {
        return value == null ? 0 : Math.Max(MinSize, value.Value.CountDigits());
    }
    
    public static NmeaIntFormat IntD1 = new("0", 1);
    public static NmeaIntFormat IntD2 = new("00", 2);
    public static NmeaIntFormat IntD3 = new("000", 3);
    public static NmeaIntFormat IntD4 = new("0000", 4);
    public static NmeaIntFormat IntD5 = new("00000", 5);
    public static NmeaIntFormat IntD6 = new("000000", 6);
    public static NmeaIntFormat IntD7 = new("0000000", 7);
    public static NmeaIntFormat IntD8 = new("00000000", 8);
}