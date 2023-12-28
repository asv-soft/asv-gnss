namespace Asv.Gnss
{
    /// <summary>
    /// Represents the GPS SubFrame 5.
    /// </summary>
    public class GPSSubFrame5 : GpsSubframeBase
    {
        /// <summary>
        /// Gets the Subframe Id.
        /// </summary>
        /// <value>
        /// The Subframe Id value.
        /// </value>
        public override byte SubframeId => 5;

        /// <summary>
        /// Deserializes the provided data without parity.
        /// </summary>
        /// <param name="dataWithoutParity">The data to be deserialized.</param>
        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

        }
    }
}