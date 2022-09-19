using System;
using Asv.IO;

namespace Asv.Gnss
{
    public static class RtcmV3Helper
    {
        public const byte SyncByte = 0xD3;

        /// <summary>
        /// rtcm ver.3 unit of gps pseudorange (m)
        /// </summary>
        public const double PRUNIT_GPS = 299792.458;

        /// <summary>
        /// rtcm 3 unit of glo pseudorange (m)
        /// </summary>
        public const double PRUNIT_GLO = 599584.916;

        /// <summary>
        /// speed of light (m/s)
        /// </summary>
        public const double CLIGHT = 299792458.0;

        /// <summary>
        /// semi-circle to radian (IS-GPS)
        /// </summary>
        public const double SC2RAD = 3.1415926535898;

        /// <summary>
        /// obs code: none or unknown
        /// </summary>
        public const byte CODE_NONE = 0;

        /// <summary>
        /// max number of obs code
        /// </summary>
        public const byte MAXCODE = 68;

        /// <summary>
        /// L1/E1  frequency (Hz)
        /// </summary>
        public const double FREQ1 = 1.57542E9;

        /// <summary>
        ///  L2     frequency (Hz) 
        /// </summary>
        public const double FREQ2 = 1.22760E9;

        /// <summary>
        /// L5/E5a/B2a frequency (Hz) 
        ///  </summary>
        public const double FREQ5 = 1.17645E9;
        /// <summary>
        /// E6/L6  frequency (Hz) 
        ///  </summary>
        public const double FREQ6 = 1.27875E9;
        /// <summary>
        /// E5b    frequency (Hz) 
        ///  </summary>
        public const double FREQ7 = 1.20714E9;
        /// <summary>
        /// E5a+b  frequency (Hz) 
        ///  </summary>
        public const double FREQ8 = 1.191795E9;
        /// <summary>
        /// S      frequency (Hz) 
        ///  </summary>
        public const double FREQ9 = 2.492028E9;
        /// <summary>
        /// GLONASS G1 base frequency (Hz) 
        ///  </summary>
        public const double FREQ1_GLO = 1.60200E9;
        /// <summary>
        /// GLONASS G1 bias frequency (Hz/n) 
        ///  </summary>
        public const double DFRQ1_GLO = 0.56250E6;
        /// <summary>
        /// GLONASS G2 base frequency (Hz) 
        ///  </summary>
        public const double FREQ2_GLO = 1.24600E9;
        /// <summary>
        /// GLONASS G2 bias frequency (Hz/n) 
        ///  </summary>
        public const double DFRQ2_GLO = 0.43750E6;
        /// <summary>
        /// GLONASS G3 frequency (Hz) 
        ///  </summary>
        public const double FREQ3_GLO = 1.202025E9;
        /// <summary>
        /// GLONASS G1a frequency (Hz) 
        ///  </summary>
        public const double FREQ1a_GLO = 1.600995E9;
        /// <summary>
        /// GLONASS G2a frequency (Hz) 
        ///  </summary>
        public const double FREQ2a_GLO = 1.248060E9;
        /// <summary>
        /// BDS B1I     frequency (Hz) 
        ///  </summary>
        public const double FREQ1_CMP = 1.561098E9;
        /// <summary>
        /// BDS B2I/B2b frequency (Hz) 
        ///  </summary>
        public const double FREQ2_CMP = 1.20714E9;
        /// <summary>
        /// BDS B3      frequency (Hz) 
        ///  </summary>
        public const double FREQ3_CMP = 1.26852E9;

        /// <summary>
        /// number of carrier frequencies
        /// </summary>
        public const byte NFREQ = 3;

        /// <summary>
        /// number of carrier frequencies of GLONASS
        /// </summary>
        public const byte NFREQGLO = 2;

        /// <summary>
        /// number of extended obs codes
        /// </summary>
        public const byte NEXOBS = 0;

        /// <summary>
        /// SNR unit (dBHz)
        /// </summary>
        public const double SNR_UNIT = 0.001;

        /// <summary>
        /// min satellite PRN number of GPS
        /// </summary>
        public const byte MINPRNGPS = 1;
        /// <summary>
        /// max satellite PRN number of GPS
        /// </summary>
        public const byte MAXPRNGPS = 32;
        /// <summary>
        /// number of GPS satellites
        /// </summary>
        public const byte NSATGPS = (MAXPRNGPS - MINPRNGPS + 1);
        public const byte NSYSGPS = 1;

        /// <summary>
        /// min satellite slot number of GLONASS
        /// </summary>
        public const byte MINPRNGLO = 1;
        /// <summary>
        /// max satellite slot number of GLONASS
        /// </summary>
        public const byte MAXPRNGLO = 27;
        /// <summary>
        /// number of GLONASS satellites
        /// </summary>
        public const byte NSATGLO = (MAXPRNGLO - MINPRNGLO + 1);
        public const byte NSYSGLO = 1;

        /// <summary>
        /// min satellite PRN number of Galileo
        /// </summary>
        public const byte MINPRNGAL = 1;
        /// <summary>
        /// max satellite PRN number of Galileo
        /// </summary>
        public const byte MAXPRNGAL = 36;
        /// <summary>
        /// number of Galileo satellites
        /// </summary>
        public const byte NSATGAL = (MAXPRNGAL - MINPRNGAL + 1);
        public const byte NSYSGAL = 1;


