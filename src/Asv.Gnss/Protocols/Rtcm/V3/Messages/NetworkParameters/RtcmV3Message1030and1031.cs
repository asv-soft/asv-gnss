using Asv.IO;
using System;

namespace Asv.Gnss
{
    public abstract class RtcmV3Message1030and1031 : RtcmV3LegacyMessageBase
    {
        public RtcmV3NetworkResidual[] SatelliteResiduals { get; set; } = Array.Empty<RtcmV3NetworkResidual>();

        private RtcmV3NetworkResidual _firstResidual = new();
        private RtcmV3NetworkResidual FirstResidual => SatelliteResiduals.Length == 0 ? _firstResidual : SatelliteResiduals[0];

        protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
        {
            var count = GetResidualCount();
            SpanBitHelper.SetBitU(buffer, ref bitIndex, ResidualEpochBitLen, ResidualsEpoch);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 12, ReferenceStationID);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 7, NRefs);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 5, count);
            for (var i = 0; i < count; i++)
            {
                var residual = SatelliteResiduals.Length == 0 ? _firstResidual : SatelliteResiduals[i];
                SpanBitHelper.SetBitU(buffer, ref bitIndex, 6, residual.SatelliteId);
                RtcmV3Serialization.WriteU(buffer, ref bitIndex, 8, residual.SOc, 0.5);
                RtcmV3Serialization.WriteU(buffer, ref bitIndex, 9, residual.SOd, 0.01);
                RtcmV3Serialization.WriteU(buffer, ref bitIndex, 6, residual.SOh, 0.1);
                RtcmV3Serialization.WriteU(buffer, ref bitIndex, 10, residual.SIc, 0.5);
                RtcmV3Serialization.WriteU(buffer, ref bitIndex, 10, residual.SId, 0.01);
            }
        }

        private int GetResidualCount()
        {
            var count = SatelliteResiduals.Length == 0 ? NumberSatelliteSignals : SatelliteResiduals.Length;
            if (count > 31 || (SatelliteResiduals.Length == 0 && count > 1))
                throw new InvalidOperationException("Satellite residual count does not match the supplied records.");
            return count;
        }

        protected override int InternalGetBitSize() => ResidualEpochBitLen + 24 + 49 * GetResidualCount();

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            ResidualsEpoch = SpanBitHelper.GetBitU(buffer, ref bitIndex, ResidualEpochBitLen);
            ReferenceStationID = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            NRefs = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            NumberSatelliteSignals = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            SatelliteResiduals = new RtcmV3NetworkResidual[NumberSatelliteSignals];
            _firstResidual = new RtcmV3NetworkResidual();
            for (var i = 0; i < SatelliteResiduals.Length; i++)
            {
                SatelliteResiduals[i] = new RtcmV3NetworkResidual
                {
                    SatelliteId = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6),
                    SOc = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8) * 0.5,
                    SOd = SpanBitHelper.GetBitU(buffer, ref bitIndex, 9) * 0.01,
                    SOh = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6) * 0.1,
                    SIc = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10) * 0.5,
                    SId = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10) * 0.01,
                };
            }
        }
        protected abstract int ResidualEpochBitLen { get; }

        /// <summary>
        /// GPS Residuals Epoch Time(TOW) - 0 – 604800 s
        /// GLONASS Residuals Epoch Time(tk) - 0 – 86400 s
        /// </summary>
        public uint ResidualsEpoch { get; set; }

        /// <summary>
        /// Number of reference stations used to derive residual statistics (1 to
        /// 127, use 127 for 127 or more stations). The number of reference
        /// stations should never be zero.If zero is encountered the rover should
        /// ignore the message.
        /// </summary>
        public byte NRefs { get; set; }

        /// <summary>
        /// The Number of Satellite Signals Processed refers to the
        /// number of satellites in the message.It does not necessarily equal the
        /// number of satellites visible to the Reference Station.
        /// </summary>
        public byte NumberSatelliteSignals { get; set; }

        /// <summary>
        /// The Reference Station ID is determined by the service provider. Its 
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
        /// May be the ID of a physical or non-physical station
        /// </summary>
        public uint ReferenceStationID { get; set; }

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
        public byte SatelliteId { get => FirstResidual.SatelliteId; set => FirstResidual.SatelliteId = value; }

        /// <summary>
        /// Constant term of standard deviation (1 sigma)
        /// for non-dispersive interpolation residuals, mm.
        /// </summary>
        public double SOc => FirstResidual.SOc;
        /// <summary>
        /// Distance dependent term of standard deviation(1 sigma)
        /// for non- dispersive interpolation residuals, ppm.
        /// </summary>
        public double SOd => FirstResidual.SOd;

        /// <summary>
        /// Height dependent term of standard deviation (1 sigma) for nondispersive interpolation residuals, ppm.
        /// The complete standard deviation for the expected non-dispersive
        /// interpolation residual is computed from DF218,DF219 and DF220
        /// using the formula.
        /// </summary>
        public double SOh => FirstResidual.SOh;

        /// <summary>
        ///  Constant term of standard deviation (1 sigma)
        ///  for dispersive interpolation residuals (as affecting GPS L1 frequency) 
        /// </summary>
        public double SIc => FirstResidual.SIc;

        /// <summary>
        /// Distance dependent term of standard deviation (1 sigma) for dispersive
        /// interpolation residuals. (as affecting GPS L1 frequency)
        /// The complete standard deviation for the expected dispersive
        /// interpolation residual is computed from DF221 and DF222 using the
        /// formula.
        /// </summary>
        public double SId => FirstResidual.SId;

    }
    public class RtcmV3NetworkResidual
    {
        public byte SatelliteId { get; set; }
        public double SOc { get; set; }
        public double SOd { get; set; }
        public double SOh { get; set; }
        public double SIc { get; set; }
        public double SId { get; set; }
    }
}
