using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a derived class of GlonassWordBase that checks for even word IDs greater than 5 during deserialization.
    /// </summary>
    public class GlonassWordEven : GlonassWordBase
    {
        /// <summary>
        /// Checks the word ID to ensure it meets certain criteria.
        /// </summary>
        /// <param name="wordId">The word ID to be checked.</param>
        /// <exception cref="Exception">Thrown when the word ID is less than or equal to 5 or not an even number.</exception>
        protected override void CheckWordId(byte wordId)
        {
            if (wordId <= 5 || wordId % 2 != 0) throw new Exception($"Word ID not equals: Word want > 5 and even number. Got {wordId}");
        }

        /// <summary>
        /// Deserializes the provided byte array and populates the object's state.
        /// </summary>
        /// <param name="data">The byte array containing the serialized data.</param>
        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            // var bitIndex = 8U;
        }
    }
}