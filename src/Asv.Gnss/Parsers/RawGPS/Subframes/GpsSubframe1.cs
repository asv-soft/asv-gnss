using Newtonsoft.Json;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a GPS subframe 1.
    /// </summary>
    public class GpsSubframe1: GpsSubframeBase
    {
        /// <summary>
        /// Gets the subframe identifier.
        /// </summary>
        /// <value>
        /// The subframe identifier.
        /// </value>
        public override byte SubframeId => 1;

        /// <summary>
        /// Deserialize the data to extract satellite information.
        /// </summary>
        /// <param name="dataWithoutParity">The byte array containing the data without parity.</param>
        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);
            var word3Start = 24U*2;
            WeekNumber = GpsRawHelper.GetBitU(dataWithoutParity, word3Start, 10);
            SatteliteAccuracy = (byte) GpsRawHelper.GetBitU(dataWithoutParity, word3Start + 13, 4);
            SatteliteHealth = (byte) GpsRawHelper.GetBitU(dataWithoutParity, word3Start + 17, 6);

            
            
            // IODC = (GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 10));
            // startWord2 += 10;
            // TOC = (GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 16)) * 16;
            // startWord2 += 16;
            // Af2 = ((sbyte) GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 8) * GpsRawHelper.P2_55);
            // startWord2 += 8;
            // Af1 = ((sbyte) GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 16) * GpsRawHelper.P2_43);
            // startWord2 += 16;
            // Af0 = ((sbyte) GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 22) * GpsRawHelper.P2_31);
            // startWord2 += 22;
        }

        // public double Af0 { get; set; }
        // public double Af1 { get; set; }
        // public double Af2 { get; set; }
        // public uint TOC { get; set; }
        // public uint IODC { get; set; }
        //
        /// <summary>
        /// Gets or sets the satellite accuracy in byte value.
        /// </summary>
        public byte SatteliteAccuracy { get; set; }

        /// <summary>
        /// Gets or sets the health status of the satellite.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The health status is represented as a byte value.
        /// </para>
        /// <para>
        /// The value ranges from 0 to 255, where 0 represents a completely malfunctioning satellite
        /// and 255 represents a perfectly functioning satellite.
        /// </para>
        /// </remarks>
        /// <value>
        /// The health status of the satellite.
        /// </value>
        public byte SatteliteHealth { get; set; }

        /// <summary>
        /// Gets or sets the week number.
        /// </summary>
        /// <value>
        /// The week number.
        /// </value>
        public uint WeekNumber { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}