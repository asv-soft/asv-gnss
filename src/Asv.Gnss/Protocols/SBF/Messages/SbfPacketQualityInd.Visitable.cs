using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketQualityInd
{
    private static readonly Field PayloadIndicatorsField = SbfComNavVisitor.Field(nameof(Indicators), new ListType(QualityIndicator.PayloadVisitorType, 0, ushort.MaxValue), "Indicators");
    private static readonly Field PayloadReservedField = SbfComNavVisitor.Field(nameof(Reserved), UInt8Type.Default, "Reserved");

    internal new static readonly StructType PayloadVisitorType = new([..SbfMessageBase.PayloadVisitorType.Fields, PayloadIndicatorsField, PayloadReservedField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueIndicators = Indicators;
        SbfComNavVisitor.VisitArray(visitor, PayloadIndicatorsField, PayloadIndicatorsField.DataType, ref valueIndicators, static () => new QualityIndicator(),
            static (IVisitor v, Field f, IFieldType t, ref QualityIndicator item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valueIndicators, Indicators))
        {
            Indicators = valueIndicators!;
        }

        var valueReserved = Reserved;
        UInt8Type.Accept(visitor, PayloadReservedField, PayloadReservedField.DataType, ref valueReserved);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved, Reserved))
        {
            Reserved = valueReserved;
        }
    }
}
