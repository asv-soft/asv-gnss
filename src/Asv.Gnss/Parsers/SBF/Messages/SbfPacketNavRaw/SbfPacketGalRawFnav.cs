namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 244 bits of a Galileo F/NAV navigation page, after deinterleaving and Viterbi decoding.
    ///
    /// NavBits contains the 244 bits of a Galileo F/NAV page.
    /// Encoding: NAVBits contains all the bits of the frame, with the exception of the synchronization field. The first received bit is stored as the
    /// MSB of NAVBits[0]. The unused bits in NAVBits[7] must be ignored
    /// by the decoding software.
    /// </summary>
    public class SbfPacketGalRawFnav : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4022;
        public override ushort MessageRevision => 0;
        public override string Name => "RawFnav";

        protected override int NavBitsU32Length => 8;

        
    }
}