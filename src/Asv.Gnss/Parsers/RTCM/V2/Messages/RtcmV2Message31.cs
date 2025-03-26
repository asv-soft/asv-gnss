using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a RTCM V2 Message Type 31: Differential GLONASS Corrections (Tentative).
    /// </summary>
    public class RtcmV2Message31 : RtcmV2MessageBase
    {
        /// <summary>
        /// Adjusts the hour component of the current UTC time based on the given zcnt value.
        /// </summary>
        /// <param name="zcnt">The zcnt value used to adjust the hour component of the current UTC time.</param>
        /// <returns>The adjusted hour component of the current GPS time based on the zcnt value.</returns>
        protected override DateTime Adjhour(double zcnt)
        {
            var time = DateTime.UtcNow;
            double tow = 0;
            var week = 0;

            RtcmV3Helper.GetFromTime(time, ref week, ref tow);

            var hour = Math.Floor(tow / 3600.0);
            var sec = tow - (hour * 3600.0);
            if (zcnt < sec - 1800.0)
            {
                zcnt += 3600.0;
            }
            else if (zcnt > sec + 1800.0)
            {
                zcnt -= 3600.0;
            }

            return RtcmV3Helper.Utc2Gps(RtcmV3Helper.GetFromGps(week, (hour * 3600) + zcnt));
        }

        /// <summary>
        /// The identifier of the RTCM message.
        /// </summary>
        public const int RtcmMessageId = 31;

        /// <summary>
        /// Gets the unique identifier for the message.
        /// </summary>
        /// <value>
        /// The identifier for the message.
        /// </value>
        public override ushort MessageId => RtcmMessageId;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public override string Name => "Differential GLONASS Corrections (Tentative)";

        /// <summary>
        /// Gets or sets the collection of observation items.
        /// </summary>
        /// <value>
        /// An array of DObservationItem containing the observation items.
        /// </value>
        public DObservationItem[] ObservationItems { get; set; }

        /// <summary>
        /// Deserializes the content of the buffer into the ObservationItems array. </summary> <param name="buffer">The buffer containing the serialized data.</param> <param name="bitIndex">The current bit index within the buffer.</param> <param name="payloadLength">The length of the payload within the buffer.</param>
        /// /
        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            byte payloadLength
        )
        {
            var itmCnt = payloadLength / 5;
            ObservationItems = new DObservationItem[itmCnt];

            for (var i = 0; i < itmCnt; i++)
            {
                var item = new DObservationItem(NavigationSystemEnum.SYS_GLO);
                item.Deserialize(buffer, ref bitIndex);
                ObservationItems[i] = item;
            }
        }
    }
}
