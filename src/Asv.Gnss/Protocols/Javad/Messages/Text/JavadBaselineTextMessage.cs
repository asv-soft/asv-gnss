using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[AB] BaselineText; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadBaselineTextMessage()
    : JavadTextMessage("BASLIN", "N,000000.00,N,A,0,0,0,0")
{
    public override string Id => "AB";
    public override string Name => "BASLIN";
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
    public char? BaselineValidity
    {
        get => Character(2);
        set => Set(2, value);
    }
    public char? ComputationMode
    {
        get => Character(3);
        set => Set(3, value);
    }
    public double? ReferenceMinusRoverX
    {
        get => Number(4);
        set => Set(4, value);
    }
    public double? ReferenceMinusRoverY
    {
        get => Number(5);
        set => Set(5, value);
    }
    public double? ReferenceMinusRoverZ
    {
        get => Number(6);
        set => Set(6, value);
    }
    public double? Position3DRmsMeters
    {
        get => Number(7);
        set => Set(7, value);
    }

    protected override void ValidateFields()
    {
        Validate("ctccffff");
    }
}
