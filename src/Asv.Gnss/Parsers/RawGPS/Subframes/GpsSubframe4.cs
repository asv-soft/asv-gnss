namespace Asv.Gnss
{
    /// <summary>
    /// Represents the GPS Subframe 4.
    /// </summary>
    public class GpsSubframe4 : GpsSubframeBase
    {
        /// <summary>
        /// Gets the ID of the subframe.
        /// </summary>
        /// <value>
        /// The ID of the subframe.
        /// </value>
        public override byte SubframeId => 4;

        /// <summary>
        /// Deserializes data without parity.
        /// </summary>
        /// <param name="dataWithoutParity">The byte array of data to be deserialized.</param>
        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

        }
    }
}