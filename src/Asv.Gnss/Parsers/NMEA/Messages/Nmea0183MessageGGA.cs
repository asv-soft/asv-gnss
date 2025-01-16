using System;
using System.Globalization;
using System.Text;
using Asv.IO;

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
    /// 15) Checksum.
    /// </summary>
    public class Nmea0183MessageGGA : Nmea0183MessageBase
    {
        /// <summary>
        /// Represents the NMEA message ID.
        /// </summary>
        public const string NmeaMessageId = "GGA";

        private const byte Unit = 0x4D;

        // Gets the message ID associated with the NMEA message.
        // This property is read-only and returns the NMEA message ID.
        // @return The message ID as a string.
        // /
        public override string MessageId => NmeaMessageId;

        /// <summary>
        /// Deserializes an array of strings into the internal properties of the class.
        /// </summary>
        /// <param name="items">Array of strings representing the properties of the object.</param>
        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            Time = Nmea0183Helper.ParseTime(items[1]);
            Latitude = Nmea0183Helper.ParseLatitude(items[2]);
            if (string.Equals(items[3], "S", StringComparison.InvariantCultureIgnoreCase))
            {
                Latitude *= -1;
            }

            Longitude = Nmea0183Helper.ParseLongitude(items[4]);
            if (string.Equals(items[5], "W", StringComparison.InvariantCultureIgnoreCase))
            {
                Longitude *= -1;
            }

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

        protected override void InternalSerialize(ref Span<byte> buffer, Encoding encoding)
        {
            Nmea0183Helper.SerializeTime(Time).CopyTo(ref buffer, encoding);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            Nmea0183Helper.SerializeLatitude(Latitude).CopyTo(ref buffer, encoding);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            InsertByte(ref buffer, Latitude < 0 ? South : North);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            Nmea0183Helper.SerializeLongitude(Longitude).CopyTo(ref buffer, encoding);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            InsertByte(ref buffer, Longitude < 0 ? West : East);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            Nmea0183Helper.SerializeGpsQuality(GpsQuality).CopyTo(ref buffer, encoding);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            (NumberOfSatellites.HasValue ? NumberOfSatellites.Value.ToString("00") : "00").CopyTo(
                ref buffer,
                encoding
            );
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            var hdop = double.IsNaN(HorizontalDilutionPrecision)
                ? string.Empty
                : Math.Round(HorizontalDilutionPrecision, 1)
                    .ToString("F1", CultureInfo.InvariantCulture);
            hdop.CopyTo(ref buffer, encoding);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            var alt = double.IsNaN(AntennaAltitudeMsl)
                ? string.Empty
                : Math.Round(AntennaAltitudeMsl, 3).ToString("F3", CultureInfo.InvariantCulture);
            alt.CopyTo(ref buffer, encoding);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            InsertByte(ref buffer, Unit);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            var sep = double.IsNaN(GeoidalSeparation)
                ? string.Empty
                : Math.Round(GeoidalSeparation, 3).ToString("F3", CultureInfo.InvariantCulture);
            sep.CopyTo(ref buffer, encoding);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            InsertByte(ref buffer, Unit);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            var ageDGps = double.IsNaN(AgeOfDifferentialGPSData)
                ? string.Empty
                : Math.Round(AgeOfDifferentialGPSData, 1)
                    .ToString("00.0", CultureInfo.InvariantCulture);
            ageDGps.CopyTo(ref buffer, encoding);
            Nmea0183MessageBase.InsertSeparator(ref buffer);
            var refId = ReferenceStationID.HasValue
                ? ReferenceStationID.Value.ToString("0000")
                : string.Empty;
            refId.CopyTo(ref buffer, encoding);
        }

        /// <summary>
        /// Gets or sets the reference station ID.
        /// </summary>
        /// <value>
        /// The reference station ID.
        /// </value>
        public int? ReferenceStationID { get; set; }

        /// <summary>
        /// Gets or sets age of differential GPS data, time in seconds since last SC104
        /// type 1 or 9 update, null field when DGPS is not used.
        /// </summary>
        public double AgeOfDifferentialGPSData { get; set; }

        public string GeoidalSeparationUnits { get; set; }

        /// <summary>
        ///  Gets or sets geoidal separation, the difference between the WGS-84 earth
        ///  ellipsoid and mean-sea-level(geoid), "-" means mean-sea-level below ellipsoid
        /// return MSL - Ellipsoid.
        /// </summary>
        public double GeoidalSeparation { get; set; }

        /// <summary>
        ///  Gets or sets units of antenna altitude, meters.
        /// </summary>
        public string AntennaAltitudeUnits { get; set; }

        /// <summary>
        ///  Gets or sets antenna Altitude above/below mean-sea-level (geoid).
        /// </summary>
        public double AntennaAltitudeMsl { get; set; }

        /// <summary>
        ///  Gets or sets horizontal Dilution of precision.
        /// </summary>
        public double HorizontalDilutionPrecision { get; set; }

        /// <summary>
        /// Gets or sets number of satellites in view, 00 - 12.
        /// </summary>
        public int? NumberOfSatellites { get; set; }

        /// <summary>
        /// Gets or sets the GPS quality of the NMEA data.
        /// </summary>
        /// <value>
        /// The GPS quality. This property represents the quality of the GPS signal received, indicating the accuracy of the reported data.
        /// </value>
        public NmeaGpsQuality GpsQuality { get; set; }

        /// <summary>
        /// Gets or sets the EastWest property represents the east-west direction.
        /// </summary>
        /// <value>
        /// A string value that represents the east-west direction.
        /// </value>
        // public string EastWest { get; set; }
        // Gets or sets the longitude of a location.
        // /
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the value of the NorthSouth property.
        /// </summary>
        /// <value>
        /// The value of the NorthSouth property.
        /// </value>
        // public string NorthSouth { get; set; }
        // Gets or sets a value representing the latitude.
        // /
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets time (UTC).
        /// </summary>
        public DateTime? Time { get; set; }
    }
}
