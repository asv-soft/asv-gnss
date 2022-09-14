using System;

namespace Asv.Gnss
{
    public static class NmeaCrc
    {
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