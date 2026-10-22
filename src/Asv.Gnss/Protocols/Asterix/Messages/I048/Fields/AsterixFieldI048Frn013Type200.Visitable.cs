using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn013Type200
{
    private static readonly Field VisitorGroundSpeedKtField = new Field.Builder().Name(nameof(GroundSpeedKt)).Title(nameof(GroundSpeedKt)).Description("GroundSpeedKt").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorHeadingDegField = new Field.Builder().Name(nameof(HeadingDeg)).Title(nameof(HeadingDeg)).Description("HeadingDeg").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorGroundSpeedKtField, VisitorHeadingDegField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueGroundSpeedKt = GroundSpeedKt;
        DoubleType.Accept(visitor, VisitorGroundSpeedKtField, ref valueGroundSpeedKt);
        if (!EqualityComparer<double>.Default.Equals(valueGroundSpeedKt, GroundSpeedKt)) GroundSpeedKt = valueGroundSpeedKt;
        var valueHeadingDeg = HeadingDeg;
        DoubleType.Accept(visitor, VisitorHeadingDegField, ref valueHeadingDeg);
        if (!EqualityComparer<double>.Default.Equals(valueHeadingDeg, HeadingDeg)) HeadingDeg = valueHeadingDeg;
    }
}
