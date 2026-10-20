using System;
using Asv.IO;

namespace Asv.Gnss;

public class MeasurementBlockMsg101 : SpanBitCompactSerializableBase
{
    public MeasurementBlockMsg101()
    {

    }

    public MeasurementBlockMsg101(double bValue)
    {
        B = bValue;
    }

    /// <summary>
    /// 8 bit. Satellite Prn
    /// </summary>
    public byte RangingSourceId { get; set; }

    public byte Iod { get; set; }

    /// <summary>
    /// 16 bit. Signed. Pseudo-range correction. ±327.67 m. RESOLUTION = 0.01 m
    /// </summary>
    public double Prc { get; set; }

    /// <summary>
    /// 16 bit. Signed. Range rate correction. ±32.767 m. RESOLUTION = 0.001 m
    /// </summary>
    public double Rrc { get; set; }

    /// <summary>
    /// 8 bit. Standard deviation. 0–5,08 m (type1)/0–50,8 m (type101). RESOLUTION = 0,02 m (type1)/0,2 m (type101)
    /// </summary>
    public double SigmaPrGnd { get; set; } = 255.0 * 0.2;

    public double? B { get; private set; }

    public override int GetBitSize()
    {
        return (7 + (B.HasValue ? 1 : 0)) * 8;
    }
    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        RangingSourceId = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        Iod = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        Prc = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 16) * 0.01;
        Rrc = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 16) * 0.001;
        SigmaPrGnd = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.2;

        if (B.HasValue)
        {
            var b = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 8);
            if (b == -128)
            {
                B = double.NaN;
            }
            else
            {
                B = b * 0.05;
            }
        }
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, RangingSourceId);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Iod);
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 16, Math.Round(Prc / 0.01));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 16, Math.Round(Rrc / 0.001));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(SigmaPrGnd / 0.2));
        if (B.HasValue)
        {
            if (double.IsNaN(B.Value))
            {
                SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 8, -128);
            }
            else
            {
                SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 8, Math.Round(B.Value / 0.05));
            }
        }
    }
}
