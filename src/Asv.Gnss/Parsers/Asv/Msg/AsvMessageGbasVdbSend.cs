using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents the slots available in the AsvGbas system.
    /// </summary>
    [Flags]
    public enum AsvGbasSlot : byte
    {
        /// <summary>
        /// Represents the Slot A in the AsvGbasSlot enumeration.
        /// </summary>
        SlotA = 0b00000001,

        /// <summary>
        /// Represents the SlotB member of the AsvGbasSlot enum.
        /// </summary>
        SlotB = 0b00000010,

        /// <summary>
        /// Represents SlotC in the AsvGbasSlot enum.
        /// </summary>
        SlotC = 0b00000100,

        /// <summary>
        /// Represents SlotD in the AsvGbasSlot enum.
        /// </summary>
        SlotD = 0b00001000,

        /// <summary>
        /// Represents the SlotE member of the AsvGbasSlot enumeration.
        /// </summary>
        SlotE = 0b00010000,

        /// <summary>
        /// Enumeration member representing SlotF in the AsvGbasSlot enum.
        /// </summary>
        SlotF = 0b00100000,

        /// <summary>
        /// Represents the SlotG member of the AsvGbasSlot enum.
        /// </summary>
        SlotG = 0b01000000,

        /// <summary>
        /// Represents the SlotH member of the AsvGbasSlot enumeration.
        /// </summary>
        SlotH = 0b10000000,
    }

    /// <summary>
    /// Represents the <c>AsvGbasMessage</c> enum.
    /// </summary>
    [Flags]
    public enum AsvGbasMessage : ulong
    {
        /// <summary>
        /// Represents the message type Msg1 in the AsvGbasMessage enum.
        /// </summary>
        Msg1 = 0b00000001,

        /// <summary>
        /// Represents the Msg101 member of the AsvGbasMessage enum.
        /// </summary>
        /// <remarks>
        /// This member is assigned the value 0b00000010.
        /// </remarks>
        Msg101 = 0b00000010,

        /// <summary>
        /// Represents the Msg2 member of the AsvGbasMessage enum.
        /// </summary>
        Msg2 = 0b00000100,

        /// <summary>
        /// Represents the message type Msg3 in the AsvGbasMessage enum.
        /// </summary>
        Msg3 = 0b00001000,

        /// <summary>
        /// Represents the Msg4 member of the AsvGbasMessage enum.
        /// </summary>
        /// <remarks>
        /// Msg4 is a flag representing a specific message in the AsvGbasMessage enum.
        /// </remarks>
        /// <seealso cref="AsvGbasMessage"/>
        /// <seealso cref="AsvGbasMessage.Msg1"/>
        /// <seealso cref="AsvGbasMessage.Msg101"/>
        /// <seealso cref="AsvGbasMessage.Msg2"/>
        /// <seealso cref="AsvGbasMessage.Msg3"/>
        /// <seealso cref="AsvGbasMessage.Msg5"/>
        Msg4 = 0b00010000,

        /// <summary>
        /// Represents the Msg5 member of the AsvGbasMessage enumeration.
        /// </summary>
        Msg5 = 0b00100000,
    }

    /// <summary>
    /// Represents the GBAS VDB Send message.
    /// </summary>
    public class AsvMessageGbasVdbSend : AsvMessageBase
    {
        /// <summary>
        /// Gets the unique identifier of the message.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        /// <remarks>
        /// This property is an overridden property and returns a fixed value of 0x0100.
        /// </remarks>
        public override ushort MessageId => 0x0100;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// A string representing the name of the property.
        /// </value>
        public override string Name => "GbasVdbSend";

        /// <summary>
        /// Deserializes the internal content using binary serialization from the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing serialized data.</param>
        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            Slot = (AsvGbasSlot)BinSerialize.ReadByte(ref buffer);
            Msgs = (AsvGbasMessage)BinSerialize.ReadULong(ref buffer);
            LastByteLength = BinSerialize.ReadByte(ref buffer);
            Data = new byte[buffer.Length];
            buffer.CopyTo(Data);
            buffer = buffer.Slice(Data.Length);
        }

        /// <summary>
        /// Serializes the internal content of an object to a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer to serialize the internal content to.</param>
        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, (byte)Slot);
            BinSerialize.WriteULong(ref buffer, (ulong)Msgs);
            BinSerialize.WriteByte(ref buffer, LastByteLength);
            Data.CopyTo(buffer);
            buffer = buffer.Slice(Data.Length);
        }

        /// <summary>
        /// Calculates the byte size of the content.
        /// </summary>
        /// <returns>The byte size of the content.</returns>
        protected override int InternalGetContentByteSize()
        {
            return Data.Length + 10;
        }

        // Randomizes the properties of the object using the provided random generator.
        // @param random The random generator to be used for randomizing the properties.
        // /
        public override void Randomize(Random random)
        {
            Sequence = (ushort)random.Next(0, ushort.MaxValue);
            TargetId = (byte)random.Next(0, byte.MaxValue);
            SenderId = (byte)random.Next(0, byte.MaxValue);
            Data = new byte[random.Next(0, AsvMessageParser.DataSize - 10)];
            random.NextBytes(Data);
            LastByteLength = (byte)random.Next(0, 7);
            Msgs = (AsvGbasMessage)
                random.Next(0, Enum.GetValues(typeof(AsvGbasMessage)).Length - 1);
            Slot = (AsvGbasSlot)random.Next(0, Enum.GetValues(typeof(AsvGbasSlot)).Length - 1);
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the length in bytes of the last byte.
        /// </summary>
        /// <value>
        /// The length in bytes of the last byte.
        /// </value>
        public byte LastByteLength { get; set; }

        /// <summary>
        /// Gets or sets property representing AsvGbasMessage objects.
        /// </summary>
        /// <value>
        /// The Msgs property represents a collection of AsvGbasMessage objects.
        /// </value>
        public AsvGbasMessage Msgs { get; set; }

        /// <summary>
        /// Gets or sets the Slot property.
        /// </summary>
        /// <value>
        /// The Slot property.
        /// </value>
        /// <remarks>
        /// This property represents a slot and is of type AsvGbasSlot.
        /// </remarks>
        public AsvGbasSlot Slot { get; set; }
    }
}
