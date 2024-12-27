using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a base class for Communication Navigation ASCII commands.
    /// </summary>
    public abstract class ComNavAsciiCommandBase : GnssMessageBase<string>
    {
        /// <summary>
        /// Serializes the class into an ASCII string.
        /// </summary>
        /// <returns>The serialized ASCII string.</returns>
        protected abstract string SerializeToAsciiString();

        /// <summary>
        /// Gets the name of the message.
        /// </summary>
        public override string Name => MessageId;

        /// <summary>
        /// Gets the protocol identifier.
        /// </summary>
        public override string ProtocolId => "ComNavAscii";

        /// <summary>
        /// Converts the class into its string equivalent.
        /// </summary>
        /// <returns>A string that represents the object.</returns>
        public override string ToString()
        {
            return SerializeToAsciiString();
        }

        /// <summary>
        /// Deserializes the object from a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to deserialize from.</param>
        /// <exception cref="NotImplementedException">Always thrown in this implementation.</exception>
        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the object to a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialize.</param>
        public override void Serialize(ref Span<byte> buffer)
        {
            SerializeToAsciiString().CopyTo(ref buffer, Encoding.ASCII);
            BinSerialize.WriteByte(ref buffer, 0x0D);
            BinSerialize.WriteByte(ref buffer, 0x0A);
        }

        /// <summary>
        /// Gets the byte size of the serialized object.
        /// </summary>
        /// <returns>The byte size of the serialized object.</returns>
        public override int GetByteSize()
        {
            return SerializeToAsciiString().Length
                + 2 /* END of message 0x0D & 0x0A */
            ;
        }
    }
}
