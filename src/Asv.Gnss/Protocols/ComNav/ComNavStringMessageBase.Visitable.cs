using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class ComNavStringMessageBase : IVisitable
{

    internal static readonly StructType PayloadVisitorType = new([]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
    }
}
