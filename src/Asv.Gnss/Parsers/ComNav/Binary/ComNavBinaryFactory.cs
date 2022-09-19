using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    public static class ComNavBinaryFactory
    {
        public static IEnumerable<Func<ComNavBinaryMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new ComNavBinaryPsrPosPacket();
            }
        }

        public static ComNavBinaryParser RegisterDefaultMessages(this ComNavBinaryParser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }
            return src;
        }
    }
}