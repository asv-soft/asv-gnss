using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI001Frn005Type090
{
    private static readonly Field VisitorVField = new Field.Builder().Name(nameof(V)).Title(nameof(V)).Description("V").DataType(BoolType.Default).Build();
    private static readonly Field VisitorGField = new Field.Builder().Name(nameof(G)).Title(nameof(G)).Description("G").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHeightFtField = new Field.Builder().Name(nameof(HeightFt)).Title(nameof(HeightFt)).Description("HeightFt").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorVField, VisitorGField, VisitorHeightFtField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueV = V;
        BoolType.Accept(visitor, VisitorVField, ref valueV);
        if (!EqualityComparer<bool>.Default.Equals(valueV, V)) V = valueV;
        var valueG = G;
        BoolType.Accept(visitor, VisitorGField, ref valueG);
        if (!EqualityComparer<bool>.Default.Equals(valueG, G)) G = valueG;
        var valueHeightFt = HeightFt;
        DoubleType.Accept(visitor, VisitorHeightFtField, ref valueHeightFt);
        if (!EqualityComparer<double>.Default.Equals(valueHeightFt, HeightFt)) HeightFt = valueHeightFt;
    }
}
