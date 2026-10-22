using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class ComNavAsciiMessageBase
{
    private static readonly Field PayloadSourceField = SbfComNavVisitor.Field(nameof(Source), UInt32Type.Default, "Source", enumType: typeof(ComNavPortEnum));
    private static readonly Field PayloadTimeStatusField = SbfComNavVisitor.Field(nameof(TimeStatus), UInt8Type.Default, "TimeStatus", enumType: typeof(ComNavTimeStatusEnum));
    private static readonly Field PayloadGpsTimeField = SbfComNavVisitor.Field(nameof(GpsTime), DateTimeType.Default, "GpsTime");
    private static readonly Field PayloadUtcTimeField = SbfComNavVisitor.Field(nameof(UtcTime), DateTimeType.Default, "UtcTime");
    private static readonly Field PayloadMessageLengthField = SbfComNavVisitor.Field(nameof(MessageLength), Int32Type.Default, "MessageLength");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavStringMessageBase.PayloadVisitorType.Fields, PayloadSourceField, PayloadTimeStatusField, PayloadGpsTimeField, PayloadUtcTimeField, PayloadMessageLengthField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSource = Source;
        var numericvalueSource = (uint)valueSource;
        UInt32Type.Accept(visitor, PayloadSourceField, PayloadSourceField.DataType, ref numericvalueSource);
        valueSource = (ComNavPortEnum)numericvalueSource;
        if (!EqualityComparer<ComNavPortEnum>.Default.Equals(valueSource, Source))
        {
            Source = valueSource;
        }

        var valueTimeStatus = TimeStatus;
        var numericvalueTimeStatus = (byte)valueTimeStatus;
        UInt8Type.Accept(visitor, PayloadTimeStatusField, PayloadTimeStatusField.DataType, ref numericvalueTimeStatus);
        valueTimeStatus = (ComNavTimeStatusEnum)numericvalueTimeStatus;
        if (!EqualityComparer<ComNavTimeStatusEnum>.Default.Equals(valueTimeStatus, TimeStatus))
        {
            TimeStatus = valueTimeStatus;
        }

        var valueGpsTime = GpsTime;
        DateTimeType.Accept(visitor, PayloadGpsTimeField, PayloadGpsTimeField.DataType, ref valueGpsTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueGpsTime, GpsTime))
        {
            GpsTime = valueGpsTime;
        }

        var valueUtcTime = UtcTime;
        DateTimeType.Accept(visitor, PayloadUtcTimeField, PayloadUtcTimeField.DataType, ref valueUtcTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueUtcTime, UtcTime))
        {
            UtcTime = valueUtcTime;
        }

        var valueMessageLength = MessageLength;
        Int32Type.Accept(visitor, PayloadMessageLengthField, PayloadMessageLengthField.DataType, ref valueMessageLength);
        if (!EqualityComparer<int>.Default.Equals(valueMessageLength, MessageLength))
        {
            MessageLength = valueMessageLength;
        }
    }
}
