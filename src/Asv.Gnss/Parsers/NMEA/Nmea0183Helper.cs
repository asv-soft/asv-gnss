using System;
using System.Globalization;

namespace Asv.Gnss
{
    public enum NmeaNavigationSystemEnum
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

    public static class Nmea0183Helper
    {
        public static bool GetPrnFromNmeaSatId(int NMEASatId, out int PRN, out NmeaNavigationSystemEnum nav)
        {
            nav = NmeaNavigationSystemEnum.SYS_NONE;
            PRN = -1;
            if (NMEASatId <= 0) return false;
            if (NMEASatId <= 32)
            {
                nav = NmeaNavigationSystemEnum.SYS_GPS;
                PRN = NMEASatId;
                return true;
            }

            if (NMEASatId <= 54)
            {
                nav = NmeaNavigationSystemEnum.SYS_SBS;
                PRN = NMEASatId + 87;
                return true;
            }

            if (NMEASatId <= 64)
            {
                return false;
            }

            if (NMEASatId <= 96)
            {
                nav = NmeaNavigationSystemEnum.SYS_GLO;
                PRN = NMEASatId - 64;
                return true;
            }

            // TODO: 
            // 1 - 32	GPS
            // 33 - 54	Various SBAS systems (EGNOS, WAAS, SDCM, GAGAN, MSAS)
            // 55 - 64	not used (might be assigned to further SBAS systems)
            // 65 - 88	GLONASS
            // 89 - 96	GLONASS (future extensions?)
            // 97 - 119	not used
            // 120 - 151	not used (SBAS PRNs occupy this range)
            // 152 - 158	Various SBAS systems (EGNOS, WAAS, SDCM, GAGAN, MSAS)
            // 159 - 172	not used
            // 173 - 182	IMES
            // 193 - 197	QZSS
            // 196 - 200	QZSS (future extensions?)
            // 201 - 235	BeiDou (u-blox, not NMEA)
            // 301 - 336	GALILEO
            // 401 - 437	BeiDou (NMEA)
            return false;
        }


        public static string TryFindSourceTitleById(string value)
        {
            switch (value)
            {
                case "AG":
                    return "Autopilot - General";
                case "AP":
                    return "Autopilot - Magnetic";

                case "CD":
                    return "Communications – Digital Selective Calling (DSC)";

                case "CR":
                    return "Communications – Receiver / Beacon Receiver";

                case "CS":
                    return "Communications – Satellite";

                case "CT":
                    return "Communications – Radio-Telephone (MF/HF)";

                case "CV":
                    return "Communications – Radio-Telephone (VHF)";

                case "CX":
                    return "Communications – Scanning Receiver";

                case "DF":
                    return "Direction Finder";

                case "EC":
                    return "Electronic Chart Display & Information System (ECDIS)";

                case "EP":
                    return "Emergency Position Indicating Beacon (EPIRB)";

                case "ER":
                    return "Engine Room Monitoring Systems";

                case "GP":
                    return "Global Positioning System (GPS)";

                case "HC":
                    return "Heading – Magnetic Compass";

                case "HE":
                    return "Heading – North Seeking Gyro";

                case "HN":
                    return "Heading – Non North Seeking Gyro";

                case "II":
                    return "Integrated Instrumentation";

                case "IN":
                    return "Integrated Navigation";

                case "LC":
                    return "Loran C";

                case "P":
                    return "Proprietary Code";

                case "RA":
                    return "RADAR and/or ARPA";

                case "SD":
                    return "Sounder, Depth";

                case "SN":
                    return "Electronic Positioning System, other/general";

                case "SS":
                    return "Sounder, Scanning";

                case "TI":
                    return "Turn Rate Indicator";

                case "VD":
                    return "Velocity Sensor, Doppler, other/general";

                case "DM":
                    return "Velocity Sensor, Speed Log, Water, Magnetic";

                case "VW":
                    return "Velocity Sensor, Speed Log, Water, Mechanical";

                case "WI":
                    return "Weather Instruments";

                case "YX":
                    return "Transducer";

                case "ZA":
                    return "Timekeeper – Atomic Clock";

                case "ZC":
                    return "Timekeeper – Chronometer";

                case "ZQ":
                    return "Timekeeper – Quartz";

                case "ZV":
                    return "Timekeeper – Radio Update, WWV or WWVH";

                default:
                    return "Unknown";

            }
        }

        /// <summary>
        /// hhmmss.ss | hhmmss | 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? ParseTime(string token)
        {
            var now = DateTime.UtcNow;
            if (!double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp)) return null;
            
            var sss = (int)((temp - (int)temp) * 1000.0);
            var hh = (int)((int)temp / 10000.0);
            var mm = (int)(((int)temp - hh * 10000.0) / 100.0);
            var ss = (int)((int)temp - hh * 10000.0 - mm * 100.0);

