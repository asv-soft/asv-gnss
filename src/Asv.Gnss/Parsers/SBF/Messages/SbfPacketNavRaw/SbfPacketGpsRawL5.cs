namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 300 bits of a GPS L5 CNAV subframe (the so-called Dc(t) data stream).
    ///
    /// NAVBits contains the 300 bits of a GPS CNAV subframe.
    /// Encoding: NAVBits contains all the bits of the frame, including the
    /// preamble. The first received bit is stored as the MSB of NAVBits[0].
    /// The unused bits in NAVBits[9] must be ignored by the decoding
    /// software.
    /// </summary>
    public class SbfPacketGpsRawL5 : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4019;
        public override ushort MessageRevision => 0;
        public override string Name => "GpsRawL5";

        protected override int NavBitsU32Length => 10;
    }
}
