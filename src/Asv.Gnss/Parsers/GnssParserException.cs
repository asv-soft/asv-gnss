using System;

namespace Asv.Gnss
{
    [Serializable]
    public class GnssParserException : Exception
    {
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

    [Serializable]
    public class GnssCrcErrorException : GnssParserException
    {
        public GnssCrcErrorException(string protocolId) : base(protocolId, $"Crc error occurred when recv '{protocolId}' message")
        {

        }
    }

    [Serializable]
    public class GnssReadNotAllDataWhenDeserializePacketErrorException : GnssParserException
    {
        public string MessageId { get; }

        public GnssReadNotAllDataWhenDeserializePacketErrorException(string protocolId, string messageId) : base(protocolId, $"Read not all data when deserialize '{protocolId}.{messageId}' message")
        {
            MessageId = messageId;
        }
    }

    [Serializable]
    public class GnssUnknownMessageException : GnssParserException
    {
        public string MessageId { get; }

        public GnssUnknownMessageException(string protocolId, string messageId) : base(protocolId, $"Unknown {protocolId} packet message number [MSG={messageId}]")
        {
            MessageId = messageId;
        }
    }

    [Serializable]
    public class GnssDeserializeMessageException : GnssParserException
    {
        public string MessageId { get; }
        public string MessageName { get; }

        public GnssDeserializeMessageException(string protocolId, string messageId, string messageName, Exception inner) : base(protocolId, $"Deserialization {protocolId}.{messageName}[ID={messageId}] packet error ", inner)
        {
            MessageId = messageId;
            MessageName = messageName;
        }
    }

    [Serializable]
    public class GnssPublishMessageException : GnssParserException
    {
        public string MessageId { get; }
        public string MessageName { get; }

        public GnssPublishMessageException(string protocolId, string messageId, string messageName, Exception inner) : base(protocolId, $"Publication {protocolId}.{messageName}[ID={messageId}] packet throw exception ", inner)
        {
            MessageId = messageId;
            MessageName = messageName;
        }
    }
}