using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a static class for calculating the CRC value for NMEA data.
    /// </summary>
    public static class NmeaCrc
    {
        /// <summary>
        /// Calculates the CRC checksum for the given buffer of bytes.
        /// </summary>
        /// <param name="buffer">The buffer of bytes to calculate the CRC checksum for.</param>
        /// <returns>The calculated CRC checksum as a hexadecimal string.</returns>
        public static string Calc(ReadOnlySpan<byte> buffer)
        {
            var crc = 0;
            foreach (var c in buffer)
            {
                if (crc == 0)
                {
                    crc = c;
                }
                else
                {
                    crc ^= c;
                }
            }

            return crc.ToString("X2");
        }
    }
}
