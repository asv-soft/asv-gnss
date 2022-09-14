using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV2Message14 : RtcmV2MessageBase
    {
        public const int RtcmMessageId = 14;
        
        private static uint AdjustGpsWeek(uint week, uint leap)
        {
            var nowGps = DateTime.UtcNow.AddSeconds(leap);
            var w = 0;
            double second = 0;

            RtcmV3Helper.GetFromTime(nowGps, ref w, ref second);
            if (w < 1560) w = 1560; /* use 2009/12/1 if time is earlier than 2009/12/1 */
            return (uint) (week + (w - week + 1) / 1024 * 1024);
        }

        public override ushort MessageId => RtcmMessageId;
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
