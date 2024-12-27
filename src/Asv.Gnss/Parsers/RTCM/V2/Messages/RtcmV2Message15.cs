using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents an RTCM version 2 message type 15: Ionospheric Delay Message (Fixed).
    /// </summary>
    public class RtcmV2Message15 : RtcmV2MessageBase
    {
        /// <summary>
        /// Represents the ID of an Rtcm message.
        /// </summary>
        public const int RtcmMessageId = 15;

        /// <summary>
        /// Gets the message ID of the RTCM message.
        /// </summary>
        /// <remarks>
        /// The message ID represents the identification number of the RTCM message.
        /// </remarks>
        /// <value>
        /// The message ID.
        /// </value>
        public override ushort MessageId => RtcmMessageId;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <returns>The name of the property.</returns>
        public override string Name => "Ionospheric Delay Message (Fixed)";

        /// <summary>
        /// Gets or sets an array of IonosphericDelayItem that represents the delays.
        /// </summary>
        /// <value>
        /// An array of IonosphericDelayItem representing the delays.
        /// </value>
        public IonosphericDelayItem[] Delays { get; set; }

        /// <summary>
        /// Deserialize the content of a buffer into an array of IonosphericDelayItem objects.
        /// </summary>
        /// <param name="buffer">The buffer containing the serialized data.</param>
        /// <param name="bitIndex">The bit index in the buffer where the deserialization should start.</param>
        /// <param name="payloadLength">The length of the payload in bytes.</param>
        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            byte payloadLength
        )
        {
            var itemCnt = (payloadLength * 8) / 36;
            Delays = new IonosphericDelayItem[itemCnt];

            for (var i = 0; i < itemCnt; i++)
            {
                Delays[i] = new IonosphericDelayItem();
                Delays[i].Deserialize(buffer, ref bitIndex);
            }
        }
    }
}
