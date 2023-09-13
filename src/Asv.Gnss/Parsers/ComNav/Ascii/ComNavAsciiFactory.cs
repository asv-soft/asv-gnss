using System;
using System.Collections.Generic;

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

        public static ComNavAsciiParser RegisterDefaultMessages(this ComNavAsciiParser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }
            return src;
        }
    }
}