using System;

namespace Asv.Gnss
{
    public static class GpsRawHelper
    {

        /// <summary>
        /// 2^-31
        /// </summary>
        public const double P2_31 = 4.656612873077393E-10;
        /// <summary>
        /// 2^-55
        /// </summary>
        public const double P2_55 = 2.775557561562891E-17;
        /// <summary>
        /// 2^-43
        /// </summary>
        public const double P2_43 = 1.136868377216160E-13;


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
            if (navBits.Length != 10) throw new Exception($"Length of {nameof(navBits)} array must be 10 u32 word (as GPS ICD subframe length)");
            var result = new byte[30];
            for (int i = 0; i < navBits.Length; i++)
            {
                var value = (navBits[i] >> 6) & 0xFF_FFFF; // skip 6 parity bits and get 24 data
                result[i * 3 + 0] = (byte)((value >> 16)  & 0xFF);
                result[i * 3 + 1] = (byte)((value >> 8) & 0xFF);
                result[i * 3 + 2] = (byte)(value & 0xFF);
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
                bits = (uint)((bits << 1) + ((buff[i / 8] >> (int)(7 - i % 8)) & 1u));
            return bits;
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
        /// Preamble: 0b10001011 or 0x8B
        /// </summary>
        /// <param name="navBits"></param>
        /// <returns></returns>
        public static byte GetPreamble(uint[] navBits)
        {
            if (navBits.Length != 10) throw new Exception($"Length of {nameof(navBits)} array must be 10 u32 word (as GPS ICD subframe length)");
            return (byte) ((navBits[0] >> 22)& 0XFF);
        }
        /// <summary>
        /// The HOW begins with the 17 MSBs of the time-of-week(TOW) count. (The full TOW count consists of the 19 LSBs of the 29-
        /// bit Z-count). These 17 bits correspond to the TOW-count at the 1.5 second epoch which occurs
        /// at the start(leading edge) of the next following subframe(reference paragraph 2.3.5)
        /// </summary>
        /// <param name="navBits"></param>
        /// <returns></returns>
        public static uint GetTow15epoch(uint[] navBits)
        {
            if (navBits.Length != 10) throw new Exception($"Length of {nameof(navBits)} array must be 10 u32 word (as GPS ICD subframe length)");
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
            if (navBits.Length != 10) throw new Exception($"Length of {nameof(navBits)} array must be 10 u32 word (as GPS ICD subframe length)");
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
                    throw new Exception($"Unknown GPS subframe ID:{Convert.ToString(subframeId, 2).PadRight(8)}");
            }
        }

        // public static int SubFrameId(UInt32[] navBits)
        // {
        //     return (navBits[1]>>)
        // }
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
