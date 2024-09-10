namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 300 bits of a QZSS L5 CNAV subframe.
    ///
    /// NAVBits contains the 300 bits of a QZSS CNAV subframe.
    /// Encoding: NAVBits contains all the bits of the frame, including the
    /// preamble. The first received bit is stored as the MSB of NAVBits[0].
    /// The unused bits in NAVBits[9] must be ignored by the decoding
    /// software.
    /// </summary>
    public class SbfPacketQzsRawL5 : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4068;
        public override ushort MessageRevision => 0;
        public override string Name => "QzsRawL5";

        protected override int NavBitsU32Length => 10;

        
    }
}