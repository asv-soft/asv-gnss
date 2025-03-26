using System;
using System.Collections.Generic;

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

        public static SbfBinaryParser RegisterDefaultMessages(this SbfBinaryParser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }

            return src;
        }
    }
}
