namespace Asv.Gnss
{
    public class GpsSubframe2 : GpsSubframeBase
    {
        public override byte SubframeId => 2;

        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

        }
    }
}