using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessageGbasVdbSend
{
    private static readonly Field PayloadDataField = ProtocolPayloadVisitor.Field(nameof(Data), new ListType(UInt8Type.Default, 0, int.MaxValue));
    private static readonly Field PayloadLastByteLengthField = ProtocolPayloadVisitor.Field(nameof(LastByteLength), UInt8Type.Default);
    private static readonly Field PayloadMsgsField = ProtocolPayloadVisitor.Field(nameof(Msgs), UInt64Type.Default, enumType: typeof(AsvGbasMessage));
    private static readonly Field PayloadSlotField = ProtocolPayloadVisitor.Field(nameof(Slot), UInt8Type.Default, enumType: typeof(AsvGbasSlot));

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadDataField, PayloadLastByteLengthField, PayloadMsgsField, PayloadSlotField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

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

        byte valueLastByteLength = LastByteLength;
        UInt8Type.Accept(visitor, PayloadLastByteLengthField, PayloadLastByteLengthField.DataType, ref valueLastByteLength);
        if (!EqualityComparer<byte>.Default.Equals(valueLastByteLength, LastByteLength))
        {
            LastByteLength = valueLastByteLength;
        }

        AsvGbasMessage valueMsgs = Msgs;
        var numericMsgs = (ulong)valueMsgs;
        UInt64Type.Accept(visitor, PayloadMsgsField, PayloadMsgsField.DataType, ref numericMsgs);
        valueMsgs = (AsvGbasMessage)numericMsgs;
        if (!EqualityComparer<AsvGbasMessage>.Default.Equals(valueMsgs, Msgs))
        {
            Msgs = valueMsgs;
        }

        AsvGbasSlot valueSlot = Slot;
        var numericSlot = (byte)valueSlot;
        UInt8Type.Accept(visitor, PayloadSlotField, PayloadSlotField.DataType, ref numericSlot);
        valueSlot = (AsvGbasSlot)numericSlot;
        if (!EqualityComparer<AsvGbasSlot>.Default.Equals(valueSlot, Slot))
        {
            Slot = valueSlot;
        }
    }
}
