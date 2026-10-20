using System;
using Asv.IO;

namespace Asv.Gnss;

public class RtcmV2Message17 : RtcmV2MessageBase
{
    public const int RtcmMessageId = 17;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "GPS ephemeris message";

    public int Satellite { get; set; }
    public uint WeekNumberRaw { get; set; }
    public int WeekNumber { get; set; }
    public double Idot { get; set; }
    public uint Iode { get; set; }
    public DateTime Toc { get; set; }
    public double AF1 { get; set; }
    public double AF2 { get; set; }
    public double Crs { get; set; }
    public double DeltaN { get; set; }
    public double Cuc { get; set; }
    public double E { get; set; }
    public short Cus { get; set; }
    public double A { get; set; }
    public ushort Toes { get; set; }
    public DateTime Toe { get; set; }
    public double Omega0 { get; set; }
    public double Cic { get; set; }
    public double I0 { get; set; }
    public double Cis { get; set; }
    public double Omega { get; set; }
    public double Crc { get; set; }
    public double OmegaDot { get; set; }
    public double M0 { get; set; }
    public ushort Iodc { get; set; }
    public double AF0 { get; set; }
    public double Tgd { get; set; }
    public byte CodeOnL2 { get; set; }
    public byte SVAccuracy { get; set; }
    public byte SVHealth { get; set; }
    public byte L2PDataFlag { get; set; }

    /// <inheritdoc />
    protected override int GetContentBitSize() => 480;

    /// <inheritdoc />
    protected override void SerializeContent(Span<byte> buffer, ref int bitIndex)
    {
        if (Satellite is < 1 or > 32)
        {
            throw new ArgumentOutOfRangeException(nameof(Satellite), "A GPS satellite PRN must be between 1 and 32.");
        }
        if (!double.IsFinite(A) || A < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(A), "The semi-major axis must be finite and nonnegative.");
        }
        if (Toc < new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc))
        {
            throw new ArgumentOutOfRangeException(nameof(Toc), "GPS time must not precede the GPS epoch.");
        }

        var tocWeek = 0;
        var tocSeconds = 0.0;
        RtcmV3Protocol.GetFromTime(Toc, ref tocWeek, ref tocSeconds);
        if (tocSeconds > 604784.0)
        {
            throw new ArgumentOutOfRangeException(nameof(Toc), "The clock reference time must not exceed 604784 seconds of the GPS week.");
        }

        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 10, WeekNumberRaw);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 14, Idot, RtcmV3Protocol.P2_43 * RtcmV3Protocol.SC2RAD);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 8, Iode);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 16, tocSeconds, 16.0);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, AF1, RtcmV3Protocol.P2_43);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 8, AF2, RtcmV3Protocol.P2_55);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, Crs, RtcmV3Protocol.P2_5);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, DeltaN, RtcmV3Protocol.P2_43 * RtcmV3Protocol.SC2RAD);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, Cuc, RtcmV3Protocol.P2_29);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 32, E, RtcmV3Protocol.P2_33);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, Cus);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 32, Math.Sqrt(A), RtcmV3Protocol.P2_19);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 16, Toes);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 32, Omega0, RtcmV3Protocol.P2_31 * RtcmV3Protocol.SC2RAD);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, Cic, RtcmV3Protocol.P2_29);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 32, I0, RtcmV3Protocol.P2_31 * RtcmV3Protocol.SC2RAD);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, Cis, RtcmV3Protocol.P2_29);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 32, Omega, RtcmV3Protocol.P2_31 * RtcmV3Protocol.SC2RAD);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 16, Crc, RtcmV3Protocol.P2_5);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 24, OmegaDot, RtcmV3Protocol.P2_43 * RtcmV3Protocol.SC2RAD);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 32, M0, RtcmV3Protocol.P2_31 * RtcmV3Protocol.SC2RAD);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 10, Iodc);
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 22, AF0, RtcmV3Protocol.P2_31);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 5, Satellite == 32 ? 0 : Satellite);
        bitIndex += 3;
        RtcmV2Serialization.WriteSigned(buffer, ref bitIndex, 8, Tgd, RtcmV3Protocol.P2_31);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 2, CodeOnL2);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 4, SVAccuracy);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 6, SVHealth);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 1, L2PDataFlag);
        bitIndex += 3;
    }

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        if (payloadLength != 60)
        {
            throw new ArgumentException("A type 17 ephemeris payload must contain exactly 20 data words.", nameof(payloadLength));
        }

        var week = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
        WeekNumberRaw = week;
        Idot = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14) * RtcmV3Protocol.P2_43 * RtcmV3Protocol.SC2RAD;
        Iode = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
        var toc = SpanBitHelper.GetBitU(buffer, ref bitIndex, 16) * 16.0;
        AF1 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Protocol.P2_43;
        AF2 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 8) * RtcmV3Protocol.P2_55;
        Crs = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Protocol.P2_5;
        DeltaN = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Protocol.P2_43 * RtcmV3Protocol.SC2RAD;
        Cuc = (short)SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Protocol.P2_29;
        E = SpanBitHelper.GetBitU(buffer, ref bitIndex, 32) * RtcmV3Protocol.P2_33;
        Cus = (short)SpanBitHelper.GetBitS(buffer, ref bitIndex, 16);
        var sqrtA = SpanBitHelper.GetBitU(buffer, ref bitIndex, 32) * RtcmV3Protocol.P2_19;
        Toes = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 16);
        Omega0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Protocol.P2_31 * RtcmV3Protocol.SC2RAD;
        Cic = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Protocol.P2_29;
        I0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Protocol.P2_31 * RtcmV3Protocol.SC2RAD;
        Cis = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Protocol.P2_29;
        Omega = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Protocol.P2_31 * RtcmV3Protocol.SC2RAD;
        Crc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Protocol.P2_5;
        OmegaDot = SpanBitHelper.GetBitS(buffer, ref bitIndex, 24) * RtcmV3Protocol.P2_43 * RtcmV3Protocol.SC2RAD;
        M0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Protocol.P2_31 * RtcmV3Protocol.SC2RAD;
        Iodc = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
        AF0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 22) * RtcmV3Protocol.P2_31;
        var prn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
        bitIndex += 3;
        Tgd = SpanBitHelper.GetBitS(buffer, ref bitIndex, 8) * RtcmV3Protocol.P2_31;
        CodeOnL2 = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
        SVAccuracy = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 4);
        SVHealth = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
        L2PDataFlag = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        bitIndex += 3;

        if (prn == 0)
        {
            prn = 32;
        }

        Satellite = RtcmV3Protocol.satno(NavigationSystemEnum.SYS_GPS, (int)prn);
        WeekNumber = AdjustGpsWeek((int)week);
        Toe = RtcmV3Protocol.GetFromGps(WeekNumber, Toes);
        Toc = RtcmV3Protocol.GetFromGps(WeekNumber, toc);
        A = sqrtA * sqrtA;
    }

    private static int AdjustGpsWeek(int week)
    {
        var now = DateTime.UtcNow;
        var currentWeek = 0;
        var seconds = 0.0;
        RtcmV3Protocol.GetFromTime(RtcmV3Protocol.Utc2Gps(now), ref currentWeek, ref seconds);
        if (currentWeek < 1560)
        {
            currentWeek = 1560;
        }

        return week + (currentWeek - week + 1) / 1024 * 1024;
    }
}
