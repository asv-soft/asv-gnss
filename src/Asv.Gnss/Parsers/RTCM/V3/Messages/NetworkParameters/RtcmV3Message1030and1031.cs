using System;
using Asv.IO;
using static System.Collections.Specialized.BitVector32;

namespace Asv.Gnss
{
    public abstract class RtcmV3Message1030and1031 : RtcmV3MessageBase
    {
        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        )
        {
            ResidualsEpoch = SpanBitHelper.GetBitU(buffer, ref bitIndex, ResidualEpochBitLen);
            ReferenceStationID = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            NRefs = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            NumberSatelliteSignals = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            SatelliteId = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            _sOcDf = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            _sOdDf = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 9);
            _sOhDf = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            _sIcDf = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
            _sIdDf = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
        }

        protected abstract int ResidualEpochBitLen { get; }

        private const double Mm05Res = 0.5;
        private const double Ppm001Res = 0.01;
        private const double Ppm01Res = 0.1;
        private byte _sOcDf; // 0 - 127 mm
        private ushort _sOdDf; // 0 - 5.11 ppm
        private byte _sOhDf; // 0 - 5.11 ppm
        private ushort _sIcDf; // 0 - 511 mm
        private ushort _sIdDf; // 0 - 10.23 ppm

        /// <summary>
        /// Gets or sets gPS Residuals Epoch Time(TOW) - 0 – 604800 s
        /// GLONASS Residuals Epoch Time(tk) - 0 – 86400 s.
        /// </summary>
        public uint ResidualsEpoch { get; set; }

        /// <summary>
        /// Gets or sets number of reference stations used to derive residual statistics (1 to
        /// 127, use 127 for 127 or more stations). The number of reference
        /// stations should never be zero.If zero is encountered the rover should
        /// ignore the message.
        /// </summary>
        public byte NRefs { get; set; }

        /// <summary>
        /// Gets or sets the Number of Satellite Signals Processed refers to the
        /// number of satellites in the message.It does not necessarily equal the
        /// number of satellites visible to the Reference Station.
        /// </summary>
        public byte NumberSatelliteSignals { get; set; }

        /// <summary>
        /// Gets or sets the Reference Station ID is determined by the service provider. Its
        /// primary purpose is to link all message data to their unique sourceName. It is
        /// useful in distinguishing between desired and undesired data in cases
        /// where more than one service may be using the same data link
        /// frequency. It is also useful in accommodating multiple reference
        /// stations within a single data link transmission.
        /// In reference network applications the Reference Station ID plays an
        /// important role, because it is the link between the observation messages
        /// of a specific reference station and its auxiliary information contained in
        /// other messages for proper operation. Thus the Service Provider should
        /// ensure that the Reference Station ID is unique within the whole
        /// network, and that ID’s should be reassigned only when absolutely
        /// necessary.
        /// Service Providers may need to coordinate their Reference Station ID
        /// assignments with other Service Providers in their region in order to
        /// avoid conflicts. This may be especially critical for equipment
        /// accessing multiple services, depending on their services and means of
        /// information distribution.
        /// May be the ID of a physical or non-physical station.
        /// </summary>
        public uint ReferenceStationID { get; set; }

        /// <summary>
        /// Gets or sets gPS:
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
        /// Gets constant term of standard deviation (1 sigma)
        /// for non-dispersive interpolation residuals, mm.
        /// </summary>
        public double SOc => _sOcDf * Mm05Res;

        /// <summary>
        /// Gets distance dependent term of standard deviation(1 sigma)
        /// for non- dispersive interpolation residuals, ppm.
        /// </summary>
        public double SOd => _sOdDf * Ppm001Res;

        /// <summary>
        /// Gets height dependent term of standard deviation (1 sigma) for nondispersive interpolation residuals, ppm.
        /// The complete standard deviation for the expected non-dispersive
        /// interpolation residual is computed from DF218,DF219 and DF220
        /// using the formula.
        /// </summary>
        public double SOh => _sOhDf * Ppm01Res;

        /// <summary>
        ///  Gets constant term of standard deviation (1 sigma)
        ///  for dispersive interpolation residuals (as affecting GPS L1 frequency).
        /// </summary>
        public double SIc => _sIcDf * Mm05Res;

        /// <summary>
        /// Gets distance dependent term of standard deviation (1 sigma) for dispersive
        /// interpolation residuals. (as affecting GPS L1 frequency)
        /// The complete standard deviation for the expected dispersive
        /// interpolation residual is computed from DF221 and DF222 using the
        /// formula.
        /// </summary>
        public double SId => _sIdDf * Ppm001Res;
    }
}