        /// <summary>
        /// min satellite PRN number of QZSS
        /// </summary>
        public const byte MINPRNQZS = 193;
        /// <summary>
        /// max satellite PRN number of QZSS
        /// </summary>
        public const byte MAXPRNQZS = 202;
        /// <summary>
        /// min satellite PRN number of QZSS L1S
        /// </summary>
        public const byte MINPRNQZS_S = 183;
        /// <summary>
        /// max satellite PRN number of QZSS L1S
        /// </summary>
        public const byte MAXPRNQZS_S = 191;
        /// <summary>
        /// number of QZSS satellites
        /// </summary>
        public const byte NSATQZS = (MAXPRNQZS - MINPRNQZS + 1);
        public const byte NSYSQZS = 1;


        /// <summary>
        /// min satellite sat number of BeiDou
        /// </summary>
        public const byte MINPRNCMP = 1;
        /// <summary>
        /// max satellite sat number of BeiDou
        /// </summary>
        public const byte MAXPRNCMP = 63;
        /// <summary>
        /// number of BeiDou satellites
        /// </summary>
        public const byte NSATCMP = (MAXPRNCMP - MINPRNCMP + 1);
        public const byte NSYSCMP = 1;


        /// <summary>
        /// min satellite sat number of IRNSS
        /// </summary>
        public const byte MINPRNIRN = 1;
        /// <summary>
        /// max satellite sat number of IRNSS
        /// </summary>
        public const byte MAXPRNIRN = 14;
        /// <summary>
        /// number of IRNSS satellites
        /// </summary>
        public const byte NSATIRN = (MAXPRNIRN - MINPRNIRN + 1);
        public const byte NSYSIRN = 1;


        /// <summary>
        /// min satellite sat number of LEO
        /// </summary>
        public const byte MINPRNLEO = 1;
        /// <summary>
        /// max satellite sat number of LEO
        /// </summary>
        public const byte MAXPRNLEO = 10;
        /// <summary>
        /// number of LEO satellites
        /// </summary>
        public const byte NSATLEO = (MAXPRNLEO - MINPRNLEO + 1);
        public const byte NSYSLEO = 1;


        /// <summary>
        /// number of systems
        /// </summary>
        public const int NSYS = (NSYSGPS + NSYSGLO + NSYSGAL + NSYSQZS + NSYSCMP + NSYSIRN + NSYSLEO);

        /// <summary>
        /// min satellite PRN number of SBAS
        /// </summary>
        public const byte MINPRNSBS = 120;
        /// <summary>
        /// max satellite PRN number of SBAS
        /// </summary>
        public const byte MAXPRNSBS = 158;
        /// <summary>
        /// number of SBAS satellites
        /// </summary>
        public const byte NSATSBS = (MAXPRNSBS - MINPRNSBS + 1);

        /// <summary>
        /// max satellite number (1 to MAXSAT)
        /// </summary>
        public const byte MAXSAT = (NSATGPS + NSATGLO + NSATGAL + NSATQZS + NSATCMP + NSATIRN + NSATSBS + NSATLEO);

        /// <summary>
        /// 
        /// </summary>
        public const byte MAXSTA = 255;


        /// <summary>
        /// max number of obs in an epoch
        /// </summary>
        public const byte MAXOBS = 96;
        /// <summary>
        /// max receiver number (1 to MAXRCV)
        /// </summary>
        public const byte MAXRCV = 64;
        /// <summary>
        /// max number of obs type in RINEX
        /// </summary>
        public const byte MAXOBSTYPE = 64;
        /// <summary>
        /// range in 1 ms
        /// </summary>
        public const double RANGE_MS = CLIGHT * 0.001;

        /// <summary>
        ///  2^-5
        /// </summary>
        public const double P2_5 = 0.03125;

        /// <summary>
        /// 2^-6
        /// </summary>
        public const double P2_6 = 0.015625;

        /// <summary>
        /// 2^-10
        /// </summary>
        public const double P2_10 = 0.0009765625;

        /// <summary>
        /// 2^-11
        /// </summary>
        public const double P2_11 = 4.882812500000000E-04;

        /// <summary>
        /// 2^-15
        /// </summary>
        public const double P2_15 = 3.051757812500000E-05;

        /// <summary>
        /// 2^-17
        /// </summary>
        public const double P2_17 = 7.629394531250000E-06;

        /// <summary>
        /// 2^-19
        /// </summary>
        public const double P2_19 = 1.907348632812500E-06;

        /// <summary>
        /// 2^-20
        /// </summary>
        public const double P2_20 = 9.536743164062500E-07;

        /// <summary>
        /// 2^-21
        /// </summary>
        public const double P2_21 = 4.768371582031250E-07;

        /// <summary>
        /// 2^-23
        /// </summary>
        public const double P2_23 = 1.192092895507810E-07;

        /// <summary>
        /// 2^-24
        /// </summary>
        public const double P2_24 = 5.960464477539063E-08;

        /// <summary>
        /// 2^-27
        /// </summary>
        public const double P2_27 = 7.450580596923828E-09;

        /// <summary>
        /// 2^-29
        /// </summary>
        public const double P2_29 = 1.862645149230957E-09;

        /// <summary>
        /// 2^-30
        /// </summary>
        public const double P2_30 = 9.313225746154785E-10;

        /// <summary>
        /// 2^-31
        /// </summary>
        public const double P2_31 = 4.656612873077393E-10;

        /// <summary>
        /// 2^-32
        /// </summary>
        public const double P2_32 = 2.328306436538696E-10;

        /// <summary>
        /// 2^-33
        /// </summary>
        public const double P2_33 = 1.164153218269348E-10;

        /// <summary>
        /// 2^-35
        /// </summary>
        public const double P2_35 = 2.910383045673370E-11;

        /// <summary>
        /// 2^-38
        /// </summary>
        public const double P2_38 = 3.637978807091710E-12;

        /// <summary>
        /// 2^-39
        /// </summary>
        public const double P2_39 = 1.818989403545856E-12;

