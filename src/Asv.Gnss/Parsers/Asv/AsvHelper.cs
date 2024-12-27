using System;

namespace Asv.Gnss
{
    public static class AsvHelper
    {
        /// <summary>
        /// min satellite PRN number of GPS.
        /// </summary>
        public const byte MINPRNGPS = 1;

        /// <summary>
        /// max satellite PRN number of GPS.
        /// </summary>
        public const byte MAXPRNGPS = 32;

        /// <summary>
        /// number of GPS satellites.
        /// </summary>
        public const byte NSATGPS = MAXPRNGPS - MINPRNGPS + 1;

        public const byte NSYSGPS = 1;

        /// <summary>
        /// min satellite slot number of GLONASS.
        /// </summary>
        public const byte MINPRNGLO = 1;

        /// <summary>
        /// max satellite slot number of GLONASS.
        /// </summary>
        public const byte MAXPRNGLO = 27;

        /// <summary>
        /// number of GLONASS satellites.
        /// </summary>
        public const byte NSATGLO = MAXPRNGLO - MINPRNGLO + 1;

        public const byte NSYSGLO = 1;

        /// <summary>
        /// min satellite PRN number of Galileo.
        /// </summary>
        public const byte MINPRNGAL = 1;

        /// <summary>
        /// max satellite PRN number of Galileo.
        /// </summary>
        public const byte MAXPRNGAL = 36;

        /// <summary>
        /// number of Galileo satellites.
        /// </summary>
        public const byte NSATGAL = MAXPRNGAL - MINPRNGAL + 1;

        public const byte NSYSGAL = 1;

        /// <summary>
        /// min satellite PRN number of QZSS.
        /// </summary>
        public const byte MINPRNQZS = 193;

        /// <summary>
        /// max satellite PRN number of QZSS.
        /// </summary>
        public const byte MAXPRNQZS = 202;

        /// <summary>
        /// min satellite PRN number of QZSS L1S.
        /// </summary>
        public const byte MINPRNQZS_S = 183;

        /// <summary>
        /// max satellite PRN number of QZSS L1S.
        /// </summary>
        public const byte MAXPRNQZS_S = 191;

        /// <summary>
        /// number of QZSS satellites.
        /// </summary>
        public const byte NSATQZS = MAXPRNQZS - MINPRNQZS + 1;

        public const byte NSYSQZS = 1;

        /// <summary>
        /// min satellite sat number of BeiDou.
        /// </summary>
        public const byte MINPRNCMP = 1;

        /// <summary>
        /// max satellite sat number of BeiDou.
        /// </summary>
        public const byte MAXPRNCMP = 63;

        /// <summary>
        /// number of BeiDou satellites.
        /// </summary>
        public const byte NSATCMP = MAXPRNCMP - MINPRNCMP + 1;

        public const byte NSYSCMP = 1;

        /// <summary>
        /// min satellite sat number of IRNSS.
        /// </summary>
        public const byte MINPRNIRN = 1;

        /// <summary>
        /// max satellite sat number of IRNSS.
        /// </summary>
        public const byte MAXPRNIRN = 14;

        /// <summary>
        /// number of IRNSS satellites.
        /// </summary>
        public const byte NSATIRN = MAXPRNIRN - MINPRNIRN + 1;

        public const byte NSYSIRN = 1;

        /// <summary>
        /// min satellite sat number of LEO.
        /// </summary>
        public const byte MINPRNLEO = 1;

        /// <summary>
        /// max satellite sat number of LEO.
        /// </summary>
        public const byte MAXPRNLEO = 10;

        /// <summary>
        /// number of LEO satellites.
        /// </summary>
        public const byte NSATLEO = MAXPRNLEO - MINPRNLEO + 1;

        public const byte NSYSLEO = 1;

        /// <summary>
        /// number of systems.
        /// </summary>
        public const int NSYS = NSYSGPS + NSYSGLO + NSYSGAL + NSYSQZS + NSYSCMP + NSYSIRN + NSYSLEO;

        /// <summary>
        /// min satellite PRN number of SBAS.
        /// </summary>
        public const byte MINPRNSBS = 120;

        /// <summary>
        /// max satellite PRN number of SBAS.
        /// </summary>
        public const byte MAXPRNSBS = 158;

        /// <summary>
        /// number of SBAS satellites.
        /// </summary>
        public const byte NSATSBS = MAXPRNSBS - MINPRNSBS + 1;

        /// <summary>
        /// max satellite number (1 to MAXSAT).
        /// </summary>
        public const byte MAXSAT =
            NSATGPS + NSATGLO + NSATGAL + NSATQZS + NSATCMP + NSATIRN + NSATSBS + NSATLEO;

        /// <summary>
        /// Asv unit of gps pseudorange (m).
        /// </summary>
        public const double PRUNIT_GPS = 299792.458;

