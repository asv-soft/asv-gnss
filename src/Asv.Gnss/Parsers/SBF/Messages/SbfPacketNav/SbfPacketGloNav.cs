using System;

namespace Asv.Gnss
{
    public class SbfPacketGloNav : SbfMessageBase
    {
        public override ushort MessageRevision => 0;
        public override ushort MessageType => 4004;
        public override string Name => "GLONav";
        
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}