        /// <summary>
        /// 2^-40
        /// </summary>
        public const double P2_40 = 9.094947017729280E-13;

        /// <summary>
        /// 2^-43
        /// </summary>
        public const double P2_43 = 1.136868377216160E-13;

        /// <summary>
        /// 2^-48
        /// </summary>
        public const double P2_48 = 3.552713678800501E-15;

        /// <summary>
        /// 2^-50
        /// </summary>
        public const double P2_50 = 8.881784197001252E-16;

        /// <summary>
        /// 2^-55
        /// </summary>
        public const double P2_55 = 2.775557561562891E-17;

        /// <summary>
        /// earth semimajor axis (WGS84) (m)
        /// </summary>
        public const double RE_WGS84 = 6378137.0;

        /// <summary>
        /// earth flattening (WGS84)
        /// </summary>
        public const double FE_WGS84 = (1.0 / 298.257223563);

        /// <summary>
        /// deg to rad
        /// </summary>
        public const double D2R = (Math.PI / 180.0);

        /// <summary>
        /// rad to deg 
        /// </summary>
        public const double R2D = (180.0 / Math.PI);

        public readonly static string[] ObsCodes = {       /* observation code strings */
    
            ""  ,"1C","1P","1W","1Y", "1M","1N","1S","1L","1E", /*  0- 9 */
            "1A","1B","1X","1Z","2C", "2D","2S","2L","2X","2P", /* 10-19 */
            "2W","2Y","2M","2N","5I", "5Q","5X","7I","7Q","7X", /* 20-29 */
            "6A","6B","6C","6X","6Z", "6S","6L","8L","8Q","8X", /* 30-39 */
            "2I","2Q","6I","6Q","3I", "3Q","3X","1I","1Q","5A", /* 40-49 */
            "5B","5C","9A","9B","9C", "9X","1D","5D","5P","5Z", /* 50-59 */
            "6E","7D","7P","7Z","8D", "8P","4A","4B","4X",""    /* 60-69 */
        };

        public readonly static string[][] CodePris={  /* code priority for each freq-index */
            /*    0         1          2          3         4         5     */
            new[] {"CPYWMNSL","PYWCMNDLSX","IQX"     ,""       ,""       ,""      ,""}, /* GPS */
            new[] {"CPABX"   ,"PCABX"     ,"IQX"     ,""       ,""       ,""      ,""}, /* GLO */
            new[] {"CABXZ"   ,"IQX"       ,"IQX"     ,"ABCXZ"  ,"IQX"    ,""      ,""}, /* GAL */
            new[] {"CLSXZ"   ,"LSX"       ,"IQXDPZ"  ,"LSXEZ"  ,""       ,""      ,""}, /* QZS */
            new[] {"C"       ,"IQX"       ,""        ,""       ,""       ,""      ,""}, /* SBS */
            new[] {"IQXDPAN" ,"IQXDPZ"    ,"DPX"     ,"IQXA"   ,"DPX"    ,""      ,""}, /* BDS */
            new[] {"ABCX"    ,"ABCX"      ,""        ,""       ,""       ,""      ,""}  /* IRN */
        };

        /* MSM signal ID Rinex Code table -------------------------------------------------------*/
        public static readonly string[] msm_sig_gps =
        {
            /* GPS: ref [17] table 3.5-91 */
            "", "1C", "1P", "1W", "", "", "", "2C", "2P", "2W", "", "", /*  1-12 */
            "", "", "2S", "2L", "2X", "", "", "", "", "5I", "5Q", "5X", /* 13-24 */
            "", "", "", "", "", "1S", "1L", "1X" /* 25-32 */
        };



        public static readonly string[] msm_sig_glo =
        {
            /* GLONASS: ref [17] table 3.5-96 */
            "", "1C", "1P", "", "", "", "", "2C", "2P", "", "", "",
            "", "", "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", ""
        };

        public static readonly string[] msm_sig_gal =
        {
            /* Galileo: ref [17] table 3.5-99 */
            "", "1C", "1A", "1B", "1X", "1Z", "", "6C", "6A", "6B", "6X", "6Z",
            "", "7I", "7Q", "7X", "", "8I", "8Q", "8X", "", "5I", "5Q", "5X",
            "", "", "", "", "", "", "", ""
        };

        public static readonly string[] msm_sig_qzs =
        {
            /* QZSS: ref [17] table 3.5-105 */
            "", "1C", "", "", "", "", "", "", "6S", "6L", "6X", "",
            "", "", "2S", "2L", "2X", "", "", "", "", "5I", "5Q", "5X",
            "", "", "", "", "", "1S", "1L", "1X"
        };

        public static readonly string[] msm_sig_sbs =
        {
            /* SBAS: ref [17] table 3.5-102 */
            "", "1C", "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "", "", "5I", "5Q", "5X",
            "", "", "", "", "", "", "", ""
        };

        public static readonly string[] msm_sig_cmp =
        {
            /* BeiDou: ref [17] table 3.5-108 */
            "", "2I", "2Q", "2X", "", "", "", "6I", "6Q", "6X", "", "",
            "", "7I", "7Q", "7X", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", ""
        };

        public static readonly string[] msm_sig_irn =
        {
            /* NavIC/IRNSS: ref [17] table 3.5-108.3 */
            "", "", "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "", "", "5A", "", "",
            "", "", "", "", "", "", "", ""
        };

