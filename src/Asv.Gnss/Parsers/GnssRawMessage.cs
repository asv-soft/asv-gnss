using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a base class for GNSS (Global Navigation Satellite System) raw messages.
    /// </summary>
    /// <typeparam name="TMsgId">The type of message ID.</typeparam>
    public abstract class GnssRawMessage<TMsgId>
    {
        /// <summary>
        /// Gets the unique identifier for the protocol.
        /// </summary>
        /// <remarks>
        /// This property returns a string representing the unique identifier for the protocol.
        /// </remarks>
        /// <returns>
        /// A string representing the unique identifier for the protocol.
        /// </returns>
        public abstract string ProtocolId { get; }

        /// <summary>
        /// Gets the identifier of the message.
        /// </summary>
        /// <returns>
        /// The identifier of the message.
        /// </returns>
        public TMsgId MessageId { get; }

        /// <summary>
        /// Gets the raw data associated with this property.
        /// </summary>
        /// <value>
        /// The raw data as an array of bytes.
        /// </value>
        public byte[] RawData { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GnssRawMessage{TMsgId}"/> class.
        /// Creates a new instance of the GnssRawMessage class.
        /// </summary>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="data">The raw data.</param>
        protected GnssRawMessage(TMsgId messageId, ReadOnlySpan<byte> data)
        {
            MessageId = messageId;
            RawData = data.ToArray();
        }
    }
}
