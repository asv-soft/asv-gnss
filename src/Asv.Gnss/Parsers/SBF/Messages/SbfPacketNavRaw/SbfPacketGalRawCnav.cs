namespace Asv.Gnss
{
    /// <summary>
    /// <para>This block contains the 492 bits of a Galileo C/NAV navigation page, after deinterleaving and Viterbi decoding.</para>
    /// <para>
    /// NAVBits contains the 492 bits of a Galileo C/NAV page.
    /// Encoding: NAVBits contains all the bits of the frame, with the exception of the synchronization field. The first received bit is stored as
    /// the MSB of NAVBits[0]. The unused bits in NAVBits[15] must be
    /// ignored by the decoding software.
    /// </para>
    /// </summary>
    public class SbfPacketGalRawCnav : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4024;
        public override ushort MessageRevision => 0;
        public override string Name => "RawCnav";

        protected override int NavBitsU32Length => 16;
    }
}
