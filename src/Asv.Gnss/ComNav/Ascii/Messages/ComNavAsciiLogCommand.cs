using System;
using System.Text;

namespace Asv.Gnss
{
    public enum ComNavFormat
    {
        Binary,
        Ascii
    }

    public enum ComNavMessageEnum
    {
        // -------------------------------------------------------
        // |                 Predefined Log Message				 |
        // -------------------------------------------------------
        /// <summary>
        /// B BD2 decoded ephemeris information 4.2.1.1
        /// </summary>
        BD2EPHEM = 71,
        /// <summary>
        /// B BD3 decoded ephemeris information 4.2.1.2
        /// </summary>
        BD3EPHEM = 72,
        /// <summary>
        /// BFRAME B 4.2.1.3
        /// </summary>
        BD3RAWNAVSU = 157,
        /// <summary>
        /// B BD2 raw almanac 4.2.1.4
        /// </summary>
        BD2RAWALM = 741,
        /// <summary>
        /// B BD2 Raw ephemeris 4.2.1.5
        /// </summary>
        BD2RAWEPHEM = 412,
        /// <summary>
        /// B 4.2.1.6
        /// </summary>
        BDSRAWNAVSU = 1695,
        /// <summary>
        /// A, B Best position data 4.2.7.1
        /// </summary>
        BESTPOS = 42,
        /// <summary>
        /// A,B, Abb Best velocity data 4.2.7.2
        /// </summary>
        BESTVEL = 99,
        /// <summary>
        /// A, B Position information in xyz. 4.2.7.3
        /// </summary>
        BESTXYZ = 241,
        /// <summary>
        /// B BINEX Record 0x00 encapsulated by Binary header & CRC-32 4.2.2.1
        /// </summary>
        BINEX00DATA = 110,
        /// <summary>
        /// B BINEX Record 0x01-01 encapsulated by Binary header & CRC-32 4.2.2.2
        /// </summary>
        BINEX0101DATA = 81,
        /// <summary>
        /// B BINEX Record 0x01-02 encapsulated by Binary header & CRC-32 4.2.2.3
        /// </summary>
        BINEX0102DATA = 82,
        /// <summary>
        /// B BINEX Record 0x01-05 encapsulated by Binary header & CRC-32 4.2.2.4
        /// </summary>
        BINEX0105DATA = 85,
        /// <summary>
        /// B BINEX Record 0x7d-00 encapsulated by Binary header & CRC-32 4.2.2.5
        /// </summary>
        BINEX7D00DATA = 114,
        /// <summary>
        /// B BINEX Record 0x7e-00 encapsulated by Binary header & CRC-32 4.2.2.6
        /// </summary>
        BINEX7E00DATA = 115,
        /// <summary>
        /// B BINEX Record 0x7f-05 encapsulated by Binary header & CRC-32 4.2.2.7
        /// </summary>
        BINEX7F05DATA = 120,
        /// <summary>
        /// A, B COM configuration Information in ASCII Format
        /// </summary>
        COMCONFIG = 317,
        /// <summary>
        /// B Decoded GLONASS Ephemeris 4.2.1.8
        /// </summary>
        GLOEPHEMERIS = 723,
        /// <summary>
        /// B GLONASS raw ephemeris message. 4.2.1.9
        /// </summary>
        GLORAWEPHEM = 792,
        /// <summary>
        /// B A single set of decoded GNSS ephemeris whose message ID is different from NovAtel® definition 4.2.1.10
        /// </summary>
        GPSEPHEM1,
        /// <summary>
        /// B Galileo ephemeris parameters 4.2.1.11
        /// </summary>
        GALEPHEMERIS = 1122,
        /// <summary>
        /// AGE B The raw Galileo FNAV page data 4.2.1.12
        /// </summary>
        GALFNAVRAWP = 1413,
        /// <summary>
        /// ORD B The raw Galileo INAV word data 4.2.1.13
        /// </summary>
        GALINAVRAWW = 1414,
        /// <summary>
        /// B GPS decoded ephemeris information 4.2.1.10
        /// </summary>
        GPSEPHEM2 = 712,
        /// <summary>
        /// A, B Heading angle message 4.2.4.1
        /// </summary>
        HEADING = 971,
        /// <summary>
        /// A,B, Abb Ionosphere and UTC parameters 4.2.9.1
        /// </summary>
        IONUTC = 8,
        /// <summary>
        /// A Log settings in each port. 4.2.3.2
        /// </summary>
        LOGLIST = 5,
        /// <summary>
        /// B Extended Satellite Information 4.2.9.2
        /// </summary>
        M925 = 925,
        /// <summary>
        /// A, B Position at time of mark input event 4.2.5.1
        /// </summary>
        MARKPOS = 181,
        /// <summary>
        /// A, B Time of mark input event 4.2.5.2
        /// </summary>
        MARKTIME = 231,
        /// <summary>
        /// B Basic Meteorograph Data Message 4.2.6.1
        /// </summary>
        METEODATA = 106,
        /// <summary>
        /// B Extended Meteorograph Data Message 4.2.6.2
        /// </summary>
        METEODATAEXT = 108,
        /// <summary>
        /// B DOP of SVs currently tracking 4.2.7.4
        /// </summary>
        PSRDOP = 174,
        /// <summary>
        /// A,B, Abb Pseudorange Position 4.2.7.5
        /// </summary>
        PSRPOS = 47,
        /// <summary>
        /// A, B Pseudorange Velocity 4.2.7.6
        /// </summary>
        PSRVEL = 100,
        /// <summary>
        /// A, B Pseudorange Cartesian position and velocity 4.2.7.7
        /// </summary>
        PSRXYZ = 243,
        /// <summary>
        /// B The SDK Log-on Message of Qianxun SI 4.2.7.8
        /// </summary>
        QXWZSDKINFOB = 901,
        /// <summary>
        /// AM B QZSS raw ephemeris informationfor subframes 4.2.1.14
        /// </summary>
        QZSSRAWSUBFR = 1330,
        /// <summary>
        /// M B QZSS raw ephemeris informationfor 4.2.1.15
        /// </summary>
        QZSSRAWEPHE = 1331,
        /// <summary>
        /// A,B, Abb Detailed range information 4.2.8.1
        /// </summary>
        RANGE = 43,
        /// <summary>
        /// A,B, Abb Compressed version of the RANGE log 4.2.8.2
        /// </summary>
        RANGECMP = 140,
        /// <summary>
        /// B Raw almanac 4.2.1.17
        /// </summary>
        RAWALM = 74,
        /// <summary>
        /// ME B The raw subframe data without parity bits,only 240bits per frame, and only outputs the sub-frames passing the check 4.2.1.16
        /// </summary>
        RAWGPSSUBFRA = 25,
        /// <summary>
        /// B Raw ephemeris 4.2.1.18
        /// </summary>
        RAWEPHEM = 41,
        /// <summary>
        /// A Raw SBAS frame data 4.2.10.1
        /// </summary>
        RAWSBASFRAME = 973,
        /// <summary>
        /// A, B Base station Position 4.2.11.1
        /// </summary>
        REFSTATION = 175,
        /// <summary>
        /// B Pseudorange correction message 4.2.8.3
        /// </summary>
        RTCMDATA1 = 396,
        /// <summary>
        /// B Satellite status (defined by ComNav) 4.2.9.3
        /// </summary>
        SATMSG = 911,
        /// <summary>
        /// B Satellite visibility 4.2.9.4
        /// </summary>
        SATVIS = 48,
        /// <summary>
        /// A, B Satellite positions in ECEF Cartesian coordinates 4.2.9.5
        /// </summary>
        SATXYZ = 270,
        /// <summary>
        /// A Do Not Use for Safety Applications 4.2.10.2
        /// </summary>
        SBAS0 = 976,
        /// <summary>
        /// A PRN Mask Assignments 4.2.10.3
        /// </summary>
        SBAS1 = 977,
        /// <summary>
        /// A Fast Corrections 4.2.10.4
        /// </summary>
        SBAS2 = 982,
        /// <summary>
        /// A Fast Corrections 4.2.10.4
        /// </summary>
        SBAS3 = 987,
        /// <summary>
        /// A Fast Corrections 4.2.10.4
        /// </summary>
        SBAS4 = 992,
        /// <summary>
        /// A Fast Corrections 4.2.10.4
        /// </summary>
        SBAS5 = 994,
        /// <summary>
        /// A Integrity Information 4.2.10.5
        /// </summary>
        SBAS6 = 995,
        /// <summary>
        /// A Fast Correction Degradation Factor 4.2.10.6
        /// </summary>
        SBAS7 = 996,
        /// <summary>
        /// A GEO Navigation Message 4.2.10.7
        /// </summary>
        SBAS9 = 997,
        /// <summary>
        /// A Degradation Factors 4.2.10.8
        /// </summary>
        SBAS10 = 978,
        /// <summary>
        /// A SBAS Network Time/UTC/GLO Time Offset Parameters Message 4.2.10.9
        /// </summary>
        SBAS12 = 979,
        /// <summary>
        /// A GEO Almanacs 4.2.10.10
        /// </summary>
        SBAS17 = 980,
        /// <summary>
        /// A Ionospheric Grid Point Masks 4.2.10.11
        /// </summary>
        SBAS18 = 981,
        /// <summary>
        /// A Mixed Fast Corrections/Long Term Satellite Error Corrections 4.2.10.12
        /// </summary>
        SBAS24 = 983,
        /// <summary>
        /// A Long Term Satellite Error Corrections 4.2.10.13
        /// </summary>
        SBAS25 = 984,
        /// <summary>
        /// A Ionospheric Delay Corrections 4.2.10.14
        /// </summary>
        SBAS26 = 985,
        /// <summary>
        /// A SBAS Service 4.2.10.15
        /// </summary>
        SBAS27 = 986,
        /// <summary>
        /// A Clock-Ephemeris Covariance Matrix Message 4.2.10.16
        /// </summary>
        SBAS28 = 975,
        /// <summary>
        /// A Null Message 4.2.10.17
        /// </summary>
        SBAS63 = 1003,
        /// <summary>
        /// B Board time information 4.2.12.1
        /// </summary>
        TIME = 101,
        /// <summary>
        /// B Satellite tracking status 4.2.3.3
        /// </summary>
        TRACKSTAT = 83,
        /// <summary>
        /// A,B, Abb Board software and hardware version 4.2.3.4
        /// </summary>
        VERSION = 37,
        // -------------------------------------------------------
        // |      				NMEA Message   			 		 |
        // -------------------------------------------------------
        /// <summary>
        /// GPS Fix Data and Undulation
        /// </summary>
        GPGGA = 218,
        /// <summary>
        /// Latitude and Longitude of Present Vessel Position
        /// </summary>
        GPGLL = 219,
        /// <summary>
        /// GPS DOP and Active Satellites
        /// </summary>
        GPGSA = 221,
        /// <summary>
        /// Only Dop Values are Valid Currently
        /// </summary>
        GPGST = 222,
        /// <summary>
        /// GPS Satellites in View
        /// </summary>
        GPGSV = 223,
        /// <summary>
        /// Actual Vessel Heading in Degrees True
        /// </summary>
        GPHDT = 228,
        /// <summary>
        /// GPS Specific Information
        /// </summary>
        GPRMC = 225,
        /// <summary>
        /// The Track Made Good and Speed Relative to the Ground
        /// </summary>
        GPVTG = 226,
        /// <summary>
        /// UTC Time and Date
        /// </summary>
        GPZDA = 227,

