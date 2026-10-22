using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI034SystemConfigurationModeS : IVisitable
{
    private static readonly Field VisitorAntennaField = new Field.Builder().Name(nameof(Antenna)).Title(nameof(Antenna)).Description("Antenna").DataType(BoolType.Default).Build();
    private static readonly Field VisitorChannelABField = new Field.Builder().Name(nameof(ChannelAB)).Title(nameof(ChannelAB)).Description("ChannelAB").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorOverloadSurveillanceField = new Field.Builder().Name(nameof(OverloadSurveillance)).Title(nameof(OverloadSurveillance)).Description("OverloadSurveillance").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMonitoringSystemConnectedField = new Field.Builder().Name(nameof(MonitoringSystemConnected)).Title(nameof(MonitoringSystemConnected)).Description("MonitoringSystemConnected").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSurveillanceCoordinationFunctionField = new Field.Builder().Name(nameof(SurveillanceCoordinationFunction)).Title(nameof(SurveillanceCoordinationFunction)).Description("SurveillanceCoordinationFunction").DataType(BoolType.Default).Build();
    private static readonly Field VisitorDataLinkFunctionField = new Field.Builder().Name(nameof(DataLinkFunction)).Title(nameof(DataLinkFunction)).Description("DataLinkFunction").DataType(BoolType.Default).Build();
    private static readonly Field VisitorOverloadSurveillanceCoordinationFunctionField = new Field.Builder().Name(nameof(OverloadSurveillanceCoordinationFunction)).Title(nameof(OverloadSurveillanceCoordinationFunction)).Description("OverloadSurveillanceCoordinationFunction").DataType(BoolType.Default).Build();
    private static readonly Field VisitorOverloadDataLinkFunctionField = new Field.Builder().Name(nameof(OverloadDataLinkFunction)).Title(nameof(OverloadDataLinkFunction)).Description("OverloadDataLinkFunction").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAntennaField, VisitorChannelABField, VisitorOverloadSurveillanceField, VisitorMonitoringSystemConnectedField, VisitorSurveillanceCoordinationFunctionField, VisitorDataLinkFunctionField, VisitorOverloadSurveillanceCoordinationFunctionField, VisitorOverloadDataLinkFunctionField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAntenna = Antenna;
        BoolType.Accept(visitor, VisitorAntennaField, ref valueAntenna);
        if (!EqualityComparer<bool>.Default.Equals(valueAntenna, Antenna)) Antenna = valueAntenna;
        var valueChannelAB = ChannelAB;
        UInt8Type.Accept(visitor, VisitorChannelABField, ref valueChannelAB);
        if (!EqualityComparer<byte>.Default.Equals(valueChannelAB, ChannelAB)) ChannelAB = valueChannelAB;
        var valueOverloadSurveillance = OverloadSurveillance;
        BoolType.Accept(visitor, VisitorOverloadSurveillanceField, ref valueOverloadSurveillance);
        if (!EqualityComparer<bool>.Default.Equals(valueOverloadSurveillance, OverloadSurveillance)) OverloadSurveillance = valueOverloadSurveillance;
        var valueMonitoringSystemConnected = MonitoringSystemConnected;
        BoolType.Accept(visitor, VisitorMonitoringSystemConnectedField, ref valueMonitoringSystemConnected);
        if (!EqualityComparer<bool>.Default.Equals(valueMonitoringSystemConnected, MonitoringSystemConnected)) MonitoringSystemConnected = valueMonitoringSystemConnected;
        var valueSurveillanceCoordinationFunction = SurveillanceCoordinationFunction;
        BoolType.Accept(visitor, VisitorSurveillanceCoordinationFunctionField, ref valueSurveillanceCoordinationFunction);
        if (!EqualityComparer<bool>.Default.Equals(valueSurveillanceCoordinationFunction, SurveillanceCoordinationFunction)) SurveillanceCoordinationFunction = valueSurveillanceCoordinationFunction;
        var valueDataLinkFunction = DataLinkFunction;
        BoolType.Accept(visitor, VisitorDataLinkFunctionField, ref valueDataLinkFunction);
        if (!EqualityComparer<bool>.Default.Equals(valueDataLinkFunction, DataLinkFunction)) DataLinkFunction = valueDataLinkFunction;
        var valueOverloadSurveillanceCoordinationFunction = OverloadSurveillanceCoordinationFunction;
        BoolType.Accept(visitor, VisitorOverloadSurveillanceCoordinationFunctionField, ref valueOverloadSurveillanceCoordinationFunction);
        if (!EqualityComparer<bool>.Default.Equals(valueOverloadSurveillanceCoordinationFunction, OverloadSurveillanceCoordinationFunction)) OverloadSurveillanceCoordinationFunction = valueOverloadSurveillanceCoordinationFunction;
        var valueOverloadDataLinkFunction = OverloadDataLinkFunction;
        BoolType.Accept(visitor, VisitorOverloadDataLinkFunctionField, ref valueOverloadDataLinkFunction);
        if (!EqualityComparer<bool>.Default.Equals(valueOverloadDataLinkFunction, OverloadDataLinkFunction)) OverloadDataLinkFunction = valueOverloadDataLinkFunction;
    }
}
