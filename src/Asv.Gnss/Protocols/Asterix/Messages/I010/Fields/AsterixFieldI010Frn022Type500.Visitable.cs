using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn022Type500
{
    private static readonly Field VisitorSdpXField = new Field.Builder().Name(nameof(SdpX)).Title(nameof(SdpX)).Description("SdpX").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorSdpYField = new Field.Builder().Name(nameof(SdpY)).Title(nameof(SdpY)).Description("SdpY").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorSdpXyField = new Field.Builder().Name(nameof(SdpXy)).Title(nameof(SdpXy)).Description("SdpXy").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorSdpXField, VisitorSdpYField, VisitorSdpXyField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSdpX = SdpX;
        DoubleType.Accept(visitor, VisitorSdpXField, ref valueSdpX);
        if (!EqualityComparer<double>.Default.Equals(valueSdpX, SdpX)) SdpX = valueSdpX;
        var valueSdpY = SdpY;
        DoubleType.Accept(visitor, VisitorSdpYField, ref valueSdpY);
        if (!EqualityComparer<double>.Default.Equals(valueSdpY, SdpY)) SdpY = valueSdpY;
        var valueSdpXy = SdpXy;
        DoubleType.Accept(visitor, VisitorSdpXyField, ref valueSdpXy);
        if (!EqualityComparer<double>.Default.Equals(valueSdpXy, SdpXy)) SdpXy = valueSdpXy;
    }
}
