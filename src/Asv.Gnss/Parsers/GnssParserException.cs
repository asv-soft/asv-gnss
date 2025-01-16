using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Exception class that represents a generic GNSS parser exception.
    /// </summary>
    [Serializable]
    public class GnssParserException : Exception
    {
        /// <summary>
        /// Gets the protocol ID associated with the exception.
        /// </summary>
        public string ProtocolId { get; }

        public GnssParserException(string protocolId, string message)
            : base(message)
        {
            ProtocolId = protocolId;
        }

        public GnssParserException(string protocolId, string message, Exception inner)
            : base(message, inner)
        {
            ProtocolId = protocolId;
        }

        public GnssParserException()
            : base() { }

        public GnssParserException(string message)
            : base(message) { }

        public GnssParserException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception class that represents a CRC error in a GNSS message.
    /// </summary>
    [Serializable]
    public class GnssCrcErrorException : GnssParserException
    {
        public GnssCrcErrorException(string protocolId)
            : base(protocolId, $"Crc error occurred when recv '{protocolId}' message") { }

        public GnssCrcErrorException(string protocolId, string message)
            : base(protocolId, message) { }

        public GnssCrcErrorException(string protocolId, string message, Exception inner)
            : base(protocolId, message, inner) { }

        public GnssCrcErrorException()
            : base() { }

        public GnssCrcErrorException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception class that represents an error where not all data was read when deserializing a GNSS packet.
    /// </summary>
    [Serializable]
    public class GnssReadNotAllDataWhenDeserializePacketErrorException : GnssParserException
    {
        /// <summary>
        /// Gets the message ID associated with the exception.
        /// </summary>
        public string MessageId { get; }

        public GnssReadNotAllDataWhenDeserializePacketErrorException(
            string protocolId,
            string messageId
        )
            : base(
                protocolId,
                $"Read not all data when deserialize '{protocolId}.{messageId}' message"
            )
        {
            MessageId = messageId;
        }

        public GnssReadNotAllDataWhenDeserializePacketErrorException(
            string protocolId,
            string message,
            Exception inner
        )
            : base(protocolId, message, inner) { }

        public GnssReadNotAllDataWhenDeserializePacketErrorException()
            : base() { }

        public GnssReadNotAllDataWhenDeserializePacketErrorException(string message)
            : base(message) { }

        public GnssReadNotAllDataWhenDeserializePacketErrorException(
            string message,
            Exception innerException
        )
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception class that represents an unknown GNSS message.
    /// </summary>
    [Serializable]
    public class GnssUnknownMessageException : GnssParserException
    {
        /// <summary>
        /// Gets the message ID associated with the exception.
        /// </summary>
        public string MessageId { get; }

        public GnssUnknownMessageException(string protocolId, string messageId)
            : base(protocolId, $"Unknown {protocolId} packet message number [MSG={messageId}]")
        {
            MessageId = messageId;
        }

        public GnssUnknownMessageException(string protocolId, string message, Exception inner)
            : base(protocolId, message, inner) { }

        public GnssUnknownMessageException()
            : base() { }

        public GnssUnknownMessageException(string message)
            : base(message) { }

        public GnssUnknownMessageException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception class that represents a deserialization error in a GNSS message.
    /// </summary>
    [Serializable]
    public class GnssDeserializeMessageException : GnssParserException
    {
        /// <summary>
        /// Gets the message ID associated with the exception.
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// Gets the message name associated with the exception.
        /// </summary>
        public string MessageName { get; }

        public GnssDeserializeMessageException(
            string protocolId,
            string messageId,
            string messageName,
            Exception inner
        )
            : base(
                protocolId,
                $"Deserialization {protocolId}.{messageName}[ID={messageId}] packet error ",
                inner
            )
        {
            MessageId = messageId;
            MessageName = messageName;
        }

        public GnssDeserializeMessageException(string protocolId, string message)
            : base(protocolId, message) { }

        public GnssDeserializeMessageException(string protocolId, string message, Exception inner)
            : base(protocolId, message, inner) { }

        public GnssDeserializeMessageException()
            : base() { }

        public GnssDeserializeMessageException(string message)
            : base(message) { }

        public GnssDeserializeMessageException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception class that represents an error in publishing a GNSS message.
    /// </summary>
    [Serializable]
    public class GnssPublishMessageException : GnssParserException
    {
        /// <summary>
        /// Gets the message ID associated with the exception.
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// Gets the message name associated with the exception.
        /// </summary>
        public string MessageName { get; }

        public GnssPublishMessageException(
            string protocolId,
            string messageId,
            string messageName,
            Exception inner
        )
            : base(
                protocolId,
                $"Publication {protocolId}.{messageName}[ID={messageId}] packet throw exception ",
                inner
            )
        {
            MessageId = messageId;
            MessageName = messageName;
        }

        public GnssPublishMessageException(string protocolId, string message)
            : base(protocolId, message) { }

        public GnssPublishMessageException(string protocolId, string message, Exception inner)
            : base(protocolId, message, inner) { }

        public GnssPublishMessageException()
            : base() { }

        public GnssPublishMessageException(string message)
            : base(message) { }

        public GnssPublishMessageException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
