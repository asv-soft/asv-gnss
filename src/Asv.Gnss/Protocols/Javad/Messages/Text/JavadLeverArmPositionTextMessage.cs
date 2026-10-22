using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[NR] LeverArmPositionText; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadLeverArmPositionTextMessage()
    : JavadTextMessage("ARMPOS", "N,000000.00,3")
{
    public override string Id => "NR";
    public override string Name => "ARMPOS";
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
    public int? PositionStatus
    {
        get => Integer(2);
        set => Set(2, value);
    }
    public string? ComputationModes
    {
        get => Text(3);
        set => Set(3, value);
    }
    public string? GeodeticDatum
    {
        get => Text(5);
        set => Set(5, value);
    }
    public string? Latitude
    {
        get => Text(6);
        set => Set(6, value);
    }
    public string? Longitude
    {
        get => Text(7);
        set => Set(7, value);
    }
    public double? AltitudeMeters
    {
        get => Number(8);
        set => Set(8, value);
    }
    public double? HorizontalPositionRmsMeters
    {
        get => Number(9);
        set => Set(9, value);
    }
    public double? VerticalPositionRmsMeters
    {
        get => Number(10);
        set => Set(10, value);
    }
    public double? LatitudeDegrees => JavadTextFields.Degrees(Latitude);
    public double? LongitudeDegrees => JavadTextFields.Degrees(Longitude);
    public ImmutableArray<int?> SatellitesPerSystem =>
        Field(4).IsOmitted
            ? []
            : JavadTextFields.List(Field(4)).Select(JavadTextFields.Integer).ToImmutableArray();

    protected override void ValidateFields()
    {
        Validate("cti");
        if (PositionStatus != 0)
            return;
        Validate("ctislsssfff");
        _ = SatellitesPerSystem;
        _ = LatitudeDegrees;
        _ = LongitudeDegrees;
    }
}
