using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss
{
    public static class ComNavAsciiFactory
    {
        public static IEnumerable<Func<ComNavAsciiMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new ComNavLogListAMessage();
                yield return () => new ComNavComConfigAMessage();
            }
        }

        public static IProtocolMessageFactoryBuilder<ComNavAsciiMessageBase, ComNavAsciiMessageId> RegisterDefaultMessages(this IProtocolMessageFactoryBuilder<ComNavAsciiMessageBase, ComNavAsciiMessageId> src)
        {
            src
                .Add<ComNavLogListAMessage>()
                .Add<ComNavComConfigAMessage>();
            return src;
        }
    }
}
