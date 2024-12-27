using System;
using System.Diagnostics;

namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 85 bits of a GLONASS L1CA or L2CA navigation string
    ///
    /// NAVBits contains the first 85 bits of a GLONASS C/A string (i.e. all bits of
    /// the string with the exception of the time mark).
    /// Encoding: The first received bit is stored as the MSB of NAVBits[0]. The
    /// unused bits in NAVBits[2] must be ignored by the decoding software.
    /// </summary>
    public class SbfPacketGloRawCa : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4026;
        public override ushort MessageRevision => 0;
        public override string Name => "GloRawCa";

        protected override int NavBitsU32Length => 3;

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            base.DeserializeContent(ref buffer);
            GlonassWord = GlonassWordFactory.Create(NAVBitsU32);
            if (GlonassWord == null)
            {
                Debug.Fail("Null reference");
            }
        }

        public GlonassWordBase GlonassWord { get; set; }
    }
}
