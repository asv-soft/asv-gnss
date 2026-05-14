using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX category 010 message.
/// </summary>
public class AsterixMessageI010 : AsterixMessage<AsterixRecordI010>
{
    /// <summary>
    /// ASTERIX category number for category 010.
    /// </summary>
    public static byte Category => 10;

    /// <inheritdoc />
    public override string Name => "I010";

    /// <inheritdoc />
    public override byte Id => Category;
}

/// <summary>
/// ASTERIX CAT010 record containing surface movement target reports.
/// </summary>
public class AsterixRecordI010 : AsterixRecord
{
    /// <summary>
    /// I010/010 Data Source Identifier.
    /// </summary>
    public AsterixFieldI010Frn001Type010? DataSourceIdentifier { get; set; }

    /// <summary>
    /// I010/000 Message Type.
    /// </summary>
    public AsterixFieldI010Frn002Type000? MessageType { get; set; }

    /// <summary>
    /// I010/020 Target Report Descriptor.
    /// </summary>
    public AsterixFieldI010Frn003Type020? TargetReportDescriptor { get; set; }

    /// <summary>
    /// I010/140 Time of Day.
    /// </summary>
    public AsterixFieldI010Frn004Type140? TimeOfDay { get; set; }

    /// <summary>
    /// I010/041 Position in WGS-84 Coordinates.
    /// </summary>
    public AsterixFieldI010Frn005Type041? PositionWgs84 { get; set; }

    /// <summary>
    /// I010/040 Measured Position in Polar Coordinates.
    /// </summary>
    public AsterixFieldI010Frn006Type040? MeasuredPositionPolarCoordinates { get; set; }

    /// <summary>
    /// I010/042 Position in Cartesian Coordinates.
    /// </summary>
    public AsterixFieldI010Frn007Type042? PositionCartesian { get; set; }

    /// <summary>
    /// I010/200 Calculated Track Velocity in Polar Coordinates.
    /// </summary>
    public AsterixFieldI010Frn008Type200? TrackVelocityPolar { get; set; }

    /// <summary>
    /// I010/202 Calculated Track Velocity in Cartesian Coordinates.
    /// </summary>
    public AsterixFieldI010Frn009Type202? TrackVelocityCartesian { get; set; }

    /// <summary>
    /// I010/161 Track Number.
    /// </summary>
    public AsterixFieldI010Frn010Type161? TrackNumber { get; set; }

    /// <summary>
    /// I010/170 Track Status.
    /// </summary>
    public AsterixFieldI010Frn011Type170? TrackStatus { get; set; }

    /// <summary>
    /// I010/060 Mode-3/A Code in Octal Representation.
    /// </summary>
    public AsterixFieldI010Frn012Type060? Mode3ACode { get; set; }

    /// <summary>
    /// I010/220 Target Address.
    /// </summary>
    public AsterixFieldI010Frn013Type220? TargetAddress { get; set; }

    /// <summary>
    /// I010/245 Target Identification.
    /// </summary>
    public AsterixFieldI010Frn014Type245? TargetIdentification { get; set; }

    /// <summary>
    /// I010/250 Mode S MB Data.
    /// </summary>
    public AsterixFieldI010Frn015Type250? ModeSMbData { get; set; }

    /// <summary>
    /// I010/300 Vehicle Fleet Identification.
    /// </summary>
    public AsterixFieldI010Frn016Type300? VehicleFleetIdentification { get; set; }

    /// <summary>
    /// I010/090 Flight Level in Binary Representation.
    /// </summary>
    public AsterixFieldI010Frn017Type090? FlightLevel { get; set; }

    /// <summary>
    /// I010/091 Measured Height.
    /// </summary>
    public AsterixFieldI010Frn018Type091? MeasuredHeight { get; set; }

    /// <summary>
    /// I010/270 Target Size and Orientation.
    /// </summary>
    public AsterixFieldI010Frn019Type270? TargetSizeAndOrientation { get; set; }

    /// <summary>
    /// I010/550 System Status.
    /// </summary>
    public AsterixFieldI010Frn020Type550? SystemStatus { get; set; }

    /// <summary>
    /// I010/310 Pre-programmed Message.
    /// </summary>
    public AsterixFieldI010Frn021Type310? PreProgrammedMessage { get; set; }

    /// <summary>
    /// I010/500 Standard Deviation of Position.
    /// </summary>
    public AsterixFieldI010Frn022Type500? PositionAccuracy { get; set; }

    /// <summary>
    /// I010/280 Presence.
    /// </summary>
    public AsterixFieldI010Frn023Type280? Presence { get; set; }

    /// <summary>
    /// I010/131 Amplitude of Primary Plot.
    /// </summary>
    public AsterixFieldI010Frn024Type131? AmplitudeOfPrimaryPlot { get; set; }

    /// <summary>
    /// I010/210 Calculated Acceleration.
    /// </summary>
    public AsterixFieldI010Frn025Type210? CalculatedAcceleration { get; set; }

