using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageZda
{
    private static readonly Field NrDateField = NmeaRinexVisitor.Field(nameof(Date), DateOnlyOptionalType.Default);
    private static readonly Field NrTimeField = NmeaRinexVisitor.Field(nameof(Time), TimeOnlyOptionalType.Default);

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrDateField, NrTimeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueDate = Date;
        DateOnlyOptionalType.Accept(visitor, NrDateField, NrDateField.DataType, ref valueDate);
        if (!EqualityComparer<DateOnly?>.Default.Equals(valueDate, Date))
        {
            Date = valueDate;
        }

        var valueTime = Time;
        TimeOnlyOptionalType.Accept(visitor, NrTimeField, NrTimeField.DataType, ref valueTime);
        if (!EqualityComparer<TimeOnly?>.Default.Equals(valueTime, Time))
        {
            Time = valueTime;
        }
    }
}