        /// <summary>
        /// Differential timing result (ComNav Proprietary)
        /// </summary>
        GPCDT = 211,
        /// <summary>
        /// Constellation Health (ComNav Proprietary)
        /// </summary>
        GPCLH = 267,
        /// <summary>
        /// Delta Range Correction (ComNav Proprietary)
        /// </summary>
        GPDRC = 265,
        /// <summary>
        /// GPS Fix Data and Undulation (ComNav Proprietary)
        /// </summary>
        GPGGARTK = 259,
        /// <summary>
        /// Pseudorange Residual (ComNav Proprietary)
        /// </summary>
        GPGRS = 220,
        /// <summary>
        /// Parameters of Attitude Angles (ComNav Proprietary)
        /// </summary>
        GPHPR = 237,
        /// <summary>
        /// Constellation Health (ComNav Proprietary)
        /// </summary>
        GPIDM = 268,
        /// <summary>
        /// ComNav Navigation Information Message (ComNav Proprietary)
        /// </summary>
        GPNAV = 264,
        /// <summary>
        /// Information about navigating to reference station. 
        /// </summary>
        GPNTR = 209,
        /// <summary>
        /// Pseudorange and Range Rate Residual (ComNav Proprietary)
        /// </summary>
        GPPRR = 271,
        /// <summary>
        /// Differential GPS and BDS Corrections (ComNav Proprietary)
        /// </summary>
        GPRRS = 263,
        /// <summary>
        /// Reference Station Coordinates (ComNav Proprietary)
        /// </summary>
        GPRSC = 266,
        /// <summary>
        /// Satellite Health Indication (ComNav Proprietary)
        /// </summary>
        GPSEH = 261,
        /// <summary>
        /// Heading, Pitch and Roll (reserved) Message (ComNav Proprietary)
        /// </summary>
        GPTRA = 207,
        /// <summary>
        /// Satellite User Range Accuracy (URA) (ComNav Proprietary)
        /// </summary>
        GPURA = 262,
        /// <summary>
        /// Position, Velocity,, Heading, Pitch and PJK information (ComNav Proprietary)
        /// </summary>
        GPYBM = 87,


