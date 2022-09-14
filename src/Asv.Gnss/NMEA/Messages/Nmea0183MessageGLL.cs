using System;

namespace Asv.Gnss
{
    /// <summary>
    /// 1) Latitude 
    /// 2) N or S (North or South) 
    /// 3) Longitude 
    /// 4) E or W (East or West) 
    /// 5) Time (UTC) 
    /// 6) Status A - Data Valid, V - Data Invalid 
    /// 7) Checksum 
    /// </summary>
    public class Nmea0183MessageGLL : Nmea0183MessageBase
    {
        public override string MessageId => "GLL";

        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            Latitude = Nmea0183Helper.ParseLatitude(items[1]);
            NorthSouth = Nmea0183Helper.ParseNorthSouth(items[2]);
            Longitude = Nmea0183Helper.ParseLongitude(items[3]);
            EastWest = Nmea0183Helper.ParseEastWest(items[4]);
            Time = Nmea0183Helper.ParseTime(items[5]);
            Status = Nmea0183Helper.ParseDataStatus(items[6]);
        }

        /// <summary>
        /// Time (UTC)
        /// </summary>
        public DateTime Time { get; set; }

        public string EastWest { get; set; }

        public double Longitude { get; set; }

        public string NorthSouth { get; set; }

        public double Latitude { get; set; }

        public NmeaDataStatus Status { get; set; }
    }
}