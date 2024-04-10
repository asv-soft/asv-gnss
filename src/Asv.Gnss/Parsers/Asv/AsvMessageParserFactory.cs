using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    /// The AsvMessageParserFactory class provides a factory for creating instances of AsvMessageParsers and registering default message types.
    public static class AsvMessageParserFactory
    {
        /// <summary>
        /// DefaultMessages is a property that provides a collection of default messages.
        /// The property returns an IEnumerable of Func<AsvMessageBase>.
        /// Each Func<AsvMessageBase> object represents a function that creates and returns an instance of an AsvMessageBase derived class.
        /// </summary>
        public static IEnumerable<Func<AsvMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new AsvMessageHeartBeat();
                yield return () => new AsvMessageGbasVdbSend();
                yield return () => new AsvMessageGbasVdbSendV2();
                yield return () => new AsvMessageGbasCuSendV2();
                yield return () => new AsvMessageGpsObservations();
                yield return () => new AsvMessageGloObservations();
                yield return () => new AsvMessageGpsRawCa();
                yield return () => new AsvMessageGloRawCa();
                yield return () => new AsvMessagePvtGeo();
            }
        }

        /// <summary>
        /// Registers the default messages into the given AsvMessageParser object.
        /// </summary>
        /// <param name="src">The AsvMessageParser object to register the default messages into.</param>
        /// <returns>The updated AsvMessageParser object with the default messages registered.</returns>
        public static AsvMessageParser RegisterDefaultMessages(this AsvMessageParser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }
            return src;
        }
    }
}