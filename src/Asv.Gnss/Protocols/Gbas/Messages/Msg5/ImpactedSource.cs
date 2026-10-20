using System;
using Asv.IO;

namespace Asv.Gnss;

public enum SourceAvailabilitySenseEnum : byte
{
    WillCease = 0,
    WillStart = 1
}

public class ImpactedSource : SpanBitCompactSerializableBase
{
    public byte RangingSourceId { get; set; }

    public SourceAvailabilitySenseEnum SourceAvailabilitySense { get; set; }

    public ushort SourceAvailabilityDuration { get; set; }

    public override int GetBitSize()
    {
        return 2 * 8;
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        RangingSourceId = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        SourceAvailabilitySense = (SourceAvailabilitySenseEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 1);
        SourceAvailabilityDuration = (ushort)(SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 7) * 10);
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, RangingSourceId);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 1, (byte)SourceAvailabilitySense);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 7, Math.Round(SourceAvailabilityDuration / 10.0));
    }
}
