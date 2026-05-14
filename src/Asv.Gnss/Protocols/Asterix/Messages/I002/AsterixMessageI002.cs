using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT002 system track messages, edition 1.0 UAP.
/// </summary>
public class AsterixMessageI002 : AsterixMessage<AsterixRecordI002>
{
    /// <summary>
    /// ASTERIX category number.
    /// </summary>
    public static byte Category => 2;

    /// <inheritdoc />
    public override string Name => "I002";

    /// <inheritdoc />
    public override byte Id => Category;
}

/// <summary>
/// ASTERIX CAT002 record using the edition 1.0 field reference number mapping.
/// </summary>
public class AsterixRecordI002 : AsterixRecord
{
    public AsterixFieldI002Frn001Type010? DataSourceIdentifier { get; set; }
    public AsterixFieldI002Frn002Type000? MessageType { get; set; }
    public AsterixFieldI002Frn003Type020? SectorNumber { get; set; }
    public AsterixFieldI002Frn004Type030? TimeOfDay { get; set; }
    public AsterixFieldI002Frn005Type041? AntennaRotationSpeed { get; set; }
    public AsterixFieldI002Frn006Type050? StationConfigurationStatus { get; set; }
    public AsterixFieldI002Frn007Type060? StationProcessingMode { get; set; }
    public AsterixFieldI002Frn008Type070? PlotCountValues { get; set; }
    public AsterixFieldI002Frn009Type100? DynamicWindowType1 { get; set; }
    public AsterixFieldI002Frn010Type090? CollimationError { get; set; }
    public AsterixFieldI002Frn011Type080? WarningErrorConditions { get; set; }
    public AsterixFieldI002Frn013TypeSp? SpecialPurposeField { get; set; }

    /// <inheritdoc />
    public override int Category => AsterixMessageI002.Category;

    /// <inheritdoc />
    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI002Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI002Frn002Type000.StaticFrn => MessageType = new(),
            AsterixFieldI002Frn003Type020.StaticFrn => SectorNumber = new(),
            AsterixFieldI002Frn004Type030.StaticFrn => TimeOfDay = new(),
            AsterixFieldI002Frn005Type041.StaticFrn => AntennaRotationSpeed = new(),
            AsterixFieldI002Frn006Type050.StaticFrn => StationConfigurationStatus = new(),
            AsterixFieldI002Frn007Type060.StaticFrn => StationProcessingMode = new(),
            AsterixFieldI002Frn008Type070.StaticFrn => PlotCountValues = new(),
            AsterixFieldI002Frn009Type100.StaticFrn => DynamicWindowType1 = new(),
            AsterixFieldI002Frn010Type090.StaticFrn => CollimationError = new(),
            AsterixFieldI002Frn011Type080.StaticFrn => WarningErrorConditions = new(),
            AsterixFieldI002Frn013TypeSp.StaticFrn => SpecialPurposeField = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI002)}")
        };
    }

    /// <inheritdoc />
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (MessageType != null) yield return MessageType;
        if (SectorNumber != null) yield return SectorNumber;
        if (TimeOfDay != null) yield return TimeOfDay;
        if (AntennaRotationSpeed != null) yield return AntennaRotationSpeed;
        if (StationConfigurationStatus != null) yield return StationConfigurationStatus;
        if (StationProcessingMode != null) yield return StationProcessingMode;
        if (PlotCountValues != null) yield return PlotCountValues;
        if (DynamicWindowType1 != null) yield return DynamicWindowType1;
        if (CollimationError != null) yield return CollimationError;
        if (WarningErrorConditions != null) yield return WarningErrorConditions;
        if (SpecialPurposeField != null) yield return SpecialPurposeField;
    }
}
