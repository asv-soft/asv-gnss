using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class MeasurementBlockMsg1 : SpanBitCompactSerializableBase
{
    public MeasurementBlockMsg1()
    {
    }

    public MeasurementBlockMsg1(params double[] bValues)
    {
        if (bValues.Length > B.Length) throw new ArgumentOutOfRangeException(nameof(bValues));
        for (var i = 0; i < bValues.Length; i++)
        {
            B[i] = bValues[i];
        }
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
    /// 8 bit. Standard deviation. 0–5.08 m. RESOLUTION = 0.02 m
    /// </summary>
    public double SigmaPrGnd { get; set; } = 255.0 * 0.02;

    /// <summary>
    /// 8 bits each. Four B parameters. Signed. Integrity parameter. ±6.35 m. RESOLUTION = 0.05 m
    /// </summary>
    public double[] B { get; } = { double.NaN, double.NaN, double.NaN, double.NaN };

    public override int GetBitSize()
    {
        return (7 + B.Length) * 8;
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        RangingSourceId = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        Iod = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        Prc = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 16) * 0.01;
        Rrc = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 16) * 0.001;
        SigmaPrGnd = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.02;
        for (var i = 0; i < B.Length; i++)
        {
            var b = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 8);
            if (b == -128) B[i] = double.NaN;
            else B[i] = b * 0.05;
        }
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, RangingSourceId);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Iod);
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 16, Math.Round(Prc / 0.01));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 16, Math.Round(Rrc / 0.001));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(SigmaPrGnd / 0.02));
        for (var i = 0; i < B.Length; i++)
        {
            if (double.IsNaN(B[i]))
            {
                SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 8, -128);
            }
            else
            {
                SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 8, Math.Round(B[i] / 0.05));
            }

        }
    }
}
