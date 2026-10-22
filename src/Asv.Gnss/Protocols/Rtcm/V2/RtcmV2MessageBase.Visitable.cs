using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV2MessageBase : IVisitable
{
    private static readonly Field PayloadUdreField = ProtocolPayloadVisitor.Field(nameof(Udre), DoubleType.Default);
    private static readonly Field PayloadSequenceNumberField = ProtocolPayloadVisitor.Field(nameof(SequenceNumber), UInt8Type.Default);
    private static readonly Field PayloadGpsTimeField = ProtocolPayloadVisitor.Field(nameof(GpsTime), DateTimeType.Default);
    private static readonly Field PayloadZCountField = ProtocolPayloadVisitor.Field(nameof(ZCount), DoubleType.Default);
    private static readonly Field PayloadReferenceStationIdField = ProtocolPayloadVisitor.Field(nameof(ReferenceStationId), UInt16Type.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadUdreField, PayloadSequenceNumberField, PayloadGpsTimeField, PayloadZCountField, PayloadReferenceStationIdField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        double valueUdre = Udre;
        DoubleType.Accept(visitor, PayloadUdreField, PayloadUdreField.DataType, ref valueUdre);
        if (!EqualityComparer<double>.Default.Equals(valueUdre, Udre))
        {
            Udre = valueUdre;
        }

        byte valueSequenceNumber = SequenceNumber;
        UInt8Type.Accept(visitor, PayloadSequenceNumberField, PayloadSequenceNumberField.DataType, ref valueSequenceNumber);
        if (!EqualityComparer<byte>.Default.Equals(valueSequenceNumber, SequenceNumber))
        {
            SequenceNumber = valueSequenceNumber;
        }

        DateTime valueGpsTime = GpsTime;
        DateTimeType.Accept(visitor, PayloadGpsTimeField, PayloadGpsTimeField.DataType, ref valueGpsTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueGpsTime, GpsTime))
        {
            GpsTime = valueGpsTime;
        }

        double valueZCount = ZCount;
        DoubleType.Accept(visitor, PayloadZCountField, PayloadZCountField.DataType, ref valueZCount);
        if (!EqualityComparer<double>.Default.Equals(valueZCount, ZCount))
        {
            ZCount = valueZCount;
        }

        ushort valueReferenceStationId = ReferenceStationId;
        UInt16Type.Accept(visitor, PayloadReferenceStationIdField, PayloadReferenceStationIdField.DataType, ref valueReferenceStationId);
        if (!EqualityComparer<ushort>.Default.Equals(valueReferenceStationId, ReferenceStationId))
        {
            ReferenceStationId = valueReferenceStationId;
        }
    }
}
