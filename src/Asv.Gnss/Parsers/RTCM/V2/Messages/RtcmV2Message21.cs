using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents an RTCM Version 2 Message 21 - RTK/High Accuracy Pseudorange Corrections (Fixed*).
    /// </summary>
    public class RtcmV2Message21 : RtcmV2MessageBase
    {
        /// <summary>
        /// Represents the ID of an RTCM message.
        /// </summary>
        public const int RtcmMessageId = 21;

        /// <summary>
        /// Gets the MessageId of the RTCM message.
        /// </summary>
        /// <remarks>
        /// This property returns the MessageId as an unsigned short value.
        /// </remarks>
        /// <returns>The MessageId of the RTCM message.</returns>
        public override ushort MessageId => RtcmMessageId;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <returns>The name of the property.</returns>
        public override string Name => "RTK/High Accuracy Pseudorange Corrections (Fixed*)";

        /// <summary>
        /// Deserializes the content from a byte span.
        /// </summary>
        /// <param name="buffer">The byte span containing the data to deserialize.</param>
        /// <param name="bitIndex">The current bit index in the buffer.</param>
        /// <param name="payloadLength">The length of the payload.</param>
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
        {

        }
    }
}