        public static string Sat2Code(int sat, int prn = 0)
        {
            var p = prn;
            var sys  = GetSatelliteSystem(sat, ref p);
            switch (sys)
            {
                case NavigationSystemEnum.SYS_GPS: return $"G{prn - MINPRNGPS + 1}";
                case NavigationSystemEnum.SYS_GLO: return $"R{prn - MINPRNGLO + 1}";
                case NavigationSystemEnum.SYS_GAL: return $"E{prn - MINPRNGAL + 1}";
                case NavigationSystemEnum.SYS_SBS: return $"S{prn - 100}";
                case NavigationSystemEnum.SYS_QZS: return $"J{prn - MINPRNQZS + 1}";
                case NavigationSystemEnum.SYS_CMP: return $"C{prn - MINPRNCMP + 1}";
                case NavigationSystemEnum.SYS_IRN: return $"I{prn - MINPRNIRN + 1}";
                default: return "";
            }
        }

        public static string GetSatelliteSystemLetter(NavigationSystemEnum sys)
        {
            switch (sys)
            {
                case NavigationSystemEnum.SYS_GPS: return "G";
                case NavigationSystemEnum.SYS_GLO: return "R";
                case NavigationSystemEnum.SYS_GAL: return "E";
                case NavigationSystemEnum.SYS_SBS: return "S";
                case NavigationSystemEnum.SYS_QZS: return "J";
                case NavigationSystemEnum.SYS_CMP: return "C";
                case NavigationSystemEnum.SYS_IRN: return "I";
                default: return "";
            }
        }

        /// <summary>
        /// Satellite number to satellite system.
        /// Convert satellite number to satellite system
        /// </summary>
        /// <param name="sat">I   satellite number (1-MAXSAT)</param>
        /// <param name="prn">IO  satellite prn/slot number (NULL: no output)</param>
        /// <returns>satellite system (SYS_GPS,SYS_GLO,...)</returns>
        public static NavigationSystemEnum GetSatelliteSystem(int sat, ref int prn)
        {
            var sys = NavigationSystemEnum.SYS_NONE;
            if (sat <= 0 || MAXSAT < sat) sat = 0;
            else if (sat <= NSATGPS)
            {
                sys = NavigationSystemEnum.SYS_GPS; sat += MINPRNGPS - 1;
            }
            else if ((sat -= NSATGPS) <= NSATGLO)
            {
                sys = NavigationSystemEnum.SYS_GLO; sat += MINPRNGLO - 1;
            }
            else if ((sat -= NSATGLO) <= NSATGAL)
            {
                sys = NavigationSystemEnum.SYS_GAL; sat += MINPRNGAL - 1;
            }
            else if ((sat -= NSATGAL) <= NSATQZS)
            {
                sys = NavigationSystemEnum.SYS_QZS; sat += MINPRNQZS - 1;
            }
            else if ((sat -= NSATQZS) <= NSATCMP)
            {
                sys = NavigationSystemEnum.SYS_CMP; sat += MINPRNCMP - 1;
            }
            else if ((sat -= NSATCMP) <= NSATIRN)
            {
                sys = NavigationSystemEnum.SYS_IRN; sat += MINPRNIRN - 1;
            }
            else if ((sat -= NSATIRN) <= NSATLEO)
            {
                sys = NavigationSystemEnum.SYS_LEO; sat += MINPRNLEO - 1;
            }
            else if ((sat -= NSATLEO) <= NSATSBS)
            {
                sys = NavigationSystemEnum.SYS_SBS; sat += MINPRNSBS - 1;
            }
            else sat = 0;
            if (prn != 0) prn = sat;
            return sys;
        }

        public static DateTime GetFromGps(int weeknumber, double seconds)
        {
            
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var week = datum.AddDays(weeknumber * 7);
            var time = week.AddSeconds(seconds);
            return time;
        }

        public static DateTime GetFromGalileo(int weeknumber, double seconds)
        {
            var datum = new DateTime(1999, 8, 22, 0, 0, 0, DateTimeKind.Utc);
            var week = datum.AddDays(weeknumber * 7);
            var time = week.AddSeconds(seconds);
            return time;
        }

        public static DateTime GetFromBeiDou(int weeknumber, double seconds)
        {
            var datum = new DateTime(2006, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var week = datum.AddDays(weeknumber * 7);
            var time = week.AddSeconds(seconds);
            return time;
        }

        public static int LeapSecondsGPS(int year, int month)
        {
            return LeapSecondsTAI(year, month) - 19;
        }

        public static int LeapSecondsTAI(int year, int month)
        {
            //http://maia.usno.navy.mil/ser7/tai-utc.dat

            var yyyymm = year * 100 + month;
            if (yyyymm >= 201701) return 37;
            if (yyyymm >= 201507) return 36;
            if (yyyymm >= 201207) return 35;
            if (yyyymm >= 200901) return 34;
            if (yyyymm >= 200601) return 33;
            if (yyyymm >= 199901) return 32;
            if (yyyymm >= 199707) return 31;
            if (yyyymm >= 199601) return 30;
            if (yyyymm >= 199407) return 29;
            if (yyyymm >= 199307) return 28;
            if (yyyymm >= 199207) return 27;
            if (yyyymm >= 199101) return 26;
            if (yyyymm >= 199001) return 25;
            if (yyyymm >= 198801) return 24;
            if (yyyymm >= 198507) return 23;
            if (yyyymm >= 198307) return 22;
            if (yyyymm >= 198207) return 21;
            if (yyyymm >= 198107) return 20;
            if (yyyymm >= 0) return 19;

            return 0;
        }

        public static DateTime Utc2Gps(DateTime t)
        {
            return t.AddSeconds(LeapSecondsGPS(t.Year, t.Month));
        }

        public static DateTime Gps2Utc(DateTime t)
        {
            return t.AddSeconds(-LeapSecondsGPS(t.Year, t.Month));
        }

        public static DateTime Gps2BeiDou(DateTime t)
        {
            return t.AddSeconds(-14.0);
        }

        public static DateTime BeiDou2Gps(DateTime t)
        {
            return t.AddSeconds(14.0);
        }

        public static void GetFromTime(DateTime time, ref int week, ref double seconds)
        {
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);

            var dif = time - datum;

            var weeks = (int) (dif.TotalDays / 7);

            week = weeks;

            dif = time - datum.AddDays(weeks * 7);

            seconds = dif.TotalSeconds;
        }

