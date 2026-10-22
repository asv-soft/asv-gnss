using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class GbasMessageBlockBase : IVisitable
{
    private static readonly Field PayloadMessageBlockIdField = ProtocolPayloadVisitor.Field(nameof(MessageBlockId), UInt8Type.Default, enumType: typeof(MessageBlockIdEnum));
    private static readonly Field PayloadGbasIdField = ProtocolPayloadVisitor.Field(nameof(GbasId), StringOptionalType.Utf8);

    internal static readonly StructType PayloadVisitorType = new([PayloadMessageBlockIdField, PayloadGbasIdField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        MessageBlockIdEnum valueMessageBlockId = MessageBlockId;
        var numericMessageBlockId = (byte)valueMessageBlockId;
        UInt8Type.Accept(visitor, PayloadMessageBlockIdField, PayloadMessageBlockIdField.DataType, ref numericMessageBlockId);
        valueMessageBlockId = (MessageBlockIdEnum)numericMessageBlockId;
        if (!EqualityComparer<MessageBlockIdEnum>.Default.Equals(valueMessageBlockId, MessageBlockId))
        {
            MessageBlockId = valueMessageBlockId;
        }

        string? valueGbasId = GbasId;
        StringOptionalType.Accept(visitor, PayloadGbasIdField, PayloadGbasIdField.DataType, ref valueGbasId);
        if (!EqualityComparer<string>.Default.Equals(valueGbasId, GbasId))
        {
            GbasId = valueGbasId!;
        }
    }
}
