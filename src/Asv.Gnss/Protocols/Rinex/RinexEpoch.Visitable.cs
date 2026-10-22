using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public readonly partial record struct RinexEpoch : IVisitable
{
    private static readonly Field NrYearField = NmeaRinexVisitor.Field(nameof(Year), Int32Type.Default, readOnly: true);
    private static readonly Field NrMonthField = NmeaRinexVisitor.Field(nameof(Month), Int32Type.Default, readOnly: true);
    private static readonly Field NrDayField = NmeaRinexVisitor.Field(nameof(Day), Int32Type.Default, readOnly: true);
    private static readonly Field NrHourField = NmeaRinexVisitor.Field(nameof(Hour), Int32Type.Default, readOnly: true);
    private static readonly Field NrMinuteField = NmeaRinexVisitor.Field(nameof(Minute), Int32Type.Default, readOnly: true);
    private static readonly Field NrSecondsField = NmeaRinexVisitor.Field(nameof(Seconds), DoubleType.Default, readOnly: true, sourceType: typeof(decimal));

    internal static readonly StructType NrVisitorType = new([NrYearField, NrMonthField, NrDayField, NrHourField, NrMinuteField, NrSecondsField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueYear = Year;
        Int32Type.Accept(visitor, NrYearField, NrYearField.DataType, ref valueYear);

        var valueMonth = Month;
        Int32Type.Accept(visitor, NrMonthField, NrMonthField.DataType, ref valueMonth);

        var valueDay = Day;
        Int32Type.Accept(visitor, NrDayField, NrDayField.DataType, ref valueDay);

        var valueHour = Hour;
        Int32Type.Accept(visitor, NrHourField, NrHourField.DataType, ref valueHour);

        var valueMinute = Minute;
        Int32Type.Accept(visitor, NrMinuteField, NrMinuteField.DataType, ref valueMinute);

        var valueSeconds = (double)Seconds;
        DoubleType.Accept(visitor, NrSecondsField, NrSecondsField.DataType, ref valueSeconds);
    }
}
