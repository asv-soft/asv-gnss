using System;
using Asv.IO;

namespace Asv.Gnss;

public enum SBASServiceProviderEnum : byte
{
    WAAS = 0,
    EGNOS = 1,
    MSAS = 2,
    GAGAN = 3,
    SDCM = 4,
    OnlyGBAS = 14,
    AnySBAS = 15
}

public partial class FasDataSet : SpanBitCompactSerializableBase
{
    /// <summary>
    /// 0.0005 arcsecond
    /// </summary>
    private const double ArcSecond = 1.388888888888889e-7;

    public byte OperationType { get; set; }

    public SBASServiceProviderEnum SbasServiceProvider { get; set; }

    /// <summary>
    /// 32 bit. Airport ID
    /// </summary>
    public string AirportId { get; set; } = string.Empty;

    public byte RunwayNumber { get; set; }

    /// <summary>
    /// 2 bit.
    /// </summary>
    public RunwayLetterEnum RunwayLetter { get; set; }

    /// <summary>
    /// 3 bit.
    /// </summary>
    public ApproachPerformanceDesignatorEnum ApproachPerformanceDesignator { get; set; }

    /// <summary>
    /// 5 bit.
    /// </summary>
    public RouteIndicatorEnum RouteIndicator { get; set; }

    public byte RPDS { get; set; }

    public string ReferencePathIdentifier { get; set; } = string.Empty;

    public double LtpFtpLatitude { get; set; }

    public double LtpFtpLongitude { get; set; }

    public double LtpFtpHeight { get; set; }

    public double DeltaFPapLatitude { get; set; }

    public double DeltaFPapLongitude { get; set; }

    public double TCH { get; set; }

    public TCHUnitsEnum TCHUnits { get; set; }

    /// <summary>
    /// 16 bit. 0 to 90°. Resolution = 0.01°
    /// </summary>
    public double GlidePathAngle { get; set; }

    /// <summary>
    /// 8 bit. 80.0 to 143.75 m. Resolution = 0.25 m
    /// </summary>
    public double CourseWidth { get; set; }

    public double DeltaLengthOffset { get; set; } = 2040;

    public double FASVAL { get; set; }

    public double FASLAL { get; set; }

    public override int GetBitSize()
    {
        return 41 * 8;
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        var length =
            SpanBitHelper.GetBitUReverse(buffer, ref bitPosition,
                8);
        if (length != GetByteSize()) throw new FormatException($"Invalid FAS data set length: {length}.");
        var crcStart = bitPosition;
        var crcLength = length - 1 /*FAST LENGTH*/ - 4 /*CRC*/ - 1 /*FASVAL*/ - 1 /*FASLAL*/;
        var crc = GbasMessageBlockBase.Crc32QCalc(buffer, crcStart, (int)crcLength * 8);
        var crcPointer = bitPosition + 34 * 8;
        var originCrc = SpanBitHelper.GetBitUReverse(buffer, ref crcPointer, 32);
        if (crc != originCrc)
        {
            throw new FormatException($"FAS CRC mismatch: want {crc}, got {originCrc}");
        }

        OperationType = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 4);
        SbasServiceProvider = (SBASServiceProviderEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 4);
        var arr = new byte[4];
        for (var i = 3; i >= 0; i--)
        {
            arr[i] = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        }

        AirportId = GbasMessageBlockBase.DecodeIA5(arr);

        RunwayNumber = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 6);
        RunwayLetter = (RunwayLetterEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 2);
        ApproachPerformanceDesignator =
            (ApproachPerformanceDesignatorEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 3);
        RouteIndicator = (RouteIndicatorEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 5);
        RPDS = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        for (var i = 3; i >= 0; i--)
        {
            arr[i] = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        }

        ReferencePathIdentifier = GbasMessageBlockBase.DecodeIA5(arr);

        LtpFtpLatitude = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 32) * ArcSecond;
        LtpFtpLongitude = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 32) * ArcSecond;
        LtpFtpHeight = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 16) * 0.1 - 512;

        DeltaFPapLatitude = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 24) * ArcSecond;
        DeltaFPapLongitude = SpanBitHelper.GetBitSReverse(buffer, ref bitPosition, 24) * ArcSecond;

        var rawTch = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 15);
        TCHUnits = (TCHUnitsEnum)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 1);
        TCH = rawTch * (TCHUnits == TCHUnitsEnum.M ? 0.05 : 0.1);

        GlidePathAngle = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 16) * 0.01;
        CourseWidth = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.25 + 80.0;
        DeltaLengthOffset = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 8.0;

        bitPosition += 32; // CRC

        FASVAL = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.1;
        FASLAL = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.2;
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        var originBitPos = bitPosition;
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8,
            GetByteSize());
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 4, OperationType);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 4, (byte)SbasServiceProvider);

        var airportId = GbasMessageBlockBase.EncodeIA5(AirportId, 4);
        for (var i = 3; i >= 0; i--)
        {
            SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, airportId[i]);
        }

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 6, RunwayNumber);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 2, (byte)RunwayLetter);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 3, (byte)ApproachPerformanceDesignator);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 5, (uint)RouteIndicator);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, RPDS);

        var referencePathIdentifier = GbasMessageBlockBase.EncodeIA5(ReferencePathIdentifier, 4);
        for (var i = 3; i >= 0; i--)
        {
            SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, referencePathIdentifier[i]);
        }

        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 32, Math.Round(LtpFtpLatitude / ArcSecond));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 32, Math.Round(LtpFtpLongitude / ArcSecond));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 16, Math.Round((LtpFtpHeight + 512.0) / 0.1));

        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 24, Math.Round(DeltaFPapLatitude / ArcSecond));
        SpanBitHelper.SetBitSReverse(buffer, ref bitPosition, 24, Math.Round(DeltaFPapLongitude / ArcSecond));

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 15,
            Math.Round(TCH / (TCHUnits == TCHUnitsEnum.M ? 0.05 : 0.1)));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 1, (byte)TCHUnits);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 16, Math.Round(GlidePathAngle / 0.01));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round((CourseWidth - 80.0) / 0.25));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(DeltaLengthOffset / 8.0));

        var crcStartPointer = originBitPos + 8 /*skip FAST length*/;
        var crcLength = bitPosition - crcStartPointer;
        var crc = GbasMessageBlockBase.Crc32QCalc(buffer, crcStartPointer, crcLength);

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 32, crc);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(FASVAL / 0.1));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, Math.Round(FASLAL / 0.2));
    }
}
