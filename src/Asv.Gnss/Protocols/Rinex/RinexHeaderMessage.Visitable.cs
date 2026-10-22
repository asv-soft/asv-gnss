using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RinexHeaderMessage
{

    internal new static readonly StructType NrVisitorType = new([..RinexMessageBase.NrVisitorType.Fields]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);
    }
}
