using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn021Type310
{
    private static readonly Field VisitorTrbField = new Field.Builder().Name(nameof(Trb)).Title(nameof(Trb)).Description("Trb").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMessageField = new Field.Builder().Name(nameof(Message)).Title(nameof(Message)).Description("Message").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTrbField, VisitorMessageField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTrb = Trb;
        BoolType.Accept(visitor, VisitorTrbField, ref valueTrb);
        if (!EqualityComparer<bool>.Default.Equals(valueTrb, Trb)) Trb = valueTrb;
        var valueMessage = Message;
        UInt8Type.Accept(visitor, VisitorMessageField, ref valueMessage);
        if (!EqualityComparer<byte>.Default.Equals(valueMessage, Message)) Message = valueMessage;
    }
}
