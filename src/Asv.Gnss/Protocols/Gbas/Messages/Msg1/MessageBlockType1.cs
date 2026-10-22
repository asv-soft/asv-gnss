using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public enum MeasurementTypeEnum : byte
{
    CaCodeL1 = 0,
    Reserved1 = 1,
    Reserved2 = 2,
    Reserved3 = 3,
    NotReserved1 = 4,
    NotReserved2 = 5,
    NotReserved3 = 6,
    NotReserved4 = 7,
}
public enum AdditionalMessageEnum
{
    AllInOne = 0,
    FirstOfTwo = 1,
    NotReserved = 2,
    SecondOfTwo = 3
}


public partial class MessageBlockType1 : GbasMessageBlockBase
{
    /// <summary>
    /// 5 * 10^-6
    /// </summary>
    private const double P5_10_6 = 0.000005;

    public const byte GbasMessageTypeId = 1;

    public override int GetBitSize()
    {
        return base.GetBitSize() + 56 + Measurements.Sum(_ => _.GetBitSize());
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        if (Measurements.Count > 31) throw new InvalidOperationException("A GBAS correction message supports at most 31 measurements.");
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 14, Math.Round(ModifiedZCount / 0.1));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 2, (uint)AdditionalMessageFlag);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 5, (byte)(Measurements.Count));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 3, (byte)MeasurementType);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(P / P5_10_6));

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, EphemerisCrc >> 8);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, EphemerisCrc & 0xFF);

        var sourceAvailabilityDuration = SourceAvailabilityDuration != null
            ? (byte)Math.Round(SourceAvailabilityDuration.Value / 10.0)
            : (byte)255;
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, sourceAvailabilityDuration);

        if (Measurements is { Count: > 0 })
        {
            var bParamNum4 = Measurements.Count(_ => _.B.Length == 4);

            if (bParamNum4 != Measurements.Count)
            {
                throw new InvalidOperationException(
                    "It is impossible to serialize application data of type 1. The number of B parameters must be equal to 4");
            }

            foreach (var t in Measurements)
            {
                t.Serialize(buffer, ref bitPosition);
            }
        }
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, int dataBitSize)
    {
        if (dataBitSize < 56) throw new FormatException("GBAS correction payload is truncated.");
        ModifiedZCount = (ushort)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 14) * 0.1;
        AdditionalMessageFlag = (AdditionalMessageEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 2);
        var measurementsNumber = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 5);
        MeasurementType = (MeasurementTypeEnum)(byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 3);
        P = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * P5_10_6;
        var msb = (ushort)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        var lsb = (ushort)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        EphemerisCrc = (ushort)((msb << 8) + lsb);
        SourceAvailabilityDuration = (ushort)((byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 10);
        if (SourceAvailabilityDuration == 2550) SourceAvailabilityDuration = null;
        if (dataBitSize != 56 + measurementsNumber * 88)
        {
            throw new FormatException("GBAS correction count does not match its payload length.");
        }

        Measurements.Clear();
        for (var i = 0; i < measurementsNumber; i++)
        {
            var measurements = new MeasurementBlockMsg1();
            Measurements.Add(measurements);
            measurements.Deserialize(buffer, ref bitPosition);
        }
    }

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

    public List<MeasurementBlockMsg1> Measurements { get; } = new();

}
