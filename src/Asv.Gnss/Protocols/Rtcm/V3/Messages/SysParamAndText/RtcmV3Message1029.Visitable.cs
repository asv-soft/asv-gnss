using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1029
{
    private static readonly Field RtcmNumberOfCharactersFollowField = RtcmV3Visitor.Field(nameof(NumberOfCharactersFollow), UInt32Type.Default, "This represents the number of fully formed Unicode characters in the message text. It is not necessarily the number of bytes that are needed to represent the characters as UTF-8. Note that for some messages it may not be possible to utilize the full range of this field, e.g. where many characters require 3 or 4 byte representations and together will exceed 255 code units");
    private static readonly Field RtcmSecondsOfDayField = RtcmV3Visitor.Field(nameof(SecondsOfDay), UInt32Type.Default, "SecondsOfDay");
    private static readonly Field RtcmModifiedJulianDayField = RtcmV3Visitor.Field(nameof(ModifiedJulianDay), UInt16Type.Default, "ModifiedJulianDay");
    private static readonly Field RtcmReferenceStationIDField = RtcmV3Visitor.Field(nameof(ReferenceStationID), UInt32Type.Default, "ReferenceStationID");
    private static readonly Field RtcmMessageField = RtcmV3Visitor.Field(nameof(Message), StringOptionalType.Utf8, "Message");
    private static readonly Field RtcmEpochTimeField = RtcmV3Visitor.Field(nameof(EpochTime), DateTimeType.Default, "EpochTime");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmNumberOfCharactersFollowField, RtcmSecondsOfDayField, RtcmModifiedJulianDayField, RtcmReferenceStationIDField, RtcmMessageField, RtcmEpochTimeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueNumberOfCharactersFollow = NumberOfCharactersFollow;
        UInt32Type.Accept(visitor, RtcmNumberOfCharactersFollowField, RtcmNumberOfCharactersFollowField.DataType, ref valueNumberOfCharactersFollow);
        if (!EqualityComparer<uint>.Default.Equals(valueNumberOfCharactersFollow, NumberOfCharactersFollow))
        {
            NumberOfCharactersFollow = valueNumberOfCharactersFollow;
        }

        var valueSecondsOfDay = SecondsOfDay;
        UInt32Type.Accept(visitor, RtcmSecondsOfDayField, RtcmSecondsOfDayField.DataType, ref valueSecondsOfDay);
        if (!EqualityComparer<uint>.Default.Equals(valueSecondsOfDay, SecondsOfDay))
        {
            SecondsOfDay = valueSecondsOfDay;
        }

        var valueModifiedJulianDay = ModifiedJulianDay;
        UInt16Type.Accept(visitor, RtcmModifiedJulianDayField, RtcmModifiedJulianDayField.DataType, ref valueModifiedJulianDay);
        if (!EqualityComparer<ushort>.Default.Equals(valueModifiedJulianDay, ModifiedJulianDay))
        {
            ModifiedJulianDay = valueModifiedJulianDay;
        }

        var valueReferenceStationID = ReferenceStationID;
        UInt32Type.Accept(visitor, RtcmReferenceStationIDField, RtcmReferenceStationIDField.DataType, ref valueReferenceStationID);
        if (!EqualityComparer<uint>.Default.Equals(valueReferenceStationID, ReferenceStationID))
        {
            ReferenceStationID = valueReferenceStationID;
        }

        string? valueMessage = Message;
        StringOptionalType.Accept(visitor, RtcmMessageField, RtcmMessageField.DataType, ref valueMessage);
        if (!EqualityComparer<string>.Default.Equals(valueMessage, Message))
        {
            Message = valueMessage!;
        }

        var valueEpochTime = EpochTime;
        DateTimeType.Accept(visitor, RtcmEpochTimeField, RtcmEpochTimeField.DataType, ref valueEpochTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueEpochTime, EpochTime))
        {
            EpochTime = valueEpochTime;
        }
    }
}
