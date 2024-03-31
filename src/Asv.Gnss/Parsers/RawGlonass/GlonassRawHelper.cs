using System;
using Asv.Common;

namespace Asv.Gnss
{
    public static class GlonassRawHelper
    {

        /// <summary>
        /// 2^-5
        /// </summary>
        public const double P2_5 = 3.125E-2;

        /// <summary>
        /// 2^-9
        /// </summary>
        public const double P2_9 = 1.953125E-3;

        /// <summary>
        /// 2^-11
        /// </summary>
        public const double P2_11 = 4.882812500000000E-04;

        /// <summary>
        /// 2^-14
        /// </summary>
        public const double P2_14 = 6.103515625E-5;

        /// <summary>
        /// 2^-15
        /// </summary>
        public const double P2_15 = 3.0517578125E-5;

        /// <summary>
        /// 2^-18
        /// </summary>
        public const double P2_18 = 3.814697265625E-6;

        /// <summary>
        /// 2^-20
        /// </summary>
        public const double P2_20 = 9.536743164062500E-07;

        /// <summary>
        /// 2^-30
        /// </summary>
        public const double P2_30 = 9.313225746154785E-10;

        /// <summary>
        /// 2^-31
        /// </summary>
        public const double P2_31 = 4.656612873077393E-10;

        /// <summary>
        /// 2^-40
        /// </summary>
        public const double P2_40 = 9.094947017729280E-13;

        public static void TimeToGps(DateTime time, ref int week, ref double seconds)
        {
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            var dif = time - datum;

            var weeks = (int)(dif.TotalDays / 7);

            week = weeks;

            dif = time - datum.AddDays(weeks * 7);

            seconds = dif.TotalSeconds;
        }

        public static DateTime GpsToTime(int weeknumber, double seconds)
        {

            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var week = datum.AddDays(weeknumber * 7);
            var time = week.AddSeconds(seconds);
            return time;
        }

        /// <summary>
        /// Get time referenced to the beginning of the frame within the current day
        /// </summary>
        /// <param name="utc">Current UTC time</param>
        /// <param name="tof">The number of seconds elapsed since the beginning of the current day (local glonass time)</param>
        /// <returns>Time referenced to the beginning of the frame within the current day (UTC)</returns>
        public static DateTime GetTof(DateTime utc, double tof)
        {
            var week = 0;
            var tow = 0.0;
            TimeToGps(utc.AddHours(3), ref week, ref tow);

            var tod = tow % 86400.0;
            tow -= tod;

            if (tof < tod - 43200.0) tof += 86400.0;
            else if (tof > tod + 43200.0) tof -= 86400.0;

            return GpsToTime(week, tow + tof).AddHours(-3);
        }

        /// <summary>
        /// Glonass Ephemeris Time
        /// </summary>
        /// <param name="utc">Current time UTC</param>
        /// <param name="tb">Index of a time interval within current day according to UTC(SU) + 03 hours 00 min</param>
        /// <returns>Glonass Ephemeris Time (UTC)</returns>
        public static DateTime GetToe(DateTime utc, uint tb)
        {
            var week = 0;
            var tow = 0.0;
            TimeToGps(utc.AddHours(3), ref week, ref tow);
            var tod = tow % 86400.0;
            tow -= tod;
            var toe = tb * 900.0;
            if (toe < tod - 43200.0) toe += 86400.0;
            else if (toe > tod + 43200.0) toe -= 86400.0;
            return GpsToTime(week, tow + toe).AddHours(-3);
        }

        /// <summary>
        /// Glonass Ephemeris Time
        /// </summary>
        /// <param name="utc">Current time UTC</param>
        /// <param name="tb">Index of a time interval within current day according to UTC(SU) + 03 hours 00 min</param>
        /// <param name="p1">Time interval between two adjacent values of tb parameter (in minutes)</param>
        /// <param name="p2">Flag of oddness ("1") or evenness ("0") of the value of tb (for intervals 30 or 60 minutes)</param>
        /// <returns>Glonass Ephemeris Time</returns>
        public static DateTime GetToe(DateTime utc, uint tb, byte p1, byte p2)
        {
            var interval = p1 switch
            {
                0 => 0.0 * 60.0,
                1 => 30.0 * 60.0,
                2 => 45.0 * 60.0,
                3 => 60.0 * 60.0,
                _ => 0.0 * 60.0
            };

            var week = 0;
            var tow = 0.0;
            TimeToGps(utc.AddHours(3), ref week, ref tow);
            var tod = tow % 86400.0;
            tow -= tod;
            var toe = (tb - 1) * 900 + interval;
            if (p2 == 1 && p1 is 1 or 3) toe += interval / 2.0;

            if (toe < tod - 43200.0) toe += 86400.0;
            else if (toe > tod + 43200.0) toe -= 86400.0;
            return GpsToTime(week, tow + toe).AddHours(-3);
        }

