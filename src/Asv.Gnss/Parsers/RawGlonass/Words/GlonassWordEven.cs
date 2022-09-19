using System;

namespace Asv.Gnss
{
    public class GlonassWordEven : GlonassWordBase
    {
        protected override void CheckWordId(byte wordId)
        {
            if (wordId <= 5 || wordId % 2 != 0) throw new Exception($"Word ID not equals: Word want > 5 and even number. Got {wordId}");
        }

        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            // var bitIndex = 8U;
        }
    }
}