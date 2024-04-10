using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a RTCM Version 2 Message Type 14: GPS Time of Week (Fixed)
    /// </summary>
    public class RtcmV2Message14 : RtcmV2MessageBase
    {
        /// <summary>
        /// Represents the RTCM message ID.
        /// </summary>
        public const int RtcmMessageId = 14;

        /// <summary>
        /// Adjusts the GPS week value based on the current GPS time and leap seconds.
        /// </summary>
        /// <param name="week">The GPS week value.</param>
        /// <param name="leap">The leap seconds to adjust the time by.</param>
        /// <returns>The adjusted GPS week value.</returns>
        private static uint AdjustGpsWeek(uint week, uint leap)
        {
            var nowGps = DateTime.UtcNow.AddSeconds(leap);
            var w = 0;
            double second = 0;

            RtcmV3Helper.GetFromTime(nowGps, ref w, ref second);
            if (w < 1560) w = 1560; /* use 2009/12/1 if time is earlier than 2009/12/1 */
            return (uint) (week + (w - week + 1) / 1024 * 1024);
        }

        /// <summary>
        /// The identifier of the message.
        /// </summary>
        /// <remarks>
        /// This property returns the unique identifier of the message.
        /// </remarks>
        /// <value>
        /// The identifier of the message.
        /// </value>
        public override ushort MessageId => RtcmMessageId;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public override string Name => "GPS Time of Week (Fixed)";

        

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
        {
            var week = SpanBitHelper.GetBitU(buffer,ref bitIndex, 10);
            var hour = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            var leap = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);

            week = AdjustGpsWeek(week, leap);
            GpsTime = RtcmV3Helper.GetFromGps((int)week, hour * 3600.0 + ZCount);
        }
    }
}
