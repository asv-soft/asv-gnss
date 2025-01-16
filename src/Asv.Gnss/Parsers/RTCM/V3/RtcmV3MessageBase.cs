using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Base class for RTCMv3 messages.
    /// </summary>
    public abstract class RtcmV3MessageBase : GnssMessageBase<ushort>
    {
        /// <summary>
        /// Gets the protocol ID of the GNSS protocol used by the RTCM V3 parser.
        /// </summary>
        /// <remarks>
        /// The protocol ID indicates the type of GNSS protocol being used.
        /// </remarks>
        /// <returns>
        /// The protocol ID of the GNSS protocol used by the RTCM V3 parser.
        /// </returns>
        /// <seealso cref="RtcmV3Parser.GnssProtocolId"/>
        public override string ProtocolId => RtcmV3Parser.GnssProtocolId;

        /// <summary>
        /// Gets or sets the reserved value.
        /// </summary>
        /// <value>
        /// The reserved value.
        /// </value>
        public byte Reserved { get; set; }

        /// <summary>
        /// Deserializes the specified buffer into the current object.
        /// </summary>
        /// <param name="buffer">The buffer containing the serialized data.</param>
        /// <exception cref="Exception">
        /// Thrown when the deserialization of the RTCMv3 message fails due to an incorrect preamble,
        /// length too small, or incorrect message number.
        /// </exception>
        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var preamble = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (preamble != RtcmV3Helper.SyncByte)
            {
                throw new Exception(
                    $"Deserialization RTCMv3 message failed: want {RtcmV3Helper.SyncByte:X}. Read {preamble:X}"
                );
            }

            Reserved = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            var messageLength = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
            if (
                messageLength
                > (
                    buffer.Length - 3 /* crc 24 bit*/
                )
            )
            {
                throw new Exception(
                    $"Deserialization RTCMv3 message failed: length too small. Want '{messageLength}'. Read = '{buffer.Length}'"
                );
            }

            var msgId = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            if (msgId != MessageId)
            {
                throw new Exception(
                    $"Deserialization RTCMv3 message failed: want message number '{MessageId}'. Read = '{msgId}'"
                );
            }

            DeserializeContent(buffer, ref bitIndex, messageLength);
            bitIndex += 3 * 8; // skip crc
            buffer =
                bitIndex % 8.0 == 0 ? buffer.Slice(bitIndex / 8) : buffer.Slice((bitIndex / 8) + 1);
        }

        /// <summary>
        /// Deserializes the content of the given buffer into the specified message structure.
        /// </summary>
        /// <param name="buffer">The buffer containing the serialized data.</param>
        /// <param name="bitIndex">The index representing the current position in the buffer.</param>
        /// <param name="messageLength">The length of the message in the buffer.</param>
        protected abstract void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        );

        /// <summary>
        /// Serializes the object into a span of bytes.
        /// </summary>
        /// <param name="buffer">The byte span to serialize into.</param>
        /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
        public override void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the byte size of the current object. </summary>
        /// <returns>
        /// The byte size of the object. </returns>
        /// <remarks>
        /// This method must be implemented by derived classes to calculate the byte size of the current object. </remarks>
        /// /
        public override int GetByteSize()
        {
            throw new NotImplementedException();
        }
    }
}
