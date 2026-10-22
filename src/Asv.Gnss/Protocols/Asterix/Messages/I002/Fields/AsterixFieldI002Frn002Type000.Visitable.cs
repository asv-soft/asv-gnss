using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI002Frn002Type000
{
    private static readonly Field VisitorMessageTypeField = new Field.Builder().Name(nameof(MessageType)).Title(nameof(MessageType)).Description("MessageType").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorMessageTypeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMessageType = MessageType;
        UInt8Type.Accept(visitor, VisitorMessageTypeField, ref valueMessageType);
        if (!EqualityComparer<byte>.Default.Equals(valueMessageType, MessageType)) MessageType = valueMessageType;
    }
}
