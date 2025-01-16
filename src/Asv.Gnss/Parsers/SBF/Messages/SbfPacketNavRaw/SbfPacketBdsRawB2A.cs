namespace Asv.Gnss
{
    /// <summary>
    /// <para>
    /// This block contains the 576 symbols of a BeiDou B-CNAV2 navigation frame, as received
    /// from the B2a signal.
    /// The receiver attempts to correct bit errors using the LDPC parity bits, but unrecoverable errors are still possible at low C/N0. It is therefore always needed to check the CRC
    /// status before using the navigation bits.
    /// </para>
    /// <para>
    /// NAVBits contains the 576 symbols of a BeiDou B2a (B-CNAV2) navigation frame.
    /// Encoding: NAVBits contains all the symbols of the frame, excluding the preamble. The first received symbol (i.e. the MSB of the PRN field)
    /// is stored as the MSB of NAVBits[0].
    /// </para>
    /// </summary>
    public class SbfPacketBdsRawB2A : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4219;
        public override ushort MessageRevision => 0;
        public override string Name => "BdsRawB2A";

        protected override int NavBitsU32Length => 18;
    }
}
