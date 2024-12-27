namespace Asv.Gnss
{
    public class Signal
    {
        public string RinexCode { get; set; }

        /// <summary>
        /// Gets or sets observation data PseudoRange (m).
        /// </summary>
        public double PseudoRange { get; set; }

        /// <summary>
        /// Gets or sets observation data carrier-phase (m).
        /// </summary>
        public double CarrierPhase { get; set; }

        /// <summary>
        /// Gets or sets observation data PhaseRangeRate (hz).
        /// </summary>
        public double PhaseRangeRate { get; set; }

        /// <summary>
        /// Gets or sets signal strength (0.001 dBHz).
        /// </summary>
        public double Cnr { get; set; }

        /// <summary>
        /// Gets or sets lock time.
        /// </summary>
        public ushort LockTime { get; set; }

        /// <summary>
        /// Gets or sets min lock time in min.
        /// </summary>
        public double MinLockTime { get; set; }

        public byte HalfCycle { get; set; }

        public byte ObservationCode { get; set; }
    }
}