        /// <summary>
        /// Transform ecef position to geodetic position.
        /// Notes  : WGS84, ellipsoidal height
        /// </summary>
        /// <param name="r">I   ecef position {x,y,z} (m)</param>
        /// <param name="pos">O   geodetic position {lat,lon,h} (rad,m)</param>
        public static void EcefToPos(double[] r, double[] pos)
        {
            var e2 = FE_WGS84 * (2.0 - FE_WGS84);
            double r2 = Dot(r, r, 2), z, zk;
            var v = RE_WGS84;

            for (z = r[2], zk = 0.0; Math.Abs(z - zk) >= 1E-4;)
            {
                zk = z;
                var sinp = z / Math.Sqrt(r2 + z * z);
                v = RE_WGS84 / Math.Sqrt(1.0 - e2 * sinp * sinp);
                z = r[2] + v * e2 * sinp;
            }

            pos[0] = r2 > 1E-12 ? Math.Atan(z / Math.Sqrt(r2)) : (r[2] > 0.0 ? Math.PI / 2.0 : -Math.PI / 2.0);
            pos[1] = r2 > 1E-12 ? Math.Atan2(r[1], r[0]) : 0.0;
            pos[2] = Math.Sqrt(r2 + z * z) - v;
        }

        /// <summary>
        /// Transform geodetic position to ecef position.
        /// Notes  : WGS84, ellipsoidal height
        /// </summary>
        /// <param name="pos">I   geodetic position {lat,lon,h} (rad,m)</param>
        /// <param name="r">O   ecef position {x,y,z} (m)</param>
        public static void PosToEcef(double[] pos, ref double[] r)
        {
            double sinp = Math.Sin(pos[0]),
                cosp = Math.Cos(pos[0]),
                sinl = Math.Sin(pos[1]),
                cosl = Math.Cos(pos[1]);
            double e2 = FE_WGS84 * (2.0 - FE_WGS84), v = RE_WGS84 / Math.Sqrt(1.0 - e2 * sinp * sinp);

            r[0] = (v + pos[2]) * cosp * cosl;
            r[1] = (v + pos[2]) * cosp * sinl;
            r[2] = (v * (1.0 - e2) + pos[2]) * sinp;
        }

        public static double Dot(double[] a, double[] b, int n)
        {
            var c = 0.0;

            while (--n >= 0) c += a[n] * b[n];
            return c;
        }

        

        /// <summary>
        /// Carrier-phase - Pseudorange in cycle
        /// </summary>
        /// <param name="cp">carrier-phase</param>
        /// <param name="pr_cyc">pseudorange in cycle</param>
        /// <returns></returns>
        public static double CarrierPhasePseudorange(double cp, double pr_cyc)
        {
            var x = (cp - pr_cyc + 1500.0) % 3000.0;
            if (x < 0)
                x += 3000;
            x -= 1500.0;
            return x;
        }


        public static double ROUND(double x)
        {
            return (int) Math.Floor(x + 0.5);
        }

        /* carrier-phase - pseudorange in cycle --------------------------------------*/

        public static double cp_pr(double cp, double pr_cyc)
        {
            var x = (cp - pr_cyc + 1500.0) % 3000.0;
            if (x < 0)
                x += 3000;
            x -= 1500.0;
            return x;
        }

