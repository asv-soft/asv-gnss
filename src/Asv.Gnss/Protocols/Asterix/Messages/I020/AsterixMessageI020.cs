using System;
using System.Collections.Generic;

namespace Asv.Gnss;

public class AsterixMessageI020 : AsterixMessage<AsterixRecordI020>
{
    public static byte Category => 20;
    public override string Name => "I020";
    public override byte Id => Category;
    
}

public class AsterixRecordI020 : AsterixRecord
{
    public AsterixFieldI020Frn001Type010? DataSourceIdentifier { get; set; }
    public AsterixFieldI020Frn002Type020? TargetReportDescriptor { get; set; }
    public AsterixFieldI020Frn003Type140? TimeOfDay { get; set; }
    public AsterixFieldI020Frn004Type041? PositionWgs84 { get; set; }
    public AsterixFieldI020Frn005Type042? PositionCartesian { get; set; }
    public AsterixFieldI020Frn006Type161? TrackNumber { get; set; }
    public AsterixFieldI020Frn007Type170? TrackStatus { get; set; }
    public AsterixFieldI020Frn008Type070? Mode3ACode { get; set; }
    public AsterixFieldI020Frn009Type202? TrackVelocityCartesian { get; set; }
    public AsterixFieldI020Frn010Type090? FlightLevel { get; set; }
    public AsterixFieldI020Frn011Type100? ModeCCode { get; set; }
    public AsterixFieldI020Frn012Type220? TargetAddress { get; set; }
    public AsterixFieldI020Frn013Type245? TargetIdentification { get; set; }
    public AsterixFieldI020Frn014Type110? MeasuredHeightCartesian { get; set; }
    public AsterixFieldI020Frn015Type105? GeometricHeightWgs84 { get; set; }
    public AsterixFieldI020Frn016Type210? CalculatedAcceleration { get; set; }
    public AsterixFieldI020Frn017Type300? VehicleFleetId { get; set; }
    public AsterixFieldI020Frn018Type310? PreProgrammedMessage { get; set; }
    public AsterixFieldI020Frn019Type500? PositionAccuracy { get; set; }
    public AsterixFieldI020Frn020Type400? ContributingDevices { get; set; }
    public AsterixFieldI020Frn021Type250? ModeSMbData { get; set; }
    public AsterixFieldI020Frn022Type230? CommsAcasCapabilityStatus { get; set; }
    public AsterixFieldI020Frn023Type260? AcasResolutionAdvisoryReport { get; set; }
    public AsterixFieldI020Frn024Type030? WarningErrorConditions { get; set; }
    public AsterixFieldI020Frn025Type055? Mode1Code { get; set; }
    public AsterixFieldI020Frn026Type050? Mode2Code { get; set; }
    public AsterixFieldI020Frn027TypeRe? ReservedExpansionField { get; set; }

    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI020Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI020Frn002Type020.StaticFrn => TargetReportDescriptor = new(),
            AsterixFieldI020Frn003Type140.StaticFrn => TimeOfDay = new(),
            AsterixFieldI020Frn004Type041.StaticFrn => PositionWgs84 = new(),
            AsterixFieldI020Frn005Type042.StaticFrn => PositionCartesian = new(),
            AsterixFieldI020Frn006Type161.StaticFrn => TrackNumber = new(),
            AsterixFieldI020Frn007Type170.StaticFrn => TrackStatus = new(),
            AsterixFieldI020Frn008Type070.StaticFrn => Mode3ACode = new(),
            AsterixFieldI020Frn009Type202.StaticFrn => TrackVelocityCartesian = new(),
            AsterixFieldI020Frn010Type090.StaticFrn => FlightLevel = new(),
            AsterixFieldI020Frn011Type100.StaticFrn => ModeCCode = new(),
            AsterixFieldI020Frn012Type220.StaticFrn => TargetAddress = new(),
            AsterixFieldI020Frn013Type245.StaticFrn => TargetIdentification = new(),
            AsterixFieldI020Frn014Type110.StaticFrn => MeasuredHeightCartesian = new(),
            AsterixFieldI020Frn015Type105.StaticFrn => GeometricHeightWgs84 = new(),
            AsterixFieldI020Frn016Type210.StaticFrn => CalculatedAcceleration = new(),
            AsterixFieldI020Frn017Type300.StaticFrn => VehicleFleetId = new(),
            AsterixFieldI020Frn018Type310.StaticFrn => PreProgrammedMessage = new(),
            AsterixFieldI020Frn019Type500.StaticFrn => PositionAccuracy = new(),
            AsterixFieldI020Frn020Type400.StaticFrn => ContributingDevices = new(),
            AsterixFieldI020Frn021Type250.StaticFrn => ModeSMbData = new(),
            AsterixFieldI020Frn022Type230.StaticFrn => CommsAcasCapabilityStatus = new(),
            AsterixFieldI020Frn023Type260.StaticFrn => AcasResolutionAdvisoryReport = new(),
            AsterixFieldI020Frn024Type030.StaticFrn => WarningErrorConditions = new(),
            AsterixFieldI020Frn025Type055.StaticFrn => Mode1Code = new(),
            AsterixFieldI020Frn026Type050.StaticFrn => Mode2Code = new(),
            AsterixFieldI020Frn027TypeRe.StaticFrn => ReservedExpansionField = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI020)}")
        };
    }

    

    public override int Category => AsterixMessageI020.Category;

    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (TargetReportDescriptor != null) yield return TargetReportDescriptor;
        if (TimeOfDay != null) yield return TimeOfDay;
        if (PositionWgs84 != null) yield return PositionWgs84;
        if (PositionCartesian != null) yield return PositionCartesian;
        if (TrackNumber != null) yield return TrackNumber;
        if (TrackStatus != null) yield return TrackStatus;
        if (Mode3ACode != null) yield return Mode3ACode;
        if (TrackVelocityCartesian != null) yield return TrackVelocityCartesian;
        if (FlightLevel != null) yield return FlightLevel;
        if (ModeCCode != null) yield return ModeCCode;
        if (TargetAddress != null) yield return TargetAddress;
        if (TargetIdentification != null) yield return TargetIdentification;
        if (MeasuredHeightCartesian != null) yield return MeasuredHeightCartesian;
        if (GeometricHeightWgs84 != null) yield return GeometricHeightWgs84;
        if (CalculatedAcceleration != null) yield return CalculatedAcceleration;
        if (VehicleFleetId != null) yield return VehicleFleetId;
        if (PreProgrammedMessage != null) yield return PreProgrammedMessage;
        if (PositionAccuracy != null) yield return PositionAccuracy;
        if (ContributingDevices != null) yield return ContributingDevices;
        if (ModeSMbData != null) yield return ModeSMbData;
        if (CommsAcasCapabilityStatus != null) yield return CommsAcasCapabilityStatus;
        if (AcasResolutionAdvisoryReport != null) yield return AcasResolutionAdvisoryReport;
        if (WarningErrorConditions != null) yield return WarningErrorConditions;
        if (Mode1Code != null) yield return Mode1Code;
        if (Mode2Code != null) yield return Mode2Code;
        if (ReservedExpansionField != null) yield return ReservedExpansionField;
    }
}