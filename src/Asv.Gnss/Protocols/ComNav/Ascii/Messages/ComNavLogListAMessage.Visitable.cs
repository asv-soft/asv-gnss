using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavLogListAMessage
{
    private static readonly Field PayloadMessagesField = SbfComNavVisitor.Field(nameof(Messages), new ListType(LogMessage.PayloadVisitorType, 0, ushort.MaxValue), "Messages");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavAsciiMessageBase.PayloadVisitorType.Fields, PayloadMessagesField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueMessages = Messages;
        SbfComNavVisitor.VisitArray(visitor, PayloadMessagesField, PayloadMessagesField.DataType, ref valueMessages, static () => new LogMessage(),
            static (IVisitor v, Field f, IFieldType t, ref LogMessage item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valueMessages, Messages))
        {
            Messages = valueMessages!;
        }
    }
}
