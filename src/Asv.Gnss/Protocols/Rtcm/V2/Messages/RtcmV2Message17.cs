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

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        var week = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
        bitIndex += 10;
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
        bitIndex += 8;
        CodeOnL2 = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
        bitIndex += 2;
        SVAccuracy = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 4);
        bitIndex += 4;
        SVHealth = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
        bitIndex += 6;
        L2PDataFlag = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        bitIndex += 1;

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
