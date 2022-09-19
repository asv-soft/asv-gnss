namespace Asv.Gnss
{
    /// <summary>
    /// GSV Satellites in view 
    ///  
    /// 1) total number of messages 
    /// 2) message number 
    /// 3) satellites in view 
    /// 4) satellite number 
    /// 5) elevation in degrees 
    /// 6) azimuth in degrees to true 
    /// 7) SNR in dB 
    /// more satellite infos like 4)-7) 
    /// n) Checksum
    /// </summary>
    public class Nmea0183MessageGSV : Nmea0183MessageBase
    {
        public const string GnssMessageId = "GSV";

        public override string MessageId => GnssMessageId;

        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            if (!string.IsNullOrEmpty(items[1])) TotalNumberOfMsg = int.Parse(items[1]);
            if (!string.IsNullOrEmpty(items[2])) MessageNumber = int.Parse(items[2]);
            if (!string.IsNullOrEmpty(items[3])) SatellitesInView = int.Parse(items[3]);

            Satellites = new Satellite[(items.Length - 4)/4];
            var index = 0;
            for (var i = 4; i < 4 + Satellites.Length * 4; i+=4)
            {
                var number = 0;
                var elevationDeg = 0;
                var azimuthDeg = 0;
                var snrdB = 0;

                if (!string.IsNullOrEmpty(items[i])) number = int.Parse(items[i]);
                if (!string.IsNullOrEmpty(items[i+1])) elevationDeg = int.Parse(items[i+1]);
                if (!string.IsNullOrEmpty(items[i+2])) azimuthDeg = int.Parse(items[i+2]);
                if (!string.IsNullOrEmpty(items[i+3])) snrdB = int.Parse(items[i+3]);
                if (Nmea0183Helper.GetPrnFromNmeaSatId(number, out var PRN, out var nav))
                {
                    Satellites[index] = new Satellite
                    {
                        Number = number,
                        ElevationDeg = elevationDeg,
                        AzimuthDeg = azimuthDeg,
                        SnrdB = snrdB,
                        ExtPRN = PRN,
                        ExtNavSys = nav,
                    };
                }
                else
                {
                    Satellites[index] = new Satellite
                    {
                        Number = number,
                        ElevationDeg = elevationDeg,
                        AzimuthDeg = azimuthDeg,
                        SnrdB = snrdB
                    };
                }
                
                index++;
            }
        }

        public int TotalNumberOfMsg { get; set; }

        public int MessageNumber { get; set; }

        public int SatellitesInView { get; set; }

        public class Satellite
        {
            public int Number { get; set; }
            public int ElevationDeg { get; set; }
            public int AzimuthDeg { get; set; }
            public int SnrdB { get; set; }
            // extended computed values
            public int? ExtPRN { get; set; }
            public NmeaNavigationSystemEnum? ExtNavSys { get; set; }
        }

        public Satellite[] Satellites { get; set; }
    }
}