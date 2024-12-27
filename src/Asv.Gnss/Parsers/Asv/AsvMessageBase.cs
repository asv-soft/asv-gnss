using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Base class for ASV (Autonomous Surface Vehicle) messages used in GNSS communication.
    /// </summary>
    public abstract class AsvMessageBase : GnssMessageBase<ushort>
    {
        /// <summary>
        /// Gets the protocol ID for the message parser.
        /// </summary>
        /// <remarks>
        /// The protocol ID is used to identify the specific GNSS protocol that the message parser
        /// is associated with.
        /// </remarks>
        /// <returns>
        /// The protocol ID for the GNSS message parser.
        /// </returns>
        public override string ProtocolId => AsvMessageParser.GnssProtocolId;

        /// <summary>
        /// Gets or sets the sequence value.
        /// </summary>
        /// <value>
        /// The sequence value.
        /// </value>
        public ushort Sequence { get; set; }

        /// <summary>
        /// Gets or sets the target ID.
        /// </summary>
        /// <remarks>
        /// The target ID is used to specify the ID of the target element.
        /// </remarks>
        public byte TargetId { get; set; }
        public byte SenderId { get; set; }

        /// <summary>
        /// Deserializes the provided byte buffer and updates the object's state accordingly.
        /// </summary>
        /// <param name="buffer">Reference to the byte buffer to deserialize.</param>
        /// <exception cref="Exception">Thrown when an error occurs during deserialization.</exception>
        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var crcSpan = buffer;
            var bitIndex = 0;
            var sync1 = BinSerialize.ReadByte(ref buffer);
            var sync2 = BinSerialize.ReadByte(ref buffer);

            if (sync1 != AsvMessageParser.Sync1 || sync2 != AsvMessageParser.Sync2)
            {
                throw new Exception($"Error to deserialize {ProtocolId}.{Name}");
            }

            var length = BinSerialize.ReadUShort(ref buffer);
            var crc = AsvCrc16.Calc(crcSpan, length + 10);
            crcSpan = crcSpan.Slice(length + 10);
            var crcIndex = (length + 10) * 8;
            var crcOrigin = BinSerialize.ReadUShort(ref crcSpan);
            if (crc != crcOrigin)
            {
                throw new Exception(
                    $"Error to deserialize {ProtocolId}.{Name}: CRC error. Want {crc}. Got {crcOrigin}"
                );
            }

            Sequence = BinSerialize.ReadUShort(ref buffer);
            SenderId = BinSerialize.ReadByte(ref buffer);
            TargetId = BinSerialize.ReadByte(ref buffer);
            var msgId = BinSerialize.ReadUShort(ref buffer);
            if (MessageId != msgId)
            {
                throw new Exception(
                    $"Error to deserialize {ProtocolId}.{Name}: Message id not equals. Want '{MessageId}. Got '{msgId}''"
                );
            }

            var dataSpan = buffer.Slice(bitIndex / 8, length);

            // var dataSpan = buffer.Slice(0, length);
            InternalContentDeserialize(ref dataSpan);
            buffer = buffer.Slice(
                length + 2 /*CRC16*/
            );
        }

        /// <summary>
        /// Serializes the data into a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to be serialized into.</param>
        public override void Serialize(ref Span<byte> buffer)
        {
            var originSpan = buffer;
            BinSerialize.WriteByte(ref buffer, AsvMessageParser.Sync1);
            BinSerialize.WriteByte(ref buffer, AsvMessageParser.Sync2);
            var length = (ushort)InternalGetContentByteSize();
            BinSerialize.WriteUShort(ref buffer, length);
            BinSerialize.WriteUShort(ref buffer, Sequence);
            BinSerialize.WriteByte(ref buffer, SenderId);
            BinSerialize.WriteByte(ref buffer, TargetId);
            BinSerialize.WriteUShort(ref buffer, MessageId);
            InternalContentSerialize(ref buffer);
            var crc = AsvCrc16.Calc(
                originSpan,
                length + 10 /*from sync1 to end of data*/
            );
            BinSerialize.WriteUShort(ref buffer, crc);
        }

        /// <summary>
        /// This method is responsible for deserializing the internal content of a buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing the internal content to be deserialized.</param>
        protected abstract void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer);

        /// <summary> Serializes the internal content into a specified buffer. </summary>
        /// <param name="buffer"> The buffer to store the serialized content. </param>
        protected abstract void InternalContentSerialize(ref Span<byte> buffer);

        /// <summary>
        /// Calculates the byte size of the content.
        /// </summary>
        /// <returns>The byte size of the content.</returns>
        protected abstract int InternalGetContentByteSize();

        /// <summary>
        /// Calculates the total byte size of an object, including the header and CRC. </summary> <returns>
        /// The byte size of the object. </returns>
        /// /
        public override int GetByteSize() =>
            10 /*HEADER*/
            + InternalGetContentByteSize()
            + 2 /*CRC*/
        ;

        /// <summary>
        /// Randomizes the objects or properties of the derived class using the provided Random object.
        /// </summary>
        /// <param name="random">The Random object used for generating random values.</param>
        public abstract void Randomize(Random random);
    }
}
