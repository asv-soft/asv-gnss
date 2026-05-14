namespace Asv.Gnss.Parsers.RTCM.V3.Messages.NetworkParameters
{
    internal class SatelliteRtkResidual
    {
        private const double mm05Res = 0.5;
        private const double ppm001Res = 0.01;
        private const double ppm01Res = 0.1;
        internal byte SOcDf { get; set; }
        internal ushort SOdDf { get; set; }
        internal byte SOhDf { get; set; }
        internal ushort SIcDf { get; set; }
        internal ushort SIdDfl { get; set; }
        /// <summary>
        /// GPS:
        /// Satellite ID number from 1 to 32 refers to the PRN code of the
        /// GPS satellite.Satellite ID’s higher than 32 are reserved for satellite
        /// signals from Satellite-Based Augmentation Systems (SBAS’s) such as
        /// the FAA’s Wide-Area Augmentation System(WAAS). SBAS PRN
        /// codes cover the range 120-138. The Satellite ID’s reserved for SBAS
        /// satellites are 40-58, so that the SBAS PRN codes are derived from the
        /// Version 3 Satellite ID codes by adding 80. 
        /// GLONASS:
        /// Satellite ID number from 1 to 24 refers to the slot
        /// number of the GLONASS satellite.A Satellite ID of zero indicates
        /// that the slot number is unknown.Satellite ID’s higher than 32 are
        /// reserved for satellite signals from Satellite-Based Augmentation
        /// Systems(SBAS’s). SBAS PRN codes cover the range 120-138. The
        /// Satellite ID’s reserved for SBAS satellites are 40-58, so that the SBAS
        /// PRN codes are derived from the Version 3 GLONASS Satellite ID
        /// codes by adding 80.
        /// 0 – The slot number is unknown
        /// 1 to 24 – Slot number of the GLONASS satellite
        /// >32 – Reserved for Satellite-Based Augmentation Systems(SBAS).
        /// Note: For GLONASS-M satellites this data field has to contain the
        /// GLONASS-M word “n”, thus the Satellite Slot Number is always
        /// known(cannot be equal to zero) for GLONASS-M satellites.
        /// </summary>
        public byte SatelliteId { get; set; }

        /// <summary>
        /// Constant term of standard deviation (1 sigma)
        /// for non-dispersive interpolation residuals, mm.
        /// </summary>
        public double SOc => SOcDf * mm05Res;
        /// <summary>
        /// Distance dependent term of standard deviation(1 sigma)
        /// for non- dispersive interpolation residuals, ppm.
        /// </summary>
        public double SOd => SOdDf * ppm001Res;

        /// <summary>
        /// Height dependent term of standard deviation (1 sigma) for nondispersive interpolation residuals, ppm.
        /// The complete standard deviation for the expected non-dispersive
        /// interpolation residual is computed from DF218,DF219 and DF220
        /// using the formula.
        /// </summary>
        public double SOh => SOhDf * ppm01Res;

        /// <summary>
        ///  Constant term of standard deviation (1 sigma)
        ///  for dispersive interpolation residuals (as affecting GPS L1 frequency) 
        /// </summary>
        public double SIc => SOhDf * mm05Res;

        /// <summary>
        /// Distance dependent term of standard deviation (1 sigma) for dispersive
        /// interpolation residuals. (as affecting GPS L1 frequency)
        /// The complete standard deviation for the expected dispersive
        /// interpolation residual is computed from DF221 and DF222 using the
        /// formula.
        /// </summary>
        public double SId => SIdDfl * ppm001Res;
    }
}