            return new DateTime(now.Year, now.Month, now.Day, hh, mm, ss, sss);

            // var hh = int.Parse(value.Substring(0, 2), CultureInfo.InvariantCulture);
            // var mm = int.Parse(value.Substring(2, 2), CultureInfo.InvariantCulture);
            // var ss = double.Parse(value.Substring(2, 4), CultureInfo.InvariantCulture);
            // return new DateTime(0,0,0,hh,mm,00).AddSeconds(ss);

        }

        /// <summary>
        /// ddmmyy
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? ParseDate(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Length != 6)
            {
                // throw new ArgumentException(string.Format("Date format incorrect in \"{0}\" (must be ddMMyy)", token));
                return null;
            }
            
            var date = Convert.ToInt32(token.Substring(0, 2));
            var month = Convert.ToInt32(token.Substring(2, 2));
            var year = Convert.ToInt32(token.Substring(4, 2)) + 2000;

            return new DateTime(year, month, date, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// dd/mm/yy
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ParseTimeWithSlashes(string token)
        {
            var splits = token.Split("/".ToCharArray());

            if (splits.Length != 3)
            {
                throw new ArgumentException(string.Format("Date format incorrect in \"{0}\" (must be dd/mm/yy)",  token));
            }
            
            var date = int.Parse(splits[0]);
            var month = int.Parse(splits[1]);
            var year = int.Parse(splits[2]) + 2000;

            return new DateTime(year, month, date);

        }
        
        /// <summary>
        /// llll.ll
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static double ParseLatitude(string token)
        {
            var temp = double.Parse(token, NumberStyles.Any, CultureInfo.InvariantCulture);
            double degree = (int)((int)temp / 100.0);
            var minutes = ((int)temp - degree * 100.0);
            var seconds = (temp - (int)temp) * 60.0;
            return degree + minutes / 60.0 + seconds / 3600.0;

            // var deg = int.Parse(token.Substring(0, 2), CultureInfo.InvariantCulture);
            // var min = double.Parse(token.Substring(2, 5), CultureInfo.InvariantCulture);
            // return deg + min / 60.0;
        }
        /// <summary>
        /// yyyyy.yy
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static double ParseLongitude(string token)
        {
            var temp = double.Parse(token, CultureInfo.InvariantCulture);

            double degree = (int)((int)temp / 100.0);
            var minutes = ((int)temp - degree * 100.0);
            var seconds = (temp - (int)temp) * 60.0;

            return degree + minutes / 60.0 + seconds / 3600.0;
            // var deg = int.Parse(token.Substring(0, 3), CultureInfo.InvariantCulture);
            // var min = double.Parse(token.Substring(2, 5), CultureInfo.InvariantCulture);
            // return deg + min / 60.0;
        }

        /// <summary>
        /// dddmm.mmm
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static double ParseCommonDegrees(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return Double.NaN;

            var temp = double.Parse(token, CultureInfo.InvariantCulture);

            double degree = (int)((int)temp / 100.0);
            var minutes = ((int)temp - degree * 100.0);
            var seconds = (temp - (int)temp) * 60.0;

            return degree + minutes / 60.0 + seconds / 3600.0;
        }

        public static string ParseNorthSouth(string value)
        {
            return value;
        }

        public static string ParseEastWest(string value)
        {
            return value;
        }

        public static NmeaGpsQuality ParseGpsQuality(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return NmeaGpsQuality.Unknown;
            return (NmeaGpsQuality) int.Parse(value, CultureInfo.InvariantCulture);
        }

        public static NmeaDataStatus ParseDataStatus(string value)
        {
            if (string.Equals(value, "A", StringComparison.InvariantCultureIgnoreCase)) return NmeaDataStatus.Valid;
            if (string.Equals(value, "V", StringComparison.InvariantCultureIgnoreCase)) return NmeaDataStatus.Invalid;
            return NmeaDataStatus.Unknown;
        }

        /// <summary>
        /// x | xx | xxx | xxxx | xxxxx | xxxxxx
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return int.Parse(value, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// x.x
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return double.NaN;
            return double.Parse(value, CultureInfo.InvariantCulture);
        }


    }

    public enum NmeaGpsQuality
    {
        Unknown = -1,
        FixNotAvailable = 0,
        GPSFix = 1,
        DifferentialGPSFix = 2,
        /// <summary>
        /// Real-Time Kinematic, fixed integers
        /// </summary>
        RTKFixed = 4,
        /// <summary>
        /// Real-Time Kinematic, float integers, OmniSTAR XP/HP or Location RTK
        /// </summary>
        RTKFloat = 5,

    }

    public enum NmeaDataStatus
    {
        Unknown,
        Valid,
        Invalid
    }
}