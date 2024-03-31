using System;

namespace Asv.Gnss
{
    public class GpsSubframe2 : GpsSubframeBase
    {
        public override byte SubframeId => 2;

        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);
            var word2Start = 24U * 2;
            iode = (int)GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 8); word2Start += 8;
            Crs = GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 16) * GpsRawHelper.P2_5; word2Start += 16;
            deln = GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 16) * GpsRawHelper.P2_43 * GpsRawHelper.SC2RAD; word2Start += 16;
            M0 = GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 32) * GpsRawHelper.P2_31 * GpsRawHelper.SC2RAD; word2Start += 32;
            Cuc = GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 16) * GpsRawHelper.P2_29; word2Start += 16;
            e = GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 32) * GpsRawHelper.P2_33; word2Start += 32;
            Cus = GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 16) * GpsRawHelper.P2_29; word2Start += 16;
            A = Math.Pow(GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 32) * GpsRawHelper.P2_19, 2); word2Start += 32;
            ToeSec = GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 16) * 16.0; word2Start += 16;
            Fit = GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 1) == 0 ? 0.0 : 4.0;
        }

        public double Fit { get; set; }

        public double M0 { get; set; }
        public double deln { get; set; }
        public double e { get; set; }
        public double A { get; set; }
        
        public double Cuc { get; set; }
        public double Cus { get; set; }
        public double Crs { get; set; }
        
        public double ToeSec { get; set; }
        public int iode { get; set; }
    }
}