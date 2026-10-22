using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavAsciiSetCommand
{
    private static readonly Field PayloadTypeField = SbfComNavVisitor.Field(nameof(Type), Int32Type.Default, "Type", enumType: typeof(ComNavSetTypeEnum));
    private static readonly Field PayloadParamsField = SbfComNavVisitor.Field(nameof(Params), new ListType(StringOptionalType.Utf8, 0, ushort.MaxValue), "Params", readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..ComNavAsciiCommandBase.PayloadVisitorType.Fields, PayloadTypeField, PayloadParamsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueType = Type;
        var numericvalueType = (int)valueType;
        Int32Type.Accept(visitor, PayloadTypeField, PayloadTypeField.DataType, ref numericvalueType);
        valueType = (ComNavSetTypeEnum)numericvalueType;
        if (!EqualityComparer<ComNavSetTypeEnum>.Default.Equals(valueType, Type))
        {
            Type = valueType;
        }

        var valueParams = Params;
        SbfComNavVisitor.VisitStrings(visitor, PayloadParamsField, valueParams);
    }
}
