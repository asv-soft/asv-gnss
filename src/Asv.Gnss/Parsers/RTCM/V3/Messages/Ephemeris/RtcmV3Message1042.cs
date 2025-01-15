using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Proprietary Message Type.
    /// Trimble Navigation Ltd.
    /// </summary>
    public class RtcmV3Message1042 : RtcmV3MessageBase
    {
        /// <summary>
        /// Rtcm Message Id
        /// </summary>
        public const int RtcmMessageId = 1042;

        /// <inheritdoc/>
        public override ushort MessageId => RtcmMessageId;

        /// <inheritdoc/>
        public override string Name => "BDS ephemeris information";

        /// <inheritdoc/>
        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        )
        {
            var sys = NavigationSystemEnum.SYS_CMP;

            var prn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            var sat = RtcmV3Helper.satno(sys, (int)prn);

            if (sat == 0)
            {
                throw new Exception($"Rtcm3 1042 satellite number error: prn={prn}");
            }

            SatelliteNumber = sat;
            SatellitePrn = prn;
            WeekRaw = SpanBitHelper.GetBitU(buffer, ref bitIndex, 13);
            Urai = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 4);
            IdotRaw = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14) * RtcmV3Helper.P2_43;
            Aode = SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            TocRaw = SpanBitHelper.GetBitU(buffer, ref bitIndex, 17) * 8;
            A2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 11) * RtcmV3EphemerisHelper.P2_66;
            A1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 22) * RtcmV3Helper.P2_50;
            A0 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 24) * RtcmV3Helper.P2_33;
            Adoc = SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            Crs = SpanBitHelper.GetBitS(buffer, ref bitIndex, 18) * RtcmV3Helper.P2_6;
            DeltaN = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_43;
            M0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31;
            Cuc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 18) * RtcmV3Helper.P2_31;
            e = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_33;
            Cus = SpanBitHelper.GetBitS(buffer, ref bitIndex, 18) * RtcmV3Helper.P2_31;
            Apow1_2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_19;
            ToeRaw = SpanBitHelper.GetBitU(buffer, ref bitIndex, 17) * 8;
            Cic = SpanBitHelper.GetBitS(buffer, ref bitIndex, 18) * RtcmV3Helper.P2_31;
            OmegaBig0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31;
            Cis = SpanBitHelper.GetBitS(buffer, ref bitIndex, 18) * RtcmV3Helper.P2_31;
            I0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31;
            Crc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 18) * RtcmV3Helper.P2_6;
            Omega = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31;
            OmegaBig = SpanBitHelper.GetBitS(buffer, ref bitIndex, 24) * RtcmV3Helper.P2_43;
            TGd1 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 10) * 0.1;
            TGd2 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 10) * 0.1;
            SvHealth = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        }

        /// <summary>
        /// satellite number in all the constellations.
        /// </summary>
        public int SatelliteNumber { get; set; }

        /// <summary>
        /// A BDS Satellite ID number from 1 to 37 refers to the PRN code of the BDS satellite.
        /// Satelilite ID's heigher then 37 are reserved. Range 1-63
        /// </summary>
        public uint SatellitePrn { get; set; }

        /// <summary>
        /// A BDS Satellite RINEX Code.
        /// </summary>
        public string SatelliteCode => RtcmV3Helper.Sat2Code(SatelliteNumber, (int)SatellitePrn);

        /// <summary>
        /// BDS Week number. Range 0 - 8191. Resolution - 1 week.
        /// Roll-over every 8192 weeks starting fromm 00:00:00UTC on Jan 1. 2006 of BDT.
        /// </summary>
        public uint WeekRaw { get; set; }

        /// <summary>
        /// BDS URAI Range 1 - 15.
        /// User Range Accuracy Index.
        /// </summary>
        public byte Urai { get; set; }

        /// <summary>
        /// BDS IDOT Range +-9.31*10e-10. Resolution ^-43
        /// Rate of inclination angle. Unit: pi/s
        /// </summary>
        public double IdotRaw { get; private set; }

        /// <summary>
        /// BDS IDOT Range 0-31.
        /// Age of Data Ephemeris.
        /// </summary>
        public uint Aode { get; private set; }

        /// <summary>
        /// The reference time of clock parameters. Unit: s.
        /// Range 0 - 604.792. Resolution: 8s.
        /// </summary>
        public uint TocRaw { get; private set; }

        /// <summary>
        /// Clock correcton parameter Unit:s.
        /// Range: +-1.38E-17 s/s^2. Resolution: 2^-66 s/s^2
        /// </summary>
        public double A2 { get; private set; }

        /// <summary>
        /// Clock correcton parameter Unit:s.
        /// Range: +-1.86E-9 s/s^2. Resolution: 2^-50 s/s^2
        /// </summary>
        public double A1 { get; private set; }

        /// <summary>
        /// Clock correcton parameter Unit:s.
        /// Range: +-9.77E-4 s/s^2. Resolution: 2^-33 s/s^2
        /// </summary>
        public double A0 { get; private set; }

        /// <summary>
        /// Age of Data Clock.
        /// Range: 0-31.
        /// </summary>
        public uint Adoc { get; private set; }

        /// <summary>
        /// Amplitude of sine harmonic correction term to the orbit radius. Unit: m.
        /// Range: +-2048. Resolution: 2^-6 m.
        /// </summary>
        public double Crs { get; private set; }

        /// <summary>
        /// Mean moution difference from computed value. Unit: pi/s.
        /// Range: +-3.73E-9 pi/s. Resolution 2^-43 pi/s
        /// </summary>
        public double DeltaN { get; private set; }

        /// <summary>
        /// Mean anomaly at reference time. Unit: pi.
        /// Range: +-pi. Resolution 2^-31 pi
        /// </summary>
        public double M0 { get; private set; }

        /// <summary>
        /// Amplitude of cusine harmonic correction term to the argument of latitude. Unit: rad.
        /// Range: +-6.10E-5. Resolution: 2^-31 rad.
        /// </summary>
        public double Cuc { get; private set; }

        /// <summary>
        /// Eccentricity.
        /// Range: 0 - 0.5. Resolution: 2^-33.
        /// </summary>
        public double e { get; private set; }

        /// <summary>
        /// Amplitude of sine harmonic correction term to the argument of latitude. Unit: rad.
        /// Range: +-6.10E-5. Resolution: 2^-31 rad.
        /// </summary>
        public double Cus { get; private set; }

        /// <summary>
        /// Square root of semi-major axis. Unit: m^1/2.
        /// Range: 0 - 8192 m^1/2. Resolution: 2^-19 m^1/2.
        /// </summary>
        public double Apow1_2 { get; private set; }

        /// <summary>
        /// Ephemeris reference time. Unit: s.
        /// Range 0 - 604.792. Resolution: 8s.
        /// </summary>
        public uint ToeRaw { get; private set; }

        /// <summary>
        /// Amplitude of cusine harmonic correction term to the argument of angle of inclination. Unit: rad.
        /// Range: +-6.10E-5. Resolution: 2^-31 rad.
        /// </summary>
        public double Cic { get; private set; }

        /// <summary>
        /// Longitude of ascending node of orbital of phase computed according to reference time. Unit: pi.
        /// Range: +-pi. Resolution 2^-31 pi
        /// </summary>
        public double OmegaBig0 { get; private set; }

        /// <summary>
        /// Amplitude of sine harmonic correction term to the argument of angle of inclination. Unit: rad.
        /// Range: +-6.10E-5. Resolution: 2^-31 rad.
        /// </summary>
        public double Cis { get; private set; }

        /// <summary>
        /// Inclination angle at reference time. Unit: pi.
        /// Range: +-pi. Resolution 2^-31 pi
        /// </summary>
        public double I0 { get; private set; }

        /// <summary>
        /// Amplitude of cosine harmonic correction term to the orbit radius. Unit: m.
        /// Range: +-2048. Resolution: 2^-6 m.
        /// </summary>
        public double Crc { get; private set; }

        /// <summary>
        /// Argument of perigree. Unit: pi.
        /// Range: +-pi. Resolution 2^-31 pi
        /// </summary>
        public double Omega { get; private set; }

        /// <summary>
        /// Rate of right ascension. Unit: pi/s.
        /// Range: +-9.54E-7 pi/s. Resolution: 2^-43 pi/s.
        /// </summary>
        public double OmegaBig { get; private set; }

        /// <summary>
        /// Equipment Group Delay Differential. Unit: ns.
        /// Range: +-51.2 ns. Resolution: 0.1 ns.
        /// </summary>
        public double TGd1 { get; private set; }

        /// <summary>
        /// Equipment Group Delay Differential. Unit: ns.
        /// Range: +-51.2 ns. Resolution: 0.1 ns.
        /// </summary>
        public double TGd2 { get; private set; }

        /// <summary>
        /// Autonomous satelite health flag.
        /// Range: 0 - 1. Resolution: 1.
        /// </summary>
        public byte SvHealth { get; private set; }

        /// <summary>
        /// BDS Week number.
        /// </summary>
        public int GetWeek(DateTime utc)
        {
            return RtcmV3EphemerisHelper.GetBdsWeek(utc, (int)WeekRaw);
        }

        public DateTime GetToc()
        {
            return RtcmV3Helper.GetFromBeiDou((int)WeekRaw, TocRaw);
        }

        public double Idot => Idot * RtcmV3Helper.SC2RAD;

        public DateTime GetToc(DateTime utc)
        {
            return RtcmV3Helper.GetFromBeiDou(GetWeek(utc), TocRaw);
        }

        public DateTime GetToe()
        {
            return RtcmV3Helper.GetFromBeiDou((int)WeekRaw, ToeRaw);
        }
    }
}
