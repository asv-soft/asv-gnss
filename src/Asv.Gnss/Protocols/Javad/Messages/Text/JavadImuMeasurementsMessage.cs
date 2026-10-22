using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[WW] ImuMeasurements; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadImuMeasurementsMessage()
    : JavadTextMessage("INSDAT", "000000.00,0,0,0,0,0,0")
{
    public override string Id => "WW";
    public override string Name => "INSDAT";
    public TimeSpan? UtcTime
    {
        get => Time(0);
        set => SetTime(0, value);
    }
    public double? AccelerationXMetersPerSecondSquared
    {
        get => Number(1);
        set => Set(1, value);
    }
    public double? AccelerationYMetersPerSecondSquared
    {
        get => Number(2);
        set => Set(2, value);
    }
    public double? AccelerationZMetersPerSecondSquared
    {
        get => Number(3);
        set => Set(3, value);
    }
    public double? AngularVelocityXDegreesPerSecond
    {
        get => Number(4);
        set => Set(4, value);
    }
    public double? AngularVelocityYDegreesPerSecond
    {
        get => Number(5);
        set => Set(5, value);
    }
    public double? AngularVelocityZDegreesPerSecond
    {
        get => Number(6);
        set => Set(6, value);
    }

    protected override void ValidateFields()
    {
        Validate("tffffff");
    }
}
