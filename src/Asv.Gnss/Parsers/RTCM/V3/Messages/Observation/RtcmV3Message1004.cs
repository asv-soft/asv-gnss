using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Table 3.5-5 Contents of the Satellite-Specific Portion of a Type 1004 Message, Each Satellite – GPS Extended RTK, L1 & L2
    ///
    /// The Type 1004 Message supports dual-frequency RTK operation, and includes an indication of the satellite carrier-to-noise (CNR) as
    /// measured by the reference station.Since the CNR does not usually change from measurement to measurement, this message type can
    /// be mixed with the Type 1003, and used only when a satellite CNR changes, thus saving broadcast link throughput.
    /// </summary>
    public class RtcmV3Message1004 : RtcmV3RTKObservableMessagesBase
    {
        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        )
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            Satellites = new GPSSatellite[SatelliteCount];

            for (var i = 0; i < SatelliteCount; i++)
            {
                var satellite = new GPSSatellite();
                satellite.Deserialize(buffer, ref bitIndex);
                Satellites[i] = satellite;
            }
        }

        public override ushort MessageId => 1004;
        public override string Name => "Extended L1&L2 GPS RTK Observables";

        public GPSSatellite[] Satellites { get; set; }
    }
}
