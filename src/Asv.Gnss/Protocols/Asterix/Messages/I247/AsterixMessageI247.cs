using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT247 version number exchange message.
/// </summary>
public class AsterixMessageI247 : AsterixMessage<AsterixRecordI247>
{
    /// <summary>
    /// ASTERIX category number.
    /// </summary>
    public static byte Category => 247;

    /// <inheritdoc />
    public override string Name => "I247";

    /// <inheritdoc />
    public override byte Id => Category;
}

/// <summary>
/// ASTERIX CAT247 record using the edition 1.2/1.3 field reference number mapping.
/// </summary>
public class AsterixRecordI247 : AsterixRecord
{
    /// <summary>
    /// I247/010 Data Source Identifier.
    /// </summary>
    public AsterixFieldI247Frn001Type010? DataSourceIdentifier { get; set; }

    /// <summary>
    /// I247/015 Service Identification.
    /// </summary>
    public AsterixFieldI247Frn002Type015? ServiceIdentification { get; set; }

    /// <summary>
    /// I247/140 Time of Day.
    /// </summary>
    public AsterixFieldI247Frn003Type140? TimeOfDay { get; set; }

    /// <summary>
    /// I247/550 Category Version Number Report.
    /// </summary>
    public AsterixFieldI247Frn004Type550? CategoryVersionNumberReport { get; set; }

    /// <summary>
    /// I247/SP Special Purpose Field.
    /// </summary>
    public AsterixFieldI247Frn006TypeSp? SpecialPurposeField { get; set; }

    /// <summary>
    /// I247/RE Reserved Expansion Field.
    /// </summary>
    public AsterixFieldI247Frn007TypeRe? ReservedExpansionField { get; set; }

    /// <inheritdoc />
    public override int Category => AsterixMessageI247.Category;

    /// <inheritdoc />
    protected override AsterixField AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI247Frn001Type010.StaticFrn => DataSourceIdentifier = new(),
            AsterixFieldI247Frn002Type015.StaticFrn => ServiceIdentification = new(),
            AsterixFieldI247Frn003Type140.StaticFrn => TimeOfDay = new(),
            AsterixFieldI247Frn004Type550.StaticFrn => CategoryVersionNumberReport = new(),
            AsterixFieldI247Frn006TypeSp.StaticFrn => SpecialPurposeField = new(),
            AsterixFieldI247Frn007TypeRe.StaticFrn => ReservedExpansionField = new(),
            _ => throw new InvalidOperationException($"Unknown field reference number {fieldReferenceNumber} for {nameof(AsterixRecordI247)}")
        };
    }

    /// <inheritdoc />
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null) yield return DataSourceIdentifier;
        if (ServiceIdentification != null) yield return ServiceIdentification;
        if (TimeOfDay != null) yield return TimeOfDay;
        if (CategoryVersionNumberReport != null) yield return CategoryVersionNumberReport;
        if (SpecialPurposeField != null) yield return SpecialPurposeField;
        if (ReservedExpansionField != null) yield return ReservedExpansionField;
    }
}
