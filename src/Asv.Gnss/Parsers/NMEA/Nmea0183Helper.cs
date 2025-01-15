using System;
using System.Buffers;
using System.Globalization;
using System.Text;

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
        SYS_ALL = 0xFF,
    }

    public static class Nmea0183Helper
    {
        public static bool GetPrnFromNmeaSatId(
            int NMEASatId,
            out int PRN,
            out NmeaNavigationSystemEnum nav
        )
        {
            nav = NmeaNavigationSystemEnum.SYS_NONE;
            PRN = -1;
            if (NMEASatId <= 0)
                return false;

            switch (NMEASatId)
            {
                case <= 32:
                    PRN = NMEASatId;
                    nav = NmeaNavigationSystemEnum.SYS_GPS;
                    return true;
                case <= 54:
                    PRN = NMEASatId + 87;
                    nav = NmeaNavigationSystemEnum.SYS_SBS;
                    return true;
                case >= 65
                and <= 96:
                    PRN = NMEASatId - 64;
                    nav = NmeaNavigationSystemEnum.SYS_GLO;
                    return true;
                case >= 120
                and <= 158:
                    PRN = NMEASatId;
                    nav = NmeaNavigationSystemEnum.SYS_SBS;
                    return true;
                case >= 193
                and <= 199:
                    PRN = NMEASatId - 192;
                    nav = NmeaNavigationSystemEnum.SYS_QZS;
                    return true;
                case >= 201
                and <= 235:
                    PRN = NMEASatId - 200;
                    nav = NmeaNavigationSystemEnum.SYS_CMP;
                    return true;
                case >= 301
                and <= 336:
                    PRN = NMEASatId - 300;
                    nav = NmeaNavigationSystemEnum.SYS_GAL;
                    return true;
                case >= 401
                and <= 414:
                    PRN = NMEASatId - 400;
                    nav = NmeaNavigationSystemEnum.SYS_IRN;
                    return true;
            }

            // 1 - 32	GPS
            // 33 - 54	Various SBAS systems (EGNOS, WAAS, SDCM, GAGAN, MSAS)
            // 55 - 64	not used (might be assigned to further SBAS systems)
            // 65 - 88	GLONASS
            // 89 - 96	GLONASS (future extensions)
            // 97 - 119	not used
            // 120 - 151	not used (SBAS PRNs occupy this range)
            // 152 - 158	Various SBAS systems (EGNOS, WAAS, SDCM, GAGAN, MSAS)
            // 159 - 172	not used
            // 173 - 182	IMES
            // 193 - 199	QZSS
            // 201 - 235	BeiDou (u-blox, not NMEA)
            // 301 - 336	GALILEO
            // 401 - 414	IRNSS
            // 415 - 437	IRNSS (future extensions)
            return false;
        }

        public static bool GetPrnFromNmeaSatId(
            string talkerId,
            int NMEASatId,
            out int PRN,
            out NmeaNavigationSystemEnum nav
        )
        {
            if (NMEASatId is >= 120 and <= 158)
            {
                PRN = NMEASatId;
                nav = NmeaNavigationSystemEnum.SYS_SBS;
                return true;
            }

            switch (talkerId)
            {
                case "GN":
                    return GetPrnFromNmeaSatId(NMEASatId, out PRN, out nav);
                case "GP":
                    switch (NMEASatId)
                    {
                        case <= 32:
                            PRN = NMEASatId;
                            nav = NmeaNavigationSystemEnum.SYS_GPS;
                            return true;
                        case <= 64:
                            PRN = NMEASatId + 87;
                            nav = NmeaNavigationSystemEnum.SYS_SBS;
                            return true;
                    }
                    break;
                case "GL":
                    if (NMEASatId is >= 65 and <= 96)
                    {
                        PRN = NMEASatId - 64;
                        nav = NmeaNavigationSystemEnum.SYS_GLO;
                        return true;
                    }
                    break;
                case "GQ":
                    switch (NMEASatId)
                    {
                        case >= 1
                        and <= 7:
                            PRN = NMEASatId;
                            nav = NmeaNavigationSystemEnum.SYS_QZS;
                            return true;
                        case >= 193
                        and <= 199:
                            PRN = NMEASatId - 192;
                            nav = NmeaNavigationSystemEnum.SYS_QZS;
                            return true;
                    }
                    break;
                case "GB":
                case "BD":
                    switch (NMEASatId)
                    {
                        case >= 1
                        and <= 35:
                            PRN = NMEASatId;
                            nav = NmeaNavigationSystemEnum.SYS_CMP;
                            return true;
                        case >= 201
                        and <= 235:
                            PRN = NMEASatId - 200;
                            nav = NmeaNavigationSystemEnum.SYS_CMP;
                            return true;
                    }
                    break;
                case "GA":
                    switch (NMEASatId)
                    {
                        case >= 1
                        and <= 36:
                            PRN = NMEASatId;
                            nav = NmeaNavigationSystemEnum.SYS_GAL;
                            return true;
                        case >= 301
                        and <= 336:
                            PRN = NMEASatId - 300;
                            nav = NmeaNavigationSystemEnum.SYS_GAL;
                            return true;
                    }
                    break;
                case "GI":
                    switch (NMEASatId)
                    {
                        case >= 1
                        and <= 14:
                            PRN = NMEASatId;
                            nav = NmeaNavigationSystemEnum.SYS_IRN;
                            return true;
                        case >= 401
                        and <= 414:
                            PRN = NMEASatId - 400;
                            nav = NmeaNavigationSystemEnum.SYS_IRN;
                            return true;
                    }
                    break;
            }

            PRN = -1;
            nav = NmeaNavigationSystemEnum.SYS_NONE;
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

                case "BD":
                    return "BeiDou Navigation Satellite System";

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

                case "GA":
                    return "Galileo Navigation Satellite System";

                case "GB":
                    return "BeiDou Navigation Satellite System";

                case "GI":
                    return "Indian Regional Navigation Satellite System (IRNSS)";

                case "GL":
                    return "GLONASS Navigation Satellite System";

                case "GN":
                    return "Global Navigation";

                case "GP":
                    return "Global Positioning System (GPS)";

                case "GQ":
                    return "Quasi-Zenith Satellite System (QZSS)";

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
            if (
                !double.TryParse(
                    token,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var temp
                )
            )
                return null;

            var sss = (int)((temp - (int)temp) * 1000.0);
            var hh = (int)((int)temp / 10000.0);
            var mm = (int)(((int)temp - hh * 10000.0) / 100.0);
            var ss = (int)((int)temp - hh * 10000.0 - mm * 100.0);

            return new DateTime(now.Year, now.Month, now.Day, hh, mm, ss, sss, DateTimeKind.Utc);

            // var hh = int.Parse(value.Substring(0, 2), CultureInfo.InvariantCulture);
            // var mm = int.Parse(value.Substring(2, 2), CultureInfo.InvariantCulture);
            // var ss = double.Parse(value.Substring(2, 4), CultureInfo.InvariantCulture);
            // return new DateTime(0,0,0,hh,mm,00).AddSeconds(ss);
        }

        public static string SerializeTime(DateTime? time)
        {
            if (!time.HasValue)
                return string.Empty;
            var hh = time.Value.Hour;
            var mm = time.Value.Minute;
            var ss = time.Value.Second;
            var sss = (int)Math.Round(time.Value.Millisecond / 10.0, 0);
            ss += sss / 100;
            mm += ss / 60;
            hh += mm / 60;
            hh %= 24;
            mm %= 60;
            ss %= 60;
            sss %= 100;
            return sss > 0
                ? $"{hh * 10000 + mm * 100 + ss:000000}.{sss.ToString().TrimEnd('0')}"
                : $"{hh * 10000 + mm * 100 + ss:000000}";
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

        public static string SerializeDate(DateTime? date)
        {
            return !date.HasValue
                ? string.Empty
                : $"{date.Value.Day * 10000 + date.Value.Month * 100 + date.Value.Year % 100:000000}";
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
                throw new ArgumentException(
                    string.Format("Date format incorrect in \"{0}\" (must be dd/mm/yy)", token)
                );
            }

            var date = int.Parse(splits[0]);
            var month = int.Parse(splits[1]);
            var year = int.Parse(splits[2]) + 2000;

            return new DateTime(year, month, date);
        }

        /// <summary>
        /// from ddmm.mm to ddmm.mmmmmmm
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static double ParseLatitude(string token)
        {
            var temp = double.Parse(token, NumberStyles.Any, CultureInfo.InvariantCulture);
            var degree = (int)temp / 100;
            var minutes = temp - degree * 100;
            return degree + minutes / 60.0;

            // var deg = int.Parse(token.Substring(0, 2), CultureInfo.InvariantCulture);
            // var min = double.Parse(token.Substring(2, 5), CultureInfo.InvariantCulture);
            // return deg + min / 60.0;
        }

        public static string SerializeLatitude(double latitude)
        {
            latitude = Math.Abs(latitude);
            var degree = (int)latitude;
            var minute = (latitude - degree) * 60.0;
            var integerMin = (int)minute;
            var fractionalMin = (int)Math.Round((minute - integerMin) * 10000000.0, 0);
            integerMin += fractionalMin / 10000000;
            degree += integerMin / 60;
            integerMin %= 60;
            fractionalMin %= 10000000;

            var strFormat = "00";
            for (var i = 0; i < 6; i++)
            {
                if (fractionalMin % 10 != 0)
                {
                    strFormat = GetLatLonFractionalStringFormat(7 - i);
                    break;
                }
                fractionalMin /= 10;
            }
            return $"{degree * 100 + integerMin:0000}.{fractionalMin.ToString(strFormat)}";
        }

        private static string GetLatLonFractionalStringFormat(int length)
        {
            return length switch
            {
                2 => "00",
                3 => "000",
                4 => "0000",
                5 => "00000",
                6 => "000000",
                7 => "0000000",
                _ => "000",
            };
        }

        /// <summary>
        /// yyyyy.yy
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static double ParseLongitude(string token)
        {
            var temp = double.Parse(token, NumberStyles.Any, CultureInfo.InvariantCulture);

            double degree = (int)((int)temp / 100.0);
            var minutes = ((int)temp - degree * 100.0);
            var seconds = (temp - (int)temp) * 60.0;

            return degree + minutes / 60.0 + seconds / 3600.0;
            // var deg = int.Parse(token.Substring(0, 3), CultureInfo.InvariantCulture);
            // var min = double.Parse(token.Substring(2, 5), CultureInfo.InvariantCulture);
            // return deg + min / 60.0;
        }

        public static string SerializeLongitude(double longitude)
        {
            longitude = Math.Abs(longitude);
            var degree = (int)longitude;
            var minute = (longitude - degree) * 60.0;
            var integerMin = (int)minute;
            var fractionalMin = (int)Math.Round((minute - integerMin) * 10000000.0, 0);
            integerMin += fractionalMin / 10000000;
            degree += integerMin / 60;
            integerMin %= 60;
            fractionalMin %= 10000000;

            var strFormat = "00";
            for (var i = 0; i < 6; i++)
            {
                if (fractionalMin % 10 != 0)
                {
                    strFormat = GetLatLonFractionalStringFormat(7 - i);
                    break;
                }
                fractionalMin /= 10;
            }
            return $"{degree * 100 + integerMin:00000}.{fractionalMin.ToString(strFormat)}";
        }

        /// <summary>
        /// dddmm.mmm
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static double ParseCommonDegrees(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Double.NaN;

            var temp = double.Parse(token, NumberStyles.Any, CultureInfo.InvariantCulture);

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
            if (string.IsNullOrWhiteSpace(value))
                return NmeaGpsQuality.Unknown;
            return (NmeaGpsQuality)int.Parse(value, CultureInfo.InvariantCulture);
        }

        public static string SerializeGpsQuality(NmeaGpsQuality quality)
        {
            return quality == NmeaGpsQuality.Unknown ? string.Empty : ((int)quality).ToString();
        }

        public static NmeaDataStatus ParseDataStatus(string value)
        {
            if (string.Equals(value, "A", StringComparison.InvariantCultureIgnoreCase))
                return NmeaDataStatus.Valid;
            if (string.Equals(value, "V", StringComparison.InvariantCultureIgnoreCase))
                return NmeaDataStatus.Invalid;
            return NmeaDataStatus.Unknown;
        }

        public static string SerializeDataStatus(NmeaDataStatus status)
        {
            return status switch
            {
                NmeaDataStatus.Valid => "A",
                NmeaDataStatus.Invalid => "V",
                _ => string.Empty,
            };
        }

        /// <summary>
        /// x | xx | xxx | xxxx | xxxxx | xxxxxx
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// x.x
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return double.NaN;
            return double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public static string GetNmeaMessage(this Nmea0183MessageBase msg)
        {
            var array = ArrayPool<byte>.Shared.Rent(1024);
            try
            {
                var buffer = new Span<byte>(array, 0, 1024);
                var origin = buffer;
                msg.Serialize(ref buffer);
                var length = origin.Length - buffer.Length;
                return Encoding.ASCII.GetString(origin[..length]);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static NmeaNavigationalStatusEnum? GetNavigationalStatus(string status)
        {
            return status switch
            {
                "S" => NmeaNavigationalStatusEnum.Safe,
                "C" => NmeaNavigationalStatusEnum.Caution,
                "U" => NmeaNavigationalStatusEnum.Unsafe,
                "V" => NmeaNavigationalStatusEnum.NotValid,
                _ => null,
            };
        }

        public static string SetNavigationalStatus(this NmeaNavigationalStatusEnum status)
        {
            return status switch
            {
                NmeaNavigationalStatusEnum.Safe => "S",
                NmeaNavigationalStatusEnum.Caution => "C",
                NmeaNavigationalStatusEnum.Unsafe => "U",
                NmeaNavigationalStatusEnum.NotValid => "V",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
            };
        }

        public static double KnotsToKph(double knots)
        {
            if (double.IsNaN(knots))
                return double.NaN;
            return knots * 1.852;
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
        Invalid,
    }

    public enum NmeaNavigationalStatusEnum
    {
        Safe,
        Caution,
        Unsafe,
        NotValid,
    }

    public enum NmeaPositionModeEnum
    {
        NoFix, //N,
        Estimated, //E
        Autonomous, //A
        Differential, //D
        RtkFloat, //F
        RtkFixed, //R
    }
}