        public static NavigationSystemEnum GetNavigationSystem(ushort messageId)
        {
            switch (messageId)
            {
                case 1071:
                case 1072:
                case 1073:
                case 1074:
                case 1075:
                case 1076:
                case 1077:
                    return NavigationSystemEnum.SYS_GPS;
                case 1081:
                case 1082:
                case 1083:
                case 1084:
                case 1085:
                case 1086:
                case 1087:
                    return NavigationSystemEnum.SYS_GLO;
                case 1091:
                case 1092:
                case 1093:
                case 1094:
                case 1095:
                case 1096:
                case 1097:
                    return NavigationSystemEnum.SYS_GAL;
                case 1101:
                case 1102:
                case 1103:
                case 1104:
                case 1105:
                case 1106:
                case 1107:
                    return NavigationSystemEnum.SYS_SBS;
                case 1111:
                case 1112:
                case 1113:
                case 1114:
                case 1115:
                case 1116:
                case 1117:
                    return NavigationSystemEnum.SYS_QZS;
                case 1121:
                case 1122:
                case 1123:
                case 1124:
                case 1125:
                case 1126:
                case 1127:
                    return NavigationSystemEnum.SYS_CMP;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DateTime GetUtc(DateTime nowUtc, double tod)
        {
            var tow = 0.0;
            var week = 0;

            var time = nowUtc;

            GetFromTime(time, ref week, ref tow);

            var todP = tow % 86400.0;
            tow -= todP;

            if (tod < todP - 43200.0)
                tod += 86400.0;
            else if (tod > todP + 43200.0)
                tod -= 86400.0;

            return GetFromGps(week, tow + tod);
        }

        /// <summary>
        /// adjust daily rollover of GLONASS time
        /// </summary>
        /// <param name="nowUtc"></param>
        /// <param name="tod"></param>
        /// <returns></returns>
        public static DateTime AdjustDailyRoverGlonassTime(DateTime nowUtc, double tod)
        {
            var tow = 0.0;
            var week = 0;

            var time = nowUtc.AddSeconds(10800.0); /* glonass time */
            
            GetFromTime(time, ref week, ref tow);
            
            var todP = tow % 86400.0;
            tow -= todP;

            if (tod < todP - 43200.0)
                tod += 86400.0;
            else if (tod > todP + 43200.0)
                tod -= 86400.0;

            time = GetFromGps(week, tow + tod);
            return Utc2Gps(time.AddSeconds(-10800.0));
        }

        /// <summary>
        /// Adjust weekly rollover of GPS time
        /// </summary>
        /// <param name="nowUtc"></param>
        /// <param name="tow"></param>
        /// <returns></returns>
        public static DateTime AdjustWeekly(DateTime nowUtc, double tow)
        {
            var towP = 0.0;
            var week = 0;

            var time = Utc2Gps(nowUtc);
            GetFromTime(time, ref week, ref towP);

            if (tow < towP - 302400.0) week += 1;
            else if (tow > towP + 302400.0) week -= 1;
            return GetFromGps(week, tow);
        }

        public static int adjgpsweek(DateTime utc, int week)
        {
            var w = 0;
            var s = 0.0;
            GetFromTime(Utc2Gps(utc), ref w, ref s);
            if (w < 1560) w = 1560; /* use 2009/12/1 if time is earlier than 2009/12/1 */
            return week + (w - week + 1) / 1024 * 1024;
        }
        
        public static DateTime epoch2time(DateTime ep)
        {
            int[] doy = { 1, 32, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335 };

            var time = new DateTime(0,0,0,0,0,0, DateTimeKind.Utc);

            var year = ep.Year;
            var mon = ep.Month;
            var day = ep.Day;

            if (year<1970||2099<year||mon<1||12<mon) return time;
    
            /* leap year if year%4==0 in 1901-2099 */
            var days = (year - 1970) * 365 + (year - 1969) / 4 + doy[mon - 1] + day - 2 + ((year % 4 == 0 && mon >= 3) ? 1 : 0);
            var sec = (int) Math.Floor(ep.Second + ep.Millisecond / 1000.0);
            
            time = time.AddDays(days).AddHours(ep.Hour).AddMinutes(ep.Minute).AddSeconds(ep.Second);
            time = time.AddSeconds(-(ep.Second - sec));    
            
            return time;
        }

    public static byte Obs2Code(string obs)
        {
            for (byte i = 0; i < ObsCodes.Length;i++) {
                if (!string.Equals(ObsCodes[i], obs)) continue;
                return i;
            }
            return 0;
        }

        public static int Code2Idx(NavigationSystemEnum sys, byte code)
        {
            var freq = 0.0;

            switch (sys)
            {
                case NavigationSystemEnum.SYS_GPS:
                    return code2freq_GPS(code, ref freq);
                case NavigationSystemEnum.SYS_SBS:
                    return code2freq_SBS(code, ref freq);
                case NavigationSystemEnum.SYS_GLO:
                    return code2freq_GLO(code, 0, ref freq);
                case NavigationSystemEnum.SYS_GAL:
                    return code2freq_GAL(code, ref freq);
                case NavigationSystemEnum.SYS_QZS:
                    return code2freq_QZS(code, ref freq);
                case NavigationSystemEnum.SYS_CMP:
                    return code2freq_BDS(code, ref freq);
                case NavigationSystemEnum.SYS_IRN:
                    return code2freq_IRN(code, ref freq);
                default:
                    return -1;
            }
        }

        public static string Code2Obs(byte code)
        {
            if (code <= CODE_NONE || MAXCODE < code) return "";
            return ObsCodes[code];
        }

        /* GPS obs code to frequency -------------------------------------------------*/
        public static int code2freq_GPS(byte code, ref double freq)
        {
            var obs = Code2Obs(code);
            if (string.IsNullOrEmpty(obs)) return -1;
            switch (obs[0])
            {
                case '1': freq = FREQ1; return 0; /* L1 */
                case '2': freq = FREQ2; return 1; /* L2 */
                case '5': freq = FREQ5; return 2; /* L5 */
            }
            return -1;
        }
        /* GLONASS obs code to frequency ---------------------------------------------*/
        public static int code2freq_GLO(byte code, int fcn, ref double freq)
        {
            var obs = Code2Obs(code);

            if (fcn < -7 || fcn > 6) return -1;
            if (string.IsNullOrEmpty(obs)) return -1;
            switch (obs[0])
            {
                case '1': freq = FREQ1_GLO + DFRQ1_GLO * fcn; return 0; /* G1 */
                case '2': freq = FREQ2_GLO + DFRQ2_GLO * fcn; return 1; /* G2 */
                case '3': freq = FREQ3_GLO; return 2; /* G3 */
                case '4': freq = FREQ1a_GLO; return 0; /* G1a */
                case '6': freq = FREQ2a_GLO; return 1; /* G2a */
            }
            return -1;
        }
        /* Galileo obs code to frequency ---------------------------------------------*/
        public static int code2freq_GAL(byte code, ref double freq)
        {
            var obs = Code2Obs(code);
            if (string.IsNullOrEmpty(obs)) return -1;
            switch (obs[0])
            {
                case '1': freq = FREQ1; return 0; /* E1 */
                case '7': freq = FREQ7; return 1; /* E5b */
                case '5': freq = FREQ5; return 2; /* E5a */
                case '6': freq = FREQ6; return 3; /* E6 */
                case '8': freq = FREQ8; return 4; /* E5ab */
            }
            return -1;
        }
        /* QZSS obs code to frequency ------------------------------------------------*/
        public static int code2freq_QZS(byte code, ref double freq)
        {
            var obs = Code2Obs(code);
            if (string.IsNullOrEmpty(obs)) return -1;
            switch (obs[0])
            {
                case '1': freq = FREQ1; return 0; /* L1 */
                case '2': freq = FREQ2; return 1; /* L2 */
                case '5': freq = FREQ5; return 2; /* L5 */
                case '6': freq = FREQ6; return 3; /* L6 */
            }
            return -1;
        }
        /* SBAS obs code to frequency ------------------------------------------------*/
        public static int code2freq_SBS(byte code, ref double freq)
        {
            var obs = Code2Obs(code);
            if (string.IsNullOrEmpty(obs)) return -1;
            switch (obs[0])
            {
                case '1': freq = FREQ1; return 0; /* L1 */
                case '5': freq = FREQ5; return 1; /* L5 */
            }
            return -1;
        }
        /* BDS obs code to frequency -------------------------------------------------*/
        public static int code2freq_BDS(byte code, ref double freq)
        {
            var obs = Code2Obs(code);
            if (string.IsNullOrEmpty(obs)) return -1;
            switch (obs[0])
            {
                case '1':
                    freq = FREQ1;
                    return 0; /* B1C */
                case '2':
                    freq = FREQ1_CMP;
                    return 0; /* B1I */
                case '7':
                    freq = FREQ2_CMP;
                    return 1; /* B2I/B2b */
                case '5':
                    freq = FREQ5;
                    return 2; /* B2a */
                case '6':
                    freq = FREQ3_CMP;
                    return 3; /* B3 */
                case '8':
                    freq = FREQ8;
                    return 4; /* B2ab */
                default:
                    return -1;
            }

        }
        /* NavIC obs code to frequency -----------------------------------------------*/
        public static int code2freq_IRN(byte code, ref double freq)
        {
            var obs = Code2Obs(code);
            if (string.IsNullOrEmpty(obs)) return -1;
            switch (obs[0])
            {
                case '5': freq = FREQ5; return 0; /* L5 */
                case '9': freq = FREQ9; return 1; /* S */
            }
            return -1;
        }

        /// <summary>
        /// * convert satellite system+prn/slot number to satellite number
        /// * args   : int    sys       I   satellite system (SYS_GPS,SYS_GLO,...)
        /// *          int    prn       I   satellite prn/slot number
        /// * return : satellite number (0:error)
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="prn"></param>
        /// <returns></returns>
        public static int satno(NavigationSystemEnum sys, int prn)
        {
            if (prn <= 0) return 0;
            switch (sys)
            {
                case NavigationSystemEnum.SYS_GPS:
                    if (prn < MINPRNGPS || MAXPRNGPS < prn) return 0;
                    return prn - MINPRNGPS + 1;
                case NavigationSystemEnum.SYS_GLO:
                    if (prn < MINPRNGLO || MAXPRNGLO < prn) return 0;
                    return NSATGPS + prn - MINPRNGLO + 1;
                case NavigationSystemEnum.SYS_GAL:
                    if (prn < MINPRNGAL || MAXPRNGAL < prn) return 0;
                    return NSATGPS + NSATGLO + prn - MINPRNGAL + 1;
                case NavigationSystemEnum.SYS_QZS:
                    if (prn < MINPRNQZS || MAXPRNQZS < prn) return 0;
                    return NSATGPS + NSATGLO + NSATGAL + prn - MINPRNQZS + 1;
                case NavigationSystemEnum.SYS_CMP:
                    if (prn < MINPRNCMP || MAXPRNCMP < prn) return 0;
                    return NSATGPS + NSATGLO + NSATGAL + NSATQZS + prn - MINPRNCMP + 1;
                case NavigationSystemEnum.SYS_IRN:
                    if (prn < MINPRNIRN || MAXPRNIRN < prn) return 0;
                    return NSATGPS + NSATGLO + NSATGAL + NSATQZS + NSATCMP + prn - MINPRNIRN + 1;
                case NavigationSystemEnum.SYS_LEO:
                    if (prn < MINPRNLEO || MAXPRNLEO < prn) return 0;
                    return NSATGPS + NSATGLO + NSATGAL + NSATQZS + NSATCMP + NSATIRN +
                        prn - MINPRNLEO + 1;
                case NavigationSystemEnum.SYS_SBS:
                    if (prn < MINPRNSBS || MAXPRNSBS < prn) return 0;
                    return NSATGPS + NSATGLO + NSATGAL + NSATQZS + NSATCMP + NSATIRN + NSATLEO +
                        prn - MINPRNSBS + 1;
            }
            return 0;
        }

        public static double Code2Freq(NavigationSystemEnum sys, byte code, int fcn)
        {
            var freq = 0.0;

            switch (sys)
            {
                case NavigationSystemEnum.SYS_GPS: code2freq_GPS(code, ref freq); break;
                case NavigationSystemEnum.SYS_GLO: code2freq_GLO(code, fcn, ref freq); break;
                case NavigationSystemEnum.SYS_GAL: code2freq_GAL(code, ref freq); break;
                case NavigationSystemEnum.SYS_QZS: code2freq_QZS(code, ref freq); break;
                case NavigationSystemEnum.SYS_SBS: code2freq_SBS(code, ref freq); break;
                case NavigationSystemEnum.SYS_CMP: code2freq_BDS(code, ref freq); break;
                case NavigationSystemEnum.SYS_IRN: code2freq_IRN(code, ref freq); break;
            }
            return freq;
        }

        public static double GetMinLockTime(byte indicator)
        {
            if (indicator == 0) return 0;
            var result = 32;
            for (var i = 1; i < indicator; i++)
            {
                result *= 2;
            }

            return result / 60000.0;
        }

        public static double GetMinLockTimeEx(ushort indicator)
        {
            if (indicator <= 63) return indicator / 60000.0;
            if (64 <= indicator && indicator <= 95) return (2 * indicator - 64) / 60000.0;
            if (96 <= indicator && indicator <= 127) return (4 * indicator - 256) / 60000.0;
            if (128 <= indicator && indicator <= 159) return (8 * indicator - 768) / 60000.0;
            if (160 <= indicator && indicator <= 191) return (16 * indicator - 2048) / 60000.0;
            if (192 <= indicator && indicator <= 223) return (32 * indicator - 5120) / 60000.0;
            if (224 <= indicator && indicator <= 255) return (64 * indicator - 12288) / 60000.0;
            if (256 <= indicator && indicator <= 287) return (128 * indicator - 28672) / 60000.0;
            if (288 <= indicator && indicator <= 319) return (256 * indicator - 65536) / 60000.0;
            if (320 <= indicator && indicator <= 351) return (512 * indicator - 147456) / 60000.0;
            if (352 <= indicator && indicator <= 383) return (1024 * indicator - 327680) / 60000.0;
            if (384 <= indicator && indicator <= 415) return (2048 * indicator - 720896) / 60000.0;
            if (416 <= indicator && indicator <= 447) return (4096 * indicator - 1572864) / 60000.0;
            if (448 <= indicator && indicator <= 479) return (8192 * indicator - 3407872) / 60000.0;
            if (480 <= indicator && indicator <= 511) return (16384 * indicator - 7340032) / 60000.0;
            if (512 <= indicator && indicator <= 543) return (32768 * indicator - 15728640) / 60000.0;
            if (544 <= indicator && indicator <= 575) return (65536 * indicator - 33554432) / 60000.0;
            if (576 <= indicator && indicator <= 607) return (131072 * indicator - 71303168) / 60000.0;
            if (608 <= indicator && indicator <= 639) return (262144 * indicator - 150994944) / 60000.0;
            if (640 <= indicator && indicator <= 671) return (524288 * indicator - 318767104) / 60000.0;
            if (672 <= indicator && indicator <= 703) return (1048576 * indicator - 671088640) / 60000.0;
            if (indicator == 704) return (2097152 * indicator - 1409286144) / 60000.0;
            if (705 <= indicator && indicator <= 1023) return 0;
            return 0.0;
        }

        public static double GetBits38(ReadOnlySpan<byte> buff, ref int pos)
        {
            return SpanBitHelper.GetBitS(buff,ref pos, 32) * 64.0 + SpanBitHelper.GetBitU(buff,ref pos, 6);
        }

        /// <summary>
        /// get sign-magnitude bits
        /// </summary>
        /// <returns></returns>
        public static double GetBitG(ReadOnlySpan<byte> buff, ref int pos, int len)
        {
            var sign = SpanBitHelper.GetBitU(buff,ref pos, 1) != 0 ? -1.0 : 1.0;
            return SpanBitHelper.GetBitU(buff, ref pos, len - 1) * sign;
        }

        public static string GetRinexCodeFromMsm(NavigationSystemEnum sys, int signalId)
        {
            return sys switch
            {
                NavigationSystemEnum.SYS_GPS => RtcmV3Helper.msm_sig_gps[signalId],
                NavigationSystemEnum.SYS_GLO => RtcmV3Helper.msm_sig_glo[signalId],
                NavigationSystemEnum.SYS_GAL => RtcmV3Helper.msm_sig_gal[signalId],
                NavigationSystemEnum.SYS_QZS => RtcmV3Helper.msm_sig_qzs[signalId],
                NavigationSystemEnum.SYS_SBS => RtcmV3Helper.msm_sig_sbs[signalId],
                NavigationSystemEnum.SYS_CMP => RtcmV3Helper.msm_sig_cmp[signalId],
                NavigationSystemEnum.SYS_IRN => RtcmV3Helper.msm_sig_irn[signalId],
                _ => ""
            };
        }
    }

    public enum NavigationSystemEnum
    {
        /// <summary>
        /// None
        /// </summary>
        SYS_NONE = 0x00,
        /// <summary>
        /// GPS
        /// </summary>
        SYS_GPS = 0x01,
        /// <summary>
        /// SBAS
        /// </summary>
        SYS_SBS = 0x02,
        /// <summary>
        /// GLONASS
        /// </summary>
        SYS_GLO = 0x04,
        /// <summary>
        /// Galileo
        /// </summary>
        SYS_GAL = 0x08,
        /// <summary>
        /// QZSS
        /// </summary>
        SYS_QZS = 0x10,
        /// <summary>
        /// BeiDou
        /// </summary>
        SYS_CMP = 0x20,
        /// <summary>
        /// IRNS
        /// </summary>
        SYS_IRN = 0x40,
        /// <summary>
        /// LEO
        /// </summary>
        SYS_LEO = 0x80,
        /// <summary>
        /// ALL
        /// </summary>
        SYS_ALL = 0xFF
    }

    public enum TimeSystemEnum
    {
        TSYS_GPS = 0,                   /* time system: GPS time */
        TSYS_UTC = 1,                   /* time system: UTC */
        TSYS_GLO = 2,                   /* time system: GLONASS time */
        TSYS_GAL = 3,                   /* time system: Galileo time */
        TSYS_QZS = 4,                   /* time system: QZSS time */
        TSYS_CMP = 5,                   /* time system: BeiDou time */
        TSYS_IRN = 6                   /* time system: IRNSS time */
    }

    public class SignalRaw
    {
        public string RinexCode { get; set; }
        public byte ObservationCode { get; set; }
        public int ObservationIndex { get; set; }
    }
}