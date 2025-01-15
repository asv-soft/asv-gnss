using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Proprietary Message Type.
    /// Trimble Navigation Ltd.
    /// </summary>
    public class RtcmV3Msg4094 : RtcmV3MessageBase
    {
        /// <summary>
        /// Rtcm Message Id
        /// </summary>
        public const int RtcmMessageId = 4094;

        /// <inheritdoc/>
        public override ushort MessageId => RtcmMessageId;

        /// <inheritdoc/>
        public override string Name => "Trimble Navigation Ltd.";

        /// <inheritdoc/>
        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        ) { }
    }
}
