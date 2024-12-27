using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class AsvGpsObservation : ISizedSpanSerializable
    {
        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var sys = NavigationSystemEnum.SYS_GPS;
            Prn = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 6);
            var code1 = AsvHelper.GetBitU(buffer, ref bitIndex, 1);
            var pr1 = (double)AsvHelper.GetBitU(buffer, ref bitIndex, 24);
            var ppr1 = AsvHelper.GetBitS(buffer, ref bitIndex, 20);
            L1LockTime = AsvHelper.GetLockTime((byte)AsvHelper.GetBitU(buffer, ref bitIndex, 7));
            var amb = AsvHelper.GetBitU(buffer, ref bitIndex, 8);
            ParticipationIndicator = AsvHelper.GetBitU(buffer, ref bitIndex, 1) == 1;
            ReasonForException = (ReasonForException)AsvHelper.GetBitU(buffer, ref bitIndex, 4);
            bitIndex += 3;
            Elevation = AsvHelper.GetBitU(buffer, ref bitIndex, 10) * 0.1;
            Azimuth = AsvHelper.GetBitS(buffer, ref bitIndex, 12) * 0.1;
            L1CNR = AsvHelper.GetBitU(buffer, ref bitIndex, 8) * 0.25;
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);

            if (Prn >= 40)
            {
                sys = NavigationSystemEnum.SYS_SBS;
                Prn += 80;
            }

            SatelliteId = AsvHelper.satno(sys, Prn);
            SatelliteCode = AsvHelper.Sat2Code(SatelliteId, Prn);

            pr1 = pr1 * 0.02 + amb * AsvHelper.PRUNIT_GPS;
            L1PseudoRange = pr1;

            // (0xFFF80000)
            if (ppr1 != -524288)
            {
                L1CarrierPhase = ppr1 * 0.0005 * 1.57542E9 / AsvHelper.CLIGHT;
            }
            else
            {
                L1CarrierPhase = double.NaN;
            }

            L1Code = code1 != 0 ? AsvHelper.CODE_L1P : AsvHelper.CODE_L1C;
        }

        public void Serialize(ref Span<byte> buffer)
        {
            var bitIndex = 0;
            AsvHelper.SetBitU(buffer, Prn >= 120 ? (uint)(Prn - 80) : (uint)Prn, ref bitIndex, 6);
            AsvHelper.SetBitU(buffer, L1Code == AsvHelper.CODE_L1C ? 0U : 1, ref bitIndex, 1);
            var amb = (uint)(L1PseudoRange / AsvHelper.PRUNIT_GPS);
            var pr1 = (uint)Math.Round((L1PseudoRange % AsvHelper.PRUNIT_GPS) * 50.0);
            AsvHelper.SetBitU(buffer, pr1, ref bitIndex, 24);
            var ppr1 = double.IsNaN(L1CarrierPhase)
                ? -524288
                : (int)Math.Round(L1CarrierPhase * 20000 * AsvHelper.CLIGHT / 1.57542E9);
            AsvHelper.SetBitS(buffer, ppr1, ref bitIndex, 20);
            AsvHelper.SetBitU(buffer, AsvHelper.GetLockTimeIndicator(L1LockTime), ref bitIndex, 7);
            AsvHelper.SetBitU(buffer, amb, ref bitIndex, 8);
            AsvHelper.SetBitU(buffer, ParticipationIndicator ? 1U : 0, ref bitIndex, 1);
            AsvHelper.SetBitU(buffer, (uint)ReasonForException, ref bitIndex, 4);
            bitIndex += 3;
            AsvHelper.SetBitU(buffer, (uint)Math.Round(Elevation * 10.0), ref bitIndex, 10);
            AsvHelper.SetBitS(buffer, (int)Math.Round(Azimuth * 10.0), ref bitIndex, 12);
            AsvHelper.SetBitU(buffer, (uint)Math.Round(L1CNR * 4.0), ref bitIndex, 8);
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);
        }

        public int GetByteSize()
        {
            return 13;
        }

        public void Randomize(Random random, int prn)
        {
            Prn = prn;
            SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GPS, Prn);
            SatelliteCode = AsvHelper.Sat2Code(SatelliteId, Prn);
            L1Code = AsvHelper.CODE_L1C;
            L1LockTime = 937;
        }

        public int Prn { get; set; }

        public int SatelliteId { get; set; }

        public string SatelliteCode { get; set; }

        public byte L1Code { get; set; }

        public double L1PseudoRange { get; set; }

        public double L1CarrierPhase { get; set; }

        public ushort L1LockTime { get; set; }

        public bool ParticipationIndicator { get; set; }

        public ReasonForException ReasonForException { get; set; }

        public double Elevation { get; set; }

        public double Azimuth { get; set; }

        public double L1CNR { get; set; }
    }
}
