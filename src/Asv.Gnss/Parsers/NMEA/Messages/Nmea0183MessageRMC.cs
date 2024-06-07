using System;

namespace Asv.Gnss
{
    /// <summary>
    /// 1) Time (UTC)
    /// 2) Status, V = Navigation receiver warning
    /// 3) Latitude
    /// 4) N or S
    /// 5) Longitude
    /// 6) E or W
    /// 7) Speed over ground, knots
    /// 8) Track made good, degrees true
    /// 9) Date, ddmmyy
    /// 10) Magnetic Variation, degrees
    /// 11) E or W
    /// </summary>
    public class Nmea0183MessageRMC : Nmea0183MessageBase
    {
        /// <summary>
        /// Represents the GNSS message ID.
        /// </summary>
        public const string GnssMessageId = "RMC";

        /// Gets the message ID associated with this message.
        /// @return The message ID as a string.
        /// /
        public override string MessageId => GnssMessageId;
        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            TimeUtc = Nmea0183Helper.ParseTime(items[1]);
            Status = items[2].Length > 0 ? items[2][0].GetPositionFixStatus() : PositionFixStatus.Unknown;
            Latitude = Nmea0183Helper.ParseLatitude(items[3]);
            if (string.Equals(items[4], "S", StringComparison.InvariantCultureIgnoreCase)) Latitude *= -1;
            
            Longitude = Nmea0183Helper.ParseLongitude(items[5]);
            if (string.Equals(items[6], "W", StringComparison.InvariantCultureIgnoreCase)) Longitude *= -1;
            SpeedOverGroundKnots = Nmea0183Helper.ParseDouble(items[7]);
            TrackMadeGoodDegreesTrue = Nmea0183Helper.ParseDouble(items[8]);
            Date = Nmea0183Helper.ParseDate(items[9]);
            if (double.TryParse(items[10], out var magneticVariation))
            {
                MagneticVariationDegrees = magneticVariation;
                if (string.Equals(items[11], "W", StringComparison.InvariantCultureIgnoreCase))
                    MagneticVariationDegrees *= -1;
            }
            else
            {
                MagneticVariationDegrees = double.NaN;
            }
        }
        
        
        
        /// <summary>
        /// UTC of position fix
        /// </summary>
        public DateTime? TimeUtc { get; set; }
        
        /// <summary>
        /// Position fix status
        /// </summary>
        public PositionFixStatus Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double SpeedOverGroundKnots { get; set; }
        public double TrackMadeGoodDegreesTrue { get; set; }
        public DateTime? Date { get; set; }
        public double MagneticVariationDegrees { get; set; }
    }

    public enum PositionFixStatus
    {
        Unknown,
        Valid, 
        Warning
    }

    public static class PositionFixStatusHelper
    {
        public static PositionFixStatus GetPositionFixStatus(this char src)
        {
            switch (src)
            {
                case 'a':
                case 'A':
                    return PositionFixStatus.Valid;
                case 'v':
                case 'V':
                    return PositionFixStatus.Warning;
                default:
                    return PositionFixStatus.Unknown;
            }
        }
    }
}