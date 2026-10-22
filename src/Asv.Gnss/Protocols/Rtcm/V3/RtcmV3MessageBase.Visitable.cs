using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3MessageBase : IVisitable
{
    internal static readonly StructType RtcmVisitorType = new([]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
    }
}
