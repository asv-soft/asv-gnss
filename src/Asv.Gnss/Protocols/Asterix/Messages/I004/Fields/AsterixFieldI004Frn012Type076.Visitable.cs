using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI004Frn012Type076
{
    private static readonly Field VisitorVerticalDeviationFtField = new Field.Builder().Name(nameof(VerticalDeviationFt)).Title(nameof(VerticalDeviationFt)).Description("VerticalDeviationFt").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorVerticalDeviationFtField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueVerticalDeviationFt = VerticalDeviationFt;
        DoubleType.Accept(visitor, VisitorVerticalDeviationFtField, ref valueVerticalDeviationFt);
        if (!EqualityComparer<double>.Default.Equals(valueVerticalDeviationFt, VerticalDeviationFt)) VerticalDeviationFt = valueVerticalDeviationFt;
    }
}
