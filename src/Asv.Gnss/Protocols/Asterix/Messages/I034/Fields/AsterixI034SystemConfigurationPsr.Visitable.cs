using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI034SystemConfigurationPsr : IVisitable
{
    private static readonly Field VisitorAntennaField = new Field.Builder().Name(nameof(Antenna)).Title(nameof(Antenna)).Description("Antenna").DataType(BoolType.Default).Build();
    private static readonly Field VisitorChannelABField = new Field.Builder().Name(nameof(ChannelAB)).Title(nameof(ChannelAB)).Description("ChannelAB").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorOverloadField = new Field.Builder().Name(nameof(Overload)).Title(nameof(Overload)).Description("Overload").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMonitoringSystemConnectedField = new Field.Builder().Name(nameof(MonitoringSystemConnected)).Title(nameof(MonitoringSystemConnected)).Description("MonitoringSystemConnected").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAntennaField, VisitorChannelABField, VisitorOverloadField, VisitorMonitoringSystemConnectedField]);

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
        var valueOverload = Overload;
        BoolType.Accept(visitor, VisitorOverloadField, ref valueOverload);
        if (!EqualityComparer<bool>.Default.Equals(valueOverload, Overload)) Overload = valueOverload;
        var valueMonitoringSystemConnected = MonitoringSystemConnected;
        BoolType.Accept(visitor, VisitorMonitoringSystemConnectedField, ref valueMonitoringSystemConnected);
        if (!EqualityComparer<bool>.Default.Equals(valueMonitoringSystemConnected, MonitoringSystemConnected)) MonitoringSystemConnected = valueMonitoringSystemConnected;
    }
}
