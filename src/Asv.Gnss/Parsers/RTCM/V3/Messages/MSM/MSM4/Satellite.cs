namespace Asv.Gnss
{
    public class Satellite
    {
        public byte SatellitePrn { get; set; }
        public Signal[] Signals { get; set; }
        public string SatelliteCode { get; set; }
    }
}
