using System;

namespace Asv.Gnss;

public static class AsvHelper
{
    public const double PRUNIT_GPS = 299792.458;
    public const double PRUNIT_GLO = 599584.916;
    public const double CLIGHT = 299792458.0;
    public const byte CODE_L1C = 1;
    public const byte CODE_L1P = 2;

    public static int satno(NavigationSystemEnum sys, int prn) => RtcmV3Protocol.satno(sys, prn);

    public static string Sat2Code(int sat, int prn = 0) => RtcmV3Protocol.Sat2Code(sat, prn);

    public static uint GetBitU(ReadOnlySpan<byte> buff, ref int pos, int len)
    {
        uint bits = 0;
        for (var i = pos; i < pos + len; i++)
        {
            bits = (uint)((bits >> 1) + (((buff[i / 8] >> (i % 8)) & 1u) << (len - 1)));
        }

        pos += len;
        return bits;
    }

    public static int GetBitS(ReadOnlySpan<byte> buff, ref int pos, int len)
    {
        var bitU = GetBitU(buff, ref pos, len);
        return len <= 0 || 32 <= len || ((int)bitU & (1 << (len - 1))) == 0
            ? (int)bitU
            : (int)bitU | (int)(~0u << len);
    }

    public static void SetBitU(Span<byte> buff, uint data, ref int pos, int len)
    {
        var mask = 1u;
        if (len is <= 0 or > 32)
        {
            return;
        }

        for (var i = pos; i < pos + len; i++, mask <<= 1)
        {
            if ((data & mask) != 0)
            {
                buff[i / 8] |= (byte)(1u << (i % 8));
            }
            else
            {
                buff[i / 8] &= (byte)~(1u << (i % 8));
            }
        }

        pos += len;
    }

    public static void SetBitS(Span<byte> buff, int data, ref int pos, int len)
    {
        if (data < 0)
        {
            data |= 1 << (len - 1);
        }
        else
        {
            data &= ~(1 << (len - 1));
        }

        SetBitU(buff, (uint)data, ref pos, len);
    }

    public static DateTime Gps2Utc(DateTime t) => GpsRawHelper.Gps2Utc(t);

    public static DateTime Utc2Gps(DateTime t) => GpsRawHelper.Utc2Gps(t);

    public static ushort GetLockTime(byte lockTimeIndicator)
    {
        return lockTimeIndicator switch
        {
            > 0 and <= 23 => lockTimeIndicator,
            > 23 and <= 47 => (ushort)(lockTimeIndicator * 2 - 24),
            > 47 and <= 71 => (ushort)(lockTimeIndicator * 4 - 120),
            > 71 and <= 95 => (ushort)(lockTimeIndicator * 8 - 408),
            > 95 and <= 119 => (ushort)(lockTimeIndicator * 16 - 1176),
            > 119 and <= 126 => (ushort)(lockTimeIndicator * 32 - 3096),
            127 => 937,
            _ => throw new ArgumentException(
                $"Lock time '{lockTimeIndicator}', must be greater than or equal to 0 and less than or equal to 127")
        };
    }

    public static byte GetLockTimeIndicator(ushort lockTime)
    {
        return lockTime switch
        {
            > 0 and <= 23 => (byte)lockTime,
            > 23 and <= 70 => (byte)((lockTime + 24) / 2),
            > 70 and <= 164 => (byte)((lockTime + 120) / 4),
            > 164 and <= 352 => (byte)((lockTime + 408) / 8),
            > 352 and <= 728 => (byte)((lockTime + 1176) / 16),
            > 728 and <= 936 => (byte)((lockTime + 3096) / 32),
            937 => 127,
            _ => throw new ArgumentException(
                $"Lock time '{lockTime}', must be greater than or equal to 0 and less than or equal to 937")
        };
    }
}
