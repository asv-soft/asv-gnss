namespace Asv.Gnss
{
    /// <summary>
    /// Represents a GPS Subframe 2.
    /// </summary>
    public class GpsSubframe2 : GpsSubframeBase
    {
        /// <summary>
        /// Gets the subframe identifier.
        /// </summary>
        /// <value>
        /// The subframe identifier.
        /// </value>
        public override byte SubframeId => 2;

        /// <summary>
        /// Deserializes the given byte array without parity.
        /// </summary>
        /// <param name="dataWithoutParity">The byte array to deserialize.</param>
        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

        }
    }
}