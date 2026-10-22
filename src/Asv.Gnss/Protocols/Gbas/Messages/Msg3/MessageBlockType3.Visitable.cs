using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MessageBlockType3
{
    private static readonly Field PayloadFilterField = ProtocolPayloadVisitor.Field(nameof(Filter), new ListType(UInt8Type.Default, 0, int.MaxValue), readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..GbasMessageBlockBase.PayloadVisitorType.Fields, PayloadFilterField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        List<byte> valueFilter = Filter;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadFilterField, PayloadFilterField.DataType, ref valueFilter,
            static () => default(byte)!,
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
    }
}
