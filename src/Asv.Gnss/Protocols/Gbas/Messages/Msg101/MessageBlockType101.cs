using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public class MessageBlockType101 : GbasMessageBlockBase
{

    public const byte GbasMessageTypeId = 101;

    /// <summary>
    /// 5 * 10^-6
    /// </summary>
    private const double P5_10_6 = 0.000005;

    public override byte MessageTypeId => GbasMessageTypeId;

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
    public double P { get; set; }

    /// <summary>
    /// 16 bit
    /// </summary>
    public ushort EphemerisCrc { get; set; }

    public ushort? SourceAvailabilityDuration { get; set; }

    public List<MeasurementBlockMsg101> Measurements { get; } = new();

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        if (Measurements.Count > 31) throw new InvalidOperationException("A GBAS correction message supports at most 31 measurements.");
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 14, Math.Round(ModifiedZCount / 0.1));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 2, (uint)AdditionalMessageFlag);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 5, Measurements.Count);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 3, (byte)MeasurementType);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(P / P5_10_6));

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 16, EphemerisCrc);

        var sourceAvailabilityDuration = SourceAvailabilityDuration != null
            ? (byte)Math.Round(SourceAvailabilityDuration.Value / 10.0)
            : (byte)255;
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, sourceAvailabilityDuration);

        var bParametersNumber = (byte)(Measurements.All(_ => _.B.HasValue) ? 1 : 0);

        if (bParametersNumber == 0 && Measurements.Any(_ => _.B.HasValue))
        {
            throw new InvalidOperationException(
                $"{nameof(bParametersNumber)} = {bParametersNumber}, but not all measurements B value == null");
        }

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 1, bParametersNumber);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 7, (byte)0);

        for (var i = 0; i < Measurements.Count; i++)
        {
            Measurements[i].Serialize(buffer, ref bitPosition);
        }
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, int dataBitSize)
    {
        if (dataBitSize < 64) throw new FormatException("GBAS correction payload is truncated.");
        ModifiedZCount = (ushort)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 14) * 0.1;
        AdditionalMessageFlag = (AdditionalMessageEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 2);
        var measCount = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 5);
        MeasurementType = (MeasurementTypeEnum)(byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 3);
        P = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * P5_10_6;
        EphemerisCrc = (ushort)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 16);
        SourceAvailabilityDuration = (ushort)((byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 10);
        if (SourceAvailabilityDuration == 2550) SourceAvailabilityDuration = null;

        var bParametersNumber = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 1);
        bitPosition += 7; // reserved

        if (dataBitSize != 64 + measCount * (bParametersNumber == 0 ? 56 : 64))
        {
            throw new FormatException("GBAS correction count does not match its payload length.");
        }

        Measurements.Clear();
        for (var i = 0; i < measCount; i++)
        {
            var measure = bParametersNumber != 0 ? new MeasurementBlockMsg101(0.0) : new MeasurementBlockMsg101();
            measure.Deserialize(buffer, ref bitPosition);
            Measurements.Add(measure);
        }
    }

    public override int GetBitSize()
    {
        return base.GetBitSize() + 64 + Measurements.Sum(_ => _.GetBitSize());
    }
}
