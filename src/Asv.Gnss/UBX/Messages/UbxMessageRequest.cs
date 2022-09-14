using System;

namespace Asv.Gnss
{
    public class UbxMessageRequest : UbxMessageBase
    {
        public override byte Class { get; }
        public override byte SubClass { get; }
        public override string Name { get; }

        public UbxMessageRequest(byte @class, byte subClass, string name)
        {
            Class = @class;
            SubClass = subClass;
            Name = name;
        }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer, int payloadByteSize)
        {
            
        }

        protected override int GetContentByteSize()
        {
            return 0;
        }
    }
}