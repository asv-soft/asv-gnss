using System;

namespace Asv.Gnss
{
    public class RtcmV2Message31 : RtcmV2MessageBase
    {
        protected override DateTime Adjhour(double zcnt)
        {
            var time = DateTime.UtcNow;
            double tow = 0;
            var week = 0;

            RtcmV3Helper.GetFromTime(time, ref week, ref tow);

            var hour = Math.Floor(tow / 3600.0);
            var sec = tow - hour * 3600.0;
            if (zcnt < sec - 1800.0) zcnt += 3600.0;
            else if (zcnt > sec + 1800.0) zcnt -= 3600.0;

            return RtcmV3Helper.Utc2Gps(RtcmV3Helper.GetFromGps(week, hour * 3600 + zcnt));
        }

        public const int RtcmMessageId = 31;

        public override ushort MessageId => RtcmMessageId;
        public override string Name => "Differential GLONASS Corrections (Tentative)";

        public DObservationItem[] ObservationItems { get; set; }

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
        {
            var itmCnt = payloadLength / 5;
            ObservationItems = new DObservationItem[itmCnt];

            for (var i = 0; i < itmCnt; i++)
            {
                var item = new DObservationItem(NavigationSystemEnum.SYS_GLO);
                item.Deserialize(buffer,ref bitIndex);
                ObservationItems[i] = item;
            }
        }
    }
}