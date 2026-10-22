using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class SbfMessageBase : IVisitable
{
    private static readonly Field PayloadUtcTimeField = SbfComNavVisitor.Field(nameof(UtcTime), DateTimeType.Default, "UtcTime");
    private static readonly Field PayloadTOWField = SbfComNavVisitor.Field(nameof(TOW), UInt32Type.Default, "Time-Of-Week : Time-tag, expressed in whole milliseconds from the beginning of the current GPS week.");
    private static readonly Field PayloadWNcField = SbfComNavVisitor.Field(nameof(WNc), UInt16Type.Default, "The GPS week number associated with the TOW. WNc is a continuous week count(hence the \"c\"). It is not affected by GPS week rollovers, which occur every 1024 weeks. By definition of the Galileo system time, WNc is also the Galileo week number plus 1024.");

    internal static readonly StructType PayloadVisitorType = new([PayloadUtcTimeField, PayloadTOWField, PayloadWNcField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueUtcTime = UtcTime;
        DateTimeType.Accept(visitor, PayloadUtcTimeField, PayloadUtcTimeField.DataType, ref valueUtcTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueUtcTime, UtcTime))
        {
            UtcTime = valueUtcTime;
        }

        var valueTOW = TOW;
        UInt32Type.Accept(visitor, PayloadTOWField, PayloadTOWField.DataType, ref valueTOW);
        if (!EqualityComparer<uint>.Default.Equals(valueTOW, TOW))
        {
            TOW = valueTOW;
        }

        var valueWNc = WNc;
        UInt16Type.Accept(visitor, PayloadWNcField, PayloadWNcField.DataType, ref valueWNc);
        if (!EqualityComparer<ushort>.Default.Equals(valueWNc, WNc))
        {
            WNc = valueWNc;
        }
    }
}