        /// <summary>
        /// Transformation of GLONASS current data information into common form
        /// </summary>
        /// <param name="na">Calendar day number within the four-year period beginning since the leap year</param>
        /// <param name="n4">Four-year interval number starting from 1996</param>
        /// <returns>Current data (GLONASS local date)</returns>
        public static DateTime GetCurrentDate(ushort na, byte n4)
        {
            var datum = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return datum.AddYears(4 * (n4 - 1)).AddDays(na - 1);
        }


        /// <summary>
        /// Transformation of GLONASS current data information into common form
        /// </summary>
        /// <param name="na">Calendar day number within the four-year period beginning since the leap year</param>
        /// <param name="utc">Current UTC time</param>
        /// <returns>Current data (GLONASS local time)</returns>
        public static DateTime GetCurrentDate(ushort na, DateTime utc)
        {
            var gloTime = utc.AddHours(3);
            var datum = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var diff = gloTime - datum;
            var td = diff.TotalDays;
            var n4 = (int)(td / 1461) + 1;
            var date = datum.AddYears(4 * (n4 - 1)).AddDays(na - 1);
            return date.AddSeconds((gloTime - date).TotalSeconds % 86400);
        }

        public static byte GetWordId(uint[] navBits)
        {
            if (navBits.Length != 3)
                throw new Exception(
                    $"Length of {nameof(navBits)} array must be 3 u32 word (as GLONASS ICD word length)");
            if (((navBits[0] >> 31) & 0x1) != 0)
                throw new Exception("Bits 85 must be 0 (as GLONASS ICD superframe structure)");
            return (byte)((navBits[0] >> 27) & 0xF); // 27 bits offset, 4 bit
        }

        public static uint GetBitU(byte[] buff, uint pos, uint len)
        {
            uint bits = 0;
            uint i;
            for (i = pos; i < pos + len; i++)
                bits = (uint)((bits << 1) + ((buff[i / 8] >> (int)(7 - i % 8)) & 1u));
            return bits;
        }

        public static void SetBitU(byte[] buff, uint pos, uint len, uint data)
        {
            var mask = 1u << (int)(len - 1);

            if (len <= 0 || 32 < len) return;

            for (var i = pos; i < pos + len; i++, mask >>= 1)
            {
                if ((data & mask) > 0)
                    buff[i / 8] |= (byte)(1u << (int)(7 - i % 8));
                else
                    buff[i / 8] &= (byte)(~(1u << (int)(7 - i % 8)));
            }
        }

        /// <summary>
        /// get sign-magnitude bits
        /// </summary>
        /// <returns></returns>
        public static double GetBitG(byte[] buff, uint pos, uint len)
        {
            double value = GetBitU(buff, pos + 1, len - 1);
            return GetBitU(buff, pos, 1) != 0 ? -value : value;
        }

        public static byte[] GetRawData(uint[] navBits)
        {
            uint bitIndex = 3;
            var result = new byte[11];
            SetBitU(result, bitIndex, 32, navBits[0]);
            bitIndex += 32;
            SetBitU(result, bitIndex, 32, navBits[1]);
            bitIndex += 32;
            SetBitU(result, bitIndex, 21, navBits[2] >> 11);
            bitIndex += 21;
            return result;
        }

        public static int GetSatelliteNumber(byte prn)
        {
            return prn < MINPRNGLO || MAXPRNGLO < prn ? 0 : NSATGPS + prn - MINPRNGLO + 1;
        }


        // min satellite PRN number of GPS
        private const int MINPRNGPS = 1;

        // max satellite PRN number of GPS
        private const int MAXPRNGPS = 32;

        // number of GPS satellites
        private const int NSATGPS = MAXPRNGPS - MINPRNGPS + 1;

        // min satellite slot number of GLONASS
        private const int MINPRNGLO = 1;

        // max satellite slot number of GLONASS
        private const int MAXPRNGLO = 24;

        // number of GLONASS satellites
        private const int NSATGLO = MAXPRNGLO - MINPRNGLO + 1;

    }
}
