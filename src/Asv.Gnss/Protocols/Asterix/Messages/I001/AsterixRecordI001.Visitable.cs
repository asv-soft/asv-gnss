using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixRecordI001
{
    private static readonly Field VisitorDataSourceIdentifierField = new Field.Builder().Name(nameof(DataSourceIdentifier)).Title(nameof(DataSourceIdentifier)).Description("DataSourceIdentifier").DataType(new OptionalStructType(AsterixFieldI001Frn001Type010.AsterixVisitorType)).Build();
    private static readonly Field VisitorTargetReportDescriptorField = new Field.Builder().Name(nameof(TargetReportDescriptor)).Title(nameof(TargetReportDescriptor)).Description("TargetReportDescriptor").DataType(new OptionalStructType(AsterixFieldI001Frn002Type020.AsterixVisitorType)).Build();
    private static readonly Field VisitorMeasuredPositionPolarCoordinatesField = new Field.Builder().Name(nameof(MeasuredPositionPolarCoordinates)).Title(nameof(MeasuredPositionPolarCoordinates)).Description("MeasuredPositionPolarCoordinates").DataType(new OptionalStructType(AsterixFieldI001Frn003Type040.AsterixVisitorType)).Build();
    private static readonly Field VisitorMode3AReplyField = new Field.Builder().Name(nameof(Mode3AReply)).Title(nameof(Mode3AReply)).Description("Mode3AReply").DataType(new OptionalStructType(AsterixFieldI001Frn004Type070.AsterixVisitorType)).Build();
    private static readonly Field VisitorModeCCodeField = new Field.Builder().Name(nameof(ModeCCode)).Title(nameof(ModeCCode)).Description("ModeCCode").DataType(new OptionalStructType(AsterixFieldI001Frn005Type090.AsterixVisitorType)).Build();
    private static readonly Field VisitorRadarPlotCharacteristicsField = new Field.Builder().Name(nameof(RadarPlotCharacteristics)).Title(nameof(RadarPlotCharacteristics)).Description("RadarPlotCharacteristics").DataType(new OptionalStructType(AsterixFieldI001Frn006Type130.AsterixVisitorType)).Build();
    private static readonly Field VisitorTruncatedTimeOfDayField = new Field.Builder().Name(nameof(TruncatedTimeOfDay)).Title(nameof(TruncatedTimeOfDay)).Description("TruncatedTimeOfDay").DataType(new OptionalStructType(AsterixFieldI001Frn007Type141.AsterixVisitorType)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorDataSourceIdentifierField, VisitorTargetReportDescriptorField, VisitorMeasuredPositionPolarCoordinatesField, VisitorMode3AReplyField, VisitorModeCCodeField, VisitorRadarPlotCharacteristicsField, VisitorTruncatedTimeOfDayField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueDataSourceIdentifier = DataSourceIdentifier;
        OptionalStructType.Accept(visitor, VisitorDataSourceIdentifierField, ref valueDataSourceIdentifier);
        if (!ReferenceEquals(valueDataSourceIdentifier, DataSourceIdentifier)) DataSourceIdentifier = valueDataSourceIdentifier;
        var valueTargetReportDescriptor = TargetReportDescriptor;
        OptionalStructType.Accept(visitor, VisitorTargetReportDescriptorField, ref valueTargetReportDescriptor);
        if (!ReferenceEquals(valueTargetReportDescriptor, TargetReportDescriptor)) TargetReportDescriptor = valueTargetReportDescriptor;
        var valueMeasuredPositionPolarCoordinates = MeasuredPositionPolarCoordinates;
        OptionalStructType.Accept(visitor, VisitorMeasuredPositionPolarCoordinatesField, ref valueMeasuredPositionPolarCoordinates);
        if (!ReferenceEquals(valueMeasuredPositionPolarCoordinates, MeasuredPositionPolarCoordinates)) MeasuredPositionPolarCoordinates = valueMeasuredPositionPolarCoordinates;
        var valueMode3AReply = Mode3AReply;
        OptionalStructType.Accept(visitor, VisitorMode3AReplyField, ref valueMode3AReply);
        if (!ReferenceEquals(valueMode3AReply, Mode3AReply)) Mode3AReply = valueMode3AReply;
        var valueModeCCode = ModeCCode;
        OptionalStructType.Accept(visitor, VisitorModeCCodeField, ref valueModeCCode);
        if (!ReferenceEquals(valueModeCCode, ModeCCode)) ModeCCode = valueModeCCode;
        var valueRadarPlotCharacteristics = RadarPlotCharacteristics;
        OptionalStructType.Accept(visitor, VisitorRadarPlotCharacteristicsField, ref valueRadarPlotCharacteristics);
        if (!ReferenceEquals(valueRadarPlotCharacteristics, RadarPlotCharacteristics)) RadarPlotCharacteristics = valueRadarPlotCharacteristics;
        var valueTruncatedTimeOfDay = TruncatedTimeOfDay;
        OptionalStructType.Accept(visitor, VisitorTruncatedTimeOfDayField, ref valueTruncatedTimeOfDay);
        if (!ReferenceEquals(valueTruncatedTimeOfDay, TruncatedTimeOfDay)) TruncatedTimeOfDay = valueTruncatedTimeOfDay;
    }
}
