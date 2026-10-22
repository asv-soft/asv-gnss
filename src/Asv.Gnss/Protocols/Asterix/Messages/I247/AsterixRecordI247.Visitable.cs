using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixRecordI247
{
    private static readonly Field VisitorDataSourceIdentifierField = new Field.Builder().Name(nameof(DataSourceIdentifier)).Title(nameof(DataSourceIdentifier)).Description("DataSourceIdentifier").DataType(new OptionalStructType(AsterixFieldI247Frn001Type010.AsterixVisitorType)).Build();
    private static readonly Field VisitorServiceIdentificationField = new Field.Builder().Name(nameof(ServiceIdentification)).Title(nameof(ServiceIdentification)).Description("ServiceIdentification").DataType(new OptionalStructType(AsterixFieldI247Frn002Type015.AsterixVisitorType)).Build();
    private static readonly Field VisitorTimeOfDayField = new Field.Builder().Name(nameof(TimeOfDay)).Title(nameof(TimeOfDay)).Description("TimeOfDay").DataType(new OptionalStructType(AsterixFieldI247Frn003Type140.AsterixVisitorType)).Build();
    private static readonly Field VisitorCategoryVersionNumberReportField = new Field.Builder().Name(nameof(CategoryVersionNumberReport)).Title(nameof(CategoryVersionNumberReport)).Description("CategoryVersionNumberReport").DataType(new OptionalStructType(AsterixFieldI247Frn004Type550.AsterixVisitorType)).Build();
    private static readonly Field VisitorSpecialPurposeFieldField = new Field.Builder().Name(nameof(SpecialPurposeField)).Title(nameof(SpecialPurposeField)).Description("SpecialPurposeField").DataType(new OptionalStructType(AsterixFieldI247Frn006TypeSp.AsterixVisitorType)).Build();
    private static readonly Field VisitorReservedExpansionFieldField = new Field.Builder().Name(nameof(ReservedExpansionField)).Title(nameof(ReservedExpansionField)).Description("ReservedExpansionField").DataType(new OptionalStructType(AsterixFieldI247Frn007TypeRe.AsterixVisitorType)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorDataSourceIdentifierField, VisitorServiceIdentificationField, VisitorTimeOfDayField, VisitorCategoryVersionNumberReportField, VisitorSpecialPurposeFieldField, VisitorReservedExpansionFieldField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueDataSourceIdentifier = DataSourceIdentifier;
        OptionalStructType.Accept(visitor, VisitorDataSourceIdentifierField, ref valueDataSourceIdentifier);
        if (!ReferenceEquals(valueDataSourceIdentifier, DataSourceIdentifier)) DataSourceIdentifier = valueDataSourceIdentifier;
        var valueServiceIdentification = ServiceIdentification;
        OptionalStructType.Accept(visitor, VisitorServiceIdentificationField, ref valueServiceIdentification);
        if (!ReferenceEquals(valueServiceIdentification, ServiceIdentification)) ServiceIdentification = valueServiceIdentification;
        var valueTimeOfDay = TimeOfDay;
        OptionalStructType.Accept(visitor, VisitorTimeOfDayField, ref valueTimeOfDay);
        if (!ReferenceEquals(valueTimeOfDay, TimeOfDay)) TimeOfDay = valueTimeOfDay;
        var valueCategoryVersionNumberReport = CategoryVersionNumberReport;
        OptionalStructType.Accept(visitor, VisitorCategoryVersionNumberReportField, ref valueCategoryVersionNumberReport);
        if (!ReferenceEquals(valueCategoryVersionNumberReport, CategoryVersionNumberReport)) CategoryVersionNumberReport = valueCategoryVersionNumberReport;
        var valueSpecialPurposeField = SpecialPurposeField;
        OptionalStructType.Accept(visitor, VisitorSpecialPurposeFieldField, ref valueSpecialPurposeField);
        if (!ReferenceEquals(valueSpecialPurposeField, SpecialPurposeField)) SpecialPurposeField = valueSpecialPurposeField;
        var valueReservedExpansionField = ReservedExpansionField;
        OptionalStructType.Accept(visitor, VisitorReservedExpansionFieldField, ref valueReservedExpansionField);
        if (!ReferenceEquals(valueReservedExpansionField, ReservedExpansionField)) ReservedExpansionField = valueReservedExpansionField;
    }
}