        // -------------------------------------------------------
        // |      				RTCM 2.X    			 		 |
        // -------------------------------------------------------

        /// <summary>
        /// Format(B) Pseudorange correction message in RTCM2.3
        /// </summary>
        RTCM1 = 107,
        /// <summary>
        /// Format(B) Type 3 Base Station Parameters
        /// </summary>
        RTCM3 = 402,
        /// <summary>
        /// Format(B) GPS Partical Correction Set
        /// </summary>
        RTCM9 = 275,
        /// <summary>
        /// Format(B) Type18 and Type 19 Raw Measurements
        /// </summary>
        RTCM1819 = 399,
        /// <summary>
        /// Format(B) Differential GLONASS Corrections
        /// </summary>
        RTCM31 = 864,
        /// <summary>
        /// Format(B) GNSS pseudorange corrections
        /// </summary>
        RTCM41,
        /// <summary>
        /// Format(B) General partial corrections
        /// </summary>
        RTCM42,


        // -------------------------------------------------------
        // |      				RTCM 3.X    			 		 |
        // -------------------------------------------------------

        /// <summary>
        /// Format(B) BDS Ephemerides (a test message)
        /// </summary>
        RTCM0063 = 89,
        /// <summary>
        /// Format(B) Extended L1-Only GPS RTK Observables
        /// </summary>
        RTCM1002 = 785,
        /// <summary>
        /// Format(B) L1 and L2 GPS RTK Observables
        /// </summary>
        RTCM1003,
        /// <summary>
        /// Format(B) Extended L1/L2 GPS RTK Observables
        /// </summary>
        RTCM1004 = 787,
        /// <summary>
        /// Format(B) RTK Base Station ARP
        /// </summary>
        RTCM1005 = 788,
        /// <summary>
        /// Format(B) Base Station ARP with Height
        /// </summary>
        RTCM1006 = 789,
        /// <summary>
        /// Format(B) Extended Antenna Descriptor and Setup Information
        /// </summary>
        RTCM1007 = 856,
        /// <summary>
        /// Format(B) Extended Antenna Descriptor and Setup Information
        /// </summary>
        RTCM1008 = 857,
        /// <summary>
        /// Format(B) Extended L1-OnlyGLONASS RTK Observables
        /// </summary>
        RTCM1010 = 898,
        /// <summary>
        /// Format(B) GLONASS L1/L2 RTK
        /// </summary>
        RTCM1011,
        /// <summary>
        /// Format(B) Extended L1 & L2 GLONASS Observables
        /// </summary>
        RTCM1012 = 900,
        /// <summary>
        /// Format(B) GPS Ephemerides
        /// </summary>
        RTCM1019 = 893,
        /// <summary>
        /// Format(B) GLONASS Ephemerides
        /// </summary>
        RTCM1020 = 895,
        /// <summary>
        /// Format(B) Receiver and Antenna Descriptors
        /// </summary>
        RTCM1033 = 999,
        /// <summary>
        /// Format(B) Extended B1, B2 or B3 BD2 RTK Observables
        /// </summary>
        RTCM1104 = 781,
        /// <summary>
        /// Format(B) GPS MSM4 — Full PRs and Phase Ranges plus CNR
        /// </summary>
        RTCM1074 = 624,
        /// <summary>
        /// Format(B) GLO MSM4 — Full PRs and Phase Ranges plus CNR
        /// </summary>
        RTCM1084 = 644,
        /// <summary>
        /// Format(B) BDS MSM4 — Full PRs and Phase Ranges plus CNR
        /// </summary>
        RTCM1124 = 674,
        /// <summary>
        /// Format(B) QZSS MSM4
        /// </summary>
        RTCM1114 = 684,
        /// <summary>
        /// Format(B) A RTCM 3.x Proprietary Message for ComNav
        /// </summary>
        RTCM4078,
        // -------------------------------------------------------
        // |                BINEX Message       				 |
        // -------------------------------------------------------
        /// <summary>
        /// Record(0x00) B Site Metadata
        /// </summary>
        BINEX00,
        /// <summary>
        /// Record(0x01-01) B Decoded GPS Ephemeris
        /// </summary>
        BINEX0101,
        /// <summary>
        /// Record(0x01-02) B Decoded GLONASS — FDMA Ephemeris
        /// </summary>
        BINEX0102,
        /// <summary>
        /// Record(0x01-05) B Decoded Beidou-2/Compass Ephemeris
        /// </summary>
        BINEX0105,
        /// <summary>
        /// Record(0x7d-00) B Receiver Internal State
        /// </summary>
        BINEX7D00,
        /// <summary>
        /// Record(0x7e-00) B Ancillary Site Data Prototyping
        /// </summary>
        BINEX7E00,
        /// <summary>
        /// Record(0x7f-05) B GNSS Observable Prototyping
        /// </summary>
        BINEX7F05,
        // -------------------------------------------------------
        // |         Trimble Proprietary Messages 				 |
        // -------------------------------------------------------
        /// <summary>
        /// B Base station satellite observation information
        /// </summary>
        CMROBS = 390,
        /// <summary>
        /// B Base station position information
        /// </summary>
        CMRREF = 391,
        /// <summary>
        /// A Time, yaw, tilt, range, mode, PDOP, and number of SVs for Moving Baseline RTK
        /// </summary>
        PTNLAVR = 224,
        /// <summary>
        /// A Time, position, position type, and DOP values
        /// </summary>
        PTNLGGK = 76,
        /// <summary>
        /// A PJK Position
        /// </summary>
        PTNLPJK = 229,
        // -------------------------------------------------------
        // |         JAVAD Proprietary Messages 				 |
        // -------------------------------------------------------
        /// <summary>
        /// [NP] Navigation Positon
        /// </summary>
        NAVPOS = 52,
        // -------------------------------------------------------
        // |                Parameter Messages   				 |
        // -------------------------------------------------------
        /// <summary>
        /// BD2 cutoff angle. 
        /// </summary>
        BD2ECUTOFF = 2001,
        /// <summary>
        /// GPS cutoff angle. 
        /// </summary>
        ECUTOFF = 2002,
        /// <summary>
        /// GLONASS cutoff angle. 
        /// </summary>
        GLOECUTOFF = 2017,
        /// <summary>
        /// Magnetic variation correction
        /// </summary>
        MAGVAR = 2018,
        /// <summary>
        /// PJK Parameters Used in PTNLPJK Message
        /// </summary>
        PJKPARA = 2013,
        /// <summary>
        /// PVT frequency. 
        /// </summary>
        PVTFREQ = 2019,
        /// <summary>
        /// Reference mode, auto-started, SPP or fixed position. 
        /// </summary>
        REFMODE = 2003,
        /// <summary>
        /// Ref-Station position in PJK mode. 
        /// </summary>
        REFPJKXYH = 2022,
        /// <summary>
        /// Registered functions list
        /// </summary>
        REGLIST = 2015,
        /// <summary>
        /// RTK frequency. 
        /// </summary>
        RTKFREQ = 2020,
        /// <summary>
        /// Time thresh of differential data could be used. 
        /// </summary>
        RTKTIMEOUT = 2008,
        /// <summary>
        /// Main system configuration parameters.
        /// </summary>
        SYSCONFIG = 2021,
        // ----------------------------------------------------------
        // | Command Messages for Weather Instrument (Meteorograph) |
        // ----------------------------------------------------------
        /// <summary>
        /// A Set date of ZZ11A Meteorograph
        /// </summary>
        ZZ11ASETDATE = 932,
        /// <summary>
        /// A Set time of ZZ11A Meteorograph
        /// </summary>
        ZZ11ASETTIME = 933,
        /// <summary>
        /// A Set ID of ZZ11A Meteorograph
        /// </summary>
        ZZ11ASETID = 934,
        /// <summary>
        /// A Set output period of ZZ11A Meteorograph
        /// </summary>
        ZZ11ASETAUTOSEND = 935,
        /// <summary>
        /// A Read date from ZZ11A Meteorograph
        /// </summary>
        ZZ11AREADDATE = 936,
        /// <summary>
        /// A Read time from ZZ11A Meteorograph
        /// </summary>
        ZZ11AREADTIME = 937,
        /// <summary>
        /// A Read ID of ZZ11A Meteorograph
        /// </summary>
        ZZ11AREADID = 938,
        /// <summary>
        /// A Read the output period of ZZ11A Meteorograph
        /// </summary>
        ZZ11AREADAUTOSEND = 939,
    }

