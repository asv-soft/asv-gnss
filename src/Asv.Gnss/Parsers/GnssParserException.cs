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

        public GnssParserException(string protocolId, string message) : base(message)
        {
            ProtocolId = protocolId;
        }

        public GnssParserException(string protocolId, string message, Exception inner) : base(message, inner)
        {
            ProtocolId = protocolId;
        }
    }

    /// <summary>
    /// Exception class that represents a CRC error in a GNSS message.
    /// </summary>
    [Serializable]
    public class GnssCrcErrorException : GnssParserException
    {
        public GnssCrcErrorException(string protocolId) : base(protocolId, $"Crc error occurred when recv '{protocolId}' message")
        {
        }
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

        public GnssReadNotAllDataWhenDeserializePacketErrorException(string protocolId, string messageId) : base(protocolId, $"Read not all data when deserialize '{protocolId}.{messageId}' message")
        {
            MessageId = messageId;
        }
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

        public GnssUnknownMessageException(string protocolId, string messageId) : base(protocolId, $"Unknown {protocolId} packet message number [MSG={messageId}]")
        {
            MessageId = messageId;
        }
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

        public GnssDeserializeMessageException(string protocolId, string messageId, string messageName, Exception inner) : base(protocolId, $"Deserialization {protocolId}.{messageName}[ID={messageId}] packet error ", inner)
        {
            MessageId = messageId;
            MessageName = messageName;
        }
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

        public GnssPublishMessageException(string protocolId, string messageId, string messageName, Exception inner) : base(protocolId, $"Publication {protocolId}.{messageName}[ID={messageId}] packet throw exception ", inner)
        {
            MessageId = messageId;
            MessageName = messageName;
        }
    }
}