using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Base class for UBX (u-blox binary) messages.
    /// </summary>
    public abstract class UbxMessageBase : GnssMessageBase<ushort>
    {
        /// <summary>
        /// Gets the protocol ID associated with this property.
        /// </summary>
        /// <remarks>
        /// The protocol ID is a string that identifies the specific protocol used in the GNSS data.
        /// </remarks>
        public override string ProtocolId => UbxBinaryParser.GnssProtocolId;

        /// <summary>
        /// Represents a property that exposes the class value in a byte format.
        /// This property is read-only. </summary>
        /// /
        public abstract byte Class { get; }

        /// <summary>
        /// Gets the value of the SubClass property.
        /// </summary>
        /// <value>
        /// A byte representing the value of the SubClass property.
        /// </value>
        public abstract byte SubClass { get; }

        /// <summary>
        /// Gets the unique identifier for the message. </summary> <value>
        /// The message ID represented as an unsigned short. </value>
        /// /
        public override ushort MessageId => (ushort)((Class << 8) | SubClass);

        /// <summary>
        /// Serializes the object to a byte buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the serialized data into.</param>
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

        /// <summary>
        /// Deserialize the UBX message from the buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing the UBX message.</param>
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

        /// <summary>
        /// Serializes the content into a byte buffer.
        /// </summary>
        /// <param name="buffer">A reference to the byte buffer used for serialization.</param>
        protected abstract void SerializeContent(ref Span<byte> buffer);

        /// <summary>
        /// Deserializes the content of the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing the serialized data.</param>
        protected abstract void DeserializeContent(ref ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Gets the size in bytes of the content.
        /// </summary>
        /// <returns>
        /// An integer representing the size in bytes of the content.
        /// </returns>
        protected abstract int GetContentByteSize();

        /// <summary>
        /// Calculates the byte size of the content along with the header and CRC size.
        /// </summary>
        /// <returns>The total byte size of the content, including the header and CRC.</returns>
        public override int GetByteSize()
        {
            return UbxHelper.HeaderOffset + 2/*CRC*/ + GetContentByteSize();
        }


        /// <summary>
        /// Randomizes the object based on the given random number generator.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        public abstract void Randomize(Random random);

    }
}