    /// <summary>
    /// Log Trigger Types
    /// </summary>
    public enum ComNavTriggerEnum
    {
        /// <summary>
        /// Synch
        /// </summary>
        ONTIME,
        /// <summary>
        /// Asynch
        /// Logs Supporting ONCHANGED and ONTRACKED
        /// 1 8 IONUTC 4.2.9.1
        /// 2 41 RAWEPHEM 4.2.1.17
        /// 3 71 BD2EPHEM 4.2.1.1
        /// 4 79 BINEX0101 4.3.4.2
        /// 5 80 BINEX0102 4.3.4.2
        /// 6 84 BINEX0105 4.3.4.2
        /// 7 89 RTCM0063 4.3.3.1
        /// 8 90 RTCM4011 NA
        /// 9 104 RTCM4013 NA
        /// 10 175 REFSTATION 4.2.11.1
        /// 11 412 BD2RAWEPHEM 4.2.1.5
        /// 12 712 GPSEPHEM 4.2.1.9
        /// 13 723 GLOEPHEMERIS 4.2.1.4
        /// 14 792 GLORAWEPHEM 4.2.1.8
        /// 15 893 RTCM1019 4.3.3.12
        /// 16 895 RTCM1020 4.3.3.13
        /// </summary>
        ONCHANGED,
        /// <summary>
        /// Polled
        /// </summary>
        ONCE
    }

