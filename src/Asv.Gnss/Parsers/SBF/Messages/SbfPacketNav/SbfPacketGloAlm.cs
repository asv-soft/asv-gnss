using System;

namespace Asv.Gnss
{
    public class SbfPacketGloAlm : SbfMessageBase
    {
        public override ushort MessageRevision => 0;
        public override ushort MessageType => 4005;
        public override string Name => "GLOAlm";
        
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}