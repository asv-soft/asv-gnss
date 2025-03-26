namespace Asv.Gnss
{
    public enum PositionFixDimensions
    {
        Unknown = 0,
        UserAltitude = 2,
        GpsAltitude = 3,
    }

    /// <summary>
    /// Garmin proprietary sentence.
    /// </summary>
    public class Nmea0183ProprietaryMessageGRMZ : Nmea0183MessageBase
    {
        public const string NmeaMessageId = "GRMZ";
        public override string MessageId => NmeaMessageId;

        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            Altitude = Nmea0183Helper.ParseDouble(items[1]);
            Unit = items[2];
            PositionFixDimension = (PositionFixDimensions)(Nmea0183Helper.ParseInt(items[3]) ?? 0);
        }

        public double Altitude { get; set; }
        public string Unit { get; set; }
        public PositionFixDimensions PositionFixDimension { get; set; }

        public static bool MessageIdGetter(string raw, out string messageid)
        {
            messageid = null;
            if (raw.Length < 5)
            {
                return false;
            }

            if (raw[0] != 'P')
            {
                return false;
            }

            if (raw[1] != 'G')
            {
                return false;
            }

            if (raw[2] != 'R')
            {
                return false;
            }

            if (raw[3] != 'M')
            {
                return false;
            }

            if (raw[4] != 'Z')
            {
                return false;
            }

            messageid = NmeaMessageId;
            return true;
        }
    }
}
