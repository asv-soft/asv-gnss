using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn023Type145
{
    private static readonly Field VisitorFlightLevelField = new Field.Builder().Name(nameof(FlightLevel)).Title(nameof(FlightLevel)).Description("FlightLevel").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorFlightLevelField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueFlightLevel = FlightLevel;
        DoubleType.Accept(visitor, VisitorFlightLevelField, ref valueFlightLevel);
        if (!EqualityComparer<double>.Default.Equals(valueFlightLevel, FlightLevel)) FlightLevel = valueFlightLevel;
    }
}