    public class ComNavAsciiLogCommand: ComNavAsciiCommandBase
    {
        public ComNavMessageEnum Type { get; set; }
        public ComNavTriggerEnum? Trigger { get; set; }
        public uint? Period { get; set; }
        public uint? OffsetTime { get; set; }
        public ComNavFormat? Format { get; set; }
        public string PortName { get; set; }

        public override string MessageId => "LOG";

        protected override string SerializeToAsciiString()
        {
            var sb = new StringBuilder();
            sb.Append("LOG ");
            if (string.IsNullOrWhiteSpace(PortName) == false)
            {
                sb.Append(PortName);
                sb.Append(" ");
            }
            sb.Append(Type.GetMessageName());
            switch (Format)
            {
                case ComNavFormat.Binary:
                    sb.Append("B");
                    break;
                case ComNavFormat.Ascii:
                    sb.Append("A");
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (Trigger.HasValue)
            {
                sb.Append(" ");
                sb.Append(Trigger.Value.GetTriggerName());
            }
            if (Period.HasValue)
            {
                sb.Append(" ");
                sb.Append(Period.Value);
            }
            if (OffsetTime.HasValue)
            {
                sb.Append(" ");
                sb.Append(OffsetTime.Value);
            }
            return sb.ToString();
        }

       
    }
}