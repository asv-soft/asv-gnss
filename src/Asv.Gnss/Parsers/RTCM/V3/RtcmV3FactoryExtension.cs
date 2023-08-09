using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    public static class RtcmV3FactoryExtension
    {
        public static IEnumerable<Func<RtcmV3MessageBase>> ExtendedMessages
        {
            get
            {
                yield return () => new RtcmV3Message1007();
                yield return () => new RtcmV3Message1008();
                yield return () => new RtcmV3Message1032();
                yield return () => new RtcmV3Message1033();

                yield return () => new RtcmV3Message1030();
                yield return () => new RtcmV3Message1031();
                yield return () => new RtcmV3Message1230();

                yield return () => new RtcmV3Msm3Msg1023();
                yield return () => new RtcmV3Msm3Msg1073();
                yield return () => new RtcmV3Msm3Msg1083();
                yield return () => new RtcmV3Msm3Msg1093();
                yield return () => new RtcmV3Msm3Msg1123();

                //yield return () => new RtcmV3Msm4Msg1025();
                //yield return () => new RtcmV3Msm4Msg1075();
                //yield return () => new RtcmV3Msm4Msg1085();
                //yield return () => new RtcmV3Msm4Msg1095();
                //yield return () => new RtcmV3Msm4Msg1125();

                //yield return () => new RtcmV3Msm6Msg1076();
                //yield return () => new RtcmV3Msm6Msg1086();
                //yield return () => new RtcmV3Msm6Msg1096();
                //yield return () => new RtcmV3Msm6Msg1116();
                //yield return () => new RtcmV3Msm6Msg1126();

                //yield return () => new RtcmV3MsmMsg1042();
                //yield return () => new RtcmV3MsmMsg1046();
            //    yield return () => new RtcmV3MsmMsg1023();

            }
        }
        public static RtcmV3Parser RegisterExtendedMessages(this RtcmV3Parser src)
        {
            foreach (var func in ExtendedMessages)
            {
                src.Register(func);
            }
            return src;
        }
    }
}
