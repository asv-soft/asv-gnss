namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 234 bits of a Galileo I/NAV navigation page, after deinterleaving and Viterbi decoding.
    /// 
    /// NAVBits contains the 234 bits of an I/NAV navigation page (in nominal
    /// or alert mode). Note that the I/NAV page is transmitted as two sub-pages
    /// (the so-called even and odd pages) of duration 1 second each (120 bits
    /// each). In this block, the even and odd pages are concatenated, even page
    /// first and odd page last. The 6 tails bits at the end of the even page are
    /// removed (hence a total of 234 bits). If the even and odd pages have been
    /// received from two different carriers (E5b and L1), bit 5 of the SourceName
    /// field is set.
    /// Encoding: NAVBits contains all the bits of the frame, with the exception of the synchronization field. The first received bit is stored as the
    /// MSB of NAVBits[0]. The unused bits in NAVBits[7] must be ignored
    /// by the decoding software.
    /// </summary>
    public class SbfPacketGalRawInav : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4023;
        public override ushort MessageRevision => 0;
        public override string Name => "GalRawInav";

        protected override int NavBitsU32Length => 8;

        
    }
}