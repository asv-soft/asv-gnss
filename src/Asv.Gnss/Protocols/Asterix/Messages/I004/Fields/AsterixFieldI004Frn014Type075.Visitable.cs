using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI004Frn014Type075
{
    private static readonly Field VisitorTransversalDistanceDeviationMField = new Field.Builder().Name(nameof(TransversalDistanceDeviationM)).Title(nameof(TransversalDistanceDeviationM)).Description("TransversalDistanceDeviationM").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTransversalDistanceDeviationMField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTransversalDistanceDeviationM = TransversalDistanceDeviationM;
        DoubleType.Accept(visitor, VisitorTransversalDistanceDeviationMField, ref valueTransversalDistanceDeviationM);
        if (!EqualityComparer<double>.Default.Equals(valueTransversalDistanceDeviationM, TransversalDistanceDeviationM)) TransversalDistanceDeviationM = valueTransversalDistanceDeviationM;
    }
}
