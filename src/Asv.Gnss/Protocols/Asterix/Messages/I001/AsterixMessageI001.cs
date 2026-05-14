using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX category 001 message.
/// CAT001 carries radar target reports; the current implementation covers the
/// fields used by the jASTERIX CAT001 edition 1.1 reference sample.
/// </summary>
public class AsterixMessageI001 : AsterixMessage<AsterixRecordI001>
{
    /// <summary>
    /// ASTERIX category identifier for CAT001.
    /// </summary>
    public static byte Category => 1;

    /// <summary>
    /// Gets the message name used by the protocol model.
    /// </summary>
    public override string Name => "I001";

    /// <summary>
    /// Gets the protocol message identifier.
    /// </summary>
    public override byte Id => Category;
}

/// <summary>
/// ASTERIX CAT001 record containing the decoded data items selected by FSPEC.
/// </summary>
public class AsterixRecordI001 : AsterixRecord
{
    /// <summary>
    /// Gets or sets I001/010 Data Source Identifier.
    /// </summary>
    public AsterixFieldI001Frn001Type010? DataSourceIdentifier { get; set; }

    /// <summary>
    /// Gets or sets I001/020 Target Report Descriptor.
    /// </summary>
    public AsterixFieldI001Frn002Type020? TargetReportDescriptor { get; set; }

    /// <summary>
    /// Gets or sets I001/040 Measured Position in Polar Coordinates.
    /// </summary>
    public AsterixFieldI001Frn003Type040? MeasuredPositionPolarCoordinates { get; set; }

    /// <summary>
    /// Gets or sets I001/070 Mode-3/A Reply.
    /// </summary>
    public AsterixFieldI001Frn004Type070? Mode3AReply { get; set; }

    /// <summary>
    /// Gets or sets I001/090 Mode-C Code.
    /// </summary>
    public AsterixFieldI001Frn005Type090? ModeCCode { get; set; }

    /// <summary>
    /// Gets or sets I001/130 Radar Plot Characteristics.
    /// </summary>
    public AsterixFieldI001Frn006Type130? RadarPlotCharacteristics { get; set; }

    /// <summary>
    /// Gets or sets I001/141 Truncated Time of Day.
    /// </summary>
    public AsterixFieldI001Frn007Type141? TruncatedTimeOfDay { get; set; }

    /// <inheritdoc />
    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI001Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI001Frn002Type020.StaticFrn => TargetReportDescriptor = new(),
            AsterixFieldI001Frn003Type040.StaticFrn => MeasuredPositionPolarCoordinates = new(),
            AsterixFieldI001Frn004Type070.StaticFrn => Mode3AReply = new(),
            AsterixFieldI001Frn005Type090.StaticFrn => ModeCCode = new(),
            AsterixFieldI001Frn006Type130.StaticFrn => RadarPlotCharacteristics = new(),
            AsterixFieldI001Frn007Type141.StaticFrn => TruncatedTimeOfDay = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI001)}")
        };
    }

    /// <summary>
    /// Gets the ASTERIX category id for this record.
    /// </summary>
    public override int Category => AsterixMessageI001.Category;

    /// <inheritdoc />
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (TargetReportDescriptor != null) yield return TargetReportDescriptor;
        if (MeasuredPositionPolarCoordinates != null) yield return MeasuredPositionPolarCoordinates;
        if (Mode3AReply != null) yield return Mode3AReply;
        if (ModeCCode != null) yield return ModeCCode;
        if (RadarPlotCharacteristics != null) yield return RadarPlotCharacteristics;
        if (TruncatedTimeOfDay != null) yield return TruncatedTimeOfDay;
    }
}
