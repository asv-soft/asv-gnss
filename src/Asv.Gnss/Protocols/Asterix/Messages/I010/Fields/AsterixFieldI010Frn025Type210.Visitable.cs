using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn025Type210
{
    private static readonly Field VisitorAxField = new Field.Builder().Name(nameof(Ax)).Title(nameof(Ax)).Description("Ax").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorAyField = new Field.Builder().Name(nameof(Ay)).Title(nameof(Ay)).Description("Ay").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAxField, VisitorAyField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAx = Ax;
        DoubleType.Accept(visitor, VisitorAxField, ref valueAx);
        if (!EqualityComparer<double>.Default.Equals(valueAx, Ax)) Ax = valueAx;
        var valueAy = Ay;
        DoubleType.Accept(visitor, VisitorAyField, ref valueAy);
        if (!EqualityComparer<double>.Default.Equals(valueAy, Ay)) Ay = valueAy;
    }
}
