using Asv.Gnss;
using Asv.IO;
using System;

namespace Asv.Gnss
{
    public class RtcmV3Message1007 : RtcmV3Message1007and1008
    {
        public const int RtcmMessageId = 1007;
        public override ushort MessageId => RtcmMessageId;
        public override string Name => "Antenna Descriptor";


    }
}