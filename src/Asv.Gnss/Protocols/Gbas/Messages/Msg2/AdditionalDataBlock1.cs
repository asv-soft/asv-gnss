using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdditionalDataBlock1 : AdditionalDataBlock
{
    public const byte BlockNumber = 1;

    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        InternalDeserialize(buffer, ref bitPosition, (byte)GetByteSize());
    }

    public override int GetBitSize()
    {
        // Block 1 is untagged: it has no length or type fields.
        return 6 * 8;
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        InternalSerialize(buffer, ref bitPosition);
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, RSDS);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(DMax / 2.0));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, (byte)Math.Round(Kmd_e_POS_GPS / 0.05));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, (byte)Math.Round(Kmd_e_GPS / 0.05));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, (byte)Math.Round(Kmd_e_POS_GLONASS / 0.05));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, (byte)Math.Round(Kmd_e_GLONASS / 0.05));
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, byte byteSize)
    {
        RSDS = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        DMax = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 2u;
        Kmd_e_POS_GPS = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.05;
        Kmd_e_GPS = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.05;
        Kmd_e_POS_GLONASS = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.05;
        Kmd_e_GLONASS = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.05;
    }

    public override byte TypeNumber => BlockNumber;

    public byte RSDS { get; set; } = 255;

    public uint DMax { get; set; }

    public double Kmd_e_POS_GPS { get; set; }

    public double Kmd_e_GPS { get; set; }

    public double Kmd_e_POS_GLONASS { get; set; }

    public double Kmd_e_GLONASS { get; set; }
}
