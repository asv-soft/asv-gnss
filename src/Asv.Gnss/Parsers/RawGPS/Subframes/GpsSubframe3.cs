namespace Asv.Gnss
{
    /// <summary>
    /// Represents a GPS Subframe 3.
    /// </summary>
    public class GpsSubframe3 : GpsSubframeBase
    {
        /// <summary>
        /// Gets the Id of the subframe.
        /// </summary>
        /// <value>
        /// The Id of the subframe.
        /// </value>
        public override byte SubframeId => 3;

        /// <summary>
        /// Deserializes the given data without parity.
        /// </summary>
        /// <param name="dataWithoutParity">The data to be deserialized.</param>
        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

        }
    }
}