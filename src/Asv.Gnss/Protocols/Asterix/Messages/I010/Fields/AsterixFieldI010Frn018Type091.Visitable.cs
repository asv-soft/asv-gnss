using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn018Type091
{
    private static readonly Field VisitorMeasuredHeightField = new Field.Builder().Name(nameof(MeasuredHeight)).Title(nameof(MeasuredHeight)).Description("MeasuredHeight").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorMeasuredHeightField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMeasuredHeight = MeasuredHeight;
        DoubleType.Accept(visitor, VisitorMeasuredHeightField, ref valueMeasuredHeight);
        if (!EqualityComparer<double>.Default.Equals(valueMeasuredHeight, MeasuredHeight)) MeasuredHeight = valueMeasuredHeight;
    }
}
