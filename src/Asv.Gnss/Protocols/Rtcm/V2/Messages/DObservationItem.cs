using System;
using Asv.IO;

namespace Asv.Gnss;

public class DObservationItem
{
    private readonly NavigationSystemEnum _system;
    private bool _hasOriginalCorrections;
    private bool _originalLargeScale;
    private short _originalPrc;
    private sbyte _originalRrc;
    private byte _originalIod;

    public DObservationItem(NavigationSystemEnum system)
    {
        _system = system;
    }

    public int SatelliteId { get; set; }
    public byte Prn { get; set; }
    public double Prc { get; set; }
    public double Rrc { get; set; }
    public byte Iod { get; set; }
    public SatUdreEnum Udre { get; set; }
    public DateTime Tk { get; set; }

    public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        var fact = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        var udre = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
        var prn = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
        var prc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16);
        var rrc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 8);
        var iod = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
        _hasOriginalCorrections = true;
        _originalLargeScale = fact != 0;
        _originalPrc = (short)prc;
        _originalRrc = (sbyte)rrc;
        _originalIod = iod;

        if (prn == 0)
        {
            prn = 32;
        }

        Prn = prn;
        if (prc == -32768 || rrc == -128)
        {
            Prc = double.NaN;
            Rrc = double.NaN;
        }
        else
        {
            Prc = prc * (fact == 1 ? 0.32 : 0.02);
            Rrc = rrc * (fact == 1 ? 0.032 : 0.002);
        }

        SatelliteId = RtcmV3Protocol.satno(_system, prn);
        Iod = _system == NavigationSystemEnum.SYS_GLO ? (byte)(iod & 0x7F) : iod;
        if (_system == NavigationSystemEnum.SYS_GLO)
        {
            Tk = GetDateTime((uint)(Iod * 30));
        }

        Udre = GetUdre(udre);
    }

    /// <summary>Writes a 40-bit satellite correction, retaining the decoded scale and invalid-value representation when possible.</summary>
    public void Serialize(Span<byte> buffer, ref int bitIndex)
    {
        if (Prn is < 1 or > 32)
        {
            throw new ArgumentOutOfRangeException(nameof(Prn), "A satellite correction requires PRN 1 through 32.");
        }
        if (_system == NavigationSystemEnum.SYS_GLO && Iod > 127)
        {
            throw new ArgumentOutOfRangeException(nameof(Iod), "The GLONASS issue-of-data value must fit seven bits.");
        }
        if (double.IsInfinity(Prc) || double.IsInfinity(Rrc))
        {
            throw new ArgumentOutOfRangeException(nameof(Prc), "Corrections must be finite or NaN for an unavailable correction.");
        }

        var largeScale = _hasOriginalCorrections && _originalLargeScale;
        double prc;
        double rrc;
        if (double.IsNaN(Prc) || double.IsNaN(Rrc))
        {
            if (_hasOriginalCorrections && (_originalPrc == short.MinValue || _originalRrc == sbyte.MinValue))
            {
                prc = _originalPrc;
                rrc = _originalRrc;
            }
            else
            {
                prc = short.MinValue;
                rrc = sbyte.MinValue;
            }
        }
        else
        {
            if (!FitsScale(Prc, Rrc, largeScale))
            {
                largeScale = !largeScale;
                if (!FitsScale(Prc, Rrc, largeScale))
                {
                    throw new ArgumentOutOfRangeException(nameof(Prc), "Corrections exceed the RTCM2 scale ranges.");
                }
            }
            prc = Math.Round(Prc / (largeScale ? 0.32 : 0.02), MidpointRounding.AwayFromZero);
            rrc = Math.Round(Rrc / (largeScale ? 0.032 : 0.002), MidpointRounding.AwayFromZero);
        }

        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 1, largeScale ? 1 : 0);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 2, (int)Udre);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 5, Prn == 32 ? 0 : Prn);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, prc);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 8, rrc);
        var iod = _system == NavigationSystemEnum.SYS_GLO ? (byte)(Iod | (_originalIod & 0x80)) : Iod;
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 8, iod);
    }

    private static bool FitsScale(double prc, double rrc, bool largeScale)
    {
        var rawPrc = Math.Round(prc / (largeScale ? 0.32 : 0.02), MidpointRounding.AwayFromZero);
        var rawRrc = Math.Round(rrc / (largeScale ? 0.032 : 0.002), MidpointRounding.AwayFromZero);
        return rawPrc is >= -32767 and <= 32767 && rawRrc is >= -127 and <= 127;
    }

    private static SatUdreEnum GetUdre(byte udre)
    {
        return udre switch
        {
            0 => SatUdreEnum.LessOne,
            1 => SatUdreEnum.BetweenOneAndFour,
            2 => SatUdreEnum.BetweenFourAndEight,
            3 => SatUdreEnum.MoreEight,
            _ => throw new ArgumentOutOfRangeException(nameof(udre))
        };
    }

    private static DateTime GetDateTime(uint tb)
    {
        var utc = DateTime.UtcNow;
        var week = 0;
        var tow = 0.0;
        RtcmV3Protocol.GetFromTime(utc, ref week, ref tow);
        var toe = (double)tb;
        var toh = tow % 3600.0;
        tow -= toh;

        if (toe < toh - 1800.0) toe += 3600.0;
        else if (toe > toh + 1800.0) toe -= 3600.0;

        return RtcmV3Protocol.GetFromGps(week, tow + toe).AddHours(3.0);
    }
}

public enum SatUdreEnum
{
    LessOne = 0,
    BetweenOneAndFour = 1,
    BetweenFourAndEight = 2,
    MoreEight = 3
}
