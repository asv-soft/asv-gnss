using System;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a base class for GLONASS words.
    /// </summary>
    public abstract class GlonassWordBase
    {
        /// <summary>
        /// Gets or sets the WordId property.
        /// </summary>
        /// <value>
        /// The WordId property.
        /// </value>
        public virtual byte WordId { get; protected set; }

        /// <summary>
        /// Deserializes the given byte array.
        /// </summary>
        /// <param name="data">The byte array to deserialize.</param>
        /// <exception cref="Exception">Thrown when the length of the data array is not 12 bytes.</exception>
        public virtual void Deserialize(byte[] data)
        {
            if (data.Length != 11)
            {
                throw new Exception(
                    $"Length of {nameof(data)} array must be 85 bit (~ 11 bytes)  (as Glonass ICD word length )"
                );
            }

            var wordId = (byte)GpsRawHelper.GetBitU(data, 4, 4);
            if (wordId > 5)
            {
                WordId = wordId;
            }

            CheckWordId(wordId);
            Array.Copy(data, RawData, 11);
        }

        /// <summary>
        /// Checks if the given word ID matches the expected WordId.
        /// </summary>
        /// <param name="wordId">The word ID to check.</param>
        protected virtual void CheckWordId(byte wordId)
        {
            if (wordId != WordId)
            {
                throw new Exception($"Word ID not equals: want {WordId}. Got {wordId}");
            }
        }

        public byte[] RawData { get; } = new byte[11];

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the current object in JSON format.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
