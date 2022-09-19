using System;

namespace Asv.Gnss
{
    public class RtcmV2Message21 : RtcmV2MessageBase
    {
        public const int RtcmMessageId = 21;

        public override ushort MessageId => RtcmMessageId;
        public override string Name => "RTK/High Accuracy Pseudorange Corrections (Fixed*)";

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
        {

        }
    }
}