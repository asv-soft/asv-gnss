using System;

namespace Asv.Gnss
{
    public static class UbxCrc16
    {
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
