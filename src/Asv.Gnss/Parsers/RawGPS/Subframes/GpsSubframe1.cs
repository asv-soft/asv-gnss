using Newtonsoft.Json;

namespace Asv.Gnss
{
    public class GpsSubframe1: GpsSubframeBase
    {
        
        public override byte SubframeId => 1;

        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);
            var word3Start = 24U*2;
            WeekNumber = GpsRawHelper.GetBitU(dataWithoutParity, word3Start, 10);
            SatteliteAccuracy = (byte) GpsRawHelper.GetBitU(dataWithoutParity, word3Start + 13, 4);
            SatteliteHealth = (byte) GpsRawHelper.GetBitU(dataWithoutParity, word3Start + 17, 6);

            
            
            // IODC = (GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 10));
            // startWord2 += 10;
            // TOC = (GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 16)) * 16;
            // startWord2 += 16;
            // Af2 = ((sbyte) GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 8) * GpsRawHelper.P2_55);
            // startWord2 += 8;
            // Af1 = ((sbyte) GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 16) * GpsRawHelper.P2_43);
            // startWord2 += 16;
            // Af0 = ((sbyte) GpsRawHelper.GetBitU(dataWithoutParity, startWord2, 22) * GpsRawHelper.P2_31);
            // startWord2 += 22;
        }

        // public double Af0 { get; set; }
        // public double Af1 { get; set; }
        // public double Af2 { get; set; }
        // public uint TOC { get; set; }
        // public uint IODC { get; set; }
        //
        public byte SatteliteAccuracy { get; set; }
        public byte SatteliteHealth { get; set; }
        public uint WeekNumber { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}