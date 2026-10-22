using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class AdditionalDataBlock : IVisitable
{
    private static readonly Field PayloadTypeNumberField = ProtocolPayloadVisitor.Field(nameof(TypeNumber), UInt8Type.Default, readOnly: true);

    internal static readonly StructType PayloadVisitorType = new([PayloadTypeNumberField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        byte valueTypeNumber = TypeNumber;
        UInt8Type.Accept(visitor, PayloadTypeNumberField, PayloadTypeNumberField.DataType, ref valueTypeNumber);
    }
}
