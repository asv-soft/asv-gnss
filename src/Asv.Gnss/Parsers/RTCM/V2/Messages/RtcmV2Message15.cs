using System;
using Asv.IO;

namespace Asv.Gnss
{
    // Represents an RTCM version 2 message type 15: Ionospheric Delay Message (Fixed).
    // /
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

    /// <summary>
    /// Represents an ionospheric delay item for a specific navigation system and satellite.
    /// </summary>
    public class IonosphericDelayItem
    {
        /// <summary>
        /// Deserializes the given buffer and populates the necessary properties. </summary> <param name="buffer">The byte buffer from which to deserialize data.</param> <param name="bitIndex">The bit index used to track the current bit being read.</param>
        /// /
        public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
        {
            var sys = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            NavigationSystem =
                sys == 0 ? NavigationSystemEnum.SYS_GPS : NavigationSystemEnum.SYS_GLO;
            Prn = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            if (Prn == 0)
            {
                Prn = 32;
            }

            IonosphericDelay = SpanBitHelper.GetBitU(buffer, ref bitIndex, 14) * 0.001;
            var rateOfChange = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14);

            if (rateOfChange == -8192)
            {
                IonosphericDelay = double.NaN;
                IonoRateOfChange = double.NaN;
            }
            else
            {
                IonoRateOfChange = rateOfChange * 0.05;
            }
        }

        /// <summary>
        /// Gets or sets the navigation system for the application.
        /// </summary>
        /// <value>
        /// The navigation system for the application.
        /// </value>
        public NavigationSystemEnum NavigationSystem { get; set; }

        /// <summary>
        /// Gets or sets the Prn property.
        /// </summary>
        public byte Prn { get; set; }

        /// <summary>
        /// Gets or sets the ionospheric delay in centimeters.
        /// </summary>
        public double IonosphericDelay { get; set; }

        /// <summary>
        /// Gets or sets the rate of change of the ionosphere measuring unit in cm/min.
        /// </summary>
        public double IonoRateOfChange { get; set; }
    }
}
