using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class Observation : IVisitable
{
    private static readonly Field PayloadPrnField = SbfComNavVisitor.Field(nameof(Prn), Int32Type.Default, "Prn");
    private static readonly Field PayloadPseudoRangeField = SbfComNavVisitor.Field(nameof(PseudoRange), DoubleType.Default, "PseudoRange");

    internal static readonly StructType PayloadVisitorType = new([PayloadPrnField, PayloadPseudoRangeField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valuePrn = Prn;
        Int32Type.Accept(visitor, PayloadPrnField, PayloadPrnField.DataType, ref valuePrn);
        if (!EqualityComparer<int>.Default.Equals(valuePrn, Prn))
        {
            Prn = valuePrn;
        }

        var valuePseudoRange = PseudoRange;
        DoubleType.Accept(visitor, PayloadPseudoRangeField, PayloadPseudoRangeField.DataType, ref valuePseudoRange);
        if (!EqualityComparer<double>.Default.Equals(valuePseudoRange, PseudoRange))
        {
            PseudoRange = valuePseudoRange;
        }
    }
}
