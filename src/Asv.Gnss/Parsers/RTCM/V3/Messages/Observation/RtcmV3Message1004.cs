using System;
using Asv.IO;

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
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            Satellites = new GPSSatellite[SatelliteCount];

            for (var i = 0; i < SatelliteCount; i++)
            {
                var satellite = new GPSSatellite();
                satellite.Deserialize(buffer,ref bitIndex);
                Satellites[i] = satellite;
            }
        }

        public override ushort MessageId => 1004;
        public override string Name => "Extended L1&L2 GPS RTK Observables";

        public GPSSatellite[] Satellites { get; set; }

    }

    public class GPSSatellite 
    {
        private static readonly int[] L2Codes = { RtcmV3Helper.CODE_L2X, RtcmV3Helper.CODE_L2P, RtcmV3Helper.CODE_L2D, RtcmV3Helper.CODE_L2W };

        private static readonly double[] Freq = { RtcmV3Helper.FREQ1, RtcmV3Helper.FREQ2 };


        public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
        {
            var sys = NavigationSystemEnum.SYS_GPS;
            var prn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);

            var code1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            double pr1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 24);
            var ppr1 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 20);
            var lock1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            var amb = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            double cnr1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);

            var code2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            var pr21 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14);
            var ppr2 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 20);
            var lock2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            double cnr2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);

            if (prn >= 40)
            {
                sys = NavigationSystemEnum.SYS_SBS;
                prn += 80;
            }

            var sat = RtcmV3Helper.satno(sys, (int)prn);

            if (sat == 0)
            {
                throw new Exception($"Rtcm3 1004 satellite number error: prn={prn}");
            }

            SatelliteNumber = sat;
            SatelliteCode = RtcmV3Helper.Sat2Code(SatelliteNumber, (int)prn);

            pr1 = pr1 * 0.02 + amb * RtcmV3Helper.PRUNIT_GPS;
            L1PseudoRange = pr1;

            
            if (ppr1 != -524288) // (0xFFF80000)
            {
                L1CarrierPhase = ppr1 * 0.0005 * Freq[0] / RtcmV3Helper.CLIGHT;
            }
            else
            {
                L1CarrierPhase = double.NaN;
            }

            L1LockTime = (byte)lock1;
            L1CNR = RtcmV3Helper.snratio(cnr1 * 0.25);
            L1Code = code1 != 0 ? RtcmV3Helper.CODE_L1P : RtcmV3Helper.CODE_L1C;

            if (pr21 != -8192) // 0xFFFFE000
            {
                L2PseudoRange = pr1 + pr21 * 0.02;
            }

            if (ppr2 != -524288) // 0xFFF80000
            {
                L2CarrierPhase = ppr2 * 0.0005 * Freq[1] / RtcmV3Helper.CLIGHT;
            }
            else
            {
                L2CarrierPhase = double.NaN;
            }

            L2LockTime = (byte)lock2;
            L2CNR = RtcmV3Helper.snratio(cnr2 * 0.25);
            L2Code = (byte)L2Codes[code2];
        }

        /// <summary>
        /// A GPS Satellite ID number from 1 to 32 refers to the PRN code of the
        /// GPS satellite. Satellite ID’s higher than 32 are reserved for satellite
        /// signals from Satellite-Based Augmentation Systems (SBAS’s) such as
        /// the FAA’s Wide-Area Augmentation System (WAAS). SBAS PRN
        /// codes cover the range 120-138. The Satellite ID’s reserved for SBAS
        /// satellites are 40-58, so that the SBAS PRN codes are derived from the
        /// Version 3 Satellite ID codes by adding 80.
        /// </summary>
        public int SatelliteNumber { get; set; }

        public string SatelliteCode { get; set; }

        /// <summary>
        /// The GPS L1 Code Indicator identifies the code being tracked by the
        /// reference station. Civil receivers can track the C/A code, and
        /// optionally the P code, while military receivers can track C/A, and can
        /// also track P and Y code, whichever is broadcast by the satellite
        /// 0 - C/A Code
        /// 1 - P(Y) Code Direct
        /// </summary>
        public byte L1Code { get; set; }

        /// <summary>
        /// The GPS L1 Pseudorange field provides the raw L1 pseudorange
        /// measurement at the reference station in meters, modulo one lightmillisecond (299,792.458 meters). The GPS L1 pseudorange
        /// measurement is reconstructed by the user receiver from the L1
        /// pseudorange field by:
        /// (GPS L1 pseudorange measurement) = (GPS L1 pseudorange field)
        /// modulo (299,792.458 m) + integer as determined from the user
        /// receiver's estimate of the reference station range, or as provided by the
        /// extended data set.
        /// 80000h - invalid L1 pseudorange; used only in the calculation of L2
        /// measurements.
        /// </summary>
        public double L1PseudoRange { get; set; }

        /// <summary>
        /// The GPS L1 PhaseRange – L1 Pseudorange field provides the
        /// information necessary to determine the L1 phase measurement. Note
        /// that the PhaseRange defined here has the same sign as the
        /// pseudorange. The PhaseRange has much higher resolution than the
        /// pseudorange, so that providing this field is just a numerical technique
        /// to reduce the length of the message. At start up and after each cycle
        /// slip, the initial ambiguity is reset and chosen so that the L1
        /// PhaseRange should match the L1 Pseudorange as closely as possible
        /// (i.e., within 1/2 L1 cycle) while not destroying the integer nature of the
        /// original carrier phase observation.
        /// The Full GPS L1 PhaseRange is constructed as follows (all quantities
        /// in units of meters):
        /// (Full L1 PhaseRange) = (L1 pseudorange as reconstructed from L1
        /// pseudorange field) + (GPS L1 PhaseRange – L1 Pseudorange field)
        /// Certain ionospheric conditions might cause the GPS L1 PhaseRange –
        /// L1 Pseudorange to diverge over time across the range limits defined.
        /// Under these circumstances the computed value needs to be adjusted
        /// (rolled over) by the equivalent of 1500 cycles in order to bring the
        /// value back within the range.
        /// See also comments in sections 3.1.6 and 3.5.1 for correction of antenna
        /// phase center variations in Network RTK applications.
        /// 80000h - L1 phase is invalid; used only in the calculation of L2
        /// measurements. 
        /// </summary>
        public double L1PhaseRange { get; set; }


        public double L1CarrierPhase { get; set; }

        /// <summary>
        /// The GPS L1 Lock Time Indicator provides a measure of the amount of
        /// time that has elapsed during which the Reference Station receiver has
        /// maintained continuous lock on that satellite signal. If a cycle slip
        /// occurs during the previous measurement cycle, the lock indicator will
        /// be reset to zero. 
        /// </summary>
        public bool L1LossOfLockIndicator { get; set; }
        
        public byte L1LockTime { get; set; }


        /// <summary>
        /// The GPS L1 CNR measurements provide the reference station's
        /// estimate of the carrier-to-noise ratio of the satellite’s signal in dB-Hz.
        /// 0 - the CNR measurement is not computed. 
        /// </summary>
        public ushort L1CNR { get; set; }

        /// <summary>
        /// The GPS L2 Code Indicator depicts which L2 code is processed by the
        /// reference station, and how it is processed.
        /// 0 - C/A or L2C code
        /// 1 - P(Y) code direct
        /// 2 - P(Y) code cross-correlated
        /// 3 - Correlated P/Y
        /// The GPS L2 Code Indicator refers to the method used by the GPS
        /// reference station receiver to recover the L2 pseudorange. The GPS L2
        /// Code Indicator should be set to “0” (C/A or L2C code) for any of the
        /// L2 civil codes. It is assumed here that a satellite will not transmit both
        /// C/A code and L2C code signals on L2 simultaneously, so that the
        /// reference station and user receivers will always utilize the same signal.
        /// The code indicator should be set to “1” if the satellite’s signal is
        /// correlated directly, i.e., either P code or Y code depending whether
        /// anti-spoofing (AS) is switched off or on. The code indicator should be
        /// set to “2” when the reference station receiver L2 pseudorange
        /// measurement is derived by adding a cross-correlated pseudorange
        /// measurement (Y2-Y1) to the measured L1 C/A code. The code
        /// indicator should be set to 3 when the GPS reference station receiver is
        /// using a proprietary method that uses only the L2 P(Y) code signal to
        /// derive L2 pseudorange. 
        /// </summary>
        public byte L2Code { get; set; }

        /// <summary>
        /// The GPS L2-L1 Pseudorange Difference field is utilized, rather than
        /// the full L2 pseudorange, in order to reduce the message length. The
        /// receiver must reconstruct the L2 code phase pseudorange by using the
        /// following formula:
        /// (GPS L2 pseudorange measurement) =
        /// (GPS L1 pseudorange as reconstructed from L1 pseudorange field) +
        /// (GPS L2-L1 pseudorange field)
        /// 2000h (-163.84m) - no valid L2 code available, or that the value
        /// exceeds the allowed range. 
        /// </summary>
        public double L2PseudoRange { get; set; }

        /// <summary>
        /// The GPS L2 PhaseRange - L1 Pseudorange field provides the
        /// information necessary to determine the L2 phase measurement. Note
        /// that the PhaseRange defined here has the same sign as the
        /// pseudorange. The PhaseRange has much higher resolution than the
        /// pseudorange, so that providing this field is just a numerical technique
        /// to reduce the length of the message. At start up and after each cycle
        /// slip, the initial ambiguity is reset and chosen so that the L2
        /// PhaseRange should match the L1 Pseudorange as closely as possible
        /// (i.e., within 1/2 L2 cycle) while not destroying the integer nature of the
        /// original carrier phase observation.
        /// The Full GPS L2 PhaseRange is constructed as follows (all quantities
        /// in units of meters):
        ///  (Full L2 PhaseRange) = (L1 pseudorange as reconstructed from L1
        /// pseudorange field) + (GPS L2 PhaseRange – L1 Pseudorange field)
        /// Certain ionospheric conditions might cause the GPS L2 PhaseRange –
        /// L1 Pseudorange to diverge over time across the range limits defined.
        /// Under these circumstances the computed value needs to be adjusted
        /// (rolled over) by the equivalent of 1500 cycles in order to bring the
        /// value back within the range. Note: A bit pattern equivalent to 80000h
        /// in this field indicates an invalid carrier phase measurement that should
        /// not be processed by the mobile receiver. This indication may be used
        /// at low signal levels where carrier tracking is temporarily lost, but code
        /// tracking is still possible.
        /// See also comments in sections 3.1.6 and 3.5.1 for correction of antenna
        /// phase center variations in Network RTK applications.
        /// </summary>
        public double L2PhaseRange { get; set; }

        public double L2CarrierPhase { get; set; }

        /// <summary>
        /// The GPS L2 Lock Time Indicator provides a measure of the amount of
        /// time that has elapsed during which the Reference Station receiver has
        /// maintained continuous lock on that satellite signal. If a cycle slip
        /// occurs during the previous measurement cycle, the lock indicator will
        /// be reset to zero. 
        /// </summary>
        public bool L2LossOfLockIndicator { get; set; }

        public byte L2LockTime { get; set; }

        /// <summary>
        /// The GPS L2 CNR measurements provide the reference station's
        /// estimate of the carrier-to-noise ratio of the satellite’s signal in dB-Hz.
        /// 0 - the CNR measurement is not computed 
        /// </summary>
        public ushort L2CNR { get; set; }
    }
}