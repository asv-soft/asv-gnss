using System;
using System.IO;
using System.Text;
using Xunit;

namespace Asv.Gnss.Test
{
    public class BitToCharTest
    {
        public static string GetRandomAlphaNumeric()
        {
            return Path.GetRandomFileName().Replace(".", "");
        }

        [Fact]
        public void TestBitToString()
        {
            var rndStr = GetRandomAlphaNumeric();
            var byteArr = Encoding.GetEncoding("ISO-8859-1").GetBytes(rndStr);
            var ind = 0;
            var len = rndStr.Length;
            var nextInd = (ind + len) * 8;
            var convertedStr = BitToCharHelper.BitArrayToString(byteArr, ref ind, len);

            Assert.NotNull(convertedStr);
            Assert.Equal(rndStr, convertedStr);
            Assert.Equal(nextInd, ind);

            var random = new Random();
            rndStr = GetRandomAlphaNumeric();
            ind = random.Next(0, rndStr.Length - 1);
            len = random.Next(0, rndStr.Length - ind);
            var bitInd = ind * 8;
            byteArr = Encoding.GetEncoding("ISO-8859-1").GetBytes(rndStr);

            convertedStr = BitToCharHelper.BitArrayToString(byteArr, ref bitInd, len);
            Assert.NotNull(convertedStr);
            Assert.Equal(rndStr.Substring(ind, len), convertedStr);
            Assert.Equal((ind + len) * 8, bitInd);
        }
    }
}

