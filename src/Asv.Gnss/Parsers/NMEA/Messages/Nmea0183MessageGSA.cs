using System.Linq;

namespace Asv.Gnss
{
    /// <summary>
    /// GSA GPS DOP and active satellites. 
    /// 1) Selection mode 
    /// 2) Mode 
    /// 3) ID of 1st satellite used for fix 
    /// 4) ID of 2nd satellite used for fix 
    /// ... 
    /// 14) ID of 12th satellite used for fix 
    /// 15) PDOP in meters 
    /// 16) HDOP in meters 
    /// 17) VDOP in meters 
    /// 18) Checksum
    /// </summary>
    public class Nmea0183MessageGSA : Nmea0183MessageBase
    {
        public override string MessageId => "GSA";

        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            SelectionMode = items[1];
            Mode = items[2];

            SatelliteId = items.Skip(3).Take(12).Where(_ => !string.IsNullOrEmpty(_)).Select(int.Parse).ToArray();

            PDop = Nmea0183Helper.ParseDouble(items[15]);
            HDop = Nmea0183Helper.ParseDouble(items[16]);
            VDop = Nmea0183Helper.ParseDouble(items[17]);
        }

        public string SelectionMode { get; set; }

        public string Mode { get; set; }

        public int[] SatelliteId { get; set; }

        public double PDop { get; set; }

        public double HDop { get; set; }

        public double VDop { get; set; }
    }
}