        /// <summary>
        /// Asv unit of glo pseudorange (m).
        /// </summary>
        public const double PRUNIT_GLO = 599584.916;

        /// <summary>
        /// speed of light (m/s).
        /// </summary>
        public const double CLIGHT = 299792458.0;

        /// <summary>
        /// semi-circle to radian (IS-GPS).
        /// </summary>
        public const double SC2RAD = 3.1415926535898;

        /// <summary>
        /// obs code: none or unknown.
        /// </summary>
        public const byte CODE_NONE = 0;

        /// <summary>
        /// obs code: L1C/A,G1C/A,E1C (GPS,GLO,GAL,QZS,SBS).
        /// </summary>
        public const byte CODE_L1C = 1;

        /// <summary>
        /// obs code: L1P,G1P,B1P (GPS,GLO,BDS).
        /// </summary>
        public const byte CODE_L1P = 2;

        /// <summary>
        /// Convert satellite system+prn/slot number to satellite number.
        /// </summary>
        /// <param name="sys">I satellite system (SYS_GPS,SYS_GLO,...).</param>
        /// <param name="prn">I satellite prn/slot number.</param>
        /// <returns>satellite number (0:error).</returns>
        public static int satno(NavigationSystemEnum sys, int prn)
        {
            if (prn <= 0)
            {
                return 0;
            }

            switch (sys)
            {
                case NavigationSystemEnum.SYS_GPS:
                    if (prn is < MINPRNGPS or > MAXPRNGPS)
                    {
                        return 0;
                    }

                    return prn - MINPRNGPS + 1;
                case NavigationSystemEnum.SYS_GLO:
                    if (prn is < MINPRNGLO or > MAXPRNGLO)
                    {
                        return 0;
                    }

                    return NSATGPS + prn - MINPRNGLO + 1;
                case NavigationSystemEnum.SYS_GAL:
                    if (prn is < MINPRNGAL or > MAXPRNGAL)
                    {
                        return 0;
                    }

                    return NSATGPS + NSATGLO + prn - MINPRNGAL + 1;
                case NavigationSystemEnum.SYS_QZS:
                    if (prn is < MINPRNQZS or > MAXPRNQZS)
                    {
                        return 0;
                    }

                    return NSATGPS + NSATGLO + NSATGAL + prn - MINPRNQZS + 1;
                case NavigationSystemEnum.SYS_CMP:
                    if (prn is < MINPRNCMP or > MAXPRNCMP)
                    {
                        return 0;
                    }

                    return NSATGPS + NSATGLO + NSATGAL + NSATQZS + prn - MINPRNCMP + 1;
                case NavigationSystemEnum.SYS_IRN:
                    if (prn is < MINPRNIRN or > MAXPRNIRN)
                    {
                        return 0;
                    }

                    return NSATGPS + NSATGLO + NSATGAL + NSATQZS + NSATCMP + prn - MINPRNIRN + 1;
                case NavigationSystemEnum.SYS_LEO:
                    if (prn is < MINPRNLEO or > MAXPRNLEO)
                    {
                        return 0;
                    }

                    return NSATGPS
                        + NSATGLO
                        + NSATGAL
                        + NSATQZS
                        + NSATCMP
                        + NSATIRN
                        + prn
                        - MINPRNLEO
                        + 1;
                case NavigationSystemEnum.SYS_SBS:
                    if (prn is < MINPRNSBS or > MAXPRNSBS)
                    {
                        return 0;
                    }

                    return NSATGPS
                        + NSATGLO
                        + NSATGAL
                        + NSATQZS
                        + NSATCMP
                        + NSATIRN
                        + NSATLEO
                        + prn
                        - MINPRNSBS
                        + 1;
            }

            return 0;
        }

        public static string Sat2Code(int sat, int prn = 0)
        {
            var p = prn;
            var sys = GetSatelliteSystem(sat, ref p);
            switch (sys)
            {
                case NavigationSystemEnum.SYS_GPS:
                    return $"G{prn - MINPRNGPS + 1}";
                case NavigationSystemEnum.SYS_GLO:
                    return $"R{prn - MINPRNGLO + 1}";
                case NavigationSystemEnum.SYS_GAL:
                    return $"E{prn - MINPRNGAL + 1}";
                case NavigationSystemEnum.SYS_SBS:
                    return $"S{prn - 100}";
                case NavigationSystemEnum.SYS_QZS:
                    return $"J{prn - MINPRNQZS + 1}";
                case NavigationSystemEnum.SYS_CMP:
                    return $"C{prn - MINPRNCMP + 1}";
                case NavigationSystemEnum.SYS_IRN:
                    return $"I{prn - MINPRNIRN + 1}";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Satellite number to satellite system.
        /// Convert satellite number to satellite system.
        /// </summary>
        /// <param name="sat">I   satellite number (1-MAXSAT).</param>
        /// <param name="prn">IO  satellite prn/slot number (NULL: no output).</param>
        /// <returns>satellite system (SYS_GPS,SYS_GLO,...).</returns>
        public static NavigationSystemEnum GetSatelliteSystem(int sat, ref int prn)
        {
            var sys = NavigationSystemEnum.SYS_NONE;
            switch (sat)
            {
                case <= 0
                or > MAXSAT:
                    sat = 0;
                    break;
                case <= NSATGPS:
                    sys = NavigationSystemEnum.SYS_GPS;
                    sat += MINPRNGPS - 1;
                    break;
                default:
                {
                    if ((sat -= NSATGPS) <= NSATGLO)
                    {
                        sys = NavigationSystemEnum.SYS_GLO;
                        sat += MINPRNGLO - 1;
                    }
                    else if ((sat -= NSATGLO) <= NSATGAL)
                    {
                        sys = NavigationSystemEnum.SYS_GAL;
                        sat += MINPRNGAL - 1;
                    }
                    else if ((sat -= NSATGAL) <= NSATQZS)
                    {
                        sys = NavigationSystemEnum.SYS_QZS;
                        sat += MINPRNQZS - 1;
                    }
                    else if ((sat -= NSATQZS) <= NSATCMP)
                    {
                        sys = NavigationSystemEnum.SYS_CMP;
                        sat += MINPRNCMP - 1;
                    }
                    else if ((sat -= NSATCMP) <= NSATIRN)
                    {
                        sys = NavigationSystemEnum.SYS_IRN;
                        sat += MINPRNIRN - 1;
                    }
                    else if ((sat -= NSATIRN) <= NSATLEO)
                    {
                        sys = NavigationSystemEnum.SYS_LEO;
                        sat += MINPRNLEO - 1;
                    }
                    else if ((sat -= NSATLEO) <= NSATSBS)
                    {
                        sys = NavigationSystemEnum.SYS_SBS;
                        sat += MINPRNSBS - 1;
                    }
                    else
                    {
                        sat = 0;
                    }

                    break;
                }
            }

            if (prn != 0)
            {
                prn = sat;
            }

            return sys;
        }

        public static uint GetBitU(ReadOnlySpan<byte> buff, ref int pos, int len)
        {
            uint bits = 0;
            int i;
            for (i = pos; i < pos + len; i++)
            {
                bits = (uint)((bits >> 1) + (((buff[i / 8] >> (i % 8)) & 1u) << (len - 1)));
            }

            pos += len;
            return bits;
        }

        public static int GetBitS(ReadOnlySpan<byte> buff, ref int pos, int len)
        {
            var bitU = GetBitU(buff, ref pos, len);
            return len <= 0 || len >= 32 || ((int)bitU & (1 << (len - 1))) == 0
                ? (int)bitU
                : (int)bitU | (int)(~0u << len);
        }

        public static void SetBitU(Span<byte> buff, uint data, ref int pos, int len)
        {
            var mask = 1u;
            int i;
            if (len is <= 0 or > 32)
            {
                return;
            }

            for (i = pos; i < pos + len; i++, mask <<= 1)
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

        public static DateTime Gps2Utc(DateTime t)
        {
            return t.AddSeconds(-LeapSecondsGPS(t.Year, t.Month));
        }

        public static DateTime Utc2Gps(DateTime t)
        {
            return t.AddSeconds(LeapSecondsGPS(t.Year, t.Month));
        }

        private static int LeapSecondsGPS(int year, int month)
        {
            return LeapSecondsTAI(year, month) - 19;
        }

        private static int LeapSecondsTAI(int year, int month)
        {
            // http://maia.usno.navy.mil/ser7/tai-utc.dat
            var yyyymm = year * 100 + month;
            return yyyymm switch
            {
                >= 201701 => 37,
                >= 201507 => 36,
                >= 201207 => 35,
                >= 200901 => 34,
                >= 200601 => 33,
                >= 199901 => 32,
                >= 199707 => 31,
                >= 199601 => 30,
                >= 199407 => 29,
                >= 199307 => 28,
                >= 199207 => 27,
                >= 199101 => 26,
                >= 199001 => 25,
                >= 198801 => 24,
                >= 198507 => 23,
                >= 198307 => 22,
                >= 198207 => 21,
                >= 198107 => 20,
                >= 0 => 19,
                _ => 0,
            };
        }

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
                    $"Lock time '{lockTimeIndicator}', must be greater than or equal to 0 and less than or equal to 127"
                ),
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
                    $"Lock time '{lockTime}', must be greater than or equal to 0 and less than or equal to 937"
                ),
            };
        }
    }
}
