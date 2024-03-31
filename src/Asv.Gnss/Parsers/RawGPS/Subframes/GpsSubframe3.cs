namespace Asv.Gnss
{
    public class GpsSubframe3 : GpsSubframeBase
    {
        public override byte SubframeId => 3;

        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);
            var word3Start = 24U * 2;
            Cic = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 16) * GpsRawHelper.P2_29; word3Start += 16;
            OMG0 = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 32) * GpsRawHelper.P2_31 * GpsRawHelper.SC2RAD; word3Start += 32;
            Cis = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 16) * GpsRawHelper.P2_29; word3Start += 16;
            i0 = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 32) * GpsRawHelper.P2_31 * GpsRawHelper.SC2RAD; word3Start += 32;
            Crc = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 16) * GpsRawHelper.P2_5; word3Start += 16;
            omg = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 32) * GpsRawHelper.P2_31 * GpsRawHelper.SC2RAD; word3Start += 32;
            OMGd = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 24) * GpsRawHelper.P2_43 * GpsRawHelper.SC2RAD; word3Start += 24;
            iode = (int)GpsRawHelper.GetBitU(dataWithoutParity, word3Start, 8); word3Start += 8;
            idot = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 14) * GpsRawHelper.P2_43 * GpsRawHelper.SC2RAD; word3Start += 14;
        }

        public int iode { get; set; }

        public double OMG0 { get; set; }
        public double i0 { get; set; }
        public double omg { get; set; }
        public double OMGd { get; set; }
        public double idot { get; set; }
        
        public double Crc { get; set; }
        public double Cic { get; set; }
        public double Cis { get; set; }
    }
}