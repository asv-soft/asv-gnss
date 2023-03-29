using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class AsvMessageGbasCuSendV2 : AsvMessageBase
    {
        public override ushort MessageId => 0x0103;
        public override string Name => "GbasCuSendV2";

        /// <summary>
        /// Номер слота текущего сообщения (A - H)
        /// </summary>
        public AsvGbasSlotMsg Slot { get; set; }

        /// <summary>
        /// Признака окончания передачи всего фрейма.
        /// По нему устройство понимает, что весь фрейм передан и можно начинать передавать новый фрейм в эфир.
        /// </summary>
        public bool IsLastSlotInFrame { get; set; }

        /// <summary>
        /// Время жизни сообщения в 500 мс ( 1 frame) отрезках.  
        /// </summary>
        public byte LifeTime { get; set; }

        /// <summary>
        /// Длина в битах скремблированого сообщения без синхронизирующей последовательности (48 бит) и стартовых нулей
        /// </summary>
        public ushort MsgLength { get; set; }

        /// <summary>
        /// Хеш CRC16, посчитанный по скремблированому сообщению без синхронизирующей последовательности (48 бит) и стартовых нулей
        /// </summary>
        public ushort MsgCrc { get; set; }

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var slotAndMsg = BinSerialize.ReadByte(ref buffer);
            Slot = (AsvGbasSlotMsg)(slotAndMsg & 0b0000_0111);
            // GbasMessageId = (byte)(slotAndMsg >> 3);

            var flags = BinSerialize.ReadByte(ref buffer);
            IsLastSlotInFrame = (flags & 0b0000_0001) != 0;
            // LastByteOffset = (byte)((flags >> 1) & 0b0000_0111);
            // ReservedFlgas = (byte)(flags >> 4);

            LifeTime = BinSerialize.ReadByte(ref buffer);

            MsgLength = BinSerialize.ReadUShort(ref buffer);

            MsgCrc = BinSerialize.ReadUShort(ref buffer);
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, (byte)((byte)Slot)); // | (GbasMessageId << 3)));
            BinSerialize.WriteByte(ref buffer, (byte)(((IsLastSlotInFrame ? 1 : 0)))); // | ((LastByteOffset & 0b0000_0111) << 1) | (ReservedFlgas << 4)));
            BinSerialize.WriteByte(ref buffer, LifeTime);
            BinSerialize.WriteUShort(ref buffer, MsgLength);
            BinSerialize.WriteUShort(ref buffer, MsgCrc);
        }

        protected override int InternalGetContentByteSize() => 7;

        public override void Randomize(Random random)
        {
            Sequence = (ushort)random.Next(0, ushort.MaxValue);
            TargetId = (byte)random.Next(0, byte.MaxValue);
            SenderId = (byte)random.Next(0, byte.MaxValue);
            Slot = (AsvGbasSlotMsg)random.Next(0, Enum.GetValues(typeof(AsvGbasSlotMsg)).Length - 1);
            IsLastSlotInFrame = random.Next() % 2 == 0;
            LifeTime = (byte)((random.Next() % 2 == 0) ? 1 : byte.MaxValue);
            MsgLength = (ushort)random.Next(0, ushort.MaxValue);
            MsgCrc = (ushort)random.Next(0, ushort.MaxValue);
        }
    }
}