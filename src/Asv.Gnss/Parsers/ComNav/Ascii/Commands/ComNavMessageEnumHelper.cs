using System.Linq;

namespace Asv.Gnss
{
    public static class ComNavMessageEnumHelper
    {
        private static readonly ComNavMessageEnum[] RtcmV2Messages =
        {
            ComNavMessageEnum.RTCM1,
            ComNavMessageEnum.RTCM3,
            ComNavMessageEnum.RTCM9,
            ComNavMessageEnum.RTCM1819,
            ComNavMessageEnum.RTCM31,
            ComNavMessageEnum.RTCM41,
            ComNavMessageEnum.RTCM42,
        };

        public static bool IsRtcmV2LogCommand(this ComNavMessageEnum src)
        {
            return RtcmV2Messages.Contains(src);
        }
    }
}
