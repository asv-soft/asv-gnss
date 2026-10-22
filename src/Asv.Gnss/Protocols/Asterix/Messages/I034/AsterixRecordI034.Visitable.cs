using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixRecordI034
{
    private static readonly Field VisitorDataSourceIdentifierField = new Field.Builder().Name(nameof(DataSourceIdentifier)).Title(nameof(DataSourceIdentifier)).Description("DataSourceIdentifier").DataType(new OptionalStructType(AsterixFieldI034Frn001Type010.AsterixVisitorType)).Build();
    private static readonly Field VisitorMessageTypeField = new Field.Builder().Name(nameof(MessageType)).Title(nameof(MessageType)).Description("MessageType").DataType(new OptionalStructType(AsterixFieldI034Frn002Type000.AsterixVisitorType)).Build();
    private static readonly Field VisitorTimeOfDayField = new Field.Builder().Name(nameof(TimeOfDay)).Title(nameof(TimeOfDay)).Description("TimeOfDay").DataType(new OptionalStructType(AsterixFieldI034Frn003Type030.AsterixVisitorType)).Build();
    private static readonly Field VisitorSectorNumberField = new Field.Builder().Name(nameof(SectorNumber)).Title(nameof(SectorNumber)).Description("SectorNumber").DataType(new OptionalStructType(AsterixFieldI034Frn004Type020.AsterixVisitorType)).Build();
    private static readonly Field VisitorAntennaRotationSpeedField = new Field.Builder().Name(nameof(AntennaRotationSpeed)).Title(nameof(AntennaRotationSpeed)).Description("AntennaRotationSpeed").DataType(new OptionalStructType(AsterixFieldI034Frn005Type041.AsterixVisitorType)).Build();
    private static readonly Field VisitorSystemConfigurationStatusField = new Field.Builder().Name(nameof(SystemConfigurationStatus)).Title(nameof(SystemConfigurationStatus)).Description("SystemConfigurationStatus").DataType(new OptionalStructType(AsterixFieldI034Frn006Type050.AsterixVisitorType)).Build();
    private static readonly Field VisitorSystemProcessingModeField = new Field.Builder().Name(nameof(SystemProcessingMode)).Title(nameof(SystemProcessingMode)).Description("SystemProcessingMode").DataType(new OptionalStructType(AsterixFieldI034Frn007Type060.AsterixVisitorType)).Build();
    private static readonly Field VisitorMessageCountValuesField = new Field.Builder().Name(nameof(MessageCountValues)).Title(nameof(MessageCountValues)).Description("MessageCountValues").DataType(new OptionalStructType(AsterixFieldI034Frn008Type070.AsterixVisitorType)).Build();
    private static readonly Field VisitorGenericPolarWindowField = new Field.Builder().Name(nameof(GenericPolarWindow)).Title(nameof(GenericPolarWindow)).Description("GenericPolarWindow").DataType(new OptionalStructType(AsterixFieldI034Frn009Type100.AsterixVisitorType)).Build();
    private static readonly Field VisitorDataFilterField = new Field.Builder().Name(nameof(DataFilter)).Title(nameof(DataFilter)).Description("DataFilter").DataType(new OptionalStructType(AsterixFieldI034Frn010Type110.AsterixVisitorType)).Build();
    private static readonly Field VisitorDataSourcePositionField = new Field.Builder().Name(nameof(DataSourcePosition)).Title(nameof(DataSourcePosition)).Description("DataSourcePosition").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorCollimationErrorField = new Field.Builder().Name(nameof(CollimationError)).Title(nameof(CollimationError)).Description("CollimationError").DataType(new OptionalStructType(AsterixFieldI034Frn012Type090.AsterixVisitorType)).Build();
    private static readonly Field VisitorReservedExpansionFieldField = new Field.Builder().Name(nameof(ReservedExpansionField)).Title(nameof(ReservedExpansionField)).Description("ReservedExpansionField").DataType(new OptionalStructType(AsterixFieldI034Frn013TypeRe.AsterixVisitorType)).Build();
    private static readonly Field VisitorSpecialPurposeFieldField = new Field.Builder().Name(nameof(SpecialPurposeField)).Title(nameof(SpecialPurposeField)).Description("SpecialPurposeField").DataType(new OptionalStructType(AsterixFieldI034Frn014TypeSp.AsterixVisitorType)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorDataSourceIdentifierField, VisitorMessageTypeField, VisitorTimeOfDayField, VisitorSectorNumberField, VisitorAntennaRotationSpeedField, VisitorSystemConfigurationStatusField, VisitorSystemProcessingModeField, VisitorMessageCountValuesField, VisitorGenericPolarWindowField, VisitorDataFilterField, VisitorDataSourcePositionField, VisitorCollimationErrorField, VisitorReservedExpansionFieldField, VisitorSpecialPurposeFieldField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueDataSourceIdentifier = DataSourceIdentifier;
        OptionalStructType.Accept(visitor, VisitorDataSourceIdentifierField, ref valueDataSourceIdentifier);
        if (!ReferenceEquals(valueDataSourceIdentifier, DataSourceIdentifier)) DataSourceIdentifier = valueDataSourceIdentifier;
        var valueMessageType = MessageType;
        OptionalStructType.Accept(visitor, VisitorMessageTypeField, ref valueMessageType);
        if (!ReferenceEquals(valueMessageType, MessageType)) MessageType = valueMessageType;
        var valueTimeOfDay = TimeOfDay;
        OptionalStructType.Accept(visitor, VisitorTimeOfDayField, ref valueTimeOfDay);
        if (!ReferenceEquals(valueTimeOfDay, TimeOfDay)) TimeOfDay = valueTimeOfDay;
        var valueSectorNumber = SectorNumber;
        OptionalStructType.Accept(visitor, VisitorSectorNumberField, ref valueSectorNumber);
        if (!ReferenceEquals(valueSectorNumber, SectorNumber)) SectorNumber = valueSectorNumber;
        var valueAntennaRotationSpeed = AntennaRotationSpeed;
        OptionalStructType.Accept(visitor, VisitorAntennaRotationSpeedField, ref valueAntennaRotationSpeed);
        if (!ReferenceEquals(valueAntennaRotationSpeed, AntennaRotationSpeed)) AntennaRotationSpeed = valueAntennaRotationSpeed;
        var valueSystemConfigurationStatus = SystemConfigurationStatus;
        OptionalStructType.Accept(visitor, VisitorSystemConfigurationStatusField, ref valueSystemConfigurationStatus);
        if (!ReferenceEquals(valueSystemConfigurationStatus, SystemConfigurationStatus)) SystemConfigurationStatus = valueSystemConfigurationStatus;
        var valueSystemProcessingMode = SystemProcessingMode;
        OptionalStructType.Accept(visitor, VisitorSystemProcessingModeField, ref valueSystemProcessingMode);
        if (!ReferenceEquals(valueSystemProcessingMode, SystemProcessingMode)) SystemProcessingMode = valueSystemProcessingMode;
        var valueMessageCountValues = MessageCountValues;
        OptionalStructType.Accept(visitor, VisitorMessageCountValuesField, ref valueMessageCountValues);
        if (!ReferenceEquals(valueMessageCountValues, MessageCountValues)) MessageCountValues = valueMessageCountValues;
        var valueGenericPolarWindow = GenericPolarWindow;
        OptionalStructType.Accept(visitor, VisitorGenericPolarWindowField, ref valueGenericPolarWindow);
        if (!ReferenceEquals(valueGenericPolarWindow, GenericPolarWindow)) GenericPolarWindow = valueGenericPolarWindow;
        var valueDataFilter = DataFilter;
        OptionalStructType.Accept(visitor, VisitorDataFilterField, ref valueDataFilter);
        if (!ReferenceEquals(valueDataFilter, DataFilter)) DataFilter = valueDataFilter;
        var valueDataSourcePosition = DataSourcePosition;
        OptionalStructType.Accept(visitor, VisitorDataSourcePositionField, ref valueDataSourcePosition);
        if (!ReferenceEquals(valueDataSourcePosition, DataSourcePosition)) DataSourcePosition = valueDataSourcePosition;
        var valueCollimationError = CollimationError;
        OptionalStructType.Accept(visitor, VisitorCollimationErrorField, ref valueCollimationError);
        if (!ReferenceEquals(valueCollimationError, CollimationError)) CollimationError = valueCollimationError;
        var valueReservedExpansionField = ReservedExpansionField;
        OptionalStructType.Accept(visitor, VisitorReservedExpansionFieldField, ref valueReservedExpansionField);
        if (!ReferenceEquals(valueReservedExpansionField, ReservedExpansionField)) ReservedExpansionField = valueReservedExpansionField;
        var valueSpecialPurposeField = SpecialPurposeField;
        OptionalStructType.Accept(visitor, VisitorSpecialPurposeFieldField, ref valueSpecialPurposeField);
        if (!ReferenceEquals(valueSpecialPurposeField, SpecialPurposeField)) SpecialPurposeField = valueSpecialPurposeField;
    }
}
