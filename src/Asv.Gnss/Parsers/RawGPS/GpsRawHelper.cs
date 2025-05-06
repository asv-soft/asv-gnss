using System;

namespace Asv.Gnss
{
    public static class GpsRawHelper
    {
        /// <summary>
        /// 2^-5.
        /// </summary>
        public const double P2_5 = 3.125E-2;

        /// <summary>
        /// 2^-11.
        /// </summary>
        public const double P2_11 = 4.882812500000000E-04;

        /// <summary>
        /// 2^-17.
        /// </summary>
        public const double P2_17 = 7.629394531250000E-06;

        /// <summary>
        /// 2^-19.
        /// </summary>
        public const double P2_19 = 1.9073486328125E-6;

        /// <summary>
        /// 2^-20.
        /// </summary>
        public const double P2_20 = 9.536743164062500E-07;

        /// <summary>
        /// 2^-21.
        /// </summary>
        public const double P2_21 = 4.768371582031250E-07;

        /// <summary>
        /// 2^-23.
        /// </summary>
        public const double P2_23 = 1.192092895507810E-07;

        /// <summary>
        /// 2^-24.
        /// </summary>
        public const double P2_24 = 5.960464477539063E-08;

        /// <summary>
        /// 2^-27.
        /// </summary>
        public const double P2_27 = 7.450580596923828E-09;

        /// <summary>
        /// 2^-29.
        /// </summary>
        public const double P2_29 = 1.862645149230957E-9;

        /// <summary>
        /// 2^-30.
        /// </summary>
        public const double P2_30 = 9.313225746154785E-10;

        /// <summary>
        /// 2^-31.
        /// </summary>
        public const double P2_31 = 4.656612873077393E-10;

        /// <summary>
        /// 2^-33.
        /// </summary>
        public const double P2_33 = 1.164153218269348E-10;

        /// <summary>
        /// 2^-38.
        /// </summary>
        public const double P2_38 = 3.637978807091710E-12;

        /// <summary>
        /// 2^-50.
        /// </summary>
        public const double P2_50 = 8.881784197001252E-16;

        /// <summary>
        /// 2^-55.
        /// </summary>
        public const double P2_55 = 2.775557561562891E-17;

        /// <summary>
        /// 2^-43.
        /// </summary>
        public const double P2_43 = 1.136868377216160E-13;

        /// <summary>
        /// semi-circle to radian (IS-GPS).
        /// </summary>
        public const double SC2RAD = 3.1415926535898;

        /// <summary>
        /// The GPS subframe preamble constant.
        /// </summary>
        public const byte GpsSubframePreamble = 0x8B;

        /// <summary>
        /// Retrieves raw data from the given navigation bits array without parity bits.
        /// </summary>
        /// <param name="navBits">The navigation bits array. Length must be 10 uint words (as GPS ICD subframe length).</param>
        /// <returns>An array of bytes containing the raw data without parity bits.</returns>
        public static byte[] GetRawDataWithoutParity(uint[] navBits)
        {
            if (navBits.Length != 10)
            {
                throw new Exception(
                    $"Length of {nameof(navBits)} array must be 10 u32 word (as GPS ICD subframe length)"
                );
            }

            var result = new byte[30];
            for (int i = 0; i < navBits.Length; i++)
            {
                var value = (navBits[i] >> 6) & 0xFF_FFFF; // skip 6 parity bits and get 24 data
                result[(i * 3) + 0] = (byte)((value >> 16) & 0xFF);
                result[(i * 3) + 1] = (byte)((value >> 8) & 0xFF);
                result[(i * 3) + 2] = (byte)(value & 0xFF);
            }

            return result;
        }

