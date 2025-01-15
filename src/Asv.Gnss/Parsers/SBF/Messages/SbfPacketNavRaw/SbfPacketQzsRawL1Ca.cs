namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 300 bits of a QZSS C/A subframe.
    ///
    /// NAVBits contains the 300 bits of a QZSS C/A subframe.
    /// Encoding: Same as GPSRawCA block
    /// </summary>
    public class SbfPacketQzsRawL1Ca : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4066;
        public override ushort MessageRevision => 0;
        public override string Name => "RawL1Ca";

        protected override int NavBitsU32Length => 10;
    }
}
