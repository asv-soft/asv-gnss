using System;

namespace Asv.Gnss.Shell
{
    public class PvtInfo
    {
        public DateTime UtcTime { get; set; }
        public DateTime GpsTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AltitudeEllipse { get; set; }
        public double AltitudeMsl { get; set; }
        public double GroundSpeed2D { get; set; }
        public double HeadingOfMotion2D { get; set; }
    }
}
