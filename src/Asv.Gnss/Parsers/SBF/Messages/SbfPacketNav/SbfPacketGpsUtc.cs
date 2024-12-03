using System;

namespace Asv.Gnss
{
    public class SbfPacketGpsUtc : SbfMessageBase
    {
        public override ushort MessageRevision => 0;
        public override ushort MessageType => 5894;
        public override string Name => "GPSUtc";
        
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}