using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class ComNavSimpleAnswerMessageBase
{
    private static readonly Field PayloadBodyField = SbfComNavVisitor.Field(nameof(Body), StringOptionalType.Utf8, "Body");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavStringMessageBase.PayloadVisitorType.Fields, PayloadBodyField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        string? valueBody = Body;
        StringOptionalType.Accept(visitor, PayloadBodyField, PayloadBodyField.DataType, ref valueBody);
        if (!EqualityComparer<string>.Default.Equals(valueBody, Body))
        {
            Body = valueBody!;
        }
    }
}
