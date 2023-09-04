using Asv.IO;
using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Proprietary Message Type.
    /// Trimble Navigation Ltd.
    /// </summary>
    public class RtcmV3Message1046 : RtcmV3MessageBase
    {
        /// <summary>
        /// Rtcm Message Id
        /// </summary>
        public const int RtcmMessageId = 1046;
        /// <inheritdoc/>
        public override ushort MessageId => RtcmMessageId;
        
        /// <inheritdoc/>
        public override string Name => "Galileo I/NAV ephemeris information";


        /// <inheritdoc/>
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            var sys = NavigationSystemEnum.SYS_GAL;

            var prn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            var sat = RtcmV3Helper.satno(sys, (int)prn);

            if (sat == 0)
            {
                throw new Exception($"Rtcm3 1046 satellite number error: prn={prn}");
            }

            SatelliteNumber = sat;
            SatellitePrn = prn;
            WeekRaw = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            IoDnav = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
            SvSisa = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            IdotRaw = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14) * RtcmV3Helper.P2_43;
            TocRaw = SpanBitHelper.GetBitU(buffer, ref bitIndex, 14) * 60;
            Af2 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 6) * RtcmV3EphemerisHelper.P2_59;
            Af1 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 21) * RtcmV3Helper.P2_46;
            Af0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 31) * RtcmV3EphemerisHelper.P2_34;
            Crs = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_5;
            DeltaN = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_43;
            M0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31;
            Cuc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_29;
            e = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_33;
            Cus = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_29;
            Apow1_2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_19;
            ToeRaw = SpanBitHelper.GetBitU(buffer, ref bitIndex, 14) * 60;
            Cic = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_29;
            OmegaBig0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31;
            Cis = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_29;
            I0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31;
            Crc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_5;
            Omega = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31;
            OmegaDot = SpanBitHelper.GetBitS(buffer, ref bitIndex, 24) * RtcmV3Helper.P2_43;
            BGdE1E5a = SpanBitHelper.GetBitS(buffer, ref bitIndex, 10) * RtcmV3Helper.P2_32;
            BGdE5bE1 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 10) * RtcmV3Helper.P2_32;
            E5bSignalHealthFlag = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            E5bDataValidity = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            E1BSignalHealthFlag = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            E1BDataValidity = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            Reserved = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        }
        /// <summary>
        /// Satellite number in all the constellations.
        /// </summary>
        public int SatelliteNumber { get; set; }
        /// <summary>
        /// A Galileo SVID parameter is coded with 6 bits.
        /// However the ma constellation which can be accomodated within the I/NAV and F/NAV frames is 36 satellites
        /// (3 planes of 12 satellites each)
        /// </summary>
        public uint SatellitePrn { get; set; }
        /// <summary>
        /// A BDS Satellite RINEX Code.
        /// </summary>
        public string SatelliteCode => RtcmV3Helper.Sat2Code(SatelliteNumber, (int)SatellitePrn);
        /// <summary>
        /// Galileo Week number. Range 0 - 4095 weeks. Resolution - 1 week.
        /// Roll-over every 4096 (about 78 years). Galileo System Time (GST).
        /// The GST start epoch is defined as 13 seconds before midnight between 21st August and 22nd August 1999.
        /// i.e. GST was equal 13 seconds at 22 nd August 1999 00:00:00 UTC.
        /// </summary>
        public uint WeekRaw { get; set; }
        /// <summary>
        /// Issue of Data for navigation data (IODnav).
        /// Each IoDNav has an associated reference time parameter disseminated within the batch.
        /// Note: the broadcast group delay validity status and signal health status are not identified by any Issue of Data value unitless.
        /// </summary>
        public uint IoDnav { get; set; }
        /// <summary>
        /// SIS Accuracy
        /// </summary>
        public byte SvSisa { get; set; }

        /// <summary>
        /// IDOT. Effective range attainable with the indicated bit allocation and scale factor. Resolution ^-43
        /// Rate of inclination angle. Unit: semi-circles/s
        /// </summary>
        public double IdotRaw { get; private set; }
        /// <summary>
        /// The reference time of clock parameters. Unit: s.
        /// Range 0 - 604.792. Resolution: 60.
        /// </summary>
        public uint TocRaw { get; private set; }
        /// <summary>
        /// Clock correcton parameter Unit: s/s^2.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-59.
        /// </summary>
        public double Af2 { get; private set; }
        /// <summary>
        /// Clock correcton parameter Unit:s/s.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-46
        /// </summary>
        public double Af1 { get; private set; }
        /// <summary>
        /// Clock correcton parameter Unit:s.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-34
        /// </summary>
        public double Af0 { get; private set; }
        /// <summary>
        /// Amplitude of sine harmonic correction term to the orbit radius. Unit: m.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-5 m.
        /// </summary>
        public double Crs { get; private set; }

        /// <summary>
        /// Mean moution difference from computed value. Unit: pi/s.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-43 pi/s
        /// </summary>
        public double DeltaN { get; private set; }

        /// <summary>
        /// Mean anomaly at reference time. Unit: pi.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-31 pi
        /// </summary>
        public double M0 { get; private set; }

        /// <summary>
        /// Amplitude of cusine harmonic correction term to the argument of latitude. Unit: rad.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-29 rad.
        /// </summary>
        public double Cuc { get; private set; }

        /// <summary>
        /// Eccentricity.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-33.
        /// </summary>
        public double e { get; private set; }

        /// <summary>
        /// Amplitude of sine harmonic correction term to the argument of latitude. Unit: rad.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-29 rad.
        /// </summary>
        public double Cus { get; private set; }

        /// <summary>
        /// Square root of semi-major axis. Unit: m^1/2.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-19 m^1/2.
        /// </summary>
        public double Apow1_2 { get; private set; }

        /// <summary>
        /// Ephemeris reference time. Unit: s.
        /// Range 0 - 604.792. Resolution: 60.
        /// </summary>
        public uint ToeRaw { get; private set; }

        /// <summary>
        /// Amplitude of cusine harmonic correction term to the argument of angle of inclination. Unit: rad.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-29 rad.
        /// </summary>
        public double Cic { get; private set; }

        /// <summary>
        /// Longitude of ascending node of orbital plane at weekly epoch. Unit: pi.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-31 pi
        /// </summary>
        public double OmegaBig0 { get; private set; }

        /// <summary>
        /// Amplitude of sine harmonic correction term to the angle of inclination. Unit: rad.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-29 rad.
        /// </summary>
        public double Cis { get; private set; }

        /// <summary>
        /// Inclination angle at reference time. Unit: pi.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-31 pi
        /// </summary>
        public double I0 { get; private set; }

        /// <summary>
        /// Amplitude of cosine harmonic correction term to the orbit radius. Unit: m.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-5 m.
        /// </summary>
        public double Crc { get; private set; }

        /// <summary>
        /// Argument of Perigree. Unit: pi.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-31 pi
        /// </summary>
        public double Omega { get; private set; }

        /// <summary>
        /// Rate of right ascension. Unit: pi/s.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-43 pi/s.
        /// </summary>
        public double OmegaDot { get; private set; }

        /// <summary>
        /// E1/E5a Broadcast Group Delay.
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-32.
        /// </summary>
        public double BGdE1E5a { get; private set; }

        /// <summary>
        /// E5b/E1 Broadcast Group Delay (reserved).
        /// Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-32.
        /// </summary>
        public double BGdE5bE1 { get; private set; }

        /// <summary>
        /// E5a Signal Health Status. Bit Values are:
        /// 0 - Signal OK
        /// 1 - Signal out of service
        /// 2 - Signal will be out of service
        /// 3 - Signal Component currently in Test
        /// </summary>
        public byte E5bSignalHealthFlag { get; private set; }

        /// <summary>
        /// The navigation data validity status transmitted on E5a.
        /// </summary>
        public byte E5bDataValidity { get; private set; }

        /// <summary>
        /// The E5b Signal Health Status. Bit Values are:
        /// 0 - Signal OK
        /// 1 - Signal out of service
        /// 2 - Signal will be out of service
        /// 3 - Signal Component currently in Test
        /// </summary>
        public byte E1BSignalHealthFlag { get; private set; }

        /// <summary>
        /// The navigation data validity status transmitted on E5b.
        /// </summary>
        public byte E1BDataValidity { get; private set; }

        /// <summary>
        /// Galileo Week number.
        /// </summary>
        public int GetWeek(DateTime utc)
        {
            return RtcmV3EphemerisHelper.GetGalWeek(utc, (int)WeekRaw);
        }


        public DateTime GetToc()
        {
            return RtcmV3Helper.GetFromGalileo((int)WeekRaw, TocRaw);
        }
        public double Idot => Idot * RtcmV3Helper.SC2RAD;
        public DateTime GetToc(DateTime utc)
        {
            return RtcmV3Helper.GetFromGalileo(GetWeek(utc), TocRaw);
        }
        public DateTime GetToe()
        {
            return RtcmV3Helper.GetFromGalileo((int)WeekRaw, ToeRaw);
        }

        public bool IsE5bSignalOk
        {
            get
            {
                return ((SignalHealthy)E5bSignalHealthFlag & SignalHealthy.OK) == SignalHealthy.OK;
            }
        }

        public bool IsE5bSignalOuOfService
        {
            get
            {
                return ((SignalHealthy)E5bSignalHealthFlag & SignalHealthy.OutOfService) == SignalHealthy.OutOfService;
            }
        }

        public bool IsE5bSignalWillOuOfService
        {
            get
            {
                return ((SignalHealthy)E5bSignalHealthFlag & SignalHealthy.WillBeOutOfService) == SignalHealthy.WillBeOutOfService;
            }
        }
        public bool IsE5bSignalInTest
        {
            get
            {
                return ((SignalHealthy)E5bSignalHealthFlag & SignalHealthy.InTest) == SignalHealthy.InTest;
            }
        }

        public bool IsE1BSignalOk
        {
            get
            {
                return ((SignalHealthy)E1BSignalHealthFlag & SignalHealthy.OK) == SignalHealthy.OK;
            }
        }

        public bool E1BSignalOuOfService
        {
            get
            {
                return ((SignalHealthy)E1BSignalHealthFlag & SignalHealthy.OutOfService) == SignalHealthy.OutOfService;
            }
        }

        public bool E1BSignalWillOuOfService
        {
            get
            {
                return ((SignalHealthy)E1BSignalHealthFlag & SignalHealthy.WillBeOutOfService) == SignalHealthy.WillBeOutOfService;
            }
        }
        public bool E1BSignalInTest
        {
            get
            {
                return ((SignalHealthy)E1BSignalHealthFlag & SignalHealthy.InTest) == SignalHealthy.InTest;
            }
        }

        [Flags]
        public enum SignalHealthy
        {
            OK = 0x0,
            OutOfService= 0x1,
            WillBeOutOfService = 0x2,
            InTest = 0x3
        }
    }
}
