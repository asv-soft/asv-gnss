using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public class MessageBlockType2 : GbasMessageBlockBase
{
    public const byte GbasMessageTypeId = 2;

    /// <summary>
    /// 1 * 10^-7
    /// </summary>
    private const double P1_10_7 = 0.0000001;

    /// <summary>
    /// 0.0005 arcsecond
    /// </summary>
    private const double ArcSecond = 1.388888888888889e-7;

    public override byte MessageTypeId => GbasMessageTypeId;

    public byte GbasReferenceReceivers { get; set; }

    public GADLEnum GroundAccuracyDesignatorLetter { get; set; }

    public GCIDEnum GbasDesignator { get; set; }

    public double LocalMagneticVariation { get; set; }

    public double Sigma_vert_iono_gradient { get; set; }

    public ushort RefractivityIndex { get; set; }

    public ushort ScaleHeight { get; set; }

    public byte RefractivityUncertainty { get; set; }

    /// <summary>
    /// 32 bit. ±90.0°. Resolution = 0.0005 arcsec
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// 32 bit. ±90.0°. Resolution = 0.0005 arcsec
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// 24 bit. ±83 886.07 m. Resolution = 0.01 m
    /// </summary>
    public double EllipsoidHeight { get; set; }

    public List<AdditionalDataBlock> AdditionalData { get; } = new();

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 2, GbasReferenceReceivers);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 2, (byte)GroundAccuracyDesignatorLetter);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 1, 0);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 3, (byte)GbasDesignator);
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 11, Math.Round(LocalMagneticVariation / 0.25));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 5, 0);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(Sigma_vert_iono_gradient / P1_10_7));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 8, Math.Round((RefractivityIndex - 400.0) / 3.0));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, (byte)Math.Round(ScaleHeight / 100.0));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, RefractivityUncertainty);
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 32, Math.Round(Latitude / ArcSecond));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 32, Math.Round(Longitude / ArcSecond));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 24, Math.Round(EllipsoidHeight / 0.01));

        if (AdditionalData.Count > 0 && (AdditionalData[0] is not AdditionalDataBlock1 ||
            AdditionalData.Skip(1).Any(block => block is AdditionalDataBlock1)))
        {
            throw new InvalidOperationException("Additional data block 1 must appear first and only once.");
        }

        foreach (var additionalDataBlock in AdditionalData)
        {
            additionalDataBlock.Serialize(buffer, ref bitPosition);
        }
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, int dataBitSize)
    {
        if (dataBitSize < 144) throw new FormatException("GBAS type 2 payload is truncated.");
        var originBitPosition = bitPosition;
        AdditionalData.Clear();
        GbasReferenceReceivers = ((byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 2));
        GroundAccuracyDesignatorLetter = (GADLEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 2);
        bitPosition += 1; // reserved
        GbasDesignator = (GCIDEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 3);
        LocalMagneticVariation = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 11) * 0.25;
        bitPosition += 5; // reserved
        Sigma_vert_iono_gradient = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * P1_10_7;
        RefractivityIndex = (ushort)(SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 8) * 3 + 400);
        ScaleHeight = (ushort)(SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 100u);
        RefractivityUncertainty = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        Latitude = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 32) * ArcSecond;
        Longitude = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 32) * ArcSecond;
        EllipsoidHeight = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 24) * 0.01;

        // if have any data
        if (dataBitSize - bitPosition + originBitPosition > 0)
        {
            var block1 = new AdditionalDataBlock1();
            AdditionalData.Add(block1);
            block1.Deserialize(buffer, ref bitPosition);
        }

        while (dataBitSize - bitPosition + originBitPosition > 0)
        {
            var startBlockPointer = bitPosition;
            startBlockPointer += 8; // skip block length
            var blockId = (byte)SpanBitHelper.GetBitUReverse(buffer, ref startBlockPointer, 8);
            var block = GbasMessageBlockFactory.CreateAdditionalDataBlockFromMsg2(blockId);
            block.Deserialize(buffer, ref bitPosition);
            AdditionalData.Add(block);
        }
    }

    public override int GetBitSize()
    {
        return base.GetBitSize() + 18 * 8 + AdditionalData.Sum(_ => _.GetBitSize());
    }
}

public enum GADLEnum : byte
{
    A = 0,
    B = 1,
    C = 2,
    Spare = 3
}

public enum GCIDEnum : byte
{
    Spare1 = 0,
    GCID1 = 1,
    GCID2 = 2,
    GCID3 = 3,
    GCID4 = 4,
    Spare2 = 5,
    Spare3 = 6,
    Unhealthy = 7,
}
