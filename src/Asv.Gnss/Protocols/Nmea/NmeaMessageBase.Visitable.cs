using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageBase : IVisitable
{
    private static readonly Field NrHasCrcField = NmeaRinexVisitor.Field(nameof(HasCrc), BoolType.Default);
    private static readonly Field NrTalkerIdField = NmeaRinexVisitor.Field(nameof(TalkerId), NmeaTalkerId.NrVisitorType);

    internal static readonly StructType NrVisitorType = new([NrHasCrcField, NrTalkerIdField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueHasCrc = HasCrc;
        BoolType.Accept(visitor, NrHasCrcField, NrHasCrcField.DataType, ref valueHasCrc);
        if (!EqualityComparer<bool>.Default.Equals(valueHasCrc, HasCrc))
        {
            HasCrc = valueHasCrc;
        }

        var valueTalkerId = TalkerId;
        StructType.Accept(visitor, NrTalkerIdField, NrTalkerIdField.DataType, valueTalkerId);
    }
}
