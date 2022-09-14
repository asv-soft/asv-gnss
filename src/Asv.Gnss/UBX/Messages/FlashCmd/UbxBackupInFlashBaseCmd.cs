using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class UbxBackupInFlashBaseCmd : UbxMessageBase
    {
        public override byte Class => 0x09;
        public override byte SubClass => 0x14;
        public override string Name => "UBX-UPD-SOS";

        protected abstract byte Command { get; }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer, int payloadByteSize)
        {
            throw new NotImplementedException();
        }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer,Command);
            BinSerialize.WriteByte(ref buffer, 0);
            BinSerialize.WriteByte(ref buffer, 0);
            BinSerialize.WriteByte(ref buffer, 0);
        }

        protected override int GetContentByteSize() => 4;
    }
}