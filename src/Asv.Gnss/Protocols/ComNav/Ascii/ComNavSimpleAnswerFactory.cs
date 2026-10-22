using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss
{
    public static class ComNavSimpleAnswerFactory
    {
        public static IEnumerable<Func<ComNavSimpleAnswerMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new ComNavSimpleOkMessage();
                yield return () => new ComNavSimpleErrorMessage();
                yield return () => new ComNavSimpleTransmitMessage();
            }
        }

        public static IProtocolMessageFactoryBuilder<ComNavSimpleAnswerMessageBase, ComNavAsciiMessageId> RegisterDefaultMessages(this IProtocolMessageFactoryBuilder<ComNavSimpleAnswerMessageBase, ComNavAsciiMessageId> src)
        {
            src
                .Add<ComNavSimpleOkMessage>()
                .Add<ComNavSimpleErrorMessage>()
                .Add<ComNavSimpleTransmitMessage>();
            return src;
        }
    }
}
