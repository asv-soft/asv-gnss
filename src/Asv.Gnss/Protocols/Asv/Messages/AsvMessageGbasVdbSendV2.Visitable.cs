using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessageGbasVdbSendV2
{
    private static readonly Field PayloadSlotField = ProtocolPayloadVisitor.Field(nameof(Slot), UInt8Type.Default, enumType: typeof(AsvGbasSlotMsg));
    private static readonly Field PayloadGbasMessageIdField = ProtocolPayloadVisitor.Field(nameof(GbasMessageId), UInt8Type.Default);
    private static readonly Field PayloadActiveSlotsField = ProtocolPayloadVisitor.Field(nameof(ActiveSlots), UInt8Type.Default, enumType: typeof(AsvGbasSlot));
    private static readonly Field PayloadLifeTimeField = ProtocolPayloadVisitor.Field(nameof(LifeTime), UInt8Type.Default);
    private static readonly Field PayloadLastByteOffsetField = ProtocolPayloadVisitor.Field(nameof(LastByteOffset), UInt8Type.Default);
    private static readonly Field PayloadReservedFlagsField = ProtocolPayloadVisitor.Field(nameof(ReservedFlags), UInt8Type.Default);
    private static readonly Field PayloadReservedFlgasField = ProtocolPayloadVisitor.Field(nameof(ReservedFlgas), UInt8Type.Default);
    private static readonly Field PayloadIsLastSlotInFrameField = ProtocolPayloadVisitor.Field(nameof(IsLastSlotInFrame), BoolType.Default);
    private static readonly Field PayloadDataField = ProtocolPayloadVisitor.Field(nameof(Data), new ListType(UInt8Type.Default, 0, int.MaxValue));

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadSlotField, PayloadGbasMessageIdField, PayloadActiveSlotsField, PayloadLifeTimeField, PayloadLastByteOffsetField, PayloadReservedFlagsField, PayloadReservedFlgasField, PayloadIsLastSlotInFrameField, PayloadDataField]);

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

        byte valueGbasMessageId = GbasMessageId;
        UInt8Type.Accept(visitor, PayloadGbasMessageIdField, PayloadGbasMessageIdField.DataType, ref valueGbasMessageId);
        if (!EqualityComparer<byte>.Default.Equals(valueGbasMessageId, GbasMessageId))
        {
            GbasMessageId = valueGbasMessageId;
        }

        AsvGbasSlot valueActiveSlots = ActiveSlots;
        var numericActiveSlots = (byte)valueActiveSlots;
        UInt8Type.Accept(visitor, PayloadActiveSlotsField, PayloadActiveSlotsField.DataType, ref numericActiveSlots);
        valueActiveSlots = (AsvGbasSlot)numericActiveSlots;
        if (!EqualityComparer<AsvGbasSlot>.Default.Equals(valueActiveSlots, ActiveSlots))
        {
            ActiveSlots = valueActiveSlots;
        }

        byte valueLifeTime = LifeTime;
        UInt8Type.Accept(visitor, PayloadLifeTimeField, PayloadLifeTimeField.DataType, ref valueLifeTime);
        if (!EqualityComparer<byte>.Default.Equals(valueLifeTime, LifeTime))
        {
            LifeTime = valueLifeTime;
        }

        byte valueLastByteOffset = LastByteOffset;
        UInt8Type.Accept(visitor, PayloadLastByteOffsetField, PayloadLastByteOffsetField.DataType, ref valueLastByteOffset);
        if (!EqualityComparer<byte>.Default.Equals(valueLastByteOffset, LastByteOffset))
        {
            LastByteOffset = valueLastByteOffset;
        }

        byte valueReservedFlags = ReservedFlags;
        UInt8Type.Accept(visitor, PayloadReservedFlagsField, PayloadReservedFlagsField.DataType, ref valueReservedFlags);
        if (!EqualityComparer<byte>.Default.Equals(valueReservedFlags, ReservedFlags))
        {
            ReservedFlags = valueReservedFlags;
        }

        byte valueReservedFlgas = ReservedFlgas;
        UInt8Type.Accept(visitor, PayloadReservedFlgasField, PayloadReservedFlgasField.DataType, ref valueReservedFlgas);
        if (!EqualityComparer<byte>.Default.Equals(valueReservedFlgas, ReservedFlgas))
        {
            ReservedFlgas = valueReservedFlgas;
        }

        bool valueIsLastSlotInFrame = IsLastSlotInFrame;
        BoolType.Accept(visitor, PayloadIsLastSlotInFrameField, PayloadIsLastSlotInFrameField.DataType, ref valueIsLastSlotInFrame);
        if (!EqualityComparer<bool>.Default.Equals(valueIsLastSlotInFrame, IsLastSlotInFrame))
        {
            IsLastSlotInFrame = valueIsLastSlotInFrame;
        }

        byte[] valueData = Data;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadDataField, PayloadDataField.DataType, ref valueData,
            static () => default(byte)!,
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!EqualityComparer<byte[]>.Default.Equals(valueData, Data))
        {
            Data = valueData;
        }
    }
}
