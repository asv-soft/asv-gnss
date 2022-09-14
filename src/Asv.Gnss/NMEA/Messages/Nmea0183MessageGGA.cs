using System;

namespace Asv.Gnss
{
    /// <summary>
    /// GGA Global Positioning System Fix Data. Time, Position and fix related data
    /// for a GPS receiver
    ///  11
    ///  1 2 3 4 5 6 7 8 9 10 | 12 13 14 15
    ///  | | | | | | | | | | | | | | |
    /// $--GGA,hhmmss.ss,llll.ll,a,yyyyy.yy,a,x,xx,x.x,x.x,M,x.x,M,x.x,xxxx*hh
    ///  1) Time (UTC)
    ///  2) Latitude
    ///  3) N or S (North or South)
    ///  4) Longitude
    ///  5) E or W (East or West)
    ///  6) GPS Quality Indicator,
    ///  0 - fix not available,
    ///  1 - GPS fix,
    ///  2 - Differential GPS fix
    ///  7) Number of satellites in view, 00 - 12
    ///  8) Horizontal Dilution of precision
    ///  9) Antenna Altitude above/below mean-sea-level (geoid)
    /// 10) Units of antenna altitude, meters
    /// 11) Geoidal separation, the difference between the WGS-84 earth
    ///  ellipsoid and mean-sea-level (geoid), "-" means mean-sea-level below ellipsoid
    /// 12) Units of geoidal separation, meters
    /// 13) Age of differential GPS data, time in seconds since last SC104
    ///  type 1 or 9 update, null field when DGPS is not used
    /// 14) Differential reference station ID, 0000-1023
    /// 15) Checksum
    /// </summary>
    public class Nmea0183MessageGGA : Nmea0183MessageBase
    {
        public const string NmeaMessageId = "GGA";

        public override string MessageId => NmeaMessageId;

        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            Time = Nmea0183Helper.ParseTime(items[1]);
            Latitude = Nmea0183Helper.ParseLatitude(items[2]);
            NorthSouth = Nmea0183Helper.ParseNorthSouth(items[3]);
            Longitude = Nmea0183Helper.ParseLongitude(items[4]);
            EastWest = Nmea0183Helper.ParseEastWest(items[5]);
            GpsQuality = Nmea0183Helper.ParseGpsQuality(items[6]);
            NumberOfSatellites = Nmea0183Helper.ParseInt(items[7]);
            HorizontalDilutionPrecision = Nmea0183Helper.ParseDouble(items[8]);
            AntennaAltitudeMsl = Nmea0183Helper.ParseDouble(items[9]);
            AntennaAltitudeUnits = items[10];
            GeoidalSeparation = Nmea0183Helper.ParseDouble(items[11]);
            GeoidalSeparationUnits = items[12];
            AgeOfDifferentialGPSData = Nmea0183Helper.ParseDouble(items[13]);
            ReferenceStationID = Nmea0183Helper.ParseInt(items[14]);
        }

        public int? ReferenceStationID { get; set; }
        /// <summary>
        /// Age of differential GPS data, time in seconds since last SC104
        /// type 1 or 9 update, null field when DGPS is not used
        /// </summary>
        public double AgeOfDifferentialGPSData { get; set; }

        public string GeoidalSeparationUnits { get; set; }
        /// <summary>
        ///  Geoidal separation, the difference between the WGS-84 earth
        ///  ellipsoid and mean-sea-level(geoid), "-" means mean-sea-level below ellipsoid
        /// </summary>
        public double GeoidalSeparation { get; set; }
        /// <summary>
        ///  Units of antenna altitude, meters
        /// </summary>
        public string AntennaAltitudeUnits { get; set; }
        /// <summary>
        ///  Antenna Altitude above/below mean-sea-level (geoid)
        /// </summary>
        public double AntennaAltitudeMsl { get; set; }

        /// <summary>
        ///  Horizontal Dilution of precision
        /// </summary>
        public double HorizontalDilutionPrecision { get; set; }

        /// <summary>
        /// Number of satellites in view, 00 - 12
        /// </summary>
        public int? NumberOfSatellites { get; set; }

        public NmeaGpsQuality GpsQuality { get; set; }

        public string EastWest { get; set; }

        public double Longitude { get; set; }

        public string NorthSouth { get; set; }

        public double Latitude { get; set; }
        /// <summary>
        /// Time (UTC)
        /// </summary>
        public DateTime Time { get; set; }
    }
}