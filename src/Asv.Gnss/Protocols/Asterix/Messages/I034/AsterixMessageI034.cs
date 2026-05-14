using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT034 system status message.
/// </summary>
public class AsterixMessageI034 : AsterixMessage<AsterixRecordI034>
{
    /// <summary>
    /// ASTERIX category number.
    /// </summary>
    public static byte Category => 34;

    /// <inheritdoc />
    public override string Name => "I034";

    /// <inheritdoc />
    public override byte Id => Category;
}

/// <summary>
/// ASTERIX CAT034 record using the edition 1.26 field reference number mapping.
/// </summary>
public class AsterixRecordI034 : AsterixRecord
{
    /// <summary>
    /// I034/010 Data Source Identifier.
    /// </summary>
    public AsterixFieldI034Frn001Type010? DataSourceIdentifier { get; set; }

    /// <summary>
    /// I034/000 Message Type.
    /// </summary>
    public AsterixFieldI034Frn002Type000? MessageType { get; set; }

    /// <summary>
    /// I034/030 Time of Day.
    /// </summary>
    public AsterixFieldI034Frn003Type030? TimeOfDay { get; set; }

    /// <summary>
    /// I034/020 Sector Number.
    /// </summary>
    public AsterixFieldI034Frn004Type020? SectorNumber { get; set; }

    /// <summary>
    /// I034/041 Antenna Rotation Speed.
    /// </summary>
    public AsterixFieldI034Frn005Type041? AntennaRotationSpeed { get; set; }

    /// <summary>
    /// I034/050 System Configuration and Status.
    /// </summary>
    public AsterixFieldI034Frn006Type050? SystemConfigurationStatus { get; set; }

    /// <summary>
    /// I034/060 System Processing Mode.
    /// </summary>
    public AsterixFieldI034Frn007Type060? SystemProcessingMode { get; set; }

    /// <summary>
    /// I034/070 Message Count Values.
    /// </summary>
    public AsterixFieldI034Frn008Type070? MessageCountValues { get; set; }

    /// <summary>
    /// I034/100 Generic Polar Window.
    /// </summary>
    public AsterixFieldI034Frn009Type100? GenericPolarWindow { get; set; }

    /// <summary>
    /// I034/110 Data Filter.
    /// </summary>
    public AsterixFieldI034Frn010Type110? DataFilter { get; set; }

    /// <summary>
    /// I034/120 3D-Position of Data Source.
    /// </summary>
    public AsterixFieldI034Frn011Type120? DataSourcePosition { get; set; }

    /// <summary>
    /// I034/090 Collimation Error.
    /// </summary>
    public AsterixFieldI034Frn012Type090? CollimationError { get; set; }

    /// <summary>
    /// I034/RE Reserved Expansion Field.
    /// </summary>
    public AsterixFieldI034Frn013TypeRe? ReservedExpansionField { get; set; }

    /// <summary>
    /// I034/SP Special Purpose Field.
    /// </summary>
    public AsterixFieldI034Frn014TypeSp? SpecialPurposeField { get; set; }

    /// <inheritdoc />
    public override int Category => AsterixMessageI034.Category;

    /// <inheritdoc />
    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI034Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI034Frn002Type000.StaticFrn => MessageType = new(),
            AsterixFieldI034Frn003Type030.StaticFrn => TimeOfDay = new(),
            AsterixFieldI034Frn004Type020.StaticFrn => SectorNumber = new(),
            AsterixFieldI034Frn005Type041.StaticFrn => AntennaRotationSpeed = new(),
            AsterixFieldI034Frn006Type050.StaticFrn => SystemConfigurationStatus = new(),
            AsterixFieldI034Frn007Type060.StaticFrn => SystemProcessingMode = new(),
            AsterixFieldI034Frn008Type070.StaticFrn => MessageCountValues = new(),
            AsterixFieldI034Frn009Type100.StaticFrn => GenericPolarWindow = new(),
            AsterixFieldI034Frn010Type110.StaticFrn => DataFilter = new(),
            AsterixFieldI034Frn011Type120.StaticFrn => DataSourcePosition = new(),
            AsterixFieldI034Frn012Type090.StaticFrn => CollimationError = new(),
            AsterixFieldI034Frn013TypeRe.StaticFrn => ReservedExpansionField = new(),
            AsterixFieldI034Frn014TypeSp.StaticFrn => SpecialPurposeField = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI034)}")
        };
    }

    /// <inheritdoc />
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (MessageType != null) yield return MessageType;
        if (TimeOfDay != null) yield return TimeOfDay;
        if (SectorNumber != null) yield return SectorNumber;
        if (AntennaRotationSpeed != null) yield return AntennaRotationSpeed;
        if (SystemConfigurationStatus != null) yield return SystemConfigurationStatus;
        if (SystemProcessingMode != null) yield return SystemProcessingMode;
        if (MessageCountValues != null) yield return MessageCountValues;
        if (GenericPolarWindow != null) yield return GenericPolarWindow;
        if (DataFilter != null) yield return DataFilter;
        if (DataSourcePosition != null) yield return DataSourcePosition;
        if (CollimationError != null) yield return CollimationError;
        if (ReservedExpansionField != null) yield return ReservedExpansionField;
        if (SpecialPurposeField != null) yield return SpecialPurposeField;
    }
}
