using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT048 radar target report message.
/// </summary>
public class AsterixMessageI048 : AsterixMessage<AsterixRecordI048>
{
    /// <summary>
    /// ASTERIX category number.
    /// </summary>
    public static byte Category => 48;

    /// <inheritdoc />
    public override string Name => "I048";

    /// <inheritdoc />
    public override byte Id => Category;
}

/// <summary>
/// ASTERIX CAT048 record using the editions 1.15 and 1.23 UAP layout.
/// </summary>
public class AsterixRecordI048 : AsterixRecord
{
    /// <summary>
    /// I048/010 Data Source Identifier.
    /// </summary>
    public AsterixFieldI048Frn001Type010? DataSourceIdentifier { get; set; }

    /// <summary>
    /// I048/140 Time of Day.
    /// </summary>
    public AsterixFieldI048Frn002Type140? TimeOfDay { get; set; }

    /// <summary>
    /// I048/020 Target Report Descriptor.
    /// </summary>
    public AsterixFieldI048Frn003Type020? TargetReportDescriptor { get; set; }

    /// <summary>
    /// I048/040 Measured Position in Polar Coordinates.
    /// </summary>
    public AsterixFieldI048Frn004Type040? MeasuredPositionPolarCoordinates { get; set; }

    /// <summary>
    /// I048/070 Mode-3/A Code in Octal Representation.
    /// </summary>
    public AsterixFieldI048Frn005Type070? Mode3ACode { get; set; }

    /// <summary>
    /// I048/090 Flight Level in Binary Representation.
    /// </summary>
    public AsterixFieldI048Frn006Type090? FlightLevel { get; set; }

    /// <summary>
    /// I048/130 Radar Plot Characteristics.
    /// </summary>
    public AsterixFieldI048Frn007Type130? RadarPlotCharacteristics { get; set; }

    /// <summary>
    /// I048/220 Aircraft Address.
    /// </summary>
    public AsterixFieldI048Frn008Type220? AircraftAddress { get; set; }

    /// <summary>
    /// I048/240 Aircraft Identification.
    /// </summary>
    public AsterixFieldI048Frn009Type240? AircraftIdentification { get; set; }

    /// <summary>
    /// I048/250 Mode S MB Data.
    /// </summary>
    public AsterixFieldI048Frn010Type250? ModeSMbData { get; set; }

    /// <summary>
    /// I048/161 Track Number.
    /// </summary>
    public AsterixFieldI048Frn011Type161? TrackNumber { get; set; }

    /// <summary>
    /// I048/042 Calculated Position in Cartesian Coordinates.
    /// </summary>
    public AsterixFieldI048Frn012Type042? CalculatedPositionCartesian { get; set; }

    /// <summary>
    /// I048/200 Calculated Track Velocity in Polar Coordinates.
    /// </summary>
    public AsterixFieldI048Frn013Type200? CalculatedTrackVelocityPolar { get; set; }

    /// <summary>
    /// I048/170 Track Status.
    /// </summary>
    public AsterixFieldI048Frn014Type170? TrackStatus { get; set; }

    /// <summary>
    /// I048/210 Track Quality.
    /// </summary>
    public AsterixFieldI048Frn015Type210? TrackQuality { get; set; }

    /// <summary>
    /// I048/030 Warning/Error Conditions.
    /// </summary>
    public AsterixFieldI048Frn016Type030? WarningErrorConditions { get; set; }

    /// <summary>
    /// I048/080 Mode-3/A Code Confidence Indicator.
    /// </summary>
    public AsterixFieldI048Frn017Type080? Mode3AConfidence { get; set; }

    /// <summary>
    /// I048/100 Mode-C Code and Confidence Indicator.
    /// </summary>
    public AsterixFieldI048Frn018Type100? ModeCCodeAndConfidence { get; set; }

    /// <summary>
    /// I048/110 Height Measured by a 3D Radar.
    /// </summary>
    public AsterixFieldI048Frn019Type110? HeightMeasuredBy3DRadar { get; set; }

    /// <summary>
    /// I048/120 Radial Doppler Speed.
    /// </summary>
    public AsterixFieldI048Frn020Type120? RadialDopplerSpeed { get; set; }

    /// <summary>
    /// I048/230 Communications/ACAS Capability and Flight Status.
    /// </summary>
    public AsterixFieldI048Frn021Type230? CommunicationsCapability { get; set; }

    /// <summary>
    /// I048/260 ACAS Resolution Advisory Report.
    /// </summary>
    public AsterixFieldI048Frn022Type260? AcasResolutionAdvisoryReport { get; set; }

    /// <summary>
    /// I048/055 Mode-1 Code in Octal Representation.
    /// </summary>
    public AsterixFieldI048Frn023Type055? Mode1Code { get; set; }

    /// <summary>
    /// I048/050 Mode-2 Code in Octal Representation.
    /// </summary>
    public AsterixFieldI048Frn024Type050? Mode2Code { get; set; }

    /// <summary>
    /// I048/065 Mode-1 Code Confidence Indicator.
    /// </summary>
    public AsterixFieldI048Frn025Type065? Mode1Confidence { get; set; }

    /// <summary>
    /// I048/060 Mode-2 Code Confidence Indicator.
    /// </summary>
    public AsterixFieldI048Frn026Type060? Mode2Confidence { get; set; }

    /// <summary>
    /// I048/SP Special Purpose Field.
    /// </summary>
    public AsterixFieldI048Frn027TypeSp? SpecialPurposeField { get; set; }

    /// <summary>
    /// I048/RE Reserved Expansion Field.
    /// </summary>
    public AsterixFieldI048Frn028TypeRe? ReservedExpansionField { get; set; }

    /// <inheritdoc />
    public override int Category => AsterixMessageI048.Category;

    /// <inheritdoc />
    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI048Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI048Frn002Type140.StaticFrn => TimeOfDay = new(),
            AsterixFieldI048Frn003Type020.StaticFrn => TargetReportDescriptor = new(),
            AsterixFieldI048Frn004Type040.StaticFrn => MeasuredPositionPolarCoordinates = new(),
            AsterixFieldI048Frn005Type070.StaticFrn => Mode3ACode = new(),
            AsterixFieldI048Frn006Type090.StaticFrn => FlightLevel = new(),
            AsterixFieldI048Frn007Type130.StaticFrn => RadarPlotCharacteristics = new(),
            AsterixFieldI048Frn008Type220.StaticFrn => AircraftAddress = new(),
            AsterixFieldI048Frn009Type240.StaticFrn => AircraftIdentification = new(),
            AsterixFieldI048Frn010Type250.StaticFrn => ModeSMbData = new(),
            AsterixFieldI048Frn011Type161.StaticFrn => TrackNumber = new(),
            AsterixFieldI048Frn012Type042.StaticFrn => CalculatedPositionCartesian = new(),
            AsterixFieldI048Frn013Type200.StaticFrn => CalculatedTrackVelocityPolar = new(),
            AsterixFieldI048Frn014Type170.StaticFrn => TrackStatus = new(),
            AsterixFieldI048Frn015Type210.StaticFrn => TrackQuality = new(),
            AsterixFieldI048Frn016Type030.StaticFrn => WarningErrorConditions = new(),
            AsterixFieldI048Frn017Type080.StaticFrn => Mode3AConfidence = new(),
            AsterixFieldI048Frn018Type100.StaticFrn => ModeCCodeAndConfidence = new(),
            AsterixFieldI048Frn019Type110.StaticFrn => HeightMeasuredBy3DRadar = new(),
            AsterixFieldI048Frn020Type120.StaticFrn => RadialDopplerSpeed = new(),
            AsterixFieldI048Frn021Type230.StaticFrn => CommunicationsCapability = new(),
            AsterixFieldI048Frn022Type260.StaticFrn => AcasResolutionAdvisoryReport = new(),
            AsterixFieldI048Frn023Type055.StaticFrn => Mode1Code = new(),
            AsterixFieldI048Frn024Type050.StaticFrn => Mode2Code = new(),
            AsterixFieldI048Frn025Type065.StaticFrn => Mode1Confidence = new(),
            AsterixFieldI048Frn026Type060.StaticFrn => Mode2Confidence = new(),
            AsterixFieldI048Frn027TypeSp.StaticFrn => SpecialPurposeField = new(),
            AsterixFieldI048Frn028TypeRe.StaticFrn => ReservedExpansionField = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI048)}")
        };
    }

    /// <inheritdoc />
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (TimeOfDay != null) yield return TimeOfDay;
        if (TargetReportDescriptor != null) yield return TargetReportDescriptor;
        if (MeasuredPositionPolarCoordinates != null) yield return MeasuredPositionPolarCoordinates;
        if (Mode3ACode != null) yield return Mode3ACode;
        if (FlightLevel != null) yield return FlightLevel;
        if (RadarPlotCharacteristics != null) yield return RadarPlotCharacteristics;
        if (AircraftAddress != null) yield return AircraftAddress;
        if (AircraftIdentification != null) yield return AircraftIdentification;
        if (ModeSMbData != null) yield return ModeSMbData;
        if (TrackNumber != null) yield return TrackNumber;
        if (CalculatedPositionCartesian != null) yield return CalculatedPositionCartesian;
        if (CalculatedTrackVelocityPolar != null) yield return CalculatedTrackVelocityPolar;
        if (TrackStatus != null) yield return TrackStatus;
        if (TrackQuality != null) yield return TrackQuality;
        if (WarningErrorConditions != null) yield return WarningErrorConditions;
        if (Mode3AConfidence != null) yield return Mode3AConfidence;
        if (ModeCCodeAndConfidence != null) yield return ModeCCodeAndConfidence;
        if (HeightMeasuredBy3DRadar != null) yield return HeightMeasuredBy3DRadar;
        if (RadialDopplerSpeed != null) yield return RadialDopplerSpeed;
        if (CommunicationsCapability != null) yield return CommunicationsCapability;
        if (AcasResolutionAdvisoryReport != null) yield return AcasResolutionAdvisoryReport;
        if (Mode1Code != null) yield return Mode1Code;
        if (Mode2Code != null) yield return Mode2Code;
        if (Mode1Confidence != null) yield return Mode1Confidence;
        if (Mode2Confidence != null) yield return Mode2Confidence;
        if (SpecialPurposeField != null) yield return SpecialPurposeField;
        if (ReservedExpansionField != null) yield return ReservedExpansionField;
    }
}
