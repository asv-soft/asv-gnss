using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class AsvMessageBase : IVisitable
{
    private static readonly Field PayloadSequenceField = ProtocolPayloadVisitor.Field(nameof(Sequence), UInt16Type.Default);
    private static readonly Field PayloadTargetIdField = ProtocolPayloadVisitor.Field(nameof(TargetId), UInt8Type.Default);
    private static readonly Field PayloadSenderIdField = ProtocolPayloadVisitor.Field(nameof(SenderId), UInt8Type.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadSequenceField, PayloadTargetIdField, PayloadSenderIdField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        ushort valueSequence = Sequence;
        UInt16Type.Accept(visitor, PayloadSequenceField, PayloadSequenceField.DataType, ref valueSequence);
        if (!EqualityComparer<ushort>.Default.Equals(valueSequence, Sequence))
        {
            Sequence = valueSequence;
        }

        byte valueTargetId = TargetId;
        UInt8Type.Accept(visitor, PayloadTargetIdField, PayloadTargetIdField.DataType, ref valueTargetId);
        if (!EqualityComparer<byte>.Default.Equals(valueTargetId, TargetId))
        {
            TargetId = valueTargetId;
        }

        byte valueSenderId = SenderId;
        UInt8Type.Accept(visitor, PayloadSenderIdField, PayloadSenderIdField.DataType, ref valueSenderId);
        if (!EqualityComparer<byte>.Default.Equals(valueSenderId, SenderId))
        {
            SenderId = valueSenderId;
        }
    }
}
