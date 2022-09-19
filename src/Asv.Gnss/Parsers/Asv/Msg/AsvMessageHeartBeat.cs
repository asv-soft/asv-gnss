using System;
using Asv.IO;

namespace Asv.Gnss
{
    public enum AsvDeviceType : ushort
    {
        Unknown = 0,
        GbasServer = 1,
        GbasModulator = 2,
        GbasMonDev = 3,
    }

    public enum AsvDeviceState : byte
    {
        Unknown = 0,
        Active = 1,
        Error = 2,
    }


    public class AsvMessageHeartBeat : AsvMessageBase
    {
        public static ushort PacketMessageId = 0x0000;

        public override ushort MessageId => PacketMessageId;
        public override string Name => "HeartBeat";

        public AsvDeviceType DeviceType { get; set; }
        public AsvDeviceState DeviceState { get; set; }
        public byte Reserved1 { get; set; }
        public byte Reserved2 { get; set; }
        public byte Reserved3 { get; set; }
        public byte Reserved4 { get; set; }

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            DeviceType = (AsvDeviceType)BinSerialize.ReadUShort(ref buffer);
            DeviceState = (AsvDeviceState)BinSerialize.ReadByte(ref buffer);
            Reserved1 = BinSerialize.ReadByte(ref buffer);
            Reserved2 = BinSerialize.ReadByte(ref buffer);
            Reserved3 = BinSerialize.ReadByte(ref buffer);
            Reserved4 = BinSerialize.ReadByte(ref buffer);
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteUShort(ref buffer, (ushort)DeviceType);
            BinSerialize.WriteByte(ref buffer,(byte)DeviceState);
            BinSerialize.WriteByte(ref buffer, Reserved1);
            BinSerialize.WriteByte(ref buffer, Reserved2);
            BinSerialize.WriteByte(ref buffer, Reserved3);
            BinSerialize.WriteByte(ref buffer, Reserved4);
        }

        protected override int InternalGetContentByteSize() => 7;

        public override void Randomize(Random random)
        {
            Tag = null;
            Sequence = (ushort)random.Next(0, ushort.MaxValue);
            TargetId = (byte)random.Next(0, byte.MaxValue);
            SenderId = (byte)random.Next(0, byte.MaxValue);
            DeviceType = (AsvDeviceType)random.Next(0,Enum.GetValues(typeof(AsvDeviceType)).Length -1);
            DeviceState = (AsvDeviceState)random.Next(0, Enum.GetValues(typeof(AsvDeviceState)).Length - 1);
            Reserved1 = (byte)random.Next(0, byte.MaxValue);
            Reserved2 = (byte)random.Next(0, byte.MaxValue);
            Reserved3 = (byte)random.Next(0, byte.MaxValue);
            Reserved4 = (byte)random.Next(0, byte.MaxValue);
        }
    }
}