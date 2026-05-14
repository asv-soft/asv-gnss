using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss
{
    public static class SbfMessageFactory
    {
        public static IEnumerable<Func<SbfMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new SbfPacketGpsRawCa();
                yield return () => new SbfPacketGloRawCa();
                yield return () => new SbfPacketMeasEpochRev1(); // TODO: not complete
                yield return () => new SbfPacketPvtGeodeticRev2();
                yield return () => new SbfPacketDOP();
                yield return () => new SbfPacketReceiverStatusRev1();
                yield return () => new SbfPacketQualityInd();
                yield return () => new SbfPacketGpsNav();
            }
        }

        public static IProtocolMessageFactoryBuilder<SbfMessageBase, ushort> RegisterDefaultMessages(this IProtocolMessageFactoryBuilder<SbfMessageBase, ushort> src)
        {
            src
                .Add<SbfPacketGpsRawCa>()
                .Add<SbfPacketGloRawCa>()
                .Add<SbfPacketMeasEpochRev1>()
                .Add<SbfPacketPvtGeodeticRev2>()
                .Add<SbfPacketDOP>()
                .Add<SbfPacketReceiverStatusRev1>()
                .Add<SbfPacketQualityInd>()
                .Add<SbfPacketGpsNav>();
            return src;
        }
    }
}
