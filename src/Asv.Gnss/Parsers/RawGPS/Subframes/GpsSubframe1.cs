using System;
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
            var word1Start = 24U * 2;
            WeekNumber = (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 10); word1Start += 10;
            Code = (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 2); word1Start += 2;
            SatelliteAccuracy = (byte) GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 4); word1Start += 4;
            SatelliteHealth = (byte) GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 6); word1Start += 6;
            iodc = 0;
            iodc |= (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 2) << 8;  word1Start += 2;
            Flag = (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 1); word1Start += 24U * 3 + 1 + 15;
            var tgd = GpsRawHelper.GetBitS(dataWithoutParity, word1Start, 8); word1Start += 8;
            iodc |= (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 8); word1Start += 8;
            TocSec = GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 16) * 16.0; word1Start += 16;
            Af2 = GpsRawHelper.GetBitS(dataWithoutParity, word1Start, 8) * GpsRawHelper.P2_55; word1Start += 8;
            Af1 = GpsRawHelper.GetBitS(dataWithoutParity, word1Start, 16) * GpsRawHelper.P2_43; word1Start += 16;
            Af0 = GpsRawHelper.GetBitS(dataWithoutParity, word1Start, 22) * GpsRawHelper.P2_31; word1Start += 24;

            Tgd[0] = tgd == -128 ? 0.0 : tgd * GpsRawHelper.P2_31;
        }

        public DateTime Toc { get; set; }

        public int Flag { get; set; }

        public int Code { get; set; }
        public double Af0 { get; set; }
        public double Af1 { get; set; }
        public double Af2 { get; set; }
        public double TocSec { get; set; }
        public int iodc { get; set; }
        
        public double[] Tgd { get; set; } = new double[1];
        
        /// <summary>
        /// Gets or sets the satellite accuracy in byte value.
        /// </summary>
        public byte SatelliteAccuracy { get; set; }
        
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
        public byte SatelliteHealth { get; set; }
        
        /// <summary>
        /// Gets or sets the week number.
        /// </summary>
        /// <value>
        /// The week number.
        /// </value>
        public int WeekNumber { get; set; }

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