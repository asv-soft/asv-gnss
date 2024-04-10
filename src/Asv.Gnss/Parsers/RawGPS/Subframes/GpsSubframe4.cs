using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents the GPS Subframe 4.
    /// </summary>
    public class GpsSubframe4 : GpsSubframeBase
    {
        public enum Subframe4Type
        {
            Almanac,
            IonoAndUtcData,
            HealthAndConfigurations,
            None
        }

        public class Ionospheric
        {
            public double Alpha0 { get; set; }
            public double Alpha1 { get; set; }
            public double Alpha2 { get; set; }
            public double Alpha3 { get; set; }

            public double Betta0 { get; set; }
            public double Betta1 { get; set; }
            public double Betta2 { get; set; }
            public double Betta3 { get; set; }
        }

        public class UtcGpsRelationship
        {
            /// <summary>
            /// constant of polynomial
            /// </summary>
            public double A0 { get; set; }

            /// <summary>
            /// first order terms of polynomial;
            /// </summary>
            public double A1 { get; set; }

            /// <summary>
            /// reference time for UTC data
            /// </summary>
            public double Tot { get; set; }

            /// <summary>
            /// UTC reference week number
            /// </summary>
            public int WNt { get; set; }

            /// <summary>
            /// delta time due to leap seconds
            /// </summary>
            public double DeltaT_LS { get; set; }

            /// <summary>
            /// UTC/GPS-time relationship
            /// </summary>
            /// <param name="te">GPS time as estimated by the user on the basis of correcting tsv for factors
            /// described in paragraph 2.5.5.2 (SPS) as well as for ionospheric and SA (dither) effects</param>
            /// <param name="wn">Current week number (derived from subframe 1)</param>
            /// <returns></returns>
            public double GetUtc(double te, int wn)
            {
                var deltaTUtc = DeltaT_LS + A0 + A1 * (te - Tot + 604800 * (wn - WNt));
                GpsRawHelper.Gps2Time(wn, deltaTUtc % 604800.0);
                return (te - deltaTUtc) % 86400.0;
            }
        }

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
            var word4Start = 24U * 2;

            var dataId = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 2);
            word4Start += 2;

            if (dataId != 1) // GPS data/ If dataId == 3 => QZSS data
            {
                Type = Subframe4Type.None;
                return;
            }

            // page number
            var svid = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 6);
            word4Start += 6;

            if (svid is >= 25 and <= 32) // page 2,3,4,5,7,8,9,10
            {
                Type = Subframe4Type.Almanac;
                SatelliteNumber = svid;
                e = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16) * GpsRawHelper.P2_21;
                word4Start += 16;
                ToaSec = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8) * 4096.0;
                word4Start += 8;
                i0 = (0.3 + GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16) * GpsRawHelper.P2_19) *
                     GpsRawHelper.SC2RAD;
                word4Start += 16;
                OMGd = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16) * GpsRawHelper.P2_38 *
                       GpsRawHelper.SC2RAD;
                word4Start += 16;
                Health = new[] { (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8) };
                word4Start += 8;
                A = Math.Pow(GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 24) * GpsRawHelper.P2_11, 2);
                word4Start += 24;
                OMG0 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24) * GpsRawHelper.P2_23 *
                       GpsRawHelper.SC2RAD;
                word4Start += 24;
                omg = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24) * GpsRawHelper.P2_23 *
                      GpsRawHelper.SC2RAD;
                word4Start += 24;
                M0 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24) * GpsRawHelper.P2_23 * GpsRawHelper.SC2RAD;
                word4Start += 24;
                var af0 = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
                word4Start += 8;
                Af1 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 11) * GpsRawHelper.P2_38;
                word4Start += 11;
                Af0 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 3) * GpsRawHelper.P2_17 +
                      af0 * GpsRawHelper.P2_20;

                return;
            }

            if (svid == 0x3F) // Page 25
            {
                Type = Subframe4Type.HealthAndConfigurations;

                /* decode as and sv config */
                SvConfig = new int[32];
                for (var sat = 1; sat <= 32; sat++)
                {
                    SvConfig[sat - 1] = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 4);
                    word4Start += 4;
                }

                word4Start += 2;
                /* decode sv health */
                Health = new int[8];
                for (var sat = 25; sat <= 32; sat++)
                {
                    Health[sat - 25] = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 6);
                    word4Start += 6;
                }

                return;
            }

            if (svid == 0x38) // Page 18
            {
                Type = Subframe4Type.IonoAndUtcData;
                Iono = new Ionospheric();
                Iono.Alpha0 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * GpsRawHelper.P2_30;
                word4Start += 8;
                Iono.Alpha1 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * GpsRawHelper.P2_27;
                word4Start += 8;
                Iono.Alpha2 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * GpsRawHelper.P2_24;
                word4Start += 8;
                Iono.Alpha3 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * GpsRawHelper.P2_24;
                word4Start += 8;

                Iono.Betta0 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 11);
                word4Start += 8;
                Iono.Betta1 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 14);
                word4Start += 8;
                Iono.Betta2 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 16);
                word4Start += 8;
                Iono.Betta3 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 16);
                word4Start += 8;

                UtcGps = new UtcGpsRelationship();

                UtcGps.A1 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24) * GpsRawHelper.P2_50;
                word4Start += 24;
                UtcGps.A0 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 32) * GpsRawHelper.P2_30;
                word4Start += 32;
                UtcGps.Tot = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 12);
                word4Start += 8;
                UtcGps.WNt = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
                word4Start += 8;
                UtcGps.DeltaT_LS = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
                word4Start += 8;

                return;
            }

            Type = Subframe4Type.None;
        }

        public Subframe4Type Type { get; set; }

        public Ionospheric Iono { get; set; }


        /// <summary>
        /// UTC/GPS-time relationship
        /// </summary>
        public UtcGpsRelationship UtcGps { get; set; }

        /// <summary>
        /// Satellite number
        /// </summary>
        public int SatelliteNumber { get; set; }

        /// <summary>
        /// sv health (0:ok).
        /// For Type=Almanac, length=1 (current satellite)
        /// For Type=HealthAndConfigurations, length=8 Sv [25:32]
        /// </summary>
        public int[] Health { get; set; }

        /// <summary>
        /// as and sv config. For Sv [1:32]
        /// </summary>
        public int[] SvConfig { get; set; }

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
        /// Almanac time (s) in week
        /// </summary>
        public double ToaSec { get; set; }

        /// <summary>
        /// SV clock parameters af0
        /// </summary>
        public double Af0 { get; set; }

        /// <summary>
        /// SV clock parameters af1
        /// </summary>
        public double Af1 { get; set; }
    }
}
