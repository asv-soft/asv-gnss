using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1013
{
    private static readonly Field RtcmSecondsOfDayField = RtcmV3Visitor.Field(nameof(SecondsOfDay), UInt32Type.Default, "SecondsOfDay");
    private static readonly Field RtcmLeapSecondsField = RtcmV3Visitor.Field(nameof(LeapSeconds), UInt8Type.Default, "LeapSeconds");
    private static readonly Field RtcmModifiedJulianDayField = RtcmV3Visitor.Field(nameof(ModifiedJulianDay), UInt16Type.Default, "ModifiedJulianDay");
    private static readonly Field RtcmEpochTimeField = RtcmV3Visitor.Field(nameof(EpochTime), DateTimeType.Default, "EpochTime");
    private static readonly Field RtcmReferenceStationIDField = RtcmV3Visitor.Field(nameof(ReferenceStationID), UInt32Type.Default, "The Reference Station ID is determined by the service provider. Its primary purpose is to link all message data to their unique sourceName. It is useful in distinguishing between desired and undesired data in cases where more than one service may be using the same data link frequency. It is also useful in accommodating multiple reference stations within a single data link transmission. In reference network applications the Reference Station ID plays an important role, because it is the link between the observation messages of a specific reference station and its auxiliary information contained in other messages for proper operation. Thus the Service Provider should ensure that the Reference Station ID is unique within the whole network, and that ID’s should be reassigned only when absolutely necessary. Service Providers may need to coordinate their Reference Station ID assignments with other Service Providers in their region in order to avoid conflicts. This may be especially critical for equipment accessing multiple services, depending on their services and means of information distribution.");
    private static readonly Field RtcmSystemMessagesField = RtcmV3Visitor.Field(nameof(SystemMessages), new ListType(SystemMessage.RtcmVisitorType, 0, RtcmV3Visitor.MaximumElements), "SystemMessages");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmSecondsOfDayField, RtcmLeapSecondsField, RtcmModifiedJulianDayField, RtcmEpochTimeField, RtcmReferenceStationIDField, RtcmSystemMessagesField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSecondsOfDay = SecondsOfDay;
        UInt32Type.Accept(visitor, RtcmSecondsOfDayField, RtcmSecondsOfDayField.DataType, ref valueSecondsOfDay);
        if (!EqualityComparer<uint>.Default.Equals(valueSecondsOfDay, SecondsOfDay))
        {
            SecondsOfDay = valueSecondsOfDay;
        }

        var valueLeapSeconds = LeapSeconds;
        UInt8Type.Accept(visitor, RtcmLeapSecondsField, RtcmLeapSecondsField.DataType, ref valueLeapSeconds);
        if (!EqualityComparer<byte>.Default.Equals(valueLeapSeconds, LeapSeconds))
        {
            LeapSeconds = valueLeapSeconds;
        }

        var valueModifiedJulianDay = ModifiedJulianDay;
        UInt16Type.Accept(visitor, RtcmModifiedJulianDayField, RtcmModifiedJulianDayField.DataType, ref valueModifiedJulianDay);
        if (!EqualityComparer<ushort>.Default.Equals(valueModifiedJulianDay, ModifiedJulianDay))
        {
            ModifiedJulianDay = valueModifiedJulianDay;
        }

        var valueEpochTime = EpochTime;
        DateTimeType.Accept(visitor, RtcmEpochTimeField, RtcmEpochTimeField.DataType, ref valueEpochTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueEpochTime, EpochTime))
        {
            EpochTime = valueEpochTime;
        }

        var valueReferenceStationID = ReferenceStationID;
        UInt32Type.Accept(visitor, RtcmReferenceStationIDField, RtcmReferenceStationIDField.DataType, ref valueReferenceStationID);
        if (!EqualityComparer<uint>.Default.Equals(valueReferenceStationID, ReferenceStationID))
        {
            ReferenceStationID = valueReferenceStationID;
        }

        var valueSystemMessages = SystemMessages;
        RtcmV3Visitor.VisitArray(visitor, RtcmSystemMessagesField, RtcmSystemMessagesField.DataType, ref valueSystemMessages,
            static () => new SystemMessage(),
            static (IVisitor v, Field f, IFieldType t, ref SystemMessage item) =>
            {
                if (item != null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valueSystemMessages, SystemMessages))
        {
            SystemMessages = valueSystemMessages!;
        }
    }
}
