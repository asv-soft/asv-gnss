using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a static class for calculating the CRC16 checksum.
    /// </summary>
    public static class UbxCrc16
    {
        /// <summary>
        /// Calculates the CRC-16 checksum for the given byte array.
        /// </summary>
        /// <param name="buff">The byte array to calculate the checksum for.</param>
        /// <returns>
        /// A tuple containing the calculated checksum values.
        /// The first value is the CRC-16 checksum of the input bytes (Crc1),
        /// and the second value is an extended checksum (Crc2).
        /// </returns>
        public static (byte Crc1, byte Crc2) Calc(ReadOnlySpan<byte> buff)
        {
            uint a = 0x00;
            uint b = 0x00;
            var i = 0;
            while (i < buff.Length)
            {
                a += buff[i++];
                b += a;
            }

            return (Crc1: (byte)(a & 0xFF), Crc2: (byte)(b & 0xFF));
        }
    }
}
