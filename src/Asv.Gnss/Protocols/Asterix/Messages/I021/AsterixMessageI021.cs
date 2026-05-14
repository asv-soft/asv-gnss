using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT021 ADS-B reports, edition 2.1 UAP.
/// </summary>
public class AsterixMessageI021 : AsterixMessage<AsterixRecordI021>
{
    /// <summary>
    /// ASTERIX category number.
    /// </summary>
    public static byte Category => 21;

    /// <inheritdoc />
    public override string Name => "I021";

    /// <inheritdoc />
    public override byte Id => Category;
}

/// <summary>
/// ASTERIX CAT021 record using the edition 2.1 field reference number mapping.
/// </summary>
public class AsterixRecordI021 : AsterixRecord
{
    public AsterixFieldI021Frn001Type010? DataSourceIdentifier { get; set; }
    public AsterixFieldI021Frn002Type040? TargetReportDescriptor { get; set; }
    public AsterixFieldI021Frn003Type161? TrackNumber { get; set; }
    public AsterixFieldI021Frn004Type015? ServiceIdentification { get; set; }
    public AsterixFieldI021Frn005Type071? TimeOfApplicabilityForPosition { get; set; }
    public AsterixFieldI021Frn006Type130? PositionWgs84 { get; set; }
    public AsterixFieldI021Frn007Type131? HighResolutionPositionWgs84 { get; set; }
    public AsterixFieldI021Frn009Type072? TimeOfApplicabilityForVelocity { get; set; }
    public AsterixFieldI021Frn010Type150? AirSpeed { get; set; }
    public AsterixFieldI021Frn011Type151? TrueAirSpeed { get; set; }
    public AsterixFieldI021Frn012Type080? TargetAddress { get; set; }
    public AsterixFieldI021Frn013Type073? TimeOfMessageReceptionForPosition { get; set; }
    public AsterixFieldI021Frn014Type074? TimeOfMessageReceptionForPositionHighPrecision { get; set; }
    public AsterixFieldI021Frn015Type075? TimeOfMessageReceptionForVelocity { get; set; }
    public AsterixFieldI021Frn017Type076? TimeOfMessageReceptionForVelocityHighPrecision { get; set; }
    public AsterixFieldI021Frn018Type140? GeometricHeight { get; set; }
    public AsterixFieldI021Frn019Type090? QualityIndicators { get; set; }
    public AsterixFieldI021Frn020Type210? MopsVersion { get; set; }
    public AsterixFieldI021Frn021Type070? Mode3ACode { get; set; }
    public AsterixFieldI021Frn022Type230? RollAngle { get; set; }
    public AsterixFieldI021Frn023Type145? FlightLevel { get; set; }
    public AsterixFieldI021Frn025Type152? MagneticHeading { get; set; }
    public AsterixFieldI021Frn026Type200? TargetStatus { get; set; }
    public AsterixFieldI021Frn027Type155? BarometricVerticalRate { get; set; }
    public AsterixFieldI021Frn028Type157? GeometricVerticalRate { get; set; }
    public AsterixFieldI021Frn029Type160? AirborneGroundVector { get; set; }
    public AsterixFieldI021Frn030Type165? TrackAngleRate { get; set; }
    public AsterixFieldI021Frn031Type077? TimeOfAsterixReportTransmission { get; set; }
    public AsterixFieldI021Frn033Type170? TargetIdentification { get; set; }
    public AsterixFieldI021Frn034Type020? EmitterCategory { get; set; }
    public AsterixFieldI021Frn035Type220? MetInformation { get; set; }
    public AsterixFieldI021Frn036Type146? SelectedAltitude { get; set; }
    public AsterixFieldI021Frn037Type148? FinalStateSelectedAltitude { get; set; }
    public AsterixFieldI021Frn038Type110? TrajectoryIntent { get; set; }
    public AsterixFieldI021Frn039Type016? ServiceManagement { get; set; }
    public AsterixFieldI021Frn041Type008? AircraftOperationalStatus { get; set; }
    public AsterixFieldI021Frn042Type271? SurfaceCapabilitiesAndCharacteristics { get; set; }
    public AsterixFieldI021Frn043Type132? MessageAmplitude { get; set; }
    public AsterixFieldI021Frn044Type250? ModeSMbData { get; set; }
    public AsterixFieldI021Frn045Type260? AcasResolutionAdvisoryReport { get; set; }
    public AsterixFieldI021Frn046Type400? ReceiverId { get; set; }
    public AsterixFieldI021Frn047Type295? DataAges { get; set; }
    public AsterixFieldI021Frn054TypeRe? ReservedExpansionField { get; set; }
    public AsterixFieldI021Frn055TypeSp? SpecialPurposeField { get; set; }

