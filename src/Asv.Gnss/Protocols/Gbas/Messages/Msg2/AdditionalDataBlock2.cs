using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdditionalDataBlock2 : AdditionalDataBlock
{
    public const byte BlockNumber = 2;

    public override int GetBitSize()
    {
        return base.GetBitSize() + 4 * 8;
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 16, ChannelNumber);
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 8, Math.Round(DeltaLatitude / 0.2));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 8, Math.Round(DeltaLongitude / 0.2));
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, byte byteSize)
    {
        ChannelNumber = (ushort)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 16);
        DeltaLatitude = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 8) * 0.2;
        DeltaLongitude = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 8) * 0.2;
    }

    public override byte TypeNumber => BlockNumber;

    /// <summary>
    /// 16 bit. 20001 to 39999
    /// </summary>
    public ushort ChannelNumber { get; set; }

    /// <summary>
    /// 8 bit. ±25.4°. Resolution = 0.2°
    /// </summary>
    public double DeltaLatitude { get; set; }

    /// <summary>
    /// 8 bit. ±25.4°. Resolution = 0.2°
    /// </summary>
    public double DeltaLongitude { get; set; }
}
