using System;
using Asv.IO;

namespace Asv.Gnss
{
    public static class RtcmV3EphemerisHelper
    {
        /// <summary>
        /// 2^-34
        /// </summary>
        public const double P2_34 = 5.8207660913467407E-11;

        /// <summary>
        /// 2^-44
        /// </summary>
        public const double P2_44 = 1.4210854715202004E-14;

        /// <summary>
        /// 2^-59
        /// </summary>
        public const double P2_59 = 1.7347234759768071E-18;

        /// <summary>
        /// 2^-66
        /// </summary>
        public const double P2_66 = 1.35525271560688E-20;

        /// <summary>
        /// Beidou started from 00:00:00UTC on Jan 1. 2006 of BDT.
        /// </summary>
        public static DateTime BdsStart => new(2006, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The GST start epoch is defined as 13 seconds before midnight between 21st August and 22nd August 1999.
        /// i.e. GST was equal 13 seconds at 22 nd August 1999 00:00:00 UTC.
        /// </summary>
        public static DateTime GalStart => new(1999, 8, 21, 23, 59, 47, DateTimeKind.Utc);

        /// <summary>
        /// Returns the week and seconds between start and current date.
        /// </summary>
        /// <param name="datum"></param>
        /// <param name="time"></param>
        /// <param name="week"></param>
        /// <param name="seconds"></param>
        public static void GetWeekFromTime(DateTime datum, DateTime time, ref int week, ref double seconds)
        {
            var dif = time - datum;
            var weeks = (int)(dif.TotalDays / 7);
            week = weeks;
            dif = time - datum.AddDays(weeks * 7);
            seconds = dif.TotalSeconds;
        }

        /// <summary>
        /// BDS Week number. Range 0 - 8191. Resolution - 1 week.
        /// Roll-over every 8192 weeks starting fromm 00:00:00UTC on Jan 1. 2006 of BDT.
        /// </summary>
        public static int GetBdsWeek(DateTime utc, int week)
        {
            int w = 0;
            var s = 0.0;
            var gpsTime = RtcmV3Helper.Utc2Gps(utc);
            var bdsTime = RtcmV3Helper.Gps2BeiDou(gpsTime);
            GetWeekFromTime(BdsStart, bdsTime, ref w, ref s);
            return week + (w - week + 1) / 8192 * 8192;
        }

        /// <summary>
        /// Galileo Week number.
        /// Roll-over every 4096 (about 78 years). Galileo System Time (GST).
        /// </summary>
        public static int GetGalWeek(DateTime utc, int week)
        {
            int w = 0;
            var s = 0.0;
            var galTime = RtcmV3Helper.Utc2Gps(utc);
            GetWeekFromTime(GalStart, galTime, ref w, ref s);
            return week + (w - week + 1) / 4096 * 4096;
        }
    }
 }