using System;

namespace Asv.Gnss
{
    /// <summary>
    /// This class inherits from the GlonassWordBase class and represents a Glonass word with an odd word ID.
    /// </summary>
    public class GlonassWordOdd : GlonassWordBase
    {
        /// <summary>
        /// Checks if the word ID meets the specified criteria.
        /// </summary>
        /// <param name="wordId">The word ID to be checked.</param>
        protected override void CheckWordId(byte wordId)
        {
            if (wordId <= 5 || wordId % 2 != 1) throw new Exception($"Word ID not equals: Word want > 5 and odd number. Got {wordId}");
        }

        /// <summary>
        /// Deserializes the provided byte array.
        /// </summary>
        /// <param name="data">The byte array containing the serialized data.</param>
        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            // var bitIndex = 8U;
        }
    }
}