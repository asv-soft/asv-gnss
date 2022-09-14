using System;

namespace Asv.Gnss
{
    public class UbxCfgAntPool : UbxCfgAnt
    {
        public override string Name => base.Name + "-POOL";

        protected override void SerializeContent(ref Span<byte> buffer)
        {

        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer, int payloadByteSize)
        {

        }

        protected override int GetContentByteSize() => 0;
    }
}