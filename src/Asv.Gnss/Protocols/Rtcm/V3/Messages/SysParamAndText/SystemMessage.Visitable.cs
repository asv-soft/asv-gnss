using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SystemMessage : IVisitable
{
    private static readonly Field RtcmIdField = RtcmV3Visitor.Field(nameof(Id), UInt16Type.Default, "Each announcement lists the Message ID as transmitted by the reference station.");
    private static readonly Field RtcmSyncFlagField = RtcmV3Visitor.Field(nameof(SyncFlag), UInt8Type.Default, "0 - Asynchronous – not transmitted on a regular basis; 1 - Synchronous – scheduled for transmission at regular intervals");
    private static readonly Field RtcmTransmissionIntervalField = RtcmV3Visitor.Field(nameof(TransmissionInterval), DoubleType.Default, "Each announcement lists the Message Transmission Interval as transmitted by the reference station. If asynchronous, the transmission interval is approximate.");

    internal static readonly StructType RtcmVisitorType = new([RtcmIdField, RtcmSyncFlagField, RtcmTransmissionIntervalField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueId = Id;
        UInt16Type.Accept(visitor, RtcmIdField, RtcmIdField.DataType, ref valueId);
        if (!EqualityComparer<ushort>.Default.Equals(valueId, Id))
        {
            Id = valueId;
        }

        var valueSyncFlag = SyncFlag;
        UInt8Type.Accept(visitor, RtcmSyncFlagField, RtcmSyncFlagField.DataType, ref valueSyncFlag);
        if (!EqualityComparer<byte>.Default.Equals(valueSyncFlag, SyncFlag))
        {
            SyncFlag = valueSyncFlag;
        }

        var valueTransmissionInterval = TransmissionInterval;
        DoubleType.Accept(visitor, RtcmTransmissionIntervalField, RtcmTransmissionIntervalField.DataType, ref valueTransmissionInterval);
        if (!EqualityComparer<double>.Default.Equals(valueTransmissionInterval, TransmissionInterval))
        {
            TransmissionInterval = valueTransmissionInterval;
        }
    }
}
