using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    /// <summary>
    /// Factory class for creating RTCMv3 messages.
    /// </summary>
    public static class RtcmV3Factory
    {
        /// <summary>
        /// Gets the default messages.
        /// </summary>
        /// <returns>An enumerable of functions that create instances of default messages.</returns>
        public static IEnumerable<Func<RtcmV3MessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new RtcmV3Message1004();
                yield return () => new RtcmV3Message1005();
                yield return () => new RtcmV3Message1006();
                yield return () => new RtcmV3Message1007();
                yield return () => new RtcmV3Message1008();
                yield return () => new RtcmV3Message1012();
                yield return () => new RtcmV3Message1013();
                yield return () => new RtcmV3Message1019();
                yield return () => new RtcmV3Message1020();
                yield return () => new RtcmV3Message1029();
                yield return () => new RtcmV3Message1033();
                yield return () => new RtcmV3Msm4Msg1074();
                yield return () => new RtcmV3Msm7Msg1077();
                yield return () => new RtcmV3Msm4Msg1084();
                yield return () => new RtcmV3Msm7Msg1087();
                yield return () => new RtcmV3Msm4Msg1094();
                yield return () => new RtcmV3Msm7Msg1097();
                yield return () => new RtcmV3Msm4Msg1104();
                yield return () => new RtcmV3Msm7Msg1107();
                yield return () => new RtcmV3Msm4Msg1114();
                yield return () => new RtcmV3Msm7Msg1117();
                yield return () => new RtcmV3Msm4Msg1124();
                yield return () => new RtcmV3Msm7Msg1127();
            }
        }

        /// <summary>
        /// Registers default messages into the RtcmV3Parser instance.
        /// </summary>
        /// <param name="src">The RtcmV3Parser instance to register the default messages into.</param>
        /// <returns>
        /// The RtcmV3Parser instance with the default messages registered.
        /// </returns>
        public static RtcmV3Parser RegisterDefaultMessages(this RtcmV3Parser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }

            return src;
        }
    }
}
