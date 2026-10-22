using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn007Type042
{
    private static readonly Field VisitorXField = new Field.Builder().Name(nameof(X)).Title(nameof(X)).Description("X").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorYField = new Field.Builder().Name(nameof(Y)).Title(nameof(Y)).Description("Y").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorXField, VisitorYField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueX = X;
        DoubleType.Accept(visitor, VisitorXField, ref valueX);
        if (!EqualityComparer<double>.Default.Equals(valueX, X)) X = valueX;
        var valueY = Y;
        DoubleType.Accept(visitor, VisitorYField, ref valueY);
        if (!EqualityComparer<double>.Default.Equals(valueY, Y)) Y = valueY;
    }
}
