using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdditionalDataBlock4 : AdditionalDataBlock
{
    public const byte BlockNumber = 4;

    public override int GetBitSize()
    {
        return base.GetBitSize() + 8;
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        byte slots = 0;
        foreach (var slotId in SlotGroupDefinition)
        {
            var slot = (byte)slotId;
            if (slot > 7) throw new ArgumentOutOfRangeException(nameof(SlotGroupDefinition));
            slots |= (byte)(1u << slot);
        }

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, slots);
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, byte byteSize)
    {
        var slots = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        SlotGroupDefinition.Clear();
        for (var i = 0; i < 8; i++)
        {
            if (((slots >> i) & 0x1) != 0) SlotGroupDefinition.Add((SlotIdEnum)i);
        }
    }

    public override byte TypeNumber => BlockNumber;

    public List<SlotIdEnum> SlotGroupDefinition { get; } = new();
}
