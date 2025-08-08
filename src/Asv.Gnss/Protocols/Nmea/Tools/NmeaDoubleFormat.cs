using System;
using System.Runtime.CompilerServices;
using Asv.Common;

namespace Asv.Gnss;

public readonly struct NmeaDoubleFormat(string format, int minSizeBeforeDot, int minSizeAfterDot)
{
    public string Format { get; } = format;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetByteSize(in double value)
    {
        if (double.IsFinite(value) == false)
        {
            return 0;
        }
        var ceil = (int)Math.Round(value, minSizeAfterDot);
        if (value < 0 && ceil == 0)
        {
            return Math.Max(minSizeBeforeDot + 1 /*Dot (.)*/ + minSizeAfterDot,
                1 /*Minus (-)*/ + ceil.CountDecDigits() + 1 /*Dot (.)*/ + minSizeAfterDot);
        }
        else
        {
            return Math.Max(minSizeBeforeDot + 1 /*Dot (.)*/ + minSizeAfterDot,
                ceil.CountDecDigits() + 1 /*Dot (.)*/ + minSizeAfterDot);
        }
    }
    
    public static readonly NmeaDoubleFormat Double1X1 = new("0.0", 1, 1);
    public static readonly NmeaDoubleFormat Double1X2 = new("0.00", 1, 2);
    public static readonly NmeaDoubleFormat Double1X3 = new("0.000", 1, 3);
    public static readonly NmeaDoubleFormat Double1X4 = new("0.0000", 1, 4);
    public static readonly NmeaDoubleFormat Double1X5 = new("0.00000", 1, 5);
    public static readonly NmeaDoubleFormat Double1X6 = new("0.000000", 1, 6);
    public static readonly NmeaDoubleFormat Double1X7 = new("0.0000000", 1, 7);
    public static readonly NmeaDoubleFormat Double2X1 = new("00.0", 2, 1);
    public static readonly NmeaDoubleFormat Double2X2 = new("00.00", 2, 2);
    public static readonly NmeaDoubleFormat Double2X3 = new("00.000", 2, 3);
    public static readonly NmeaDoubleFormat Double2X4 = new("00.0000", 2, 4);
    public static readonly NmeaDoubleFormat Double2X5 = new("00.00000", 2, 5);
    public static readonly NmeaDoubleFormat Double2X6 = new("00.000000", 2, 6);
    public static readonly NmeaDoubleFormat Double3X1 = new("000.0", 3, 1);
    public static readonly NmeaDoubleFormat Double3X2 = new("000.00", 3, 2);
    public static readonly NmeaDoubleFormat Double3X3 = new("000.000", 3, 3);
    public static readonly NmeaDoubleFormat Double3X4 = new("000.0000", 3, 4);
    public static readonly NmeaDoubleFormat Double3X5 = new("000.00000", 3, 5);
    public static readonly NmeaDoubleFormat Double3X6 = new("000.000000", 3, 6);
    public static readonly NmeaDoubleFormat Double4X1 = new("0000.0", 4, 1);
    public static readonly NmeaDoubleFormat Double4X2 = new("0000.00", 4, 2);
    public static readonly NmeaDoubleFormat Double4X3 = new("0000.000", 4, 3);
    public static readonly NmeaDoubleFormat Double4X4 = new("0000.0000", 4, 4);
    public static readonly NmeaDoubleFormat Double4X5 = new("0000.00000", 4, 5);
    public static readonly NmeaDoubleFormat Double4X6 = new("0000.000000", 4, 6);
    public static readonly NmeaDoubleFormat Double4X7 = new("0000.0000000", 4, 7);
    public static readonly NmeaDoubleFormat Double5X1 = new("00000.0", 5, 1);
    public static readonly NmeaDoubleFormat Double5X2 = new("00000.00", 5, 2);
    public static readonly NmeaDoubleFormat Double5X3 = new("00000.000", 5, 3);
    public static readonly NmeaDoubleFormat Double5X4 = new("00000.0000", 5, 4);
    public static readonly NmeaDoubleFormat Double5X5 = new("00000.00000", 5, 5);
    public static readonly NmeaDoubleFormat Double5X6 = new("00000.000000", 5, 6);
    public static readonly NmeaDoubleFormat Double6X1 = new("000000.0", 6, 1);
    public static readonly NmeaDoubleFormat Double6X2 = new("000000.00", 6, 2);
    public static readonly NmeaDoubleFormat Double6X3 = new("000000.000", 6, 3);
    public static readonly NmeaDoubleFormat Double6X4 = new("000000.0000", 6, 4);
    public static readonly NmeaDoubleFormat Double6X5 = new("000000.00000", 6, 5);
    public static readonly NmeaDoubleFormat Double6X6 = new("000000.000000", 6, 6);
        
}