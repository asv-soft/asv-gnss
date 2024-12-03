using System;

namespace Asv.Gnss
{
    public class SbfPacketGpsAlm : SbfMessageBase
    {
        public override ushort MessageRevision => 0;
        public override ushort MessageType => 5892;
        public override string Name => "GPSAlm";
        
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}