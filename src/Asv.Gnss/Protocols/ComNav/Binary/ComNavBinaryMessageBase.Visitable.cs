using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class ComNavBinaryMessageBase : IVisitable
{
    private static readonly Field PayloadMessageLengthField = SbfComNavVisitor.Field(nameof(MessageLength), UInt16Type.Default, "MessageLength");
    private static readonly Field PayloadReserved7Field = SbfComNavVisitor.Field(nameof(Reserved7), UInt16Type.Default, "Reserved7");
    private static readonly Field PayloadReserved6Field = SbfComNavVisitor.Field(nameof(Reserved6), UInt32Type.Default, "Reserved6");
    private static readonly Field PayloadTimeStatusField = SbfComNavVisitor.Field(nameof(TimeStatus), UInt8Type.Default, "TimeStatus", enumType: typeof(ComNavTimeStatusEnum));
    private static readonly Field PayloadReserved4Field = SbfComNavVisitor.Field(nameof(Reserved4), UInt8Type.Default, "Reserved4");
    private static readonly Field PayloadReserved3Field = SbfComNavVisitor.Field(nameof(Reserved3), UInt16Type.Default, "Reserved3");
    private static readonly Field PayloadSourceField = SbfComNavVisitor.Field(nameof(Source), UInt32Type.Default, "Source", enumType: typeof(ComNavPortEnum));
    private static readonly Field PayloadUtcTimeField = SbfComNavVisitor.Field(nameof(UtcTime), DateTimeType.Default, "UtcTime");
    private static readonly Field PayloadGpsTimeField = SbfComNavVisitor.Field(nameof(GpsTime), DateTimeType.Default, "GPS Time");
    private static readonly Field PayloadReceiverSwVersionField = SbfComNavVisitor.Field(nameof(ReceiverSwVersion), UInt16Type.Default, "This is a value (0 - 65535) that represents the receiver software build number");
    private static readonly Field PayloadGpsMSecsField = SbfComNavVisitor.Field(nameof(GpsMSecs), UInt32Type.Default, "Milliseconds from the beginning of the GPS week.");
    private static readonly Field PayloadGpsWeekField = SbfComNavVisitor.Field(nameof(GpsWeek), UInt16Type.Default, "GPS week number.");

    internal static readonly StructType PayloadVisitorType = new([PayloadMessageLengthField, PayloadReserved7Field, PayloadReserved6Field, PayloadTimeStatusField, PayloadReserved4Field, PayloadReserved3Field, PayloadSourceField, PayloadUtcTimeField, PayloadGpsTimeField, PayloadReceiverSwVersionField, PayloadGpsMSecsField, PayloadGpsWeekField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueMessageLength = MessageLength;
        UInt16Type.Accept(visitor, PayloadMessageLengthField, PayloadMessageLengthField.DataType, ref valueMessageLength);
        if (!EqualityComparer<ushort>.Default.Equals(valueMessageLength, MessageLength))
        {
            MessageLength = valueMessageLength;
        }

        var valueReserved7 = Reserved7;
        UInt16Type.Accept(visitor, PayloadReserved7Field, PayloadReserved7Field.DataType, ref valueReserved7);
        if (!EqualityComparer<ushort>.Default.Equals(valueReserved7, Reserved7))
        {
            Reserved7 = valueReserved7;
        }

        var valueReserved6 = Reserved6;
        UInt32Type.Accept(visitor, PayloadReserved6Field, PayloadReserved6Field.DataType, ref valueReserved6);
        if (!EqualityComparer<uint>.Default.Equals(valueReserved6, Reserved6))
        {
            Reserved6 = valueReserved6;
        }

        var valueTimeStatus = TimeStatus;
        var numericvalueTimeStatus = (byte)valueTimeStatus;
        UInt8Type.Accept(visitor, PayloadTimeStatusField, PayloadTimeStatusField.DataType, ref numericvalueTimeStatus);
        valueTimeStatus = (ComNavTimeStatusEnum)numericvalueTimeStatus;
        if (!EqualityComparer<ComNavTimeStatusEnum>.Default.Equals(valueTimeStatus, TimeStatus))
        {
            TimeStatus = valueTimeStatus;
        }

        var valueReserved4 = Reserved4;
        UInt8Type.Accept(visitor, PayloadReserved4Field, PayloadReserved4Field.DataType, ref valueReserved4);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved4, Reserved4))
        {
            Reserved4 = valueReserved4;
        }

        var valueReserved3 = Reserved3;
        UInt16Type.Accept(visitor, PayloadReserved3Field, PayloadReserved3Field.DataType, ref valueReserved3);
        if (!EqualityComparer<ushort>.Default.Equals(valueReserved3, Reserved3))
        {
            Reserved3 = valueReserved3;
        }

        var valueSource = Source;
        var numericvalueSource = (uint)valueSource;
        UInt32Type.Accept(visitor, PayloadSourceField, PayloadSourceField.DataType, ref numericvalueSource);
        valueSource = (ComNavPortEnum)numericvalueSource;
        if (!EqualityComparer<ComNavPortEnum>.Default.Equals(valueSource, Source))
        {
            Source = valueSource;
        }

        var valueUtcTime = UtcTime;
        DateTimeType.Accept(visitor, PayloadUtcTimeField, PayloadUtcTimeField.DataType, ref valueUtcTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueUtcTime, UtcTime))
        {
            UtcTime = valueUtcTime;
        }

        var valueGpsTime = GpsTime;
        DateTimeType.Accept(visitor, PayloadGpsTimeField, PayloadGpsTimeField.DataType, ref valueGpsTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueGpsTime, GpsTime))
        {
            GpsTime = valueGpsTime;
        }

        var valueReceiverSwVersion = ReceiverSwVersion;
        UInt16Type.Accept(visitor, PayloadReceiverSwVersionField, PayloadReceiverSwVersionField.DataType, ref valueReceiverSwVersion);
        if (!EqualityComparer<ushort>.Default.Equals(valueReceiverSwVersion, ReceiverSwVersion))
        {
            ReceiverSwVersion = valueReceiverSwVersion;
        }

        var valueGpsMSecs = GpsMSecs;
        UInt32Type.Accept(visitor, PayloadGpsMSecsField, PayloadGpsMSecsField.DataType, ref valueGpsMSecs);
        if (!EqualityComparer<uint>.Default.Equals(valueGpsMSecs, GpsMSecs))
        {
            GpsMSecs = valueGpsMSecs;
        }

        var valueGpsWeek = GpsWeek;
        UInt16Type.Accept(visitor, PayloadGpsWeekField, PayloadGpsWeekField.DataType, ref valueGpsWeek);
        if (!EqualityComparer<ushort>.Default.Equals(valueGpsWeek, GpsWeek))
        {
            GpsWeek = valueGpsWeek;
        }
    }
}
