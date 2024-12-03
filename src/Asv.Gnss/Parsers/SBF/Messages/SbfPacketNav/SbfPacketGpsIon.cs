using System;

namespace Asv.Gnss
{
    public class SbfPacketGpsIon : SbfMessageBase
    {
        public override ushort MessageRevision => 0;
        public override ushort MessageType => 5893;
        public override string Name => "GPSIon";
        
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}