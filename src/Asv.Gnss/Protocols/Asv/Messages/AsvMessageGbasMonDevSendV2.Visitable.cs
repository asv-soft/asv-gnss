using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessageGbasMonDevSendV2
{
    private static readonly Field PayloadSlotField = ProtocolPayloadVisitor.Field(nameof(Slot), UInt8Type.Default, enumType: typeof(AsvGbasSlotMsg));
    private static readonly Field PayloadIsLastSlotInFrameField = ProtocolPayloadVisitor.Field(nameof(IsLastSlotInFrame), BoolType.Default);
    private static readonly Field PayloadLifeTimeField = ProtocolPayloadVisitor.Field(nameof(LifeTime), UInt8Type.Default);
    private static readonly Field PayloadMsgLengthField = ProtocolPayloadVisitor.Field(nameof(MsgLength), UInt16Type.Default);
    private static readonly Field PayloadMsgCrcField = ProtocolPayloadVisitor.Field(nameof(MsgCrc), UInt16Type.Default);

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadSlotField, PayloadIsLastSlotInFrameField, PayloadLifeTimeField, PayloadMsgLengthField, PayloadMsgCrcField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        AsvGbasSlotMsg valueSlot = Slot;
        var numericSlot = (byte)valueSlot;
        UInt8Type.Accept(visitor, PayloadSlotField, PayloadSlotField.DataType, ref numericSlot);
        valueSlot = (AsvGbasSlotMsg)numericSlot;
        if (!EqualityComparer<AsvGbasSlotMsg>.Default.Equals(valueSlot, Slot))
        {
            Slot = valueSlot;
        }

        bool valueIsLastSlotInFrame = IsLastSlotInFrame;
        BoolType.Accept(visitor, PayloadIsLastSlotInFrameField, PayloadIsLastSlotInFrameField.DataType, ref valueIsLastSlotInFrame);
        if (!EqualityComparer<bool>.Default.Equals(valueIsLastSlotInFrame, IsLastSlotInFrame))
        {
            IsLastSlotInFrame = valueIsLastSlotInFrame;
        }

        byte valueLifeTime = LifeTime;
        UInt8Type.Accept(visitor, PayloadLifeTimeField, PayloadLifeTimeField.DataType, ref valueLifeTime);
        if (!EqualityComparer<byte>.Default.Equals(valueLifeTime, LifeTime))
        {
            LifeTime = valueLifeTime;
        }

        ushort valueMsgLength = MsgLength;
        UInt16Type.Accept(visitor, PayloadMsgLengthField, PayloadMsgLengthField.DataType, ref valueMsgLength);
        if (!EqualityComparer<ushort>.Default.Equals(valueMsgLength, MsgLength))
        {
            MsgLength = valueMsgLength;
        }

        ushort valueMsgCrc = MsgCrc;
        UInt16Type.Accept(visitor, PayloadMsgCrcField, PayloadMsgCrcField.DataType, ref valueMsgCrc);
        if (!EqualityComparer<ushort>.Default.Equals(valueMsgCrc, MsgCrc))
        {
            MsgCrc = valueMsgCrc;
        }
    }
}
