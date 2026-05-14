using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss
{
    public static class ComNavBinaryFactory
    {
        public static IEnumerable<Func<ComNavBinaryMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new ComNavBinaryPsrPosPacket();
                yield return () => new ComNavBinaryPsrDopPacket();
                yield return () => new ComNavBinaryRawGpsSubFramePacket();
                yield return () => new ComNavBinaryRawGloEphemPacket();
                yield return () => new ComNavBinaryObservationInfo();
            }
        }

         /// <summary>
         /// Registers the default messages in the specified ComNav binary parser.
         /// </summary>
         /// <param name="src">The ComNav binary parser where the default messages will be registered.</param>
         /// <returns>The ComNav binary parser with the default messages registered.</returns>
         public static IProtocolMessageFactoryBuilder<ComNavBinaryMessageBase, ushort> RegisterDefaultMessages(this IProtocolMessageFactoryBuilder<ComNavBinaryMessageBase, ushort> src)
         {
             src
                 .Add<ComNavBinaryPsrPosPacket>()
                 .Add<ComNavBinaryPsrDopPacket>()
                 .Add<ComNavBinaryRawGpsSubFramePacket>()
                 .Add<ComNavBinaryRawGloEphemPacket>()
                 .Add<ComNavBinaryObservationInfo>();
             return src;
         }
     }
 }
