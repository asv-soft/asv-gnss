using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[NP] NavigationPosition; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadNavigationPositionMessage()
    : JavadTextMessage("NAVPOS", "N,000000.00,3")
{
    public override string Id => "NP";
    public override string Name => "NAVPOS";
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
    public char? GeoidalSeparationValidity
    {
        get => Character(9);
        set => Set(9, value);
    }
    public double? GeoidalSeparationMeters
    {
        get => Number(10);
        set => Set(10, value);
    }
    public double? Hdop
    {
        get => Number(11);
        set => Set(11, value);
    }
    public double? Vdop
    {
        get => Number(12);
        set => Set(12, value);
    }
    public double? HorizontalPositionRmsMeters
    {
        get => Number(13);
        set => Set(13, value);
    }
    public double? VerticalPositionRmsMeters
    {
        get => Number(14);
        set => Set(14, value);
    }
    public double? HorizontalVelocityKilometersPerHour
    {
        get => Number(15);
        set => Set(15, value);
    }
    public double? VerticalVelocityKilometersPerHour
    {
        get => Number(16);
        set => Set(16, value);
    }
    public double? TrueHeadingDegrees
    {
        get => Number(17);
        set => Set(17, value);
    }
    public char? MagneticHeadingValidity
    {
        get => Character(18);
        set => Set(18, value);
    }
    public double? MagneticHeadingDegrees
    {
        get => Number(19);
        set => Set(19, value);
    }
    public double? HorizontalVelocityRmsMetersPerSecond
    {
        get => Number(20);
        set => Set(20, value);
    }
    public double? VerticalVelocityRmsMetersPerSecond
    {
        get => Number(21);
        set => Set(21, value);
    }
    public double? DataLinkQualityPercent
    {
        get => Number(22);
        set => Set(22, value);
    }
    public int? CorrectionsAgeSeconds
    {
        get => Integer(23);
        set => Set(23, value);
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
        Validate("ctislsssfcffffffffcffffi");
        _ = SatellitesPerSystem;
        _ = LatitudeDegrees;
        _ = LongitudeDegrees;
    }
}
