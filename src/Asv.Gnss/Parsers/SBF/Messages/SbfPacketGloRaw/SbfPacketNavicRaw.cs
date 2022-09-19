namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 292 bits of a NavIC/IRNSS subframe.
    ///
    /// NavBits contains the 292 bits of a NavIC/IRNSS subframe.
    /// Encoding: NAVBits contains all the bits of the frame, with the exception of the preamble. The first received bit is stored as the MSB of
    /// NAVBits[0]. The unused bits in NAVBits[9] must be ignored by the
    /// decoding software.
    /// </summary>
    public class SbfPacketNavicRaw : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4093;
        public override ushort MessageRevision => 0;
        public override string Name => "NavicRaw";

        protected override int NavBitsU32Length => 10;

    }

}