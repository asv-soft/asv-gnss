using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Source https://docs.novatel.com/OEM7/Content/Messages/32_Bit_CRC.htm
    /// </summary>
    public static class ComNavCrc32
    {
        private static uint _crc32Polynomial = 0xEDB88320;

        private static uint Crc32Value(uint i)
        {
            int j;
            var ulCrc = i;
            for (j = 8; j > 0; j--)
            {
                if ((ulCrc & 1) != 0)
                    ulCrc = (ulCrc >> 1) ^ _crc32Polynomial;
                else
                    ulCrc >>= 1;
            }
            return ulCrc;
        }
        
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