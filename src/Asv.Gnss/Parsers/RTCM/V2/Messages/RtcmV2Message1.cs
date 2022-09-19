using System;

namespace Asv.Gnss
{
    public class RtcmV2Message1 : RtcmV2MessageBase
    {
        public const int RtcmMessageId = 1;

        public override ushort MessageId => RtcmMessageId;
        public override string Name => "Differential GPS Corrections (Fixed)";

        public DObservationItem[] ObservationItems { get; set; }

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