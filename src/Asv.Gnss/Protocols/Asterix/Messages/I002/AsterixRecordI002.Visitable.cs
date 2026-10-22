using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixRecordI002
{
    private static readonly Field VisitorDataSourceIdentifierField = new Field.Builder().Name(nameof(DataSourceIdentifier)).Title(nameof(DataSourceIdentifier)).Description("DataSourceIdentifier").DataType(new OptionalStructType(AsterixFieldI002Frn001Type010.AsterixVisitorType)).Build();
    private static readonly Field VisitorMessageTypeField = new Field.Builder().Name(nameof(MessageType)).Title(nameof(MessageType)).Description("MessageType").DataType(new OptionalStructType(AsterixFieldI002Frn002Type000.AsterixVisitorType)).Build();
    private static readonly Field VisitorSectorNumberField = new Field.Builder().Name(nameof(SectorNumber)).Title(nameof(SectorNumber)).Description("SectorNumber").DataType(new OptionalStructType(AsterixFieldI002Frn003Type020.AsterixVisitorType)).Build();
    private static readonly Field VisitorTimeOfDayField = new Field.Builder().Name(nameof(TimeOfDay)).Title(nameof(TimeOfDay)).Description("TimeOfDay").DataType(new OptionalStructType(AsterixFieldI002Frn004Type030.AsterixVisitorType)).Build();
    private static readonly Field VisitorAntennaRotationSpeedField = new Field.Builder().Name(nameof(AntennaRotationSpeed)).Title(nameof(AntennaRotationSpeed)).Description("AntennaRotationSpeed").DataType(new OptionalStructType(AsterixFieldI002Frn005Type041.AsterixVisitorType)).Build();
    private static readonly Field VisitorStationConfigurationStatusField = new Field.Builder().Name(nameof(StationConfigurationStatus)).Title(nameof(StationConfigurationStatus)).Description("StationConfigurationStatus").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorStationProcessingModeField = new Field.Builder().Name(nameof(StationProcessingMode)).Title(nameof(StationProcessingMode)).Description("StationProcessingMode").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorPlotCountValuesField = new Field.Builder().Name(nameof(PlotCountValues)).Title(nameof(PlotCountValues)).Description("PlotCountValues").DataType(new OptionalStructType(AsterixFieldI002Frn008Type070.AsterixVisitorType)).Build();
    private static readonly Field VisitorDynamicWindowType1Field = new Field.Builder().Name(nameof(DynamicWindowType1)).Title(nameof(DynamicWindowType1)).Description("DynamicWindowType1").DataType(new OptionalStructType(AsterixFieldI002Frn009Type100.AsterixVisitorType)).Build();
    private static readonly Field VisitorCollimationErrorField = new Field.Builder().Name(nameof(CollimationError)).Title(nameof(CollimationError)).Description("CollimationError").DataType(new OptionalStructType(AsterixFieldI002Frn010Type090.AsterixVisitorType)).Build();
    private static readonly Field VisitorWarningErrorConditionsField = new Field.Builder().Name(nameof(WarningErrorConditions)).Title(nameof(WarningErrorConditions)).Description("WarningErrorConditions").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorSpecialPurposeFieldField = new Field.Builder().Name(nameof(SpecialPurposeField)).Title(nameof(SpecialPurposeField)).Description("SpecialPurposeField").DataType(new OptionalStructType(AsterixFieldI002Frn013TypeSp.AsterixVisitorType)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorDataSourceIdentifierField, VisitorMessageTypeField, VisitorSectorNumberField, VisitorTimeOfDayField, VisitorAntennaRotationSpeedField, VisitorStationConfigurationStatusField, VisitorStationProcessingModeField, VisitorPlotCountValuesField, VisitorDynamicWindowType1Field, VisitorCollimationErrorField, VisitorWarningErrorConditionsField, VisitorSpecialPurposeFieldField]);

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
        var valueSectorNumber = SectorNumber;
        OptionalStructType.Accept(visitor, VisitorSectorNumberField, ref valueSectorNumber);
        if (!ReferenceEquals(valueSectorNumber, SectorNumber)) SectorNumber = valueSectorNumber;
        var valueTimeOfDay = TimeOfDay;
        OptionalStructType.Accept(visitor, VisitorTimeOfDayField, ref valueTimeOfDay);
        if (!ReferenceEquals(valueTimeOfDay, TimeOfDay)) TimeOfDay = valueTimeOfDay;
        var valueAntennaRotationSpeed = AntennaRotationSpeed;
        OptionalStructType.Accept(visitor, VisitorAntennaRotationSpeedField, ref valueAntennaRotationSpeed);
        if (!ReferenceEquals(valueAntennaRotationSpeed, AntennaRotationSpeed)) AntennaRotationSpeed = valueAntennaRotationSpeed;
        var valueStationConfigurationStatus = StationConfigurationStatus;
        OptionalStructType.Accept(visitor, VisitorStationConfigurationStatusField, ref valueStationConfigurationStatus);
        if (!ReferenceEquals(valueStationConfigurationStatus, StationConfigurationStatus)) StationConfigurationStatus = valueStationConfigurationStatus;
        var valueStationProcessingMode = StationProcessingMode;
        OptionalStructType.Accept(visitor, VisitorStationProcessingModeField, ref valueStationProcessingMode);
        if (!ReferenceEquals(valueStationProcessingMode, StationProcessingMode)) StationProcessingMode = valueStationProcessingMode;
        var valuePlotCountValues = PlotCountValues;
        OptionalStructType.Accept(visitor, VisitorPlotCountValuesField, ref valuePlotCountValues);
        if (!ReferenceEquals(valuePlotCountValues, PlotCountValues)) PlotCountValues = valuePlotCountValues;
        var valueDynamicWindowType1 = DynamicWindowType1;
        OptionalStructType.Accept(visitor, VisitorDynamicWindowType1Field, ref valueDynamicWindowType1);
        if (!ReferenceEquals(valueDynamicWindowType1, DynamicWindowType1)) DynamicWindowType1 = valueDynamicWindowType1;
        var valueCollimationError = CollimationError;
        OptionalStructType.Accept(visitor, VisitorCollimationErrorField, ref valueCollimationError);
        if (!ReferenceEquals(valueCollimationError, CollimationError)) CollimationError = valueCollimationError;
        var valueWarningErrorConditions = WarningErrorConditions;
        OptionalStructType.Accept(visitor, VisitorWarningErrorConditionsField, ref valueWarningErrorConditions);
        if (!ReferenceEquals(valueWarningErrorConditions, WarningErrorConditions)) WarningErrorConditions = valueWarningErrorConditions;
        var valueSpecialPurposeField = SpecialPurposeField;
        OptionalStructType.Accept(visitor, VisitorSpecialPurposeFieldField, ref valueSpecialPurposeField);
        if (!ReferenceEquals(valueSpecialPurposeField, SpecialPurposeField)) SpecialPurposeField = valueSpecialPurposeField;
    }
}
