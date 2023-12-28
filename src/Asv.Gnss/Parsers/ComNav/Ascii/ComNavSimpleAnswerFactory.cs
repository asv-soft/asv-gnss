using System;
using System.Collections.Generic;

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

        public static ComNavSimpleAnswerParser RegisterDefaultMessages(this ComNavSimpleAnswerParser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }

            return src;
        }
    }
}