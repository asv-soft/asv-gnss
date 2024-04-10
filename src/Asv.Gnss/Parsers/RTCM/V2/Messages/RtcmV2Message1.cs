using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a message of type 1 in the RTCMv2 protocol.
    /// This message contains Differential GPS Corrections (Fixed).
    /// </summary>
    public class RtcmV2Message1 : RtcmV2MessageBase
    {
        /// <summary>
        /// Represents the ID of an RTCM message.
        /// </summary>
        public const int RtcmMessageId = 1;

        /// <summary>
        /// Gets the message ID of the message.
        /// </summary>
        /// <value>
        /// The message ID.
        /// </value>
        /// <remarks>
        /// The MessageId property returns the message ID as a <see cref="ushort"/> value.
        /// </remarks>
        public override ushort MessageId => RtcmMessageId;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <returns>The name of the property.</returns>
        public override string Name => "Differential GPS Corrections (Fixed)";

        /// <summary>
        /// Gets or sets the observation items.
        /// </summary>
        /// <value>
        /// The observation items.
        /// </value>
        public DObservationItem[] ObservationItems { get; set; }

        /// <summary>
        /// Deserializes the content from the given buffer using the provided parameters.
        /// </summary>
        /// <param name="buffer">The buffer containing the serialized data.</param>
        /// <param name="bitIndex">The starting bit index in the buffer.</param>
        /// <param name="payloadLength">The length of the payload in bytes.</param>
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
        {
            var itmCnt = payloadLength / 5;
            ObservationItems = new DObservationItem[itmCnt];

            for (var i = 0; i < itmCnt; i++)
            {
                var item = new DObservationItem(NavigationSystemEnum.SYS_GPS);
                item.Deserialize(buffer,ref bitIndex);
                ObservationItems[i] = item;
            }
        }
    }

    public enum SatUdreEnum
    {
        /// <summary>
        /// One-sigma differential error less or equal than 1 met
        /// </summary>
        LessOne = 0,

        /// <summary>
        /// One-sigma differential error from 1 to 4 met
        /// </summary>
        BetweenOneAndFour = 1,

        /// <summary>
        /// One-sigma differential error from 4 to 8 met
        /// </summary>
        BetweenFourAndEight = 2,

        /// <summary>
        /// One-sigma differential error more than 8 met
        /// </summary>
        MoreEight = 3
    }
}