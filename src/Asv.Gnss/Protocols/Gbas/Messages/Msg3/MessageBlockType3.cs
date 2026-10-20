using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public class MessageBlockType3 : GbasMessageBlockBase
{
    public const byte GbasMessageTypeId = 3;

    public const byte FilterValue = 0xAA;

    public override byte MessageTypeId => GbasMessageTypeId;

    public List<byte> Filter { get; } = new();

    public override int GetBitSize()
    {
        return base.GetBitSize() + Filter.Count * 8;
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        foreach (var t in Filter)
        {
            SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, t);
        }
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, int dataBitSize)
    {
        var filterLength = dataBitSize / 8;

        Filter.Clear();
        for (var i = 0; i < filterLength; i++)
        {
            Filter.Add((byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8));
        }
    }
}
