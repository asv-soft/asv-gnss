using System;

namespace Asv.Gnss
{
    public static class GlonassRawHelper
    {
        /// <summary>
        /// 2^-11
        /// </summary>
        public const double P2_11 = 4.882812500000000E-04;

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

        public static void GetFromTime(DateTime time, ref int week, ref double seconds)
        {
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            var dif = time - datum;

            var weeks = (int)(dif.TotalDays / 7);

            week = weeks;

            dif = time - datum.AddDays(weeks * 7);

            seconds = dif.TotalSeconds;
        }

        public static DateTime GetFromUtc(int weeknumber, double seconds)
        {

            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var week = datum.AddDays(weeknumber * 7);
            var time = week.AddSeconds(seconds);
            return time;
        }

        public static byte GetWordId(uint[] navBits)
        {
            if (navBits.Length != 3) throw new Exception($"Length of {nameof(navBits)} array must be 3 u32 word (as GLONASS ICD word length)");
            if (navBits[0] >> 31 != 0) throw new Exception("Bits 85 must be 0 (as GLONASS ICD superframe structure)");
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
            SetBitU(result, bitIndex, 32, navBits[0]); bitIndex += 32;
            SetBitU(result, bitIndex, 32, navBits[1]); bitIndex += 32;
            SetBitU(result, bitIndex, 21, navBits[2] >> 11); bitIndex += 21;
            return result;
        }
    }
}
