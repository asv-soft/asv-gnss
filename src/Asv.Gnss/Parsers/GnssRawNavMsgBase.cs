using System;

namespace Asv.Gnss
{
    public class GnssRawNavMsgBase
    {
        public DateTime UtcTime { get; set; }

        public NavSysEnum NavSystem { get; set; }

        public uint[] RawData { get; set; }

        /// <summary>
        /// Satellite ID
        /// </summary>
        public int SatId { get; set; }

        public int SatPrn { get; set; }

        /// <summary>
        /// RINEX satellite code
        /// </summary>
        public string RinexSatCode { get; set; }
        public GnssSignalTypeEnum SignalType { get; set; }

        public string RindexSignalCode { get; set; }

        public double CarrierFreq { get; set; }
    }

    public class GpsRawCa : GnssRawNavMsgBase
    {
        public GpsSubframeBase GpsSubFrame { get; set; }
    }

    public class GloRawCa : GnssRawNavMsgBase
    {
        public GlonassWordBase GlonassWord { get; set; }
    }

    public enum NavSysEnum
    {
        Unknown,
        GPS,
        SBAS,
        GLONASS,
        Galileo,
        QZSS,
        BeiDou,
        IRNS,
        LEO,
        MSS,
        NavIC,
    }

    public enum GnssSignalTypeEnum
    {
        Unknown,
        Reserved,
        L1CA,
        L1P,
        L2P,
        L2C,
        L5,
        L1C,
        L2CA,
        L3,
        B1C,
        B1A,
        B1Q,
        B2a,
        E1_L1A,
        E1_L1BC,
        E6_E6A,
        E6_E6BC,
        E5a,
        E5b,
        LBand,
        E5_AltBoc,
        L6,
        B1I,
        B2I,
        B2Q,
        B3I,
        B3Q,
        B3A,
        L1S,
        B2b,
        B2_AltBoc,
        S,
    }
}
