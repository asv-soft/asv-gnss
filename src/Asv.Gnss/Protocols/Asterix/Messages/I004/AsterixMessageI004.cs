using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT004 safety-net messages, edition 1.4 UAP.
/// </summary>
public class AsterixMessageI004 : AsterixMessage<AsterixRecordI004>
{
    /// <summary>
    /// ASTERIX category number.
    /// </summary>
    public static byte Category => 4;

    /// <inheritdoc />
    public override string Name => "I004";

    /// <inheritdoc />
    public override byte Id => Category;
}

/// <summary>
/// ASTERIX CAT004 record using the edition 1.4 field reference number mapping.
/// </summary>
public class AsterixRecordI004 : AsterixRecord
{
    public AsterixFieldI004Frn001Type010? DataSourceIdentifier { get; set; }
    public AsterixFieldI004Frn002Type000? MessageType { get; set; }
    public AsterixFieldI004Frn003Type015? SdpsIdentifier { get; set; }
    public AsterixFieldI004Frn004Type020? TimeOfMessage { get; set; }
    public AsterixFieldI004Frn005Type040? AlertIdentifier { get; set; }
    public AsterixFieldI004Frn006Type045? AlertStatus { get; set; }
    public AsterixFieldI004Frn007Type060? SafetyNetFunctionStatus { get; set; }
    public AsterixFieldI004Frn008Type030? TrackNumber1 { get; set; }
    public AsterixFieldI004Frn009Type170? AircraftIdentification1 { get; set; }
    public AsterixFieldI004Frn010Type120? ConflictCharacteristics { get; set; }
    public AsterixFieldI004Frn011Type070? ConflictTimingAndSeparation { get; set; }
    public AsterixFieldI004Frn012Type076? VerticalDeviation { get; set; }
    public AsterixFieldI004Frn013Type074? LongitudinalDeviation { get; set; }
    public AsterixFieldI004Frn014Type075? TransversalDistanceDeviation { get; set; }
    public AsterixFieldI004Frn015Type100? AreaDefinition { get; set; }
    public AsterixFieldI004Frn016Type035? TrackNumber2 { get; set; }
    public AsterixFieldI004Frn017Type171? AircraftIdentification2 { get; set; }
    public AsterixFieldI004Frn018Type110? FdpsSectorControlIdentification { get; set; }
    public AsterixFieldI004Frn020TypeRe? ReservedExpansionField { get; set; }
    public AsterixFieldI004Frn021TypeSp? SpecialPurposeField { get; set; }

    /// <inheritdoc />
    public override int Category => AsterixMessageI004.Category;

    /// <inheritdoc />
    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI004Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI004Frn002Type000.StaticFrn => MessageType = new(),
            AsterixFieldI004Frn003Type015.StaticFrn => SdpsIdentifier = new(),
            AsterixFieldI004Frn004Type020.StaticFrn => TimeOfMessage = new(),
            AsterixFieldI004Frn005Type040.StaticFrn => AlertIdentifier = new(),
            AsterixFieldI004Frn006Type045.StaticFrn => AlertStatus = new(),
            AsterixFieldI004Frn007Type060.StaticFrn => SafetyNetFunctionStatus = new(),
            AsterixFieldI004Frn008Type030.StaticFrn => TrackNumber1 = new(),
            AsterixFieldI004Frn009Type170.StaticFrn => AircraftIdentification1 = new(),
            AsterixFieldI004Frn010Type120.StaticFrn => ConflictCharacteristics = new(),
            AsterixFieldI004Frn011Type070.StaticFrn => ConflictTimingAndSeparation = new(),
            AsterixFieldI004Frn012Type076.StaticFrn => VerticalDeviation = new(),
            AsterixFieldI004Frn013Type074.StaticFrn => LongitudinalDeviation = new(),
            AsterixFieldI004Frn014Type075.StaticFrn => TransversalDistanceDeviation = new(),
            AsterixFieldI004Frn015Type100.StaticFrn => AreaDefinition = new(),
            AsterixFieldI004Frn016Type035.StaticFrn => TrackNumber2 = new(),
            AsterixFieldI004Frn017Type171.StaticFrn => AircraftIdentification2 = new(),
            AsterixFieldI004Frn018Type110.StaticFrn => FdpsSectorControlIdentification = new(),
            AsterixFieldI004Frn020TypeRe.StaticFrn => ReservedExpansionField = new(),
            AsterixFieldI004Frn021TypeSp.StaticFrn => SpecialPurposeField = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI004)}")
        };
    }

    /// <inheritdoc />
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (MessageType != null) yield return MessageType;
        if (SdpsIdentifier != null) yield return SdpsIdentifier;
        if (TimeOfMessage != null) yield return TimeOfMessage;
        if (AlertIdentifier != null) yield return AlertIdentifier;
        if (AlertStatus != null) yield return AlertStatus;
        if (SafetyNetFunctionStatus != null) yield return SafetyNetFunctionStatus;
        if (TrackNumber1 != null) yield return TrackNumber1;
        if (AircraftIdentification1 != null) yield return AircraftIdentification1;
        if (ConflictCharacteristics != null) yield return ConflictCharacteristics;
        if (ConflictTimingAndSeparation != null) yield return ConflictTimingAndSeparation;
        if (VerticalDeviation != null) yield return VerticalDeviation;
        if (LongitudinalDeviation != null) yield return LongitudinalDeviation;
        if (TransversalDistanceDeviation != null) yield return TransversalDistanceDeviation;
        if (AreaDefinition != null) yield return AreaDefinition;
        if (TrackNumber2 != null) yield return TrackNumber2;
        if (AircraftIdentification2 != null) yield return AircraftIdentification2;
        if (FdpsSectorControlIdentification != null) yield return FdpsSectorControlIdentification;
        if (ReservedExpansionField != null) yield return ReservedExpansionField;
        if (SpecialPurposeField != null) yield return SpecialPurposeField;
    }
}
