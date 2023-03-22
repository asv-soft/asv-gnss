using System;

namespace Asv.Gnss
{
    public abstract class GnssRawMessage<TMsgId>
    {
        public abstract string ProtocolId { get; }
        public TMsgId MessageId { get; }
        public byte[] RawData { get; private set; }

        protected GnssRawMessage(TMsgId messageId,ReadOnlySpan<byte> data)
        {
            MessageId = messageId;
            RawData = data.ToArray();
        }
    }
}