    /// <inheritdoc />
    public override int Category => AsterixMessageI021.Category;

    /// <inheritdoc />
    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI021Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI021Frn002Type040.StaticFrn => TargetReportDescriptor = new(),
            AsterixFieldI021Frn003Type161.StaticFrn => TrackNumber = new(),
            AsterixFieldI021Frn004Type015.StaticFrn => ServiceIdentification = new(),
            AsterixFieldI021Frn005Type071.StaticFrn => TimeOfApplicabilityForPosition = new(),
            AsterixFieldI021Frn006Type130.StaticFrn => PositionWgs84 = new(),
            AsterixFieldI021Frn007Type131.StaticFrn => HighResolutionPositionWgs84 = new(),
            AsterixFieldI021Frn009Type072.StaticFrn => TimeOfApplicabilityForVelocity = new(),
            AsterixFieldI021Frn010Type150.StaticFrn => AirSpeed = new(),
            AsterixFieldI021Frn011Type151.StaticFrn => TrueAirSpeed = new(),
            AsterixFieldI021Frn012Type080.StaticFrn => TargetAddress = new(),
            AsterixFieldI021Frn013Type073.StaticFrn => TimeOfMessageReceptionForPosition = new(),
            AsterixFieldI021Frn014Type074.StaticFrn => TimeOfMessageReceptionForPositionHighPrecision = new(),
            AsterixFieldI021Frn015Type075.StaticFrn => TimeOfMessageReceptionForVelocity = new(),
            AsterixFieldI021Frn017Type076.StaticFrn => TimeOfMessageReceptionForVelocityHighPrecision = new(),
            AsterixFieldI021Frn018Type140.StaticFrn => GeometricHeight = new(),
            AsterixFieldI021Frn019Type090.StaticFrn => QualityIndicators = new(),
            AsterixFieldI021Frn020Type210.StaticFrn => MopsVersion = new(),
            AsterixFieldI021Frn021Type070.StaticFrn => Mode3ACode = new(),
            AsterixFieldI021Frn022Type230.StaticFrn => RollAngle = new(),
            AsterixFieldI021Frn023Type145.StaticFrn => FlightLevel = new(),
            AsterixFieldI021Frn025Type152.StaticFrn => MagneticHeading = new(),
            AsterixFieldI021Frn026Type200.StaticFrn => TargetStatus = new(),
            AsterixFieldI021Frn027Type155.StaticFrn => BarometricVerticalRate = new(),
            AsterixFieldI021Frn028Type157.StaticFrn => GeometricVerticalRate = new(),
            AsterixFieldI021Frn029Type160.StaticFrn => AirborneGroundVector = new(),
            AsterixFieldI021Frn030Type165.StaticFrn => TrackAngleRate = new(),
            AsterixFieldI021Frn031Type077.StaticFrn => TimeOfAsterixReportTransmission = new(),
            AsterixFieldI021Frn033Type170.StaticFrn => TargetIdentification = new(),
            AsterixFieldI021Frn034Type020.StaticFrn => EmitterCategory = new(),
            AsterixFieldI021Frn035Type220.StaticFrn => MetInformation = new(),
            AsterixFieldI021Frn036Type146.StaticFrn => SelectedAltitude = new(),
            AsterixFieldI021Frn037Type148.StaticFrn => FinalStateSelectedAltitude = new(),
            AsterixFieldI021Frn038Type110.StaticFrn => TrajectoryIntent = new(),
            AsterixFieldI021Frn039Type016.StaticFrn => ServiceManagement = new(),
            AsterixFieldI021Frn041Type008.StaticFrn => AircraftOperationalStatus = new(),
            AsterixFieldI021Frn042Type271.StaticFrn => SurfaceCapabilitiesAndCharacteristics = new(),
            AsterixFieldI021Frn043Type132.StaticFrn => MessageAmplitude = new(),
            AsterixFieldI021Frn044Type250.StaticFrn => ModeSMbData = new(),
            AsterixFieldI021Frn045Type260.StaticFrn => AcasResolutionAdvisoryReport = new(),
            AsterixFieldI021Frn046Type400.StaticFrn => ReceiverId = new(),
            AsterixFieldI021Frn047Type295.StaticFrn => DataAges = new(),
            AsterixFieldI021Frn054TypeRe.StaticFrn => ReservedExpansionField = new(),
            AsterixFieldI021Frn055TypeSp.StaticFrn => SpecialPurposeField = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI021)}")
        };
    }

    /// <inheritdoc />
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (TargetReportDescriptor != null) yield return TargetReportDescriptor;
        if (TrackNumber != null) yield return TrackNumber;
        if (ServiceIdentification != null) yield return ServiceIdentification;
        if (TimeOfApplicabilityForPosition != null) yield return TimeOfApplicabilityForPosition;
        if (PositionWgs84 != null) yield return PositionWgs84;
        if (HighResolutionPositionWgs84 != null) yield return HighResolutionPositionWgs84;
        if (TimeOfApplicabilityForVelocity != null) yield return TimeOfApplicabilityForVelocity;
        if (AirSpeed != null) yield return AirSpeed;
        if (TrueAirSpeed != null) yield return TrueAirSpeed;
        if (TargetAddress != null) yield return TargetAddress;
        if (TimeOfMessageReceptionForPosition != null) yield return TimeOfMessageReceptionForPosition;
        if (TimeOfMessageReceptionForPositionHighPrecision != null) yield return TimeOfMessageReceptionForPositionHighPrecision;
        if (TimeOfMessageReceptionForVelocity != null) yield return TimeOfMessageReceptionForVelocity;
        if (TimeOfMessageReceptionForVelocityHighPrecision != null) yield return TimeOfMessageReceptionForVelocityHighPrecision;
        if (GeometricHeight != null) yield return GeometricHeight;
        if (QualityIndicators != null) yield return QualityIndicators;
        if (MopsVersion != null) yield return MopsVersion;
        if (Mode3ACode != null) yield return Mode3ACode;
        if (RollAngle != null) yield return RollAngle;
        if (FlightLevel != null) yield return FlightLevel;
        if (MagneticHeading != null) yield return MagneticHeading;
        if (TargetStatus != null) yield return TargetStatus;
        if (BarometricVerticalRate != null) yield return BarometricVerticalRate;
        if (GeometricVerticalRate != null) yield return GeometricVerticalRate;
        if (AirborneGroundVector != null) yield return AirborneGroundVector;
        if (TrackAngleRate != null) yield return TrackAngleRate;
        if (TimeOfAsterixReportTransmission != null) yield return TimeOfAsterixReportTransmission;
        if (TargetIdentification != null) yield return TargetIdentification;
        if (EmitterCategory != null) yield return EmitterCategory;
        if (MetInformation != null) yield return MetInformation;
        if (SelectedAltitude != null) yield return SelectedAltitude;
        if (FinalStateSelectedAltitude != null) yield return FinalStateSelectedAltitude;
        if (TrajectoryIntent != null) yield return TrajectoryIntent;
        if (ServiceManagement != null) yield return ServiceManagement;
        if (AircraftOperationalStatus != null) yield return AircraftOperationalStatus;
        if (SurfaceCapabilitiesAndCharacteristics != null) yield return SurfaceCapabilitiesAndCharacteristics;
        if (MessageAmplitude != null) yield return MessageAmplitude;
        if (ModeSMbData != null) yield return ModeSMbData;
        if (AcasResolutionAdvisoryReport != null) yield return AcasResolutionAdvisoryReport;
        if (ReceiverId != null) yield return ReceiverId;
        if (DataAges != null) yield return DataAges;
        if (ReservedExpansionField != null) yield return ReservedExpansionField;
        if (SpecialPurposeField != null) yield return SpecialPurposeField;
    }
}
