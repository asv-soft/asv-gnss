using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixRecordI004
{
    private static readonly Field VisitorDataSourceIdentifierField = new Field.Builder().Name(nameof(DataSourceIdentifier)).Title(nameof(DataSourceIdentifier)).Description("DataSourceIdentifier").DataType(new OptionalStructType(AsterixFieldI004Frn001Type010.AsterixVisitorType)).Build();
    private static readonly Field VisitorMessageTypeField = new Field.Builder().Name(nameof(MessageType)).Title(nameof(MessageType)).Description("MessageType").DataType(new OptionalStructType(AsterixFieldI004Frn002Type000.AsterixVisitorType)).Build();
    private static readonly Field VisitorSdpsIdentifierField = new Field.Builder().Name(nameof(SdpsIdentifier)).Title(nameof(SdpsIdentifier)).Description("SdpsIdentifier").DataType(new OptionalStructType(AsterixFieldI004Frn003Type015.AsterixVisitorType)).Build();
    private static readonly Field VisitorTimeOfMessageField = new Field.Builder().Name(nameof(TimeOfMessage)).Title(nameof(TimeOfMessage)).Description("TimeOfMessage").DataType(new OptionalStructType(AsterixFieldI004Frn004Type020.AsterixVisitorType)).Build();
    private static readonly Field VisitorAlertIdentifierField = new Field.Builder().Name(nameof(AlertIdentifier)).Title(nameof(AlertIdentifier)).Description("AlertIdentifier").DataType(new OptionalStructType(AsterixFieldI004Frn005Type040.AsterixVisitorType)).Build();
    private static readonly Field VisitorAlertStatusField = new Field.Builder().Name(nameof(AlertStatus)).Title(nameof(AlertStatus)).Description("AlertStatus").DataType(new OptionalStructType(AsterixFieldI004Frn006Type045.AsterixVisitorType)).Build();
    private static readonly Field VisitorSafetyNetFunctionStatusField = new Field.Builder().Name(nameof(SafetyNetFunctionStatus)).Title(nameof(SafetyNetFunctionStatus)).Description("SafetyNetFunctionStatus").DataType(new OptionalStructType(AsterixFieldI004Frn007Type060.AsterixVisitorType)).Build();
    private static readonly Field VisitorTrackNumber1Field = new Field.Builder().Name(nameof(TrackNumber1)).Title(nameof(TrackNumber1)).Description("TrackNumber1").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorAircraftIdentification1Field = new Field.Builder().Name(nameof(AircraftIdentification1)).Title(nameof(AircraftIdentification1)).Description("AircraftIdentification1").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorConflictCharacteristicsField = new Field.Builder().Name(nameof(ConflictCharacteristics)).Title(nameof(ConflictCharacteristics)).Description("ConflictCharacteristics").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorConflictTimingAndSeparationField = new Field.Builder().Name(nameof(ConflictTimingAndSeparation)).Title(nameof(ConflictTimingAndSeparation)).Description("ConflictTimingAndSeparation").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorVerticalDeviationField = new Field.Builder().Name(nameof(VerticalDeviation)).Title(nameof(VerticalDeviation)).Description("VerticalDeviation").DataType(new OptionalStructType(AsterixFieldI004Frn012Type076.AsterixVisitorType)).Build();
    private static readonly Field VisitorLongitudinalDeviationField = new Field.Builder().Name(nameof(LongitudinalDeviation)).Title(nameof(LongitudinalDeviation)).Description("LongitudinalDeviation").DataType(new OptionalStructType(AsterixFieldI004Frn013Type074.AsterixVisitorType)).Build();
    private static readonly Field VisitorTransversalDistanceDeviationField = new Field.Builder().Name(nameof(TransversalDistanceDeviation)).Title(nameof(TransversalDistanceDeviation)).Description("TransversalDistanceDeviation").DataType(new OptionalStructType(AsterixFieldI004Frn014Type075.AsterixVisitorType)).Build();
    private static readonly Field VisitorAreaDefinitionField = new Field.Builder().Name(nameof(AreaDefinition)).Title(nameof(AreaDefinition)).Description("AreaDefinition").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorTrackNumber2Field = new Field.Builder().Name(nameof(TrackNumber2)).Title(nameof(TrackNumber2)).Description("TrackNumber2").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorAircraftIdentification2Field = new Field.Builder().Name(nameof(AircraftIdentification2)).Title(nameof(AircraftIdentification2)).Description("AircraftIdentification2").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorFdpsSectorControlIdentificationField = new Field.Builder().Name(nameof(FdpsSectorControlIdentification)).Title(nameof(FdpsSectorControlIdentification)).Description("FdpsSectorControlIdentification").DataType(new OptionalStructType(AsterixFieldI004Frn018Type110.AsterixVisitorType)).Build();
    private static readonly Field VisitorReservedExpansionFieldField = new Field.Builder().Name(nameof(ReservedExpansionField)).Title(nameof(ReservedExpansionField)).Description("ReservedExpansionField").DataType(new OptionalStructType(new StructType([]))).Build();
    private static readonly Field VisitorSpecialPurposeFieldField = new Field.Builder().Name(nameof(SpecialPurposeField)).Title(nameof(SpecialPurposeField)).Description("SpecialPurposeField").DataType(new OptionalStructType(new StructType([]))).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorDataSourceIdentifierField, VisitorMessageTypeField, VisitorSdpsIdentifierField, VisitorTimeOfMessageField, VisitorAlertIdentifierField, VisitorAlertStatusField, VisitorSafetyNetFunctionStatusField, VisitorTrackNumber1Field, VisitorAircraftIdentification1Field, VisitorConflictCharacteristicsField, VisitorConflictTimingAndSeparationField, VisitorVerticalDeviationField, VisitorLongitudinalDeviationField, VisitorTransversalDistanceDeviationField, VisitorAreaDefinitionField, VisitorTrackNumber2Field, VisitorAircraftIdentification2Field, VisitorFdpsSectorControlIdentificationField, VisitorReservedExpansionFieldField, VisitorSpecialPurposeFieldField]);

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
        var valueSdpsIdentifier = SdpsIdentifier;
        OptionalStructType.Accept(visitor, VisitorSdpsIdentifierField, ref valueSdpsIdentifier);
        if (!ReferenceEquals(valueSdpsIdentifier, SdpsIdentifier)) SdpsIdentifier = valueSdpsIdentifier;
        var valueTimeOfMessage = TimeOfMessage;
        OptionalStructType.Accept(visitor, VisitorTimeOfMessageField, ref valueTimeOfMessage);
        if (!ReferenceEquals(valueTimeOfMessage, TimeOfMessage)) TimeOfMessage = valueTimeOfMessage;
        var valueAlertIdentifier = AlertIdentifier;
        OptionalStructType.Accept(visitor, VisitorAlertIdentifierField, ref valueAlertIdentifier);
        if (!ReferenceEquals(valueAlertIdentifier, AlertIdentifier)) AlertIdentifier = valueAlertIdentifier;
        var valueAlertStatus = AlertStatus;
        OptionalStructType.Accept(visitor, VisitorAlertStatusField, ref valueAlertStatus);
        if (!ReferenceEquals(valueAlertStatus, AlertStatus)) AlertStatus = valueAlertStatus;
        var valueSafetyNetFunctionStatus = SafetyNetFunctionStatus;
        OptionalStructType.Accept(visitor, VisitorSafetyNetFunctionStatusField, ref valueSafetyNetFunctionStatus);
        if (!ReferenceEquals(valueSafetyNetFunctionStatus, SafetyNetFunctionStatus)) SafetyNetFunctionStatus = valueSafetyNetFunctionStatus;
        var valueTrackNumber1 = TrackNumber1;
        OptionalStructType.Accept(visitor, VisitorTrackNumber1Field, ref valueTrackNumber1);
        if (!ReferenceEquals(valueTrackNumber1, TrackNumber1)) TrackNumber1 = valueTrackNumber1;
        var valueAircraftIdentification1 = AircraftIdentification1;
        OptionalStructType.Accept(visitor, VisitorAircraftIdentification1Field, ref valueAircraftIdentification1);
        if (!ReferenceEquals(valueAircraftIdentification1, AircraftIdentification1)) AircraftIdentification1 = valueAircraftIdentification1;
        var valueConflictCharacteristics = ConflictCharacteristics;
        OptionalStructType.Accept(visitor, VisitorConflictCharacteristicsField, ref valueConflictCharacteristics);
        if (!ReferenceEquals(valueConflictCharacteristics, ConflictCharacteristics)) ConflictCharacteristics = valueConflictCharacteristics;
        var valueConflictTimingAndSeparation = ConflictTimingAndSeparation;
        OptionalStructType.Accept(visitor, VisitorConflictTimingAndSeparationField, ref valueConflictTimingAndSeparation);
        if (!ReferenceEquals(valueConflictTimingAndSeparation, ConflictTimingAndSeparation)) ConflictTimingAndSeparation = valueConflictTimingAndSeparation;
        var valueVerticalDeviation = VerticalDeviation;
        OptionalStructType.Accept(visitor, VisitorVerticalDeviationField, ref valueVerticalDeviation);
        if (!ReferenceEquals(valueVerticalDeviation, VerticalDeviation)) VerticalDeviation = valueVerticalDeviation;
        var valueLongitudinalDeviation = LongitudinalDeviation;
        OptionalStructType.Accept(visitor, VisitorLongitudinalDeviationField, ref valueLongitudinalDeviation);
        if (!ReferenceEquals(valueLongitudinalDeviation, LongitudinalDeviation)) LongitudinalDeviation = valueLongitudinalDeviation;
        var valueTransversalDistanceDeviation = TransversalDistanceDeviation;
        OptionalStructType.Accept(visitor, VisitorTransversalDistanceDeviationField, ref valueTransversalDistanceDeviation);
        if (!ReferenceEquals(valueTransversalDistanceDeviation, TransversalDistanceDeviation)) TransversalDistanceDeviation = valueTransversalDistanceDeviation;
        var valueAreaDefinition = AreaDefinition;
        OptionalStructType.Accept(visitor, VisitorAreaDefinitionField, ref valueAreaDefinition);
        if (!ReferenceEquals(valueAreaDefinition, AreaDefinition)) AreaDefinition = valueAreaDefinition;
        var valueTrackNumber2 = TrackNumber2;
        OptionalStructType.Accept(visitor, VisitorTrackNumber2Field, ref valueTrackNumber2);
        if (!ReferenceEquals(valueTrackNumber2, TrackNumber2)) TrackNumber2 = valueTrackNumber2;
        var valueAircraftIdentification2 = AircraftIdentification2;
        OptionalStructType.Accept(visitor, VisitorAircraftIdentification2Field, ref valueAircraftIdentification2);
        if (!ReferenceEquals(valueAircraftIdentification2, AircraftIdentification2)) AircraftIdentification2 = valueAircraftIdentification2;
        var valueFdpsSectorControlIdentification = FdpsSectorControlIdentification;
        OptionalStructType.Accept(visitor, VisitorFdpsSectorControlIdentificationField, ref valueFdpsSectorControlIdentification);
        if (!ReferenceEquals(valueFdpsSectorControlIdentification, FdpsSectorControlIdentification)) FdpsSectorControlIdentification = valueFdpsSectorControlIdentification;
        var valueReservedExpansionField = ReservedExpansionField;
        OptionalStructType.Accept(visitor, VisitorReservedExpansionFieldField, ref valueReservedExpansionField);
        if (!ReferenceEquals(valueReservedExpansionField, ReservedExpansionField)) ReservedExpansionField = valueReservedExpansionField;
        var valueSpecialPurposeField = SpecialPurposeField;
        OptionalStructType.Accept(visitor, VisitorSpecialPurposeFieldField, ref valueSpecialPurposeField);
        if (!ReferenceEquals(valueSpecialPurposeField, SpecialPurposeField)) SpecialPurposeField = valueSpecialPurposeField;
    }
}