    /// <summary>
    /// I010/SP Special Purpose Field.
    /// </summary>
    public AsterixFieldI010Frn026TypeSp? SpecialPurposeField { get; set; }

    /// <summary>
    /// I010/RE Reserved Expansion Field.
    /// </summary>
    public AsterixFieldI010Frn027TypeRe? ReservedExpansionField { get; set; }

    /// <inheritdoc />
    public override int Category => AsterixMessageI010.Category;

    /// <inheritdoc />
    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI010Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI010Frn002Type000.StaticFrn => MessageType = new(),
            AsterixFieldI010Frn003Type020.StaticFrn => TargetReportDescriptor = new(),
            AsterixFieldI010Frn004Type140.StaticFrn => TimeOfDay = new(),
            AsterixFieldI010Frn005Type041.StaticFrn => PositionWgs84 = new(),
            AsterixFieldI010Frn006Type040.StaticFrn => MeasuredPositionPolarCoordinates = new(),
            AsterixFieldI010Frn007Type042.StaticFrn => PositionCartesian = new(),
            AsterixFieldI010Frn008Type200.StaticFrn => TrackVelocityPolar = new(),
            AsterixFieldI010Frn009Type202.StaticFrn => TrackVelocityCartesian = new()
            {
                IsSensisEncoding = PositionWgs84 != null && TrackVelocityPolar == null
            },
            AsterixFieldI010Frn010Type161.StaticFrn => TrackNumber = new(),
            AsterixFieldI010Frn011Type170.StaticFrn => TrackStatus = new(),
            AsterixFieldI010Frn012Type060.StaticFrn => Mode3ACode = new(),
            AsterixFieldI010Frn013Type220.StaticFrn => TargetAddress = new(),
            AsterixFieldI010Frn014Type245.StaticFrn => TargetIdentification = new(),
            AsterixFieldI010Frn015Type250.StaticFrn => ModeSMbData = new(),
            AsterixFieldI010Frn016Type300.StaticFrn => VehicleFleetIdentification = new(),
            AsterixFieldI010Frn017Type090.StaticFrn => FlightLevel = new(),
            AsterixFieldI010Frn018Type091.StaticFrn => MeasuredHeight = new(),
            AsterixFieldI010Frn019Type270.StaticFrn => TargetSizeAndOrientation = new(),
            AsterixFieldI010Frn020Type550.StaticFrn => SystemStatus = new(),
            AsterixFieldI010Frn021Type310.StaticFrn => PreProgrammedMessage = new(),
            AsterixFieldI010Frn022Type500.StaticFrn => PositionAccuracy = new(),
            AsterixFieldI010Frn023Type280.StaticFrn => Presence = new(),
            AsterixFieldI010Frn024Type131.StaticFrn => AmplitudeOfPrimaryPlot = new(),
            AsterixFieldI010Frn025Type210.StaticFrn => CalculatedAcceleration = new(),
            AsterixFieldI010Frn026TypeSp.StaticFrn => SpecialPurposeField = new(),
            AsterixFieldI010Frn027TypeRe.StaticFrn => ReservedExpansionField = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI010)}")
        };
    }

    /// <inheritdoc />
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (MessageType != null) yield return MessageType;
        if (TargetReportDescriptor != null) yield return TargetReportDescriptor;
        if (TimeOfDay != null) yield return TimeOfDay;
        if (PositionWgs84 != null) yield return PositionWgs84;
        if (MeasuredPositionPolarCoordinates != null) yield return MeasuredPositionPolarCoordinates;
        if (PositionCartesian != null) yield return PositionCartesian;
        if (TrackVelocityPolar != null) yield return TrackVelocityPolar;
        if (TrackVelocityCartesian != null) yield return TrackVelocityCartesian;
        if (TrackNumber != null) yield return TrackNumber;
        if (TrackStatus != null) yield return TrackStatus;
        if (Mode3ACode != null) yield return Mode3ACode;
        if (TargetAddress != null) yield return TargetAddress;
        if (TargetIdentification != null) yield return TargetIdentification;
        if (ModeSMbData != null) yield return ModeSMbData;
        if (VehicleFleetIdentification != null) yield return VehicleFleetIdentification;
        if (FlightLevel != null) yield return FlightLevel;
        if (MeasuredHeight != null) yield return MeasuredHeight;
        if (TargetSizeAndOrientation != null) yield return TargetSizeAndOrientation;
        if (SystemStatus != null) yield return SystemStatus;
        if (PreProgrammedMessage != null) yield return PreProgrammedMessage;
        if (PositionAccuracy != null) yield return PositionAccuracy;
        if (Presence != null) yield return Presence;
        if (AmplitudeOfPrimaryPlot != null) yield return AmplitudeOfPrimaryPlot;
        if (CalculatedAcceleration != null) yield return CalculatedAcceleration;
        if (SpecialPurposeField != null) yield return SpecialPurposeField;
        if (ReservedExpansionField != null) yield return ReservedExpansionField;
    }
}
