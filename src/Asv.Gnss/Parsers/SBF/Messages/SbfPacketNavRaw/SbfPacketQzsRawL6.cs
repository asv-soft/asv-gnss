namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 2000 bits of a QZSS L6 message.
    /// The receiver attempts to correct bit errors using the Reed-Solomon parity symbols.
    /// The block contains the corrected bits and there is no need to have a Reed-Solomon decoder
    /// at the user side. The Parity field indicates whether the error recovery was successful or
    /// not.
    ///
    /// NAVBits contains the 2000 bits of a QZSS L6 message.
    /// Encoding: NAVBits contains all the bits of the message after ReedSolomon decoding, including the preamble and the Reed-Solomon parity
    /// symbols themselves. The first received bit is stored as the MSB of
    /// NAVBits[0]. The unused bits in NAVBits[63] must be ignored by the
    /// decoding software.
    /// </summary>
    public class SbfPacketQzsRawL6 : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4069;
        public override ushort MessageRevision => 0;
        public override string Name => "QzsRawL6";

        protected override int NavBitsU32Length => 63;
    }
}
