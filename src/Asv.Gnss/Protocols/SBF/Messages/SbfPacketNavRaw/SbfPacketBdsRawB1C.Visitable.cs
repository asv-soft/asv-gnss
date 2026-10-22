using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketBdsRawB1C
{

    internal new static readonly StructType PayloadVisitorType = new([..SbfPacketGnssRawNavMsgBase.PayloadVisitorType.Fields]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);
    }
}
