using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn046Type400
{
    private static readonly Field VisitorReceiverIdField = new Field.Builder().Name(nameof(ReceiverId)).Title(nameof(ReceiverId)).Description("ReceiverId").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorReceiverIdField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueReceiverId = ReceiverId;
        UInt8Type.Accept(visitor, VisitorReceiverIdField, ref valueReceiverId);
        if (!EqualityComparer<byte>.Default.Equals(valueReceiverId, ReceiverId)) ReceiverId = valueReceiverId;
    }
}
