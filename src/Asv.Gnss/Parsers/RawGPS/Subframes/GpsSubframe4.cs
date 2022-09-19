namespace Asv.Gnss
{
    public class GpsSubframe4 : GpsSubframeBase
    {
        public override byte SubframeId => 4;

        public override void Deserialize(byte[] dataWithoutParity)
        {
            base.Deserialize(dataWithoutParity);

        }
    }
}