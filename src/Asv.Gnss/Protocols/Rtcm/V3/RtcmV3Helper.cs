using System;

namespace Asv.Gnss;

public static class RtcmV3Helper
{
    public const byte SyncByte = RtcmV3Protocol.SyncByte;
    public const double PRUNIT_GPS = RtcmV3Protocol.PRUNIT_GPS;
    public const double PRUNIT_GLO = RtcmV3Protocol.PRUNIT_GLO;
    public const double CLIGHT = RtcmV3Protocol.CLIGHT;
    public const double FREQ1 = RtcmV3Protocol.FREQ1;
    public const double FREQ2 = RtcmV3Protocol.FREQ2;
    public const double SC2RAD = RtcmV3Protocol.SC2RAD;
    public const double RANGE_MS = RtcmV3Protocol.RANGE_MS;
    public const double R2D = RtcmV3Protocol.R2D;
    public const double P2_5 = RtcmV3Protocol.P2_5;
    public const double P2_6 = RtcmV3Protocol.P2_6;
    public const double P2_10 = RtcmV3Protocol.P2_10;
    public const double P2_11 = RtcmV3Protocol.P2_11;
    public const double P2_19 = RtcmV3Protocol.P2_19;
    public const double P2_20 = RtcmV3Protocol.P2_20;
    public const double P2_24 = RtcmV3Protocol.P2_24;
    public const double P2_29 = RtcmV3Protocol.P2_29;
    public const double P2_30 = RtcmV3Protocol.P2_30;
    public const double P2_31 = RtcmV3Protocol.P2_31;
    public const double P2_32 = RtcmV3Protocol.P2_32;
    public const double P2_33 = RtcmV3Protocol.P2_33;
    public const double P2_40 = RtcmV3Protocol.P2_40;
    public const double P2_43 = RtcmV3Protocol.P2_43;
    public const double P2_46 = RtcmV3Protocol.P2_46;
    public const double P2_50 = RtcmV3Protocol.P2_50;
    public const double P2_55 = RtcmV3Protocol.P2_55;
    public const byte MINPRNQZS = RtcmV3Protocol.MINPRNQZS;
    public const byte MINPRNSBS = RtcmV3Protocol.MINPRNSBS;

    public const byte CODE_L1C = RtcmV3Protocol.CODE_L1C;
    public const byte CODE_L1P = RtcmV3Protocol.CODE_L1P;
    public const byte CODE_L2C = RtcmV3Protocol.CODE_L2C;
    public const byte CODE_L2X = RtcmV3Protocol.CODE_L2X;
    public const byte CODE_L2P = RtcmV3Protocol.CODE_L2P;
    public const byte CODE_L2D = RtcmV3Protocol.CODE_L2D;
    public const byte CODE_L2W = RtcmV3Protocol.CODE_L2W;

    public static int satno(NavigationSystemEnum sys, int prn) => RtcmV3Protocol.satno(sys, prn);
    public static string Sat2Code(int sat, int prn = 0) => RtcmV3Protocol.Sat2Code(sat, prn);
    public static ushort snratio(double snr) => RtcmV3Protocol.snratio(snr);
    public static DateTime GetFromGps(int weeknumber, double seconds) => RtcmV3Protocol.GetFromGps(weeknumber, seconds);
    public static DateTime GetFromGalileo(int weeknumber, double seconds) => RtcmV3Protocol.GetFromGalileo(weeknumber, seconds);
    public static DateTime GetFromBeiDou(int weeknumber, double seconds) => RtcmV3Protocol.GetFromBeiDou(weeknumber, seconds);
    public static DateTime Utc2Gps(DateTime t) => RtcmV3Protocol.Utc2Gps(t);
    public static DateTime Gps2BeiDou(DateTime t) => RtcmV3Protocol.Gps2BeiDou(t);
    public static void GetFromTime(DateTime time, ref int week, ref double seconds) => RtcmV3Protocol.GetFromTime(time, ref week, ref seconds);
    public static void EcefToPos(double[] r, double[] pos) => RtcmV3Protocol.EcefToPos(r, pos);
    public static NavigationSystemEnum GetNavigationSystem(ushort messageId) => RtcmV3Protocol.GetNavigationSystem(messageId);
    public static DateTime GetUtc(DateTime nowUtc, double tod) => RtcmV3Protocol.GetUtc(nowUtc, tod);
    public static DateTime AdjustDailyRoverGlonassTime(DateTime nowUtc, double tod) => RtcmV3Protocol.AdjustDailyRoverGlonassTime(nowUtc, tod);
    public static DateTime AdjustWeekly(DateTime nowUtc, double tow) => RtcmV3Protocol.AdjustWeekly(nowUtc, tow);
    public static int adjgpsweek(DateTime utc, int week) => RtcmV3Protocol.adjgpsweek(utc, week);
    public static byte Obs2Code(string obs) => RtcmV3Protocol.Obs2Code(obs);
    public static int Code2Idx(NavigationSystemEnum sys, byte code) => RtcmV3Protocol.Code2Idx(sys, code);
    public static double Code2Freq(NavigationSystemEnum sys, byte code, int fcn = 0) => RtcmV3Protocol.Code2Freq(sys, code, fcn);
    public static double GetMinLockTime(byte indicator) => RtcmV3Protocol.GetMinLockTime(indicator);
    public static double GetMinLockTimeEx(ushort indicator) => RtcmV3Protocol.GetMinLockTimeEx(indicator);
    public static double GetBits38(ReadOnlySpan<byte> buff, ref int pos) => RtcmV3Protocol.GetBits38(buff, ref pos);
    public static double GetBitG(ReadOnlySpan<byte> buff, ref int pos, int len) => RtcmV3Protocol.GetBitG(buff, ref pos, len);
    public static string GetRinexCodeFromMsm(NavigationSystemEnum sys, int signalId) => RtcmV3Protocol.GetRinexCodeFromMsm(sys, signalId);
}
