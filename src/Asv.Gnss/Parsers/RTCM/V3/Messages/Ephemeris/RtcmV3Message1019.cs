using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV3Message1019 : RtcmV3MessageBase
    {
        public const int RtcmMessageId = 1019;

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            var utc = DateTime.UtcNow;
            var sys = NavigationSystemEnum.SYS_GPS;

            var prn = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 6);
            var week = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 10);
            SvAccuracy = (int)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 4);
            CodeL2 = (int)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 2);
            Idot = SpanBitHelper.GetBitS(buffer,ref bitIndex, 14) * RtcmV3Helper.P2_43 * RtcmV3Helper.SC2RAD;
            Iode = (int)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 8);
            var toc = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 16) * 16.0;
            Af2 = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 8) * RtcmV3Helper.P2_55;
            Af1 = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 16) * RtcmV3Helper.P2_43;
            Af0 = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 22) * RtcmV3Helper.P2_31;
            Iodc = (int)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 10);
            Crs = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 16) * RtcmV3Helper.P2_5;
            Deln = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 16) * RtcmV3Helper.P2_43 * RtcmV3Helper.SC2RAD;
            M0 = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 32) * RtcmV3Helper.P2_31 * RtcmV3Helper.SC2RAD;
            Cuc = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 16) * RtcmV3Helper.P2_29;
            E = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 32) * RtcmV3Helper.P2_33;
            Cus = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 16) * RtcmV3Helper.P2_29;
            var sqrtA = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 32) * RtcmV3Helper.P2_19;
            Toes = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 16) * 16.0;
            Cic = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 16) * RtcmV3Helper.P2_29;
            Omg0 = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 32) * RtcmV3Helper.P2_31 * RtcmV3Helper.SC2RAD;
            Cis = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 16) * RtcmV3Helper.P2_29;
            I0 = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 32) * RtcmV3Helper.P2_31 * RtcmV3Helper.SC2RAD;
            Crc = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 16) * RtcmV3Helper.P2_5;
            Omg = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 32) * RtcmV3Helper.P2_31 * RtcmV3Helper.SC2RAD;
            OmgD = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 24) * RtcmV3Helper.P2_43 * RtcmV3Helper.SC2RAD;
            Tgd[0] = SpanBitHelper.GetBitS(buffer, ref  bitIndex, 8) * RtcmV3Helper.P2_31;
            SvHealth = (int)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 6);
            FlagL2PData = (int)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 1);
            Fit = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 1) == 1 ? 0.0 : 4.0; /* 0:4hr,1:>4hr */

            if (prn >= 40)
            {
                sys = NavigationSystemEnum.SYS_SBS;
                prn += 80;
            }

            var sat = RtcmV3Helper.satno(sys, (int)prn);

            if (sat == 0)
            {
                throw new Exception($"Rtcm3 1019 satellite number error: prn={prn}");
            }

            SatelliteNumber = sat;
            Week = RtcmV3Helper.adjgpsweek(utc, (int)week);

            var time = RtcmV3Helper.Utc2Gps(utc);

            var tt = RtcmV3Helper.GetFromGps(Week, Toes) - time;

            if (tt.TotalSeconds < -302400.0) Week++;
            else if (tt.TotalSeconds >= 302400.0) Week--;

            Toe = RtcmV3Helper.GetFromGps(Week, Toes);
            Toc = RtcmV3Helper.GetFromGps(Week, toc);

            TTrans = time;

            A = sqrtA * sqrtA;
            SatellitePrn = (int)prn;
            SatelliteCode = RtcmV3Helper.Sat2Code(sat, (int)prn);
        }

        public override ushort MessageId => RtcmMessageId;

        public override string Name => "GPS Satellite Ephemeris Data";

        /// <summary>
        /// /* satellite number */
        /// </summary>
        public int SatelliteNumber { get; set; }

        public int SatellitePrn { get; set; }
        
        public string SatelliteCode { get; set; }
        /// <summary>
        /// IODE
        /// </summary>
        public int Iode { get; set; }
        /// <summary>
        /// IODC
        /// </summary>
        public int Iodc { get; set; }
        /// <summary>
        /// /* SV accuracy (URA index) */
        /// </summary>
        public int SvAccuracy { get; set; }
        /// <summary>
        /// /* SV health (0:ok) */
        /// </summary>
        public int SvHealth { get; set; }
        /// <summary>
        /// /* GPS/QZS: gps week, GAL: galileo week */
        /// </summary>
        public int Week { get; set; }
        /// <summary>
        /// GPS/QZS: code on L2.
        /// GAL: data sourceName defined as rinex 3.03.
        /// BDS: data sourceName (0:unknown,1:B1I,2:B1Q,3:B2I,4:B2Q,5:B3I,6:B3Q)
        /// </summary>
        public int CodeL2 { get; set; }
        /// <summary>
        /// GPS/QZS: L2 P data flag
        /// BDS: nav type (0:unknown,1:IGSO/MEO,2:GEO)
        /// </summary>
        public int FlagL2PData { get; set; }

        /// <summary>
        /// Toe
        /// </summary>
        public DateTime Toe { get; set; }
        /// <summary>
        /// Toc
        /// </summary>
        public DateTime Toc { get; set; }
        /// <summary>
        /// T_trans
        /// </summary>
        public DateTime TTrans { get; set; }

        /* SV orbit parameters */
        /// <summary>
        /// Root of the Semi-Major Axis (km)
        /// </summary>
        public double A { get; set; }
        /// <summary>
        /// Eccentricity (e)
        /// </summary>
        public double E { get; set; }
        /// <summary>
        /// Inclination Angle at Reference Time (i)
        /// </summary>
        public double I0 { get; set; }
        /// <summary>
        /// Longitude of Ascending Node of Orbit Plane at Weekly Epoc (LΩ)
        /// </summary>
        public double Omg0 { get; set; }
        /// <summary>
        /// Argument of Perigee (ω) 
        /// </summary>
        public double Omg { get; set; }
        /// <summary>
        /// Mean Anomaly at Reference Time (m)
        /// </summary>
        public double M0 { get; set; }
        public double Deln { get; set; }
        /// <summary>
        /// Rate of Right Ascension (dΩ/dt)
        /// </summary>
        public double OmgD { get; set; }
        public double Idot { get; set; }
        public double Crc { get; set; }
        public double Crs { get; set; }
        public double Cuc { get; set; }
        public double Cus { get; set; }
        public double Cic { get; set; }
        public double Cis { get; set; }

        /// <summary>
        /// /* Toe (s) in week */
        /// </summary>
        public double Toes { get; set; }

        /// <summary>
        /// /* fit interval (h) */
        /// </summary>
        public double Fit { get; set; }

        /// <summary>
        /// SV clock parameters af0
        /// </summary>
        public double Af0 { get; set; }
        /// <summary>
        /// SV clock parameters af1
        /// </summary>
        public double Af1 { get; set; }
        /// <summary>
        /// SV clock parameters af2
        /// </summary>
        public double Af2 { get; set; }

        /// <summary>
        /// group delay parameters.
        /// GPS/QZS:tgd[0]=TGD.
        /// GAL:tgd[0]=BGD_E1E5a,tgd[1]=BGD_E1E5b.
        /// CMP:tgd[0]=TGD_B1I ,tgd[1]=TGD_B2I/B2b,tgd[2]=TGD_B1Cp.
        /// tgd[3]=TGD_B2ap,tgd[4]=ISC_B1Cd.
        /// tgd[5]=ISC_B2ad
        /// </summary>
        public double[] Tgd { get; set; } = new double[6];

        /// <summary>
        /// Adot for CNAV
        /// </summary>
        public double Adot { get; set; }
        /// <summary>
        /// ndot for CNAV
        /// </summary>
        public double Ndot { get; set; }

        
    }
}