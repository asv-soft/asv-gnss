using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public readonly partial struct NmeaTalkerId : IVisitable
{
    private static readonly Field NrIdField = NmeaRinexVisitor.Field(nameof(Id), StringOptionalType.Utf8, readOnly: true);
    private static readonly Field NrTypeField = NmeaRinexVisitor.Field(nameof(Type), Int32Type.Default, readOnly: true, sourceType: typeof(NmeaTalkerClass));

    internal static readonly StructType NrVisitorType = new([NrIdField, NrTypeField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        string? valueId = Id;
        StringOptionalType.Accept(visitor, NrIdField, NrIdField.DataType, ref valueId);

        var valueType = (int)Type;
        Int32Type.Accept(visitor, NrTypeField, NrTypeField.DataType, ref valueType);
    }
}
