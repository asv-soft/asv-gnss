using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI004Frn006Type045
{
    private static readonly Field VisitorAlertStatusField = new Field.Builder().Name(nameof(AlertStatus)).Title(nameof(AlertStatus)).Description("AlertStatus").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAlertStatusField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAlertStatus = AlertStatus;
        UInt8Type.Accept(visitor, VisitorAlertStatusField, ref valueAlertStatus);
        if (!EqualityComparer<byte>.Default.Equals(valueAlertStatus, AlertStatus)) AlertStatus = valueAlertStatus;
    }
}
