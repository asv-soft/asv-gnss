using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a factory for creating RTCM V2 messages.
    /// </summary>
    public static class RtcmV2Factory
    {
        /// <summary>
        /// A property that returns a collection of default messages.
        /// </summary>
        /// <remarks>
        /// The DefaultMessages property is a static property that returns an IEnumerable
        /// of Func objects, which are delegates to create instances of different message types.
        /// These message types are derived from the RtcmV2MessageBase class.
        /// </remarks>
        /// <returns>
        /// An IEnumerable of Func<RtcmV2MessageBase> objects that can be used to create instances
        /// of default messages.
        /// </returns>
        public static IEnumerable<Func<RtcmV2MessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new RtcmV2Message1();
                yield return () => new RtcmV2Message14();
                yield return () => new RtcmV2Message15();
                yield return () => new RtcmV2Message17();
                yield return () => new RtcmV2Message21();
                yield return () => new RtcmV2Message31();
            }
        }

        /// <summary>
        /// Registers the default messages in the RtcmV2Parser.
        /// </summary>
        /// <param name="src">The RtcmV2Parser instance.</param>
        /// <returns>The updated RtcmV2Parser instance after registering the default messages.</returns>
        public static RtcmV2Parser RegisterDefaultMessages(this RtcmV2Parser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }
            return src;
        }
    }
}
