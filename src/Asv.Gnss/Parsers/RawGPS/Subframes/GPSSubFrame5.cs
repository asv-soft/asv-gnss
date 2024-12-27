using System;

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

        public enum Subframe5Type
        {
            Almanac,
            ToaAndHealth,
            None,
        }

        /// <summary>
        /// Deserializes the provided data without parity.
        /// </summary>
        /// <param name="dataWithoutParity">The data to be deserialized.</param>
        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

            var word4Start = 24U * 2;

            var dataId = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 2);
            word4Start += 2;

            // GPS data/ If dataId == 3 => QZSS data
            if (dataId != 1)
            {
                Type = Subframe5Type.None;
                return;
            }

            // page number
            var svid = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 6);
            word4Start += 6;

            // page 1-24
            if (svid is >= 1 and <= 24)
            {
                Type = Subframe5Type.Almanac;
                SatelliteNumber = svid;
                e = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16) * GpsRawHelper.P2_21;
                word4Start += 16;
                ToaSec = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8) * 4096.0;
                word4Start += 8;
                i0 =
                    (
                        0.3
                        + GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16)
                            * GpsRawHelper.P2_19
                    ) * GpsRawHelper.SC2RAD;
                word4Start += 16;
                OMGd =
                    GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16)
                    * GpsRawHelper.P2_38
                    * GpsRawHelper.SC2RAD;
                word4Start += 16;
                Health = new[] { (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8) };
                word4Start += 8;
                A = Math.Pow(
                    GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 24) * GpsRawHelper.P2_11,
                    2
                );
                word4Start += 24;
                OMG0 =
                    GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24)
                    * GpsRawHelper.P2_23
                    * GpsRawHelper.SC2RAD;
                word4Start += 24;
                omg =
                    GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24)
                    * GpsRawHelper.P2_23
                    * GpsRawHelper.SC2RAD;
                word4Start += 24;
                M0 =
                    GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24)
                    * GpsRawHelper.P2_23
                    * GpsRawHelper.SC2RAD;
                word4Start += 24;
                var af0 = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
                word4Start += 8;
                Af1 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 11) * GpsRawHelper.P2_38;
                word4Start += 11;
                Af0 =
                    GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 3) * GpsRawHelper.P2_17
                    + af0 * GpsRawHelper.P2_20;
                return;
            }

            /* page 25 */
            if (svid == 0x33)
            {
                Type = Subframe5Type.ToaAndHealth;
                ToaSec = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8) * 4096.0;
                word4Start += 8;
                Week = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
                word4Start += 8;

                /* decode sv health */
                Health = new int[24];
                for (var sat = 1; sat <= 24; sat++)
                {
                    Health[sat - 1] = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 6);
                    word4Start += 6;
                }

                return;
            }

            Type = Subframe5Type.None;
        }

        public Subframe5Type Type { get; set; }

        /// <summary>
        /// Gets or sets satellite number.
        /// </summary>
        public int SatelliteNumber { get; set; }

        /// <summary>
        /// Gets or sets sv health (0:ok).
        /// For Type=Almanac, length=1 (current satellite)
        /// For Type=ToaAndHealth, length=24 Sv [1:24].
        /// </summary>
        public int[] Health { get; set; }

        /// <summary>
        /// Gets or sets gPS/QZS: gps week for all 32 satellites.
        /// </summary>
        public int Week { get; set; }

        #region SV orbit parameters

        public double A { get; set; }
        public double e { get; set; }
        public double i0 { get; set; }
        public double OMG0 { get; set; }
        public double omg { get; set; }
        public double M0 { get; set; }
        public double OMGd { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets almanac time (s) in week.
        /// </summary>
        public double ToaSec { get; set; }

        /// <summary>
        /// Gets or sets sV clock parameters af0.
        /// </summary>
        public double Af0 { get; set; }

        /// <summary>
        /// Gets or sets sV clock parameters af1.
        /// </summary>
        public double Af1 { get; set; }
    }
}
