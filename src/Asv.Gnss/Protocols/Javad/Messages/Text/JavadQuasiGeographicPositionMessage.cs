using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[SY] QuasiGeographicPosition; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadQuasiGeographicPositionMessage()
    : JavadTextMessage("QDIR", "000000.00,A,0000.0000000,N,00000.0000000,E,0,0,1,1,0,0,0,0,N")
{
    public override string Id => "SY";
    public override string Name => "QDIR";
    public TimeSpan? UtcTime
    {
        get => Time(0);
        set => SetTime(0, value);
    }
    public char? Indicator
    {
        get => Character(1);
        set => Set(1, value);
    }
    public double? LatitudeDegreesMinutes
    {
        get => Number(2);
        set => Set(2, value);
    }
    public char? LatitudeHemisphere
    {
        get => Character(3);
        set => Set(3, value);
    }
    public double? LongitudeDegreesMinutes
    {
        get => Number(4);
        set => Set(4, value);
    }
    public char? LongitudeHemisphere
    {
        get => Character(5);
        set => Set(5, value);
    }
    public double? GroundSpeedKnots
    {
        get => Number(6);
        set => Set(6, value);
    }
    public double? TrueCourseDegrees
    {
        get => Number(7);
        set => Set(7, value);
    }
    public int? Day
    {
        get => Integer(8);
        set => Set(8, value);
    }
    public int? Month
    {
        get => Integer(9);
        set => Set(9, value);
    }
    public int? YearModulo100
    {
        get => Integer(10);
        set => Set(10, value);
    }
    public double? HeadingDegrees
    {
        get => Number(11);
        set => Set(11, value);
    }
    public double? QuasiCourseDegrees
    {
        get => Number(12);
        set => Set(12, value);
    }
    public double? QuasiHeadingDegrees
    {
        get => Number(13);
        set => Set(13, value);
    }
    public char? AttitudeSolutionType
    {
        get => Character(14);
        set => Set(14, value);
    }

    protected override void ValidateFields()
    {
        Validate("tcfcfcffiiifffc");
    }
}
