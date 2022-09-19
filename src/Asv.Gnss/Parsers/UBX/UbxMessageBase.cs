using System;
using Asv.IO;

namespace Asv.Gnss
{
    

    public abstract class UbxMessageBase : GnssMessageBase<ushort>
    {
        public override string ProtocolId => UbxBinaryParser.GnssProtocolId;
        public abstract byte Class { get; }
        public abstract byte SubClass { get; }
        public override ushort MessageId => (ushort)((Class << 8) | SubClass);

        public override void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, UbxHelper.SyncByte1);
            BinSerialize.WriteByte(ref buffer, UbxHelper.SyncByte2);

            var crcSpan = buffer;

            BinSerialize.WriteByte(ref buffer, Class);
            BinSerialize.WriteByte(ref buffer, SubClass);
            
            var size = (ushort)GetContentByteSize();
            BinSerialize.WriteUShort(ref buffer, size);

            var writeSpan = buffer.Slice(0, size);
            SerializeContent(ref writeSpan);
            
            buffer = buffer.Slice(size);
            crcSpan = crcSpan.Slice(0, size + 4 /*ID + Length*/);
            var crc = UbxCrc16.Calc(crcSpan);
            BinSerialize.WriteByte(ref buffer, crc.Crc1);
            BinSerialize.WriteByte(ref buffer, crc.Crc2);
        }

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {

            if (BinSerialize.ReadByte(ref buffer) != UbxHelper.SyncByte1 || BinSerialize.ReadByte(ref buffer) != UbxHelper.SyncByte2)
            {
                throw new Exception($"Deserialization UBX message failed: want {UbxHelper.SyncByte1:X} {UbxHelper.SyncByte2:X}. Read {buffer[0]:X} {buffer[1]:X}");
            }

            var msgId = (ushort)((BinSerialize.ReadByte(ref buffer) << 8) | BinSerialize.ReadByte(ref buffer));
            if (msgId != MessageId)
            {
                throw new Exception($"Deserialization UBX message failed: want message number '{UbxHelper.GetMessageName(MessageId)}'. Read = '{UbxHelper.GetMessageName(msgId)}'");
            }

            var payloadLength = BinSerialize.ReadUShort(ref buffer);
           
            var readSpan = buffer.Slice(0, payloadLength);
            DeserializeContent(ref readSpan);
            buffer = buffer.Slice(payloadLength);

            var crc = BinSerialize.ReadUShort(ref buffer);
        }

        protected abstract void SerializeContent(ref Span<byte> buffer);
        protected abstract void DeserializeContent(ref ReadOnlySpan<byte> buffer);
        protected abstract int GetContentByteSize();

        public override int GetByteSize()
        {
            return UbxHelper.HeaderOffset + 2/*CRC*/ + GetContentByteSize();
        }


        public abstract void Randomize(Random random);

    }
}