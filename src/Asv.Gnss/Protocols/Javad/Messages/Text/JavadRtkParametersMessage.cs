using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[RK] RtkParameters; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadRtkParametersMessage()
    : JavadTextMessage("RTKPAR", "N,000000.00,N,0,0,0,0,0,0,0")
{
    public override string Id => "RK";
    public override string Name => "RTKPAR";
    public char? UtcValidity
    {
        get => Character(0);
        set => Set(0, value);
    }
    public TimeSpan? UtcTime
    {
        get => Time(1);
        set => SetTime(1, value);
    }
    public char? RtkDataValidity
    {
        get => Character(2);
        set => Set(2, value);
    }
    public int? AmbiguityFixProgressPercent
    {
        get => Integer(3);
        set => Set(3, value);
    }
    public double? AmbiguityFixProbability
    {
        get => Number(4);
        set => Set(4, value);
    }
    public double? PositionChiSquared
    {
        get => Number(5);
        set => Set(5, value);
    }
    public double? ReceiverTimeShiftMeters
    {
        get => Number(6);
        set => Set(6, value);
    }
    public double? TimeShiftDerivativeMetersPerSecond
    {
        get => Number(7);
        set => Set(7, value);
    }
    public double? IonosphereRmsErrorMeters
    {
        get => Number(8);
        set => Set(8, value);
    }
    public double? CorrectionsAgeSeconds
    {
        get => Number(9);
        set => Set(9, value);
    }

    protected override void ValidateFields()
    {
        Validate("ctciffffff");
    }
}
