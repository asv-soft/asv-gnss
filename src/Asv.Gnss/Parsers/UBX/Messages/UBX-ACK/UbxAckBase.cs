using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class UbxAckBase : UbxMessageBase
    {
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            AckClassId = BinSerialize.ReadByte(ref buffer);
            AckSubclassId = BinSerialize.ReadByte(ref buffer);
        }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, AckClassId);
            BinSerialize.WriteByte(ref buffer, AckSubclassId);
        }

        protected override int GetContentByteSize() => 2;
        /// <summary>
        /// Class ID of the Acknowledged Message
        /// </summary>
        public byte AckClassId { get; set; }
        /// <summary>
        /// Message ID of the Acknowledged Message
        /// </summary>
        public byte AckSubclassId { get; set; }

        public override void Randomize(Random random)
        {
            AckClassId = (byte)random.Next(0, byte.MaxValue);
            AckSubclassId = (byte)random.Next(0, byte.MaxValue);
        }
    }
}