using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[ZZ] ImuAntennaVelocity; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadImuAntennaVelocityMessage()
    : JavadTextMessage("INSVEL", "N,000000.00,0,0,0")
{
    public override string Id => "ZZ";
    public override string Name => "INSVEL";
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
    public double? NorthVelocityMetersPerSecond
    {
        get => Number(2);
        set => Set(2, value);
    }
    public double? EastVelocityMetersPerSecond
    {
        get => Number(3);
        set => Set(3, value);
    }
    public double? DownVelocityMetersPerSecond
    {
        get => Number(4);
        set => Set(4, value);
    }

    protected override void ValidateFields()
    {
        Validate("ctfff");
    }
}
