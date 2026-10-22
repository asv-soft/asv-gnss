using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RinexMessageBase : IVisitable
{
    private static readonly Field NrHeaderField = NmeaRinexVisitor.Field(nameof(Header), RinexHeader.NrVisitorType);

    internal static readonly StructType NrVisitorType = new([NrHeaderField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueHeader = Header;
        if (valueHeader != null) StructType.Accept(visitor, NrHeaderField, NrHeaderField.DataType, valueHeader);
        else visitor.VisitUnknown(NrHeaderField, NrHeaderField.DataType);
    }
}
