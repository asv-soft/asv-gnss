using System;
using Asv.IO;

namespace Asv.Gnss
{

    [Flags]
    public enum AsvGbasSlot : byte
    {
        SlotA = 0b00000001,
        SlotB = 0b00000010,
        SlotC = 0b00000100,
        SlotD = 0b00001000,
        SlotE = 0b00010000,
        SlotF = 0b00100000,
        SlotG = 0b01000000,
        SlotH = 0b10000000,
    }

    [Flags]
    public enum AsvGbasMessage : ulong
    {
        Msg1 =   0b00000001,
        Msg101 = 0b00000010,
        Msg2 =   0b00000100,
        Msg3 =   0b00001000,
        Msg4 =   0b00010000,
        Msg5 =   0b00100000,
    }

    public class AsvMessageGbasVdbSend:AsvMessageBase
    {
        public override ushort MessageId => 0x0100;
        public override string Name => "GbasVdbSend";

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            Slot = (AsvGbasSlot)BinSerialize.ReadByte(ref buffer);
            Msgs = (AsvGbasMessage)BinSerialize.ReadULong(ref buffer);
            LastByteLength = BinSerialize.ReadByte(ref buffer);
            Data = new byte[buffer.Length];
            buffer.CopyTo(Data);
            buffer = buffer.Slice(Data.Length);
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer,(byte)Slot);
            BinSerialize.WriteULong(ref buffer, (ulong)Msgs);
            BinSerialize.WriteByte(ref buffer, LastByteLength);
            Data.CopyTo(buffer);
            buffer = buffer.Slice(Data.Length);
        }

        protected override int InternalGetContentByteSize()
        {
            return Data.Length + 10;
        }

        public override void Randomize(Random random)
        {
            Sequence = (ushort)random.Next(0, ushort.MaxValue);
            TargetId = (byte)random.Next(0,byte.MaxValue);
            SenderId = (byte)random.Next(0, byte.MaxValue);
            Data = new byte[random.Next(0, AsvMessageParser.DataSize - 10/*  */)];
            random.NextBytes(Data);
            LastByteLength = (byte)random.Next(0, 7);
            Msgs = (AsvGbasMessage)random.Next(0, Enum.GetValues(typeof(AsvGbasMessage)).Length - 1);
            Slot = (AsvGbasSlot)random.Next(0, Enum.GetValues(typeof(AsvGbasSlot)).Length - 1);
        }

        public byte[] Data { get; set; }
        public byte LastByteLength { get; set; }
        public AsvGbasMessage Msgs { get; set; }
        public AsvGbasSlot Slot { get; set; }
    }
}