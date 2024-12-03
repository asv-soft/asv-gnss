using System;

namespace Asv.Gnss
{
    public class SbfPacketGloTime : SbfMessageBase
    {
        public override ushort MessageRevision => 0;
        public override ushort MessageType => 4036;
        public override string Name => "GLOTime";
        
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}