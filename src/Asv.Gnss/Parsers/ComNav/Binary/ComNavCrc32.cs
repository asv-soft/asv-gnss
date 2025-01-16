using System;

namespace Asv.Gnss
{
    /// <summary>
    /// SourceName https://docs.novatel.com/OEM7/Content/Messages/32_Bit_CRC.htm.
    /// </summary>
    public static class ComNavCrc32
    {
        /// <summary>
        /// The polynomial used for calculating CRC32 checksums.
        /// </summary>
        private const uint _crc32Polynomial = 0xEDB88320;

        /// <summary>
        /// Computes the CRC32 value for a given input.
        /// </summary>
        /// <param name="i">The input value to compute the CRC32 for.</param>
        /// <returns>The computed CRC32 value.</returns>
        private static uint Crc32Value(uint i)
        {
            int j;
            var ulCrc = i;
            for (j = 8; j > 0; j--)
            {
                if ((ulCrc & 1) != 0)
                {
                    ulCrc = (ulCrc >> 1) ^ _crc32Polynomial;
                }
                else
                {
                    ulCrc >>= 1;
                }
            }

            return ulCrc;
        }

        /// <summary>
        /// Calculates the CRC32 value of a given byte array using the specified seed and count.
        /// </summary>
        /// <param name="buffer">The byte array to calculate the CRC32 value for.</param>
        /// <param name="seed">The starting position in the byte array.</param>
        /// <param name="count">The number of bytes to calculate the CRC32 for.</param>
        /// <returns>The calculated CRC32 value.</returns>
        public static uint Calc(byte[] buffer, int seed, int count)
        {
            uint ulCrc = 0;

            for (var i = 0; i < count; i++)
            {
                var ulTemp1 = (ulCrc >> 8) & 0x00FFFFFF;

                var ulTemp2 = Crc32Value((ulCrc ^ buffer[seed + i]) & 0xFF);

                ulCrc = ulTemp1 ^ ulTemp2;
            }

            return ulCrc;
        }

        /// <summary>
        /// Calculates the CRC32 checksum of a given buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to calculate the CRC32 checksum for.</param>
        /// <param name="count">The number of bytes in the buffer to calculate the CRC32 checksum for.</param>
        /// <returns>The calculated CRC32 checksum value.</returns>
        public static uint Calc(ReadOnlySpan<byte> buffer, int count)
        {
            uint ulCrc = 0;

            for (var i = 0; i < count; i++)
            {
                var ulTemp1 = (ulCrc >> 8) & 0x00FFFFFF;

                var ulTemp2 = Crc32Value((ulCrc ^ buffer[i]) & 0xFF);

                ulCrc = ulTemp1 ^ ulTemp2;
            }

            return ulCrc;
        }
    }
}