        /// <summary>
        /// Sets a range of bits in the given byte array to the specified value.
        /// </summary>
        /// <param name="buff">The byte array to modify.</param>
        /// <param name="pos">The starting position of the range.</param>
        /// <param name="len">The length of the range.</param>
        /// <param name="data">The value to set the bits to.</param>
        public static void SetBitU(byte[] buff, uint pos, uint len, uint data)
        {
            var mask = 1u << (int)(len - 1);

            if (len <= 0 || len > 32)
            {
                return;
            }

            for (var i = pos; i < pos + len; i++, mask >>= 1)
            {
                if ((data & mask) > 0)
                {
                    buff[i / 8] |= (byte)(1u << (int)(7 - (i % 8)));
                }
                else
                {
                    buff[i / 8] &= (byte)(~(1u << (int)(7 - (i % 8))));
                }
            }
        }

        /// <summary>
        /// Gets the unsigned value of a specified range of bits from a byte array.
        /// </summary>
        /// <param name="buff">The byte array from which to retrieve the bits.</param>
        /// <param name="pos">The starting position of the bit range.</param>
        /// <param name="len">The length of the bit range.</param>
        /// <returns>The unsigned value of the specified range of bits.</returns>
        public static uint GetBitU(byte[] buff, uint pos, uint len)
        {
            uint bits = 0;
            uint i;
            for (i = pos; i < pos + len; i++)
            {
                bits = (uint)((bits << 1) + ((buff[i / 8] >> (int)(7 - (i % 8))) & 1u));
            }

            return bits;
        }

        public static int GetBitS(byte[] buff, uint pos, int len)
        {
            var bits = GetBitU(buff, pos, (uint)len);
            if (len <= 0 || len >= 32 || (bits & (1u << (len - 1))) == 0)
            {
                return (int)bits;
            }

            return (int)(bits | (~0u << len)); /* extend sign */
        }

        /// <summary>
        /// Checks if the given preamble is equal to the GPS subframe preamble.
        /// </summary>
        /// <param name="navBits">An array of unsigned integers representing the navigation bits.</param>
        /// <returns>
        /// True if the preamble is equal to the GPS subframe preamble;
        /// otherwise, false.
        /// </returns>
        public static bool CheckPreamble(uint[] navBits)
        {
            var val = GetPreamble(navBits);
            return val == GpsSubframePreamble;
        }

        /// <summary>
        /// Preamble: 0b10001011 or 0x8B.
        /// </summary>
        public static byte GetPreamble(uint[] navBits)
        {
            if (navBits.Length != 10)
            {
                throw new Exception(
                    $"Length of {nameof(navBits)} array must be 10 u32 word (as GPS ICD subframe length)"
                );
            }

            return (byte)((navBits[0] >> 22) & 0XFF);
        }

        /// <summary>
        /// The HOW begins with the 17 MSBs of the time-of-week(TOW) count. (The full TOW count consists of the 19 LSBs of the 29-
        /// bit Z-count). These 17 bits correspond to the TOW-count at the 1.5 second epoch which occurs
        /// at the start(leading edge) of the next following subframe(reference paragraph 2.3.5).
        /// </summary>
        public static uint GetTow15epoch(uint[] navBits)
        {
            if (navBits.Length != 10)
            {
                throw new Exception(
                    $"Length of {nameof(navBits)} array must be 10 u32 word (as GPS ICD subframe length)"
                );
            }

            return (navBits[1] >> 13) & 0x1FFFF; // 17 bits
        }

        /// <summary>
        /// Gets the subframe ID from the given array of navigation bits.
        /// </summary>
        /// <param name="navBits">An array of navigation bit words.</param>
        /// <returns>The subframe ID.</returns>
        /// <exception cref="Exception">Thrown if the length of the navBits array is not 10.</exception>
        /// <remarks>
        /// This method extracts the subframe ID from the navBits array. The navBits array must have a length of 10,
        /// which corresponds to the GPS ICD subframe length. The subframe ID is obtained by shifting the second word
        /// of the navBits array by 8 bits to the right, and then applying a bitwise AND operation with 0x07 to extract
        /// the lower 3 bits.
        /// </remarks>
        public static byte GetSubframeId(uint[] navBits)
        {
            if (navBits.Length != 10)
            {
                throw new Exception(
                    $"Length of {nameof(navBits)} array must be 10 u32 word (as GPS ICD subframe length)"
                );
            }

            var subframeId = (byte)(navBits[1] >> 8) & 0x07; // 8 bits offset, 3 bit
            return GetSubframeId(subframeId);
        }

        /// <summary>
        /// Converts a subframe ID to its corresponding value.
        /// </summary>
        /// <param name="subframeId">The subframe ID to convert.</param>
        /// <returns>The corresponding value of the subframe ID.</returns>
        public static byte GetSubframeId(int subframeId)
        {
            switch (subframeId)
            {
                case 0b001:
                    return 1;
                case 0b010:
                    return 2;
                case 0b011:
                    return 3;
                case 0b100:
                    return 4;
                case 0b101:
                    return 5;
                default:
                    throw new Exception(
                        $"Unknown GPS subframe ID:{Convert.ToString(subframeId, 2).PadRight(8)}"
                    );
            }
        }

        /// <summary>
        /// Adjust GPS week number using cpu time.
        /// </summary>
        /// <param name="utc">CPU UTC time.</param>
        /// <param name="week">Not-adjusted GPS week number.</param>
        /// <returns>Adjusted GPS week number.</returns>
        public static int AdjustGpsWeek(DateTime utc, int week)
        {
            var w = 0;
            var tow = 0.0;
            Time2Gps(Utc2Gps(utc), ref w, ref tow);
            if (w < 1560)
            {
                w = 1560; /* use 2009/12/1 if time is earlier than 2009/12/1 */
            }

            return week + ((w - week + 512) / 1024 * 1024);
        }

        /// <summary>
        /// Convert UTC to GPS Time considering leap seconds.
        /// </summary>
        /// <param name="t">Time expressed in UTC.</param>
        /// <returns>Time expressed in GPS Time.</returns>
        public static DateTime Utc2Gps(DateTime t)
        {
            return t.AddSeconds(LeapSecondsGPS(t.Year, t.Month));
        }

        /// <summary>
        /// Convert GPS to UTC Time considering leap seconds.
        /// </summary>
        /// <param name="t">Time expressed in GPS.</param>
        /// <returns>Time expressed in UTC Time.</returns>
        public static DateTime Gps2Utc(DateTime t)
        {
            return t.AddSeconds(-LeapSecondsGPS(t.Year, t.Month));
        }

        /// <summary>
        /// Convert DateTime struct to week and tow in gps time.
        /// </summary>
        /// <param name="time">DateTime struct.</param>
        /// <param name="week">Week number in GPS time.</param>
        /// <param name="tow">Time of week in GPS time (s).</param>
        public static void Time2Gps(DateTime time, ref int week, ref double tow)
        {
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var dif = time - datum;
            var weeks = (int)(dif.TotalDays / 7);
            week = weeks;
            dif = time - datum.AddDays(weeks * 7);
            tow = dif.TotalSeconds;
        }

        /// <summary>
        /// Convert week and tow in GPS Time to DateTime struct.
        /// </summary>
        /// <param name="week">Week number in GPS Time.</param>
        /// <param name="sec">Time of week in GPS Time (s).</param>
        /// <returns></returns>
        public static DateTime Gps2Time(int week, double sec)
        {
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            return datum.AddDays(week * 7).AddSeconds(sec);
        }

        private static int LeapSecondsGPS(int year, int month)
        {
            return LeapSecondsTAI(year, month) - 19;
        }

        private static int LeapSecondsTAI(int year, int month)
        {
            // http://maia.usno.navy.mil/ser7/tai-utc.dat
            var yyyymm = (year * 100) + month;
            if (yyyymm >= 201701)
            {
                return 37;
            }

            if (yyyymm >= 201507)
            {
                return 36;
            }

            if (yyyymm >= 201207)
            {
                return 35;
            }

            if (yyyymm >= 200901)
            {
                return 34;
            }

            if (yyyymm >= 200601)
            {
                return 33;
            }

            if (yyyymm >= 199901)
            {
                return 32;
            }

            if (yyyymm >= 199707)
            {
                return 31;
            }

            if (yyyymm >= 199601)
            {
                return 30;
            }

            if (yyyymm >= 199407)
            {
                return 29;
            }

            if (yyyymm >= 199307)
            {
                return 28;
            }

            if (yyyymm >= 199207)
            {
                return 27;
            }

            if (yyyymm >= 199101)
            {
                return 26;
            }

            if (yyyymm >= 199001)
            {
                return 25;
            }

            if (yyyymm >= 198801)
            {
                return 24;
            }

            if (yyyymm >= 198507)
            {
                return 23;
            }

            if (yyyymm >= 198307)
            {
                return 22;
            }

            if (yyyymm >= 198207)
            {
                return 21;
            }

            if (yyyymm >= 198107)
            {
                return 20;
            }

            if (yyyymm >= 0)
            {
                return 19;
            }

            return 0;
        }
    }

    // public static class IcdHelper
    // {
    //     public static uint GetBitUReverse(byte[] buff, uint pos, uint len)
    //     {
    //         uint bits = 0;
    //         for (var i = (int)(pos + len) - 1; i >= pos; i--)
    //             bits = (uint)((bits << 1) + ((buff[i / 8] >> 7 - i % 8) & 1u));
    //         return bits;
    //     }
    //
    //     public static byte[] GetGpsRawSubFrame(uint[] buffer, uint offsetBits, out uint tow, out byte sfNum)
    //     {
    //         byte sync = 0x8B;
    //
    //
    //         var bitIndex = offsetBits + 6;
    //         var preamb = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex +16 , 8); bitIndex += 24 + 2 + 6;
    //         // if (preamb != sync) throw new Exception($"Preamb = 0x{preamb:X2}. Sync = 0x{sync:X2}");
    //         tow = RtcmV3Helper.GetBitU(buffer, bitIndex, 17); bitIndex += 17 + 2;
    //         sfNum = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 3); bitIndex += 3 + 2 + 2; // 64 bit
    //         var result = new byte[24];
    //         for (var i = 0; i < 8; i++)
    //         {
    //             bitIndex += 6;
    //             result[i * 3] = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 8); bitIndex += 8;
    //             result[i * 3 + 1] = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 8); bitIndex += 8;
    //             result[i * 3 + 2] = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 8); bitIndex += 8;
    //             bitIndex += 2;
    //         }
    //
    //         return result;
    //     }
    //
    //     public static byte[] GetGlonassRawSubFrame(byte[] buffer, uint offsetBits, out uint tow, out byte sfNum)
    //     {
    //         byte sync = 0x8B;
    //         var bitIndex = offsetBits + 6;
    //         var preamb = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 8); bitIndex += 24 + 2 + 6;
    //         if (preamb != sync) throw new Exception($"Preamb = 0x{preamb:X2}. Sync = 0x{sync:X2}");
    //         tow = RtcmV3Helper.GetBitU(buffer, bitIndex, 17); bitIndex += 17 + 2;
    //         sfNum = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 3); bitIndex += 3 + 2 + 2; // 64 bit
    //         var result = new byte[24];
    //         for (var i = 0; i < 8; i++)
    //         {
    //             bitIndex += 6;
    //             result[i * 3] = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 8); bitIndex += 8;
    //             result[i * 3 + 1] = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 8); bitIndex += 8;
    //             result[i * 3 + 2] = (byte)RtcmV3Helper.GetBitU(buffer, bitIndex, 8); bitIndex += 8;
    //             bitIndex += 2;
    //         }
    //
    //         return result;
    //     }
    // }
}
