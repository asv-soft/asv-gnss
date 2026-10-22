using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdditionalDataBlock4
{
    private static readonly Field PayloadSlotGroupDefinitionField = ProtocolPayloadVisitor.Field(nameof(SlotGroupDefinition), new ListType(UInt8Type.Default, 0, int.MaxValue), readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..AdditionalDataBlock.PayloadVisitorType.Fields, PayloadSlotGroupDefinitionField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        List<SlotIdEnum> valueSlotGroupDefinition = SlotGroupDefinition;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadSlotGroupDefinitionField, PayloadSlotGroupDefinitionField.DataType, ref valueSlotGroupDefinition,
            static () => default(SlotIdEnum)!,
            static (IVisitor v, Field f, IFieldType t, ref SlotIdEnum item) =>
            {
                var numericitem = (byte)item;
                UInt8Type.Accept(v, f, t, ref numericitem);
                item = (SlotIdEnum )numericitem;
            });
    }
}
