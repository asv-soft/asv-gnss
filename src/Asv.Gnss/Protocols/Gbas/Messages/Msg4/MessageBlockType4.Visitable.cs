using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MessageBlockType4
{
    private static readonly Field PayloadFasDataSetField = ProtocolPayloadVisitor.Field(nameof(FasDataSet), new ListType(Asv.Gnss.FasDataSet.PayloadVisitorType, 0, int.MaxValue), readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..GbasMessageBlockBase.PayloadVisitorType.Fields, PayloadFasDataSetField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        List<FasDataSet> valueFasDataSet = FasDataSet;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadFasDataSetField, PayloadFasDataSetField.DataType, ref valueFasDataSet,
            static () => new FasDataSet(),
            static (IVisitor v, Field f, IFieldType t, ref FasDataSet item) =>
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
    }
}
