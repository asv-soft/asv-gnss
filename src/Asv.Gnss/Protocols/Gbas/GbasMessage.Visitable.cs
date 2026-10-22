using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class GbasMessage : IVisitable
{
    private static readonly Field PayloadSsIdField = ProtocolPayloadVisitor.Field(nameof(SsId), UInt8Type.Default, enumType: typeof(SlotIdEnum));
    private static readonly Field PayloadApplicationDataField = ProtocolPayloadVisitor.Field(nameof(ApplicationData), new ListType(GbasMessageBlockBase.PayloadVisitorType, 0, int.MaxValue), readOnly: true);

    internal static readonly StructType PayloadVisitorType = new([PayloadSsIdField, PayloadApplicationDataField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        SlotIdEnum valueSsId = SsId;
        var numericSsId = (byte)valueSsId;
        UInt8Type.Accept(visitor, PayloadSsIdField, PayloadSsIdField.DataType, ref numericSsId);
        valueSsId = (SlotIdEnum)numericSsId;
        if (!EqualityComparer<SlotIdEnum>.Default.Equals(valueSsId, SsId))
        {
            SsId = valueSsId;
        }

        List<GbasMessageBlockBase> valueApplicationData = ApplicationData;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadApplicationDataField, PayloadApplicationDataField.DataType, ref valueApplicationData,
            static () => default(GbasMessageBlockBase)!,
            static (IVisitor v, Field f, IFieldType t, ref GbasMessageBlockBase item) =>
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
