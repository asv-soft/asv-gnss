using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[QQ] ImuAttitude; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadImuAttitudeMessage()
    : JavadTextMessage("INSATT", "N,000000.00,0,0,0")
{
    public override string Id => "QQ";
    public override string Name => "INSATT";
    public char? SolutionValidity
    {
        get => Character(0);
        set => Set(0, value);
    }
    public TimeSpan? UtcTime
    {
        get => Time(1);
        set => SetTime(1, value);
    }
    public double? RollDegrees
    {
        get => Number(2);
        set => Set(2, value);
    }
    public double? PitchDegrees
    {
        get => Number(3);
        set => Set(3, value);
    }
    public double? HeadingDegrees
    {
        get => Number(4);
        set => Set(4, value);
    }

    protected override void ValidateFields()
    {
        Validate("ctfff");
    }
}
