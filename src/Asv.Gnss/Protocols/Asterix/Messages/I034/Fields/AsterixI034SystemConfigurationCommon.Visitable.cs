using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI034SystemConfigurationCommon : IVisitable
{
    private static readonly Field VisitorNogoField = new Field.Builder().Name(nameof(Nogo)).Title(nameof(Nogo)).Description("Nogo").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRdpcField = new Field.Builder().Name(nameof(Rdpc)).Title(nameof(Rdpc)).Description("Rdpc").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRdprField = new Field.Builder().Name(nameof(Rdpr)).Title(nameof(Rdpr)).Description("Rdpr").DataType(BoolType.Default).Build();
    private static readonly Field VisitorOverloadRdpField = new Field.Builder().Name(nameof(OverloadRdp)).Title(nameof(OverloadRdp)).Description("OverloadRdp").DataType(BoolType.Default).Build();
    private static readonly Field VisitorOverloadTransmissionField = new Field.Builder().Name(nameof(OverloadTransmission)).Title(nameof(OverloadTransmission)).Description("OverloadTransmission").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMonitoringSystemConnectedField = new Field.Builder().Name(nameof(MonitoringSystemConnected)).Title(nameof(MonitoringSystemConnected)).Description("MonitoringSystemConnected").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTimeSourceValidField = new Field.Builder().Name(nameof(TimeSourceValid)).Title(nameof(TimeSourceValid)).Description("TimeSourceValid").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorNogoField, VisitorRdpcField, VisitorRdprField, VisitorOverloadRdpField, VisitorOverloadTransmissionField, VisitorMonitoringSystemConnectedField, VisitorTimeSourceValidField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueNogo = Nogo;
        BoolType.Accept(visitor, VisitorNogoField, ref valueNogo);
        if (!EqualityComparer<bool>.Default.Equals(valueNogo, Nogo)) Nogo = valueNogo;
        var valueRdpc = Rdpc;
        BoolType.Accept(visitor, VisitorRdpcField, ref valueRdpc);
        if (!EqualityComparer<bool>.Default.Equals(valueRdpc, Rdpc)) Rdpc = valueRdpc;
        var valueRdpr = Rdpr;
        BoolType.Accept(visitor, VisitorRdprField, ref valueRdpr);
        if (!EqualityComparer<bool>.Default.Equals(valueRdpr, Rdpr)) Rdpr = valueRdpr;
        var valueOverloadRdp = OverloadRdp;
        BoolType.Accept(visitor, VisitorOverloadRdpField, ref valueOverloadRdp);
        if (!EqualityComparer<bool>.Default.Equals(valueOverloadRdp, OverloadRdp)) OverloadRdp = valueOverloadRdp;
        var valueOverloadTransmission = OverloadTransmission;
        BoolType.Accept(visitor, VisitorOverloadTransmissionField, ref valueOverloadTransmission);
        if (!EqualityComparer<bool>.Default.Equals(valueOverloadTransmission, OverloadTransmission)) OverloadTransmission = valueOverloadTransmission;
        var valueMonitoringSystemConnected = MonitoringSystemConnected;
        BoolType.Accept(visitor, VisitorMonitoringSystemConnectedField, ref valueMonitoringSystemConnected);
        if (!EqualityComparer<bool>.Default.Equals(valueMonitoringSystemConnected, MonitoringSystemConnected)) MonitoringSystemConnected = valueMonitoringSystemConnected;
        var valueTimeSourceValid = TimeSourceValid;
        BoolType.Accept(visitor, VisitorTimeSourceValidField, ref valueTimeSourceValid);
        if (!EqualityComparer<bool>.Default.Equals(valueTimeSourceValid, TimeSourceValid)) TimeSourceValid = valueTimeSourceValid;
    }
}
