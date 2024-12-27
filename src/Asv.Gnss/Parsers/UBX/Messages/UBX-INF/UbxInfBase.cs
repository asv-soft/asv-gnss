using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class UbxInfBase : UbxMessageBase
    {
        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteBlock(ref buffer, Encoding.ASCII.GetBytes(Message + "\0"));
        }

        protected override int GetContentByteSize() =>
            Encoding.ASCII.GetByteCount(Message)
            + 1 /* '\0' char at the end*/
        ;

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            Message =
                buffer.Length == 0 ? string.Empty : buffer.GetString(Encoding.ASCII).Trim('\0');
            buffer = buffer.Slice(buffer.Length);
        }

        public string Message { get; set; }

        public override void Randomize(Random random)
        {
            Message = $"Test message {random.Next()}";
        }
    }
}
