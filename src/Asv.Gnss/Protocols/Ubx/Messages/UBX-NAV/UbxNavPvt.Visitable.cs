using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class UbxNavPvt
{
    private static readonly Field MagneticDeclinationAccuracyField = UbxVisitor.Field(nameof(MagneticDeclinationAccuracy), DoubleType.Default, "Magnetic declination accuracy. Only supported in ADR 4.10 and later. deg", "deg");
    private static readonly Field MagneticDeclinationField = UbxVisitor.Field(nameof(MagneticDeclination), DoubleType.Default, "Magnetic declination. Only supported in ADR 4.10 and later. deg", "deg");
    private static readonly Field HeadingOfVehicle2DField = UbxVisitor.Field(nameof(HeadingOfVehicle2D), DoubleType.Default, "Vehicle heading in degrees. This deserializer sets the property to NaN when IsValidVehicleHeading is false.", "deg");
    private static readonly Field IsValidLLHField = UbxVisitor.Field(nameof(IsValidLLH), BoolType.Default, "Whether longitude, latitude and both height fields are valid; this is the inverse of the wire invalidLlh bit.");
    private static readonly Field PositionDOPField = UbxVisitor.Field(nameof(PositionDOP), DoubleType.Default, "Position dilution of precision, dimensionless; wire resolution is 0.01.");
    private static readonly Field HeadingAccuracyEstimateField = UbxVisitor.Field(nameof(HeadingAccuracyEstimate), DoubleType.Default, "Heading accuracy estimate (both motion and vehicle), deg", "deg");
    private static readonly Field SpeedAccuracyEstimateField = UbxVisitor.Field(nameof(SpeedAccuracyEstimate), DoubleType.Default, "Speed accuracy estimate, m\\s", "m/s");
    private static readonly Field HeadingOfMotion2DField = UbxVisitor.Field(nameof(HeadingOfMotion2D), DoubleType.Default, "Heading of motion (2-D), deg", "deg");
    private static readonly Field GroundSpeed2DField = UbxVisitor.Field(nameof(GroundSpeed2D), DoubleType.Default, "Ground Speed (2-D), m\\s", "m/s");
    private static readonly Field VelocityDownField = UbxVisitor.Field(nameof(VelocityDown), DoubleType.Default, "NED down velocity, m\\s", "m/s");
    private static readonly Field VelocityEastField = UbxVisitor.Field(nameof(VelocityEast), DoubleType.Default, "NED east velocity, m\\s", "m/s");
    private static readonly Field VelocityNorthField = UbxVisitor.Field(nameof(VelocityNorth), DoubleType.Default, "NED north velocity, m\\s", "m/s");
    private static readonly Field VerticalAccuracyEstimateField = UbxVisitor.Field(nameof(VerticalAccuracyEstimate), DoubleType.Default, "Vertical accuracy estimate, m", "m");
    private static readonly Field HorizontalAccuracyEstimateField = UbxVisitor.Field(nameof(HorizontalAccuracyEstimate), DoubleType.Default, "Horizontal accuracy estimate, m", "m");
    private static readonly Field AltMslField = UbxVisitor.Field(nameof(AltMsl), DoubleType.Default, "Height above mean sea level, m", "m");
    private static readonly Field AltElipsoidField = UbxVisitor.Field(nameof(AltElipsoid), DoubleType.Default, "Height above ellipsoid, m", "m");
    private static readonly Field LatitudeField = UbxVisitor.Field(nameof(Latitude), DoubleType.Default, "WGS84 geodetic latitude in degrees; converted from the wire scale of 0.0000001 degree.", "deg");
    private static readonly Field LongitudeField = UbxVisitor.Field(nameof(Longitude), DoubleType.Default, "WGS84 geodetic longitude in degrees; converted from the wire scale of 0.0000001 degree.", "deg");
    private static readonly Field NumberOfSatellitesField = UbxVisitor.Field(nameof(NumberOfSatellites), UInt8Type.Default, "Number of satellites used in Nav Solution");
    private static readonly Field UTCConfirmedTimeField = UbxVisitor.Field(nameof(UTCConfirmedTime), BoolType.Default, "UTC Time of Day could be confirmed (see Time Validity section for details)");
    private static readonly Field UTCConfirmedDateField = UbxVisitor.Field(nameof(UTCConfirmedDate), BoolType.Default, "UTC Date validity could be confirmed (see Time Validity section for details)");
    private static readonly Field UTCConfirmedAvailableField = UbxVisitor.Field(nameof(UTCConfirmedAvailable), BoolType.Default, "1 = information about UTC Date and Time of Day validity confirmation is available (see Time Validity section for details) This flag is only supported in Protocol Versions 19.00, 19.10, 20.10, 20.20, 20.30, 22.00, 23.00, 23.01, 27 and 28");
    private static readonly Field PsmStateField = UbxVisitor.Field(nameof(PsmState), UInt8Type.Default, "Power-save state from flags bits 2 through 4; the R23 NAV-PVT field table does not define these bits, so interpret only with the applicable firmware interface description.");
    private static readonly Field CarrierSolutionField = UbxVisitor.Enum<int>(nameof(CarrierSolution), Int32Type.Default, "Carrier-phase ambiguity status: unavailable, float, or fixed. This status alone does not establish a valid navigation fix.",
        new EnumValue<int>((int)UbxCarrierSolutionStatus.NoCarrierSolution, nameof(UbxCarrierSolutionStatus.NoCarrierSolution)),
        new EnumValue<int>((int)UbxCarrierSolutionStatus.FloatingAmbiguities, nameof(UbxCarrierSolutionStatus.FloatingAmbiguities)),
        new EnumValue<int>((int)UbxCarrierSolutionStatus.FixedAmbiguities, nameof(UbxCarrierSolutionStatus.FixedAmbiguities)));
    private static readonly Field GnssFixOKField = UbxVisitor.Field(nameof(GnssFixOK), BoolType.Default, "Whether the solution satisfies the receiver DOP and accuracy masks. Check this flag as well as FixType before using the position.");
    private static readonly Field IsAppliedDifferentialCorrectionsField = UbxVisitor.Field(nameof(IsAppliedDifferentialCorrections), BoolType.Default, "Differential corrections were applied");
    private static readonly Field IsValidVehicleHeadingField = UbxVisitor.Field(nameof(IsValidVehicleHeading), BoolType.Default, "Whether the vehicle heading is valid (headVehValid); this is separate from differential-correction status.");
    private static readonly Field FixTypeField = UbxVisitor.Enum<byte>(nameof(FixType), UInt8Type.Default, "GNSSfix Type",
        new EnumValue<byte>((byte)UbxGnssFixType.NoFix, nameof(UbxGnssFixType.NoFix)),
        new EnumValue<byte>((byte)UbxGnssFixType.DeadReckoningOnly, nameof(UbxGnssFixType.DeadReckoningOnly)),
        new EnumValue<byte>((byte)UbxGnssFixType.Fix2D, nameof(UbxGnssFixType.Fix2D)),
        new EnumValue<byte>((byte)UbxGnssFixType.Fix3D, nameof(UbxGnssFixType.Fix3D)),
        new EnumValue<byte>((byte)UbxGnssFixType.GNSSDeadReckoning, nameof(UbxGnssFixType.GNSSDeadReckoning)),
        new EnumValue<byte>((byte)UbxGnssFixType.TimeOnlyFix, nameof(UbxGnssFixType.TimeOnlyFix)));
    private static readonly Field UTCFractionOfSecondField = UbxVisitor.Field(nameof(UTCFractionOfSecond), DoubleType.Default, "Signed UTC fractional-second correction in nanoseconds, from -1000000000 to 1000000000; this property retains the unscaled wire value.", "ns");
    private static readonly Field UTCTimeAccuracyEstimateField = UbxVisitor.Field(nameof(UTCTimeAccuracyEstimate), DoubleType.Default, "Estimated UTC time accuracy in seconds; converted from the wire nanosecond value.", "s");
    private static readonly Field IsValidMagneticDeclinationField = UbxVisitor.Field(nameof(IsValidMagneticDeclination), BoolType.Default, "valid magnetic declination");
    private static readonly Field UTCTimeOfDayIsFullyResolvedField = UbxVisitor.Field(nameof(UTCTimeOfDayIsFullyResolved), BoolType.Default, "Whether UTC time has no whole-second ambiguity; also check the date and time validity flags.");
    private static readonly Field UTCTimeIsConfirmationField = UbxVisitor.Field(nameof(UTCTimeIsConfirmation), BoolType.Default, "valid UTC time of day (see Time Validity section for details)");
    private static readonly Field UTCDateIsConfirmationField = UbxVisitor.Field(nameof(UTCDateIsConfirmation), BoolType.Default, "valid UTC Date (see Time Validity section for details)");
    private static readonly Field SecField = UbxVisitor.Field(nameof(Sec), UInt8Type.Default, "Seconds of minute, range 0..60 (UTC)", "s");
    private static readonly Field MinField = UbxVisitor.Field(nameof(Min), UInt8Type.Default, "Minute of hour, range 0..59 (UTC)", "min");
    private static readonly Field HourField = UbxVisitor.Field(nameof(Hour), UInt8Type.Default, "Hour of day, range 0..23 (UTC)", "h");
    private static readonly Field DayField = UbxVisitor.Field(nameof(Day), UInt8Type.Default, "Day of month, range 1..31 (UTC)");
    private static readonly Field MonthField = UbxVisitor.Field(nameof(Month), UInt8Type.Default, "Month, range 1..12 (UTC)");
    private static readonly Field YearField = UbxVisitor.Field(nameof(Year), UInt16Type.Default, "Year (UTC)");
    private static readonly Field iTOWField = UbxVisitor.Field(nameof(iTOW), UInt64Type.Default, "GPS time of week of the navigation epoch in milliseconds.", "ms");
    private static readonly Field MovingBaseLocationField = UbxVisitor.Field(nameof(MovingBaseLocation), UbxVisitor.GlobalPositionType, "Derived geodetic position in degrees and meters above the ellipsoid. Updated when GnssFixOK is set and FixType is at least Fix3D; otherwise the previous value is retained.");
    private static readonly Field GpsTimeField = UbxVisitor.Field(nameof(GpsTime), DateTimeType.Default, "Derived GPS time reconstructed from iTOW and the week inferred from UtcTime. DateTime.MinValue with UTC kind indicates an invalid or unrepresentable receiver date or time.");
    private static readonly Field UtcTimeField = UbxVisitor.Field(nameof(UtcTime), DateTimeType.Default, "UTC timestamp assembled from valid calendar fields and the signed nanosecond correction, rounded to 100 ns. Leap-second label 60 is normalized to the following minute. DateTime.MinValue with UTC kind indicates an invalid or unrepresentable receiver date or time; check the fully-resolved flag before precise time use.");

    /// <summary>Describes the public navigation payload fields exposed to visitors.</summary>
    public static StructType StructType { get; } = new([
        MagneticDeclinationAccuracyField,
        MagneticDeclinationField,
        HeadingOfVehicle2DField,
        IsValidLLHField,
        PositionDOPField,
        HeadingAccuracyEstimateField,
        SpeedAccuracyEstimateField,
        HeadingOfMotion2DField,
        GroundSpeed2DField,
        VelocityDownField,
        VelocityEastField,
        VelocityNorthField,
        VerticalAccuracyEstimateField,
        HorizontalAccuracyEstimateField,
        AltMslField,
        AltElipsoidField,
        LatitudeField,
        LongitudeField,
        NumberOfSatellitesField,
        UTCConfirmedTimeField,
        UTCConfirmedDateField,
        UTCConfirmedAvailableField,
        PsmStateField,
        CarrierSolutionField,
        GnssFixOKField,
        IsAppliedDifferentialCorrectionsField,
        IsValidVehicleHeadingField,
        FixTypeField,
        UTCFractionOfSecondField,
        UTCTimeAccuracyEstimateField,
        IsValidMagneticDeclinationField,
        UTCTimeOfDayIsFullyResolvedField,
        UTCTimeIsConfirmationField,
        UTCDateIsConfirmationField,
        SecField,
        MinField,
        HourField,
        DayField,
        MonthField,
        YearField,
        iTOWField,
        MovingBaseLocationField,
        GpsTimeField,
        UtcTimeField
    ]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        DoubleType.Accept(visitor, MagneticDeclinationAccuracyField, MagneticDeclinationAccuracyField.DataType, ref _magneticDeclinationAccuracy);
        DoubleType.Accept(visitor, MagneticDeclinationField, MagneticDeclinationField.DataType, ref _magneticDeclination);
        DoubleType.Accept(visitor, HeadingOfVehicle2DField, HeadingOfVehicle2DField.DataType, ref _headingOfVehicle2D);
        BoolType.Accept(visitor, IsValidLLHField, IsValidLLHField.DataType, ref _isValidLLH);
        DoubleType.Accept(visitor, PositionDOPField, PositionDOPField.DataType, ref _positionDOP);
        DoubleType.Accept(visitor, HeadingAccuracyEstimateField, HeadingAccuracyEstimateField.DataType, ref _headingAccuracyEstimate);
        DoubleType.Accept(visitor, SpeedAccuracyEstimateField, SpeedAccuracyEstimateField.DataType, ref _speedAccuracyEstimate);
        DoubleType.Accept(visitor, HeadingOfMotion2DField, HeadingOfMotion2DField.DataType, ref _headingOfMotion2D);
        DoubleType.Accept(visitor, GroundSpeed2DField, GroundSpeed2DField.DataType, ref _groundSpeed2D);
        DoubleType.Accept(visitor, VelocityDownField, VelocityDownField.DataType, ref _velocityDown);
        DoubleType.Accept(visitor, VelocityEastField, VelocityEastField.DataType, ref _velocityEast);
        DoubleType.Accept(visitor, VelocityNorthField, VelocityNorthField.DataType, ref _velocityNorth);
        DoubleType.Accept(visitor, VerticalAccuracyEstimateField, VerticalAccuracyEstimateField.DataType, ref _verticalAccuracyEstimate);
        DoubleType.Accept(visitor, HorizontalAccuracyEstimateField, HorizontalAccuracyEstimateField.DataType, ref _horizontalAccuracyEstimate);
        DoubleType.Accept(visitor, AltMslField, AltMslField.DataType, ref _altMsl);
        DoubleType.Accept(visitor, AltElipsoidField, AltElipsoidField.DataType, ref _altElipsoid);
        DoubleType.Accept(visitor, LatitudeField, LatitudeField.DataType, ref _latitude);
        DoubleType.Accept(visitor, LongitudeField, LongitudeField.DataType, ref _longitude);
        UInt8Type.Accept(visitor, NumberOfSatellitesField, NumberOfSatellitesField.DataType, ref _numberOfSatellites);
        BoolType.Accept(visitor, UTCConfirmedTimeField, UTCConfirmedTimeField.DataType, ref _utcConfirmedTime);
        BoolType.Accept(visitor, UTCConfirmedDateField, UTCConfirmedDateField.DataType, ref _utcConfirmedDate);
        BoolType.Accept(visitor, UTCConfirmedAvailableField, UTCConfirmedAvailableField.DataType, ref _utcConfirmedAvailable);
        UInt8Type.Accept(visitor, PsmStateField, PsmStateField.DataType, ref _psmState);
        var carrierSolutionValue = (int)CarrierSolution;
        Int32Type.Accept(visitor, CarrierSolutionField, CarrierSolutionField.DataType, ref carrierSolutionValue);
        CarrierSolution = (UbxCarrierSolutionStatus)carrierSolutionValue;
        BoolType.Accept(visitor, GnssFixOKField, GnssFixOKField.DataType, ref _gnssFixOK);
        BoolType.Accept(visitor, IsAppliedDifferentialCorrectionsField, IsAppliedDifferentialCorrectionsField.DataType, ref _isAppliedDifferentialCorrections);
        BoolType.Accept(visitor, IsValidVehicleHeadingField, IsValidVehicleHeadingField.DataType, ref _isValidVehicleHeading);
        var fixTypeValue = (byte)FixType;
        UInt8Type.Accept(visitor, FixTypeField, FixTypeField.DataType, ref fixTypeValue);
        FixType = (UbxGnssFixType)fixTypeValue;
        DoubleType.Accept(visitor, UTCFractionOfSecondField, UTCFractionOfSecondField.DataType, ref _utcFractionOfSecond);
        DoubleType.Accept(visitor, UTCTimeAccuracyEstimateField, UTCTimeAccuracyEstimateField.DataType, ref _utcTimeAccuracyEstimate);
        BoolType.Accept(visitor, IsValidMagneticDeclinationField, IsValidMagneticDeclinationField.DataType, ref _isValidMagneticDeclination);
        BoolType.Accept(visitor, UTCTimeOfDayIsFullyResolvedField, UTCTimeOfDayIsFullyResolvedField.DataType, ref _utcTimeOfDayIsFullyResolved);
        BoolType.Accept(visitor, UTCTimeIsConfirmationField, UTCTimeIsConfirmationField.DataType, ref _utcTimeIsConfirmation);
        BoolType.Accept(visitor, UTCDateIsConfirmationField, UTCDateIsConfirmationField.DataType, ref _utcDateIsConfirmation);
        UInt8Type.Accept(visitor, SecField, SecField.DataType, ref _sec);
        UInt8Type.Accept(visitor, MinField, MinField.DataType, ref _min);
        UInt8Type.Accept(visitor, HourField, HourField.DataType, ref _hour);
        UInt8Type.Accept(visitor, DayField, DayField.DataType, ref _day);
        UInt8Type.Accept(visitor, MonthField, MonthField.DataType, ref _month);
        UInt16Type.Accept(visitor, YearField, YearField.DataType, ref _year);
        UInt64Type.Accept(visitor, iTOWField, iTOWField.DataType, ref _iTOW);
        UbxVisitor.VisitGlobalPosition(visitor, MovingBaseLocationField, ref _movingBaseLocation);
        DateTimeType.Accept(visitor, GpsTimeField, GpsTimeField.DataType, ref _gpsTime);
        DateTimeType.Accept(visitor, UtcTimeField, UtcTimeField.DataType, ref _utcTime);
    }
}
