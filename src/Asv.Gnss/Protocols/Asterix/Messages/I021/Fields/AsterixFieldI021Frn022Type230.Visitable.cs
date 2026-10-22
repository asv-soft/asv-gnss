using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn022Type230
{
    private static readonly Field VisitorRollAngleDegField = new Field.Builder().Name(nameof(RollAngleDeg)).Title(nameof(RollAngleDeg)).Description("RollAngleDeg").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRollAngleDegField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRollAngleDeg = RollAngleDeg;
        DoubleType.Accept(visitor, VisitorRollAngleDegField, ref valueRollAngleDeg);
        if (!EqualityComparer<double>.Default.Equals(valueRollAngleDeg, RollAngleDeg)) RollAngleDeg = valueRollAngleDeg;
    }
}
