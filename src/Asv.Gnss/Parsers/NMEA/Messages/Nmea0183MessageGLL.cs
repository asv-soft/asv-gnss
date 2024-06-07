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
        /// <summary>
        /// Gets the message ID.
        /// </summary>
        /// <value>
        /// The message ID.
        /// </value>
        public override string MessageId => "GLL";

        /// <summary>
        /// Deserializes an array of string items and assigns them to the appropriate properties of the current object.
        /// </summary>
        /// <param name="items">An array of string items containing the data to be deserialized.</param>
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
        public DateTime? Time { get; set; }

        /// <summary>
        /// Gets or sets the value of the EastWest property.
        /// </summary>
        /// <value>
        /// A string representing the EastWest property value.
        /// </value>
        public string EastWest { get; set; }

        /// <summary>
        /// Gets or sets the longitude of a location.
        /// </summary>
        /// <value>The longitude value.</value>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the string representing the North or South direction.
        /// </summary>
        /// <value>
        /// The string representing the North or South direction.
        /// </value>
        public string NorthSouth { get; set; }

        /// <summary>
        /// Gets or sets the latitude value for a location.
        /// </summary>
        /// <value>
        /// The latitude value for a location.
        /// </value>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the status of the NmeaData.
        /// </summary>
        /// <value>
        /// The status of the NmeaData.
        /// </value>
        public NmeaDataStatus Status { get; set; }
    }
}