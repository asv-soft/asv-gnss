using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a GbasCuSendV2 message.
    /// </summary>
    public class AsvMessageGbasCuSendV2 : AsvMessageBase
    {
        /// <summary>
        /// Gets the unique identifier for the message.
        /// </summary>
        /// <remarks>
        /// The MessageId property represents the unique identifier value for the message.
        /// </remarks>
        /// <returns>The unique identifier for the message.</returns>
        public override ushort MessageId => 0x0103;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public override string Name => "GbasCuSendV2";

        /// <summary>
        /// Gets or sets the slot of the current message (A - H).
        /// </summary>
        /// <value>
        /// The current message slot.
        /// </value>
        public AsvGbasSlotMsg Slot { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the last slot in the frame.
        /// When this is true, the device understands that the entire frame has been transmitted
        /// and it is now safe to start transmitting a new frame.
        /// </summary>
        public bool IsLastSlotInFrame { get; set; }

        /// <summary>
        /// Gets or sets the lifetime of a message in 500 ms (1 frame) increments.
        /// </summary>
        /// <value>
        /// The lifetime of the message in 500 ms increments.
        /// </value>
        public byte LifeTime { get; set; }

        /// <summary>
        /// Gets or sets the length, in bits, of the scrambled message without the synchronization sequence (48 bits) and leading zeros.
        /// </summary>
        public ushort MsgLength { get; set; }

        /// <summary>
        /// Represents the CRC16 hash calculated from the scrambled message without the synchronizing sequence (48 bits) and leading zeroes.
        /// </summary>
        public ushort MsgCrc { get; set; }

        /// <summary>
        /// Deserializes the internal content of the object from a byte buffer. </summary> <param name="buffer">The byte buffer that contains the serialized data.</param> <remarks>
        /// This method is called internally to deserialize the internal content of the object
        /// from a byte buffer. It extracts specific values from the buffer and assigns them
        /// to the corresponding properties of the object. </remarks>
        /// /
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

        /// <summary>
        /// Serializes the internal content of the object and writes it into the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the serialized data into.</param>
        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, (byte)((byte)Slot)); // | (GbasMessageId << 3)));
            BinSerialize.WriteByte(ref buffer, (byte)(((IsLastSlotInFrame ? 1 : 0)))); // | ((LastByteOffset & 0b0000_0111) << 1) | (ReservedFlgas << 4)));
            BinSerialize.WriteByte(ref buffer, LifeTime);
            BinSerialize.WriteUShort(ref buffer, MsgLength);
            BinSerialize.WriteUShort(ref buffer, MsgCrc);
        }

        /// <summary>
        /// Calculates the byte size of the content.
        /// </summary>
        /// <returns>The byte size of the content.</returns>
        protected override int InternalGetContentByteSize() => 7;

        /// <summary>
        /// Randomizes the values of the properties of the object based on the given random generator.
        /// </summary>
        /// <param name="random">The random generator object to use.</param>
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