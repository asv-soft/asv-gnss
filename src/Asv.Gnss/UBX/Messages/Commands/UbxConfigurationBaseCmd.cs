using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class UbxConfigurationBaseCmd : UbxMessageBase
    {
        public override byte Class => 0x06;
        public override byte SubClass => 0x09;
        public override string Name => "UBX-CFG-CFG";

        protected enum ConfAction
        {
            Clear,
            Save,
            Load
        }

        protected override int GetContentByteSize() => 12;

        protected abstract ConfAction Action { get; }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer, int payloadByteSize)
        {
            throw new NotImplementedException();
        }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            if (Action == ConfAction.Clear || Action == ConfAction.Save)
            {
                BinSerialize.WriteByte(ref buffer,0xF8);
                BinSerialize.WriteByte(ref buffer, 0xF8);
            }
            else
            {
                BinSerialize.WriteByte(ref buffer, 0x00);
                BinSerialize.WriteByte(ref buffer, 0x00);
            }
            BinSerialize.WriteByte(ref buffer, 0x00);
            BinSerialize.WriteByte(ref buffer, 0x00);

            if (Action == ConfAction.Save)
            {
                BinSerialize.WriteByte(ref buffer, 0xF8);
                BinSerialize.WriteByte(ref buffer, 0xF8);
            }
            else
            {
                BinSerialize.WriteByte(ref buffer, 0x00);
                BinSerialize.WriteByte(ref buffer, 0x00);
            }
            BinSerialize.WriteByte(ref buffer, 0x00);
            BinSerialize.WriteByte(ref buffer, 0x00);

            if (Action == ConfAction.Load)
            {
                BinSerialize.WriteByte(ref buffer, 0xF8);
                BinSerialize.WriteByte(ref buffer, 0xF8);
            }
            else
            {
                BinSerialize.WriteByte(ref buffer, 0x00);
                BinSerialize.WriteByte(ref buffer, 0x00);
            }
            BinSerialize.WriteByte(ref buffer, 0x00);
            BinSerialize.WriteByte(ref buffer, 0x00);

            BinSerialize.WriteByte(ref buffer, 0x01); // Battery backed RAM
        }
    }
}