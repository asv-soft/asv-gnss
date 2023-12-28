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

        /// <summary>
        /// Retrieves the week number and number of seconds from a given DateTime
        /// </summary>
        /// <param name="time">The DateTime value for which the week number and number of seconds are to be obtained</param>
        /// <param name="week">A reference to an integer variable that will hold the week number</param>
        /// <param name="seconds">A reference to a double variable that will hold the number of seconds</param>
        public static void GetFromTime(DateTime time, ref int week, ref double seconds)
        {
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            var dif = time - datum;

            var weeks = (int)(dif.TotalDays / 7);

            week = weeks;

            dif = time - datum.AddDays(weeks * 7);

            seconds = dif.TotalSeconds;
        }

        /// <summary>
        /// Converts the given week number and seconds to a DateTime object in UTC time zone.
        /// </summary>
        /// <param name="weeknumber">The week number.</param>
        /// <param name="seconds">The seconds value.</param>
        /// <returns>A DateTime object representing the specified week number and seconds in UTC.</returns>
        public static DateTime GetFromUtc(int weeknumber, double seconds)
        {

            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var week = datum.AddDays(weeknumber * 7);
            var time = week.AddSeconds(seconds);
            return time;
        }

        /// <summary>
        /// Retrieves the word ID from the specified array of unsigned integers.
        /// </summary>
        /// <param name="navBits">The array of unsigned integers representing the navigation bits.</param>
        /// <returns>The word ID extracted from the navigation bits.</returns>
        /// <exception cref="Exception">
        /// Thrown when the length of the <paramref name="navBits"/> array is not equal to 3 u32 words,
        /// or when the value of bits 85 is not 0 (as per GLONASS ICD superframe structure).
        /// </exception>
        public static byte GetWordId(uint[] navBits)
        {
            if (navBits.Length != 3) throw new Exception($"Length of {nameof(navBits)} array must be 3 u32 word (as GLONASS ICD word length)");
            if (navBits[0] >> 31 != 0) throw new Exception("Bits 85 must be 0 (as GLONASS ICD superframe structure)");
            return (byte)((navBits[0] >> 27) & 0xF); // 27 bits offset, 4 bit
        }

        /// <summary>
        /// Retrieves the bits from a byte array starting at a specified position.
        /// </summary>
        /// <param name="buff">The byte array from which to retrieve the bits.</param>
        /// <param name="pos">The starting position within the byte array.</param>
        /// <param name="len">The number of bits to retrieve.</param>
        /// <returns>
        /// The retrieved bits as an unsigned integer.
        /// </returns>
        public static uint GetBitU(byte[] buff, uint pos, uint len)
        {
            uint bits = 0;
            uint i;
            for (i = pos; i < pos + len; i++)
                bits = (uint)((bits << 1) + ((buff[i / 8] >> (int)(7 - i % 8)) & 1u));
            return bits;
        }

        /// <summary>
        /// Sets bits in a byte array at specified position for specified length using data.
        /// </summary>
        /// <param name="buff">The byte array to modify.</param>
        /// <param name="pos">The starting position to set bits.</param>
        /// <param name="len">The length of bits to set.</param>
        /// <param name="data">The data to set in the byte array.</param>
        /// <remarks>
        /// The method sets bits in the given byte array at the specified position.
        /// It uses the data to set the bits.
        /// The position must be within the range of the byte array.
        /// The length of bits must be greater than 0 and less or equal to 32.
        /// The method modifies the input byte array directly.
        /// </remarks>
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

        /// <summary>
        /// Convert an array of 32-bit unsigned integers into a byte array.
        /// </summary>
        /// <param name="navBits">The array of 32-bit unsigned integers containing the data.</param>
        /// <returns>A byte array representing the raw data.</returns>
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
