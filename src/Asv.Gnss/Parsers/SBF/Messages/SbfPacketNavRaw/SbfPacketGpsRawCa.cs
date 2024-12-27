using System;

namespace Asv.Gnss
{
    /// <summary>
    /// This block contains the 300 bits of a GPS C/A subframe. It is generated each time a new
    /// subframe is received, i.e.every 6 seconds.
    ///
    /// NAVBits contains the 300 bits of a GPS C/A subframe.
    /// Encoding: For easier parsing, the bits are stored as a succession of
    /// 10 32-bit words. Since the actual words in the subframe are 30-bit long,
    /// two unused bits are inserted in each 32-bit word. More specifically, each
    /// 32-bit word has the following format:
    /// Bits 0-5: 6 parity bits (referred to as D25 to D30 in the GPS ICD), XOR-ed
    /// with the last transmitted bit of the previous word (D∗ 30)).
    /// Bits 6-29: sourceName data bits (referred to as dn in the GPS ICD). The first
    /// received bit is the MSB.
    /// Bits 30-31: Reserved
    /// </summary>
    public class SbfPacketGpsRawCa : SbfPacketGnssRawNavMsgBase
    {
        public override ushort MessageType => 4017;
        public override ushort MessageRevision => 0;
        public override string Name => "GpsRawCa";

        protected override int NavBitsU32Length => 10;

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            base.DeserializeContent(ref buffer);
            GpsSubFrame = GpsSubFrameFactory.Create(NAVBitsU32);
        }

        public GpsSubframeBase GpsSubFrame { get; set; }
    }
}
