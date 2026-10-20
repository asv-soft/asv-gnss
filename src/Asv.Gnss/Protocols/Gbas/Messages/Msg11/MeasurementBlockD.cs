using System;
using Asv.IO;

namespace Asv.Gnss;

public class MeasurementBlockD : SpanBitCompactSerializableBase
{
    public MeasurementBlockD()
    {
        SigmaPrGndD = 255.0 * 0.02;
        SigmaPrGndD30 = 255.0 * 0.02;
    }

    /// <summary>
    /// 8 bit. Satellite Prn
    /// </summary>
    public byte RangingSourceId { get; set; }

    /// <summary>
    /// 16 bit. Signed. Pseudo-range correction. ±327.67 m. RESOLUTION = 0.01 m
    /// </summary>
    public double Prc30 { get; set; }

    /// <summary>
    /// 16 bit. Signed. Range rate correction. ±32.767 m. RESOLUTION = 0.001 m
    /// </summary>
    public double Rrc30 { get; set; }

    /// <summary>
    /// 8 bit. Standard deviation. 0–5,08 m. RESOLUTION = 0,02 m
    /// </summary>
    public double SigmaPrGndD { get; set; }

    /// <summary>
    /// 8 bit. Standard deviation. 0–5,08 m. RESOLUTION = 0,02 m
    /// </summary>
    public double SigmaPrGndD30 { get; set; }

    public override int GetBitSize()
    {
        return 7 * 8;
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        RangingSourceId = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);

        Prc30 = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 16) * 0.01;
        Rrc30 = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 16) * 0.001;

        SigmaPrGndD = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.02;
        SigmaPrGndD30 = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.02;
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, RangingSourceId);
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 16, Math.Round(Prc30 / 0.01));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 16, Math.Round(Rrc30 / 0.001));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(SigmaPrGndD / 0.02));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(SigmaPrGndD30 / 0.02));
    }
}
