using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI004Frn013Type074
{
    private static readonly Field VisitorLongitudinalDeviationMField = new Field.Builder().Name(nameof(LongitudinalDeviationM)).Title(nameof(LongitudinalDeviationM)).Description("LongitudinalDeviationM").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorLongitudinalDeviationMField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueLongitudinalDeviationM = LongitudinalDeviationM;
        DoubleType.Accept(visitor, VisitorLongitudinalDeviationMField, ref valueLongitudinalDeviationM);
        if (!EqualityComparer<double>.Default.Equals(valueLongitudinalDeviationM, LongitudinalDeviationM)) LongitudinalDeviationM = valueLongitudinalDeviationM;
    }
}
