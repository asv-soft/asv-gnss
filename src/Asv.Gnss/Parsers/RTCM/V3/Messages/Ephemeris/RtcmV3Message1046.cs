using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Proprietary Message Type.
    /// Trimble Navigation Ltd.
    /// </summary>
    public class RtcmV3Message1046 : RtcmV3MessageBase
    {
        /// <summary>
        /// Rtcm Message Id
        /// </summary>
        public const int RtcmMessageId = 1046;
        /// <inheritdoc/>
        public override ushort MessageId => RtcmMessageId;
        
        /// <inheritdoc/>
        public override string Name => "Galileo I/NAV ephemeris information";

        /// <inheritdoc/>
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
        }
    }
}
