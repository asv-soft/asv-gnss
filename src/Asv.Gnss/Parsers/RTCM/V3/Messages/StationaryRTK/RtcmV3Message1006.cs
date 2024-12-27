using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV3Message1006 : RtcmV3Message1005and1006
    {
        public const int RtcmMessageId = 1006;
        public override ushort MessageId => RtcmMessageId;
        public override string Name => "Stationary RTK Reference Station ARP with Antenna Height";

        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        )
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            Height = SpanBitHelper.GetBitU(buffer, ref bitIndex, 16) * 0.0001;
        }
    }
}
