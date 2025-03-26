namespace Asv.Gnss
{
    /// <summary>
    /// <para>This block contains the 300 bits of a GPS L2C CNAV subframe (the so-called Dc(t) data stream).</para>
    /// <para>
    /// NAVBits contains the 300 bits of a GPS CNAV subframe.
    /// Encoding: NAVBits contains all the bits of the frame, including the
    /// preamble. The first received bit is stored as the MSB of NAVBits[0].
    /// The unused bits in NAVBits[9] must be ignored by the decoding
    /// software.
    /// </para>
    /// </summary>
    public class SbfPacketGpsRawL2C : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4018;
        public override ushort MessageRevision => 0;
        public override string Name => "GpsRawL2C";

        protected override int NavBitsU32Length => 10;
    }
}
