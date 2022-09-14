using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class AsvMessageGbasVdbSendV2 : AsvMessageBase
    {
        public override ushort MessageId => 0x0102;
        public override string Name => "GbasVdbSendV2";

        /// <summary>
        /// Номер слота текущего сообщения (A - H)
        /// </summary>
        public AsvGbasSlotMsg Slot { get; set; }
        /// <summary>
        /// Тип сообщения GBAS в пакете
        /// </summary>
        public byte GbasMessageId { get; set; }
        /// <summary>
        /// Активные слоты GBAS в данный момент
        /// </summary>
        public AsvGbasSlot ActiveSlots { get; set; }
        /// <summary>
        /// Время жизни сообщения в 500 мс ( 1 frame) отрезках.  
        /// </summary>
        public byte LifeTime { get; set; }


        /// <summary>
        /// Размер значащих битов в последнем байте сообщения (0 - 7)
        /// </summary>
        public byte LastByteOffset { get; set; }
        /// <summary>
        /// Зарезервированы для дальнейшего использования.
        /// </summary>
        public byte ReservedFlgas { get; set; }
        /// <summary>
        /// Признака окончания передачи всего фрейма.
        /// По нему устройство понимает, что весь фрейм передан и можно начинать передавать новый фрейм в эфир.
        /// </summary>
        public bool IsLastSlotInFrame { get; set; }
        /// <summary>
        /// Данные для отправки по VDB
        /// </summary>
        public byte[] Data { get; set; }

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var slotAndMsg = BinSerialize.ReadByte(ref buffer);
            Slot = (AsvGbasSlotMsg)(slotAndMsg & 0b0000_0111);
            GbasMessageId = (byte)(slotAndMsg >> 3);

            ActiveSlots = (AsvGbasSlot)BinSerialize.ReadByte(ref buffer);

            LifeTime = BinSerialize.ReadByte(ref buffer);

            var flags = BinSerialize.ReadByte(ref buffer);

            LastByteOffset = (byte)(flags & 0b0000_0111);
            IsLastSlotInFrame = ((flags >> 3) & 0b0000_0001) != 0;
            ReservedFlgas = (byte)(flags >> 4);
            Data = new byte[buffer.Length];
            buffer.CopyTo(Data);
            buffer = buffer.Slice(Data.Length);
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, (byte)((byte)Slot | (GbasMessageId << 3)));
            BinSerialize.WriteByte(ref buffer,(byte)ActiveSlots);
            BinSerialize.WriteByte(ref buffer, LifeTime);
            BinSerialize.WriteByte(ref buffer, (byte)((LastByteOffset & 0b0000_0111) | ((IsLastSlotInFrame ? 1:0) << 3) | (ReservedFlgas << 4)) );
            Data.CopyTo(buffer);
            buffer = buffer.Slice(Data.Length);
        }

        protected override int InternalGetContentByteSize() => Data.Length + 4;

        public override void Randomize(Random random)
        {
            Sequence = (ushort)random.Next(0, ushort.MaxValue);
            TargetId = (byte)random.Next(0, byte.MaxValue);
            SenderId = (byte)random.Next(0, byte.MaxValue);
            Data = new byte[random.Next(0, AsvMessageParser.MaxMessageSize - 10)];
            random.NextBytes(Data);
            LastByteOffset = (byte)random.Next(0, 7);
            ReservedFlgas = (byte)random.Next(0, (int)Math.Pow(2, 4));
            ActiveSlots = (AsvGbasSlot)random.Next(0, Enum.GetValues(typeof(AsvGbasSlot)).Length - 1);
            Slot = (AsvGbasSlotMsg)random.Next(0, Enum.GetValues(typeof(AsvGbasSlotMsg)).Length - 1);
            IsLastSlotInFrame = random.Next() % 2 == 0;
        }
    }


    public enum AsvGbasSlotMsg : byte
    {
        SlotA = 0,
        SlotB = 1,
        SlotC = 2,
        SlotD = 3,
        SlotE = 4,
        SlotF = 5,
        SlotG = 6,
        SlotH = 7,
    }
}