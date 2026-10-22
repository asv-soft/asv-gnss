using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavDgpsTxIdCommand
{
    private static readonly Field PayloadTypeField = SbfComNavVisitor.Field(nameof(Type), Int32Type.Default, "Gets or sets the type of DgpsTxId.", enumType: typeof(DgpsTxIdEnum));
    private static readonly Field PayloadIdField = SbfComNavVisitor.Field(nameof(Id), UInt32Type.Default, "Gets or sets the ID of the property.");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavAsciiCommandBase.PayloadVisitorType.Fields, PayloadTypeField, PayloadIdField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueType = Type;
        var numericvalueType = (int)valueType;
        Int32Type.Accept(visitor, PayloadTypeField, PayloadTypeField.DataType, ref numericvalueType);
        valueType = (DgpsTxIdEnum)numericvalueType;
        if (!EqualityComparer<DgpsTxIdEnum>.Default.Equals(valueType, Type))
        {
            Type = valueType;
        }

        var valueId = Id;
        UInt32Type.Accept(visitor, PayloadIdField, PayloadIdField.DataType, ref valueId);
        if (!EqualityComparer<uint>.Default.Equals(valueId, Id))
        {
            Id = valueId;
        }
    }
}
