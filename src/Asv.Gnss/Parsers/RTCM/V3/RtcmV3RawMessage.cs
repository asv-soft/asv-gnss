using System;

namespace Asv.Gnss
{
    public class RtcmV3RawMessage : GnssRawMessage<ushort>
    {
        public override string ProtocolId => RtcmV3Parser.GnssProtocolId;
        public RtcmV3RawMessage(ushort messageId, ReadOnlySpan<byte> data) : base(messageId, data)
        {
            
        }
    }
}