namespace Asv.Gnss
{
    /// <summary>
    /// <para>This block contains the 300 bits of a BeiDou navigation page, as received from the B1I, B2I or B3I signal.</para>
    /// <para>
    /// AVBits contains the 300 deinterleaved bits of a BeiDou navigation
    /// subframe.
    /// Encoding: NAVBits contains all the bits of the subframe, including
    /// the preamble and the parity bits. The first received bit is stored as the
    /// MSB of NAVBits[0]. The 20 unused bits in NAVBits[9] must be
    /// ignored by the decoding software. The bits are deinterleaved.
    /// </para>
    /// </summary>
    public class SbfPacketBdsRaw : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4047;
        public override ushort MessageRevision => 0;
        public override string Name => "BdsRaw";

        protected override int NavBitsU32Length => 10;
    }
}
