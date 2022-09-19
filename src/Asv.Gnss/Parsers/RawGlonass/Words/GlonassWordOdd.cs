using System;

namespace Asv.Gnss
{
    public class GlonassWordOdd : GlonassWordBase
    {
        protected override void CheckWordId(byte wordId)
        {
            if (wordId <= 5 || wordId % 2 != 1) throw new Exception($"Word ID not equals: Word want > 5 and odd number. Got {wordId}");
        }

        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            // var bitIndex = 8U;
        }
    }
}