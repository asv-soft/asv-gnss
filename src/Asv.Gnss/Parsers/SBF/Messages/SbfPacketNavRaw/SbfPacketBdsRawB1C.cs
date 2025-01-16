namespace Asv.Gnss
{
    /// <summary>
    /// <para>
    /// This block contains the 1800 symbols of a BeiDou B-CNAV1 navigation frame (itself containing three subframes), as received from the B1C signal.
    /// The symbols are deinterleaved. The receiver attempts to correct bit errors using the
    /// LDPC parity bits, but unrecoverable errors are still possible at low C/N0. It is therefore
    /// always needed to check the CRC status before using the navigation bits. A separate CRC
    /// check is provided for subframe 2 and 3.
    /// </para>
    /// <para>
    /// NAVBits contains the 1800 deinterleaved symbols of a BeiDou B1C
    /// (B-CNAV1) navigation frame.
    /// Encoding: NAVBits contains all the symbols of the frame. The first
    /// received symbol (i.e. the first symbol of subframe 1) is stored as the MSB
    /// of NAVBits[0]. The 24 unused bits in NAVBits[56] must be ignored by
    /// the decoding software.
    /// </para>
    /// </summary>
    public class SbfPacketBdsRawB1C : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4218;
        public override ushort MessageRevision => 0;
        public override string Name => "BdsRawB1C";

        protected override int NavBitsU32Length => 57;
    }
}
