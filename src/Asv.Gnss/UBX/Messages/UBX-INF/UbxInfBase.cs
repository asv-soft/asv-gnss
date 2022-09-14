using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class UbxInfBase : UbxMessageBase
    {
        protected override void SerializeContent(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override int GetContentByteSize()
        {
            throw new NotImplementedException();
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer, int payloadByteSize)
        {
            var span = buffer.Slice(0, payloadByteSize);
            Message = payloadByteSize == 0 ? string.Empty : span.GetString(Encoding.ASCII).Trim('\0');
            buffer.Slice(payloadByteSize);
        }

        public string Message { get; set; }
    }
}