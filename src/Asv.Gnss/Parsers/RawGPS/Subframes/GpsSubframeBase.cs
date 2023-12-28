using System;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    /// <summary>
    /// Base class for GPS subframes.
    /// </summary>
    public abstract class GpsSubframeBase
    {
        /// <summary>
        /// Gets or sets the TOW1_5Epoh property.
        /// </summary>
        /// <value>
        /// The TOW1_5Epoh property represents a uint value.
        /// </value>
        public uint TOW1_5Epoh { get; set; }

        /// <summary>
        /// Gets the identifier for the subframe.
        /// </summary>
        /// <remarks>
        /// The SubframeId property returns a byte value corresponding to the identifier
        /// of the subframe. This identifier uniquely identifies the subframe within
        /// its parent frame.
        /// </remarks>
        /// <value>
        /// A byte value representing the identifier for the subframe.
        /// </value>
        public abstract byte SubframeId { get; }

        /// <summary>
        /// Deserialize method deserializes the given byte array into data fields of the object.
        /// </summary>
        /// <param name="dataWithoutParity">The byte array containing the data to be deserialized.</param>
        /// <exception cref="Exception">Thrown when the length of the input array is not equal to 30 bytes or when the preamble does not match the expected value or when the subframe ID does not match the expected value.</exception>
        public virtual void Deserialize(byte[] dataWithoutParity)
        {
            if (dataWithoutParity.Length != 30) throw new Exception($"Length of {nameof(dataWithoutParity)} array must be 24 bit x 10 word = 30 bytes  (as GPS ICD subframe length )");
            if (dataWithoutParity[0] != GpsRawHelper.GpsSubframePreamble) throw new Exception($"Preamble error. Want {GpsRawHelper.GpsSubframePreamble}. Got {dataWithoutParity[0]}");
            TOW1_5Epoh = GpsRawHelper.GetBitU(dataWithoutParity, 24, 17); // 2-nd word 1-17 bit
            var subframeId = GpsRawHelper.GetSubframeId((byte)GpsRawHelper.GetBitU(dataWithoutParity, 24 + 19, 3));
            if (subframeId != SubframeId) throw new Exception($"Subframe ID not equals: want {SubframeId}. Got {subframeId}");
        }

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}