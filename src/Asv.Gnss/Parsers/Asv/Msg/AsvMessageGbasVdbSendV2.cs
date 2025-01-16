using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a GBAS VDB Send V2 message in the ASV protocol.
    /// </summary>
    public class AsvMessageGbasVdbSendV2 : AsvMessageBase
    {
        /// <summary>
        /// Gets the identifier for the message.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public override ushort MessageId => 0x0102;

        /// <summary>
        /// Gets the name of the property is "GbasVdbSendV2".
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the name of the property.
        /// </value>
        public override string Name => "GbasVdbSendV2";

        /// <summary>
        /// Gets or sets the slot number of the current message (A - H).
        /// </summary>
        public AsvGbasSlotMsg Slot { get; set; }

        /// <summary>
        /// Gets or sets the GBAS message ID in the packet.
        /// </summary>
        /// <remarks>
        /// The GBAS message ID is a byte value that represents the type of the message in the GBAS packet.
        /// </remarks>
        public byte GbasMessageId { get; set; }

        /// <summary>
        /// Gets or sets the active slots for GBAS at the moment.
        /// </summary>
        public AsvGbasSlot ActiveSlots { get; set; }

        /// <summary>
        /// Gets or sets the lifetime of a message in 500ms (1 frame) intervals.
        /// </summary>
        /// <remarks>
        /// The Lifetime property determines how long a message will exist before being removed or expired.
        /// It is measured in 500ms intervals, meaning a value of 1 will result in a message being removed after 500ms,
        /// a value of 2 will result in a message being removed after 1 second, and so on.
        /// </remarks>
        /// <value>
        /// The lifetime of a message in 500ms intervals.
        /// </value>
        public byte LifeTime { get; set; }

        /// <summary>
        /// Gets or sets the offset of the last byte in the message.
        /// </summary>
        public byte LastByteOffset { get; set; }

        /// <summary>
        /// Gets or sets the reserved flags for future use.
        /// </summary>
        /// <value>
        /// The reserved flags.
        /// </value>
        public byte ReservedFlgas { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the last slot in the frame.
        /// </summary>
        /// <remarks>
        /// This property is used to determine if the entire frame has been transmitted.
        /// When set to true, it indicates that the entire frame has been transmitted and
        /// it is safe to begin transmitting a new frame.
        /// </remarks>
        public bool IsLastSlotInFrame { get; set; }

        /// <summary>
        /// Gets or sets data for sending over VDB.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Deserialize the internal content of the object from a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer containing the serialized data.</param>
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

        /// <summary>
        /// Method to serialize internal content. </summary> <param name="buffer">The buffer to store serialized data.</param>
        /// /
        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, (byte)((byte)Slot | (GbasMessageId << 3)));
            BinSerialize.WriteByte(ref buffer, (byte)ActiveSlots);
            BinSerialize.WriteByte(ref buffer, LifeTime);
            BinSerialize.WriteByte(
                ref buffer,
                (byte)(
                    (LastByteOffset & 0b0000_0111)
                    | ((IsLastSlotInFrame ? 1 : 0) << 3)
                    | (ReservedFlgas << 4)
                )
            );
            Data.CopyTo(buffer);
            buffer = buffer.Slice(Data.Length);
        }

        /// <summary>
        /// Calculates the byte size of the content.
        /// </summary>
        /// <returns>The byte size of the content.</returns>
        protected override int InternalGetContentByteSize() => Data.Length + 4;

        /// <summary>
        /// Randomizes the properties of an object using the specified random number generator.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        public override void Randomize(Random random)
        {
            Sequence = (ushort)random.Next(0, ushort.MaxValue);
            TargetId = (byte)random.Next(0, byte.MaxValue);
            SenderId = (byte)random.Next(0, byte.MaxValue);
            Data = new byte[random.Next(0, AsvMessageParser.MaxMessageSize - 10)];
            random.NextBytes(Data);
            LastByteOffset = (byte)random.Next(0, 7);
            ReservedFlgas = (byte)random.Next(0, (int)Math.Pow(2, 4));
            ActiveSlots = (AsvGbasSlot)
                random.Next(0, Enum.GetValues(typeof(AsvGbasSlot)).Length - 1);
            Slot = (AsvGbasSlotMsg)
                random.Next(0, Enum.GetValues(typeof(AsvGbasSlotMsg)).Length - 1);
            IsLastSlotInFrame = random.Next() % 2 == 0;
        }
    }

    /// <summary>
    /// Represents the slot messages for AsvGbas.
    /// </summary>
    public enum AsvGbasSlotMsg : byte
    {
        /// <summary>
        /// Represents the Slot A member of the AsvGbasSlotMsg enum.
        /// </summary>
        /// <remarks>
        /// This member has a value of 0.
        /// </remarks>
        SlotA = 0,

        /// <summary>
        /// Represents SlotB of the AsvGbasSlotMsg enumeration.
        /// </summary>
        /// <remarks>
        /// SlotB is the second slot in the AsvGbasSlotMsg enumeration.
        /// </remarks>
        SlotB = 1,

        /// <summary>
        /// Represents Slot C.
        /// </summary>
        /// <remarks>
        /// This member of the AsvGbasSlotMsg enum represents Slot C.
        /// </remarks>
        SlotC = 2,

        /// <summary>
        /// Represents the SlotD member of the AsvGbasSlotMsg enum.
        /// </summary>
        SlotD = 3,

        /// <summary>
        /// Represents the SlotE member of the AsvGbasSlotMsg enum.
        /// </summary>
        /// <remarks>
        /// This member has a value of 4.
        /// </remarks>
        SlotE = 4,

        /// <summary>
        /// Represents the SlotF member of the AsvGbasSlotMsg enum.
        /// </summary>
        /// <remarks>
        /// This member has a value of 5.
        /// </remarks>
        SlotF = 5,

        /// <summary>
        /// Represents the SlotG member of the AsvGbasSlotMsg enum.
        /// </summary>
        SlotG = 6,

        /// <summary>
        /// Represents a specific slot message with the value 7.
        /// </summary>
        SlotH = 7,
    }
}
