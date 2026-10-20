using Asv.IO;
using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Table 3.5-14 Contents of the Satellite-Specific Portion of a Type 1012 Message, Each Satellite – GLONASS Extended RTK, L1 & L2
    ///
    /// The Type 1012 Message supports dual-frequency RTK operation, and includes an indication of the satellite carrier-to-noise (CNR) as
    /// measured by the reference station.Since the CNR does not usually change from measurement to measurement, this message type can
    /// be mixed with the Type 1011, and used only when a satellite CNR changes, thus saving broadcast link throughput.
    /// </summary>
    public class RtcmV3Message1012 : RtcmV3RTKObservableMessagesBase
    {
        protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
        {
            if (Satellites.Length > 31) throw new ArgumentOutOfRangeException(nameof(Satellites));
            SatelliteCount = (byte)Satellites.Length;
            base.InternalSerialize(buffer, ref bitIndex);
            foreach (var satellite in Satellites) satellite.Serialize(buffer, ref bitIndex);
        }

        protected override int InternalGetBitSize() => base.InternalGetBitSize() + Satellites.Length * 130;

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            Satellites = new GLONASSSatellite[SatelliteCount];

            for (var i = 0; i < SatelliteCount; i++)
            {
                var satellite = new GLONASSSatellite();
                satellite.Deserialize(buffer, ref bitIndex);
                Satellites[i] = satellite;
            }
        }

        public override ushort MessageId => 1012;
        public override string Name => "Extended L1&L2 GLONASS RTK Observables";

        public GLONASSSatellite[] Satellites { get; set; } = Array.Empty<GLONASSSatellite>();
    }

    public class GLONASSSatellite
    {
        public void Serialize(Span<byte> buffer, ref int bitIndex)
        {
            RtcmV3Serialization.WriteU(buffer, ref bitIndex, 6, SatellitePrn, 1);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, L1Code == RtcmV3Helper.CODE_L1P ? 1 : 0);
            RtcmV3Serialization.WriteU(buffer, ref bitIndex, 5, (L1Frequency - RtcmV3Protocol.FREQ1_GLO) / RtcmV3Protocol.DFRQ1_GLO + 7, 1);
            var ambiguity = (uint)Math.Floor(L1PseudoRange / RtcmV3Helper.PRUNIT_GLO);
            RtcmV3Serialization.WriteU(buffer, ref bitIndex, 25, L1PseudoRange - ambiguity * RtcmV3Helper.PRUNIT_GLO, 0.02);
            SpanBitHelper.SetBitS(buffer, ref bitIndex, 20, double.IsFinite(L1CarrierPhase)
                ? (int)Math.Round(L1CarrierPhase * RtcmV3Helper.CLIGHT / L1Frequency / 0.0005) : -524288);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 7, L1LockTime);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 7, ambiguity);
            RtcmV3Serialization.WriteU(buffer, ref bitIndex, 8, L1CNR * RtcmV3Protocol.SNR_UNIT, 0.25);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 2, L2Code == RtcmV3Helper.CODE_L2P ? 1 : 0);
            SpanBitHelper.SetBitS(buffer, ref bitIndex, 14, double.IsFinite(L2PseudoRange) && L2PseudoRange != 0
                ? (int)Math.Round((L2PseudoRange - L1PseudoRange) / 0.02) : -8192);
            SpanBitHelper.SetBitS(buffer, ref bitIndex, 20, double.IsFinite(L2CarrierPhase)
                ? (int)Math.Round(L2CarrierPhase * RtcmV3Helper.CLIGHT / L2Frequency / 0.0005) : -524288);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 7, L2LockTime);
            RtcmV3Serialization.WriteU(buffer, ref bitIndex, 8, L2CNR * RtcmV3Protocol.SNR_UNIT, 0.25);
        }

        public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
        {
            var sys = NavigationSystemEnum.SYS_GLO;

            var prn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            var code1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            var fcn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 5) /* fcn+7 */;
            double pr1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 25);
            var ppr1 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 20);
            var lock1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            var amb = SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            double cnr1 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            var code2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            var pr21 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14);
            var ppr2 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 20);
            var lock2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            double cnr2 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);

            var sat = RtcmV3Helper.satno(sys, (int)prn);

            if (sat == 0)
            {
                throw new Exception($"Rtcm3 1012 satellite number error: prn={prn}");
            }

            SatellitePrn = (int)prn;
            SatelliteNumber = sat;
            SatelliteCode = RtcmV3Helper.Sat2Code(SatelliteNumber, (int)prn);


            pr1 = pr1 * 0.02 + amb * RtcmV3Helper.PRUNIT_GLO;
            L1PseudoRange = pr1;
            L1Frequency = RtcmV3Helper.Code2Freq(sys, RtcmV3Helper.CODE_L1C, (int)fcn - 7);

            if (ppr1 != -524288) // (0xFFF80000)
            {
                L1CarrierPhase = ppr1 * 0.0005 * L1Frequency / RtcmV3Helper.CLIGHT;
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


            L2Frequency = RtcmV3Helper.Code2Freq(sys, RtcmV3Helper.CODE_L2C, (int)fcn - 7);

            if (ppr2 != -524288) // 0xFFF80000
            {
                L2CarrierPhase = ppr2 * 0.0005 * L2Frequency / RtcmV3Helper.CLIGHT;
            }
            else
            {
                L2CarrierPhase = double.NaN;
            }

            L2LockTime = (byte)lock2;
            L2CNR = RtcmV3Helper.snratio(cnr2 * 0.25);
            L2Code = code2 != 0 ? RtcmV3Helper.CODE_L2P : RtcmV3Helper.CODE_L2C;
        }

        public int SatellitePrn { get; set; }

        public int SatelliteNumber { get; set; }

        public string SatelliteCode { get; set; }

        public byte L1Code { get; set; }
        
        public double L1Frequency { get; set; }
        public double L1PseudoRange { get; set; }

        public double L1PhaseRange { get; set; }

        public double L1CarrierPhase { get; set; }

        public bool L1LossOfLockIndicator { get; set; }

        public byte L1LockTime { get; set; }

        public ushort L1CNR { get; set; }

        public byte L2Code { get; set; }

        public double L2Frequency { get; set; }
        public double L2PseudoRange { get; set; }

        public double L2PhaseRange { get; set; }

        public double L2CarrierPhase { get; set; }

        public bool L2LossOfLockIndicator { get; set; }

        public byte L2LockTime { get; set; }

        public ushort L2CNR { get; set; }
    }
}
