namespace Asv.Gnss
{
    public class GPSSubFrame5 : GpsSubframeBase
    {
        public override byte SubframeId => 5;

        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

        }
    }
}