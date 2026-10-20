using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public class MessageBlockType11 : GbasMessageBlockBase
{
    public const byte GbasMessageTypeId = 11;

    /// <summary>
    /// 5 * 10^-6
    /// </summary>
    private const double P5_10_6 = 0.000005;

    public override byte MessageTypeId => GbasMessageTypeId;

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        if (Measurements.Count > 31) throw new InvalidOperationException("A GBAS correction message supports at most 31 measurements.");
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 14, Math.Round(ModifiedZCount / 0.1));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 2, (uint)AdditionalMessageFlag);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 5, (byte)Measurements.Count);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 3, (byte)MeasurementType);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(PD / P5_10_6));

        foreach (var t in Measurements)
        {
            t.Serialize(buffer, ref bitPosition);
        }
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, int dataBitSize)
    {
        if (dataBitSize < 32) throw new FormatException("GBAS correction payload is truncated.");
        ModifiedZCount = (ushort)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 14) * 0.1;
        AdditionalMessageFlag = (AdditionalMessageEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 2);
        var measNum = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 5);
        MeasurementType = (MeasurementTypeEnum)(byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 3);
        PD = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * P5_10_6;

        if (dataBitSize != 32 + measNum * 56)
        {
            throw new FormatException("GBAS correction count does not match its payload length.");
        }

        Measurements.Clear();
        for (var i = 0; i < measNum; i++)
        {
            var measurements = new MeasurementBlockD();
            measurements.Deserialize(buffer, ref bitPosition);
            Measurements.Add(measurements);
        }

    }

    public override int GetBitSize()
    {
        return base.GetBitSize() + 32 + Measurements.Sum(_ => _.GetBitSize());
    }

    /// <summary>
    /// 14 bit. 0 to 1 199.9 s. RESOLUTION = 0.1 s
    /// </summary>
    public double ModifiedZCount { get; set; }

    /// <summary>
    /// 2 bit. 0 - All in one, 1 - first of two, 2 - not reserved, 3 - second of two
    /// </summary>
    public AdditionalMessageEnum AdditionalMessageFlag { get; set; }

    /// <summary>
    /// 3 bit
    /// </summary>
    public MeasurementTypeEnum MeasurementType { get; set; }

    /// <summary>
    /// 8 bit, Ephemeris decorrelation parameter. 0 to 1.275 × 10^–3 m/m. RESOLUTION = 5 × 10^–6m/m
    /// </summary>
    public double PD { get; set; }

    public List<MeasurementBlockD> Measurements { get; } = new();
}
