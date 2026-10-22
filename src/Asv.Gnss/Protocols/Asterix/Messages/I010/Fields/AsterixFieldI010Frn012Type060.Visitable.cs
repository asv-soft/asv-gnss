using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn012Type060
{
    private static readonly Field VisitorVField = new Field.Builder().Name(nameof(V)).Title(nameof(V)).Description("V").DataType(BoolType.Default).Build();
    private static readonly Field VisitorGField = new Field.Builder().Name(nameof(G)).Title(nameof(G)).Description("G").DataType(BoolType.Default).Build();
    private static readonly Field VisitorLField = new Field.Builder().Name(nameof(L)).Title(nameof(L)).Description("L").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSquawkField = new Field.Builder().Name(nameof(Squawk)).Title(nameof(Squawk)).Description("Squawk").DataType(UInt16Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorVField, VisitorGField, VisitorLField, VisitorSquawkField]);

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
        var valueL = L;
        BoolType.Accept(visitor, VisitorLField, ref valueL);
        if (!EqualityComparer<bool>.Default.Equals(valueL, L)) L = valueL;
        var valueSquawk = Squawk;
        UInt16Type.Accept(visitor, VisitorSquawkField, ref valueSquawk);
        if (!EqualityComparer<ushort>.Default.Equals(valueSquawk, Squawk)) Squawk = valueSquawk;
    }
}
