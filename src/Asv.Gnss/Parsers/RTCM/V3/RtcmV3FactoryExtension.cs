using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    /// <summary>
    /// Add messages that are not in the default set.
    /// </summary>
    public static class RtcmV3FactoryExtension
    {
        /// <summary>
        /// Gets messages that are not in the default set.
        /// </summary>
        public static IEnumerable<Func<RtcmV3MessageBase>> ExtendedMessages
        {
            get
            {
                yield return () => new RtcmV3Message1030();
                yield return () => new RtcmV3Message1031();
                yield return () => new RtcmV3Message1032();

                yield return () => new RtcmV3Message1230();

                yield return () => new RtcmV3Msm3Msg1073();
                yield return () => new RtcmV3Msm3Msg1083();
                yield return () => new RtcmV3Msm3Msg1093();
                yield return () => new RtcmV3Msm3Msg1123();

                yield return () => new RtcmV3Msm5Msg1075();
                yield return () => new RtcmV3Msm5Msg1085();
                yield return () => new RtcmV3Msm5Msg1095();
                yield return () => new RtcmV3Msm5Msg1125();

                yield return () => new RtcmV3Msg4094();

                // yield return () => new RtcmV3Msm6Msg1076();
                // yield return () => new RtcmV3Msm6Msg1086();
                // yield return () => new RtcmV3Msm6Msg1096();
                // yield return () => new RtcmV3Msm6Msg1116();
                // yield return () => new RtcmV3Msm6Msg1126();
                yield return () => new RtcmV3Message1042();
                yield return () => new RtcmV3Message1046();

                // yield return () => new RtcmV3MsmMsg1023();
            }
        }

        /// <summary>
        /// Registers extended messages.
        /// </summary>
        /// <returns>A parser for RTCMv3 messages.</returns>
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
