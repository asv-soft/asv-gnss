using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn018Type140
{
    private static readonly Field VisitorGeometricHeightFtField = new Field.Builder().Name(nameof(GeometricHeightFt)).Title(nameof(GeometricHeightFt)).Description("GeometricHeightFt").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorGeometricHeightFtField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueGeometricHeightFt = GeometricHeightFt;
        DoubleType.Accept(visitor, VisitorGeometricHeightFtField, ref valueGeometricHeightFt);
        if (!EqualityComparer<double>.Default.Equals(valueGeometricHeightFt, GeometricHeightFt)) GeometricHeightFt = valueGeometricHeightFt;
    }
}
