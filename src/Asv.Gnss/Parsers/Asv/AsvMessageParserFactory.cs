using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    public static class AsvMessageParserFactory
    {
        public static IEnumerable<Func<AsvMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new AsvMessageHeartBeat();
                yield return () => new AsvMessageGbasVdbSend();
                yield return () => new AsvMessageGbasVdbSendV2();
                yield return () => new AsvMessageGbasCuSendV2();
            }
        }

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