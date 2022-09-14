namespace Asv.Gnss
{
    public class GpsSubframe3 : GpsSubframeBase
    {
        public override byte SubframeId => 3;

        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

        }
    }
}