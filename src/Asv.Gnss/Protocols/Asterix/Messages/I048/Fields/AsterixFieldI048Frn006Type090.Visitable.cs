using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn006Type090
{
    private static readonly Field VisitorVField = new Field.Builder().Name(nameof(V)).Title(nameof(V)).Description("V").DataType(BoolType.Default).Build();
    private static readonly Field VisitorGField = new Field.Builder().Name(nameof(G)).Title(nameof(G)).Description("G").DataType(BoolType.Default).Build();
    private static readonly Field VisitorFlightLevelField = new Field.Builder().Name(nameof(FlightLevel)).Title(nameof(FlightLevel)).Description("FlightLevel").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorVField, VisitorGField, VisitorFlightLevelField]);

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
        var valueFlightLevel = FlightLevel;
        DoubleType.Accept(visitor, VisitorFlightLevelField, ref valueFlightLevel);
        if (!EqualityComparer<double>.Default.Equals(valueFlightLevel, FlightLevel)) FlightLevel = valueFlightLevel;
    }
}
