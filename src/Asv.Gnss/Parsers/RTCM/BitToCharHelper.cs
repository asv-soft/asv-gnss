using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    public static class BitToCharHelper
    {
        /// <summary>
        /// string from bit array
        /// </summary>
        public static string BitArrayToString(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int count
        )
        {
            var byteArr = new byte[count];
            for (int i = 0; i < count; i++)
            {
                var b = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
                byteArr[i] = b;
            }
            return Encoding.GetEncoding("ISO-8859-1").GetString(byteArr);
        }
    }
}
