using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class AsvGloObservation : ISizedSpanSerializable
    {
        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var sys = NavigationSystemEnum.SYS_GLO;
            Prn = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 6);
            var code1 = AsvHelper.GetBitU(buffer, ref bitIndex, 1);
            Frequency = 1602000000 + ((AsvHelper.GetBitU(buffer, ref bitIndex, 5) - 7) * 562500);
            var pr1 = (double)AsvHelper.GetBitU(buffer, ref bitIndex, 25);
            var ppr1 = AsvHelper.GetBitS(buffer, ref bitIndex, 20);
            L1LockTime = AsvHelper.GetLockTime((byte)AsvHelper.GetBitU(buffer, ref bitIndex, 7));
            var amb = AsvHelper.GetBitU(buffer, ref bitIndex, 7);
            ParticipationIndicator = AsvHelper.GetBitU(buffer, ref bitIndex, 1) == 1;
            ReasonForException = (ReasonForException)AsvHelper.GetBitU(buffer, ref bitIndex, 4);
            bitIndex += 6;
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

            pr1 = (pr1 * 0.02) + (amb * AsvHelper.PRUNIT_GLO);
            L1PseudoRange = pr1;

            // (0xFFF80000)
            if (ppr1 != -524288)
            {
                var lam1 = AsvHelper.CLIGHT / Frequency;
                L1CarrierPhase = ppr1 * 0.0005 / lam1;
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

            AsvHelper.SetBitU(buffer, (uint)(Prn >= 40 ? Prn - 80 : Prn), ref bitIndex, 6);
            AsvHelper.SetBitU(
                buffer,
                (uint)(L1Code == AsvHelper.CODE_L1C ? 0 : 1),
                ref bitIndex,
                1
            );
            AsvHelper.SetBitU(
                buffer,
                (uint)(((Frequency - 1602000000) / 562500) + 7),
                ref bitIndex,
                5
            );

            var amb = (uint)(L1PseudoRange / AsvHelper.PRUNIT_GLO);
            var pr1 = (uint)Math.Round((L1PseudoRange % AsvHelper.PRUNIT_GLO) * 50.0);
            AsvHelper.SetBitU(buffer, pr1, ref bitIndex, 25);
            var ppr1 = double.IsNaN(L1CarrierPhase)
                ? -524288
                : (int)Math.Round(L1CarrierPhase * 20000 * AsvHelper.CLIGHT / Frequency);
            AsvHelper.SetBitS(buffer, ppr1, ref bitIndex, 20);
            AsvHelper.SetBitU(buffer, AsvHelper.GetLockTimeIndicator(L1LockTime), ref bitIndex, 7);
            AsvHelper.SetBitU(buffer, amb, ref bitIndex, 7);
            AsvHelper.SetBitU(buffer, (uint)(ParticipationIndicator ? 1 : 0), ref bitIndex, 1);
            AsvHelper.SetBitU(buffer, (uint)ReasonForException, ref bitIndex, 4);
            bitIndex += 6;
            AsvHelper.SetBitU(buffer, (uint)Math.Round(Elevation * 10), ref bitIndex, 10);
            AsvHelper.SetBitS(buffer, (int)Math.Round(Azimuth * 10), ref bitIndex, 12);
            AsvHelper.SetBitU(buffer, (uint)Math.Round(L1CNR * 4), ref bitIndex, 8);
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);
        }

        public int GetByteSize()
        {
            return 14;
        }

        /// <summary>
        /// Gets or sets Prn.
        /// </summary>
        public int Prn { get; set; }

        /// <summary>
        /// Gets or sets SatelliteId.
        /// </summary>
        public int SatelliteId { get; set; }

        /// <summary>
        /// Gets or sets SatelliteCode.
        /// </summary>
        public string SatelliteCode { get; set; }

        /// <summary>
        /// Gets or sets L1Code.
        /// </summary>
        public byte L1Code { get; set; }

        /// <summary>
        /// Gets or sets Frequency.
        /// </summary>
        public long Frequency { get; set; }

        /// <summary>
        /// Gets or sets L1PseudoRange.
        /// </summary>
        public double L1PseudoRange { get; set; }

        /// <summary>
        /// Gets or sets L1CarrierPhase.
        /// </summary>
        public double L1CarrierPhase { get; set; }

        /// <summary>
        /// Gets or sets L1LockTime.
        /// </summary>
        public ushort L1LockTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there is a participation.
        /// </summary>
        public bool ParticipationIndicator { get; set; }

        /// <summary>
        /// Gets or sets ReasonForException.
        /// </summary>
        public ReasonForException ReasonForException { get; set; }

        /// <summary>
        /// Gets or sets Elevation.
        /// </summary>
        public double Elevation { get; set; }

        /// <summary>
        /// Gets or sets Azimuth.
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// Gets or sets L1CNR.
        /// </summary>
        public double L1CNR { get; set; }

        public void Randomize(Random random, int prn)
        {
            Prn = prn;
            SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GLO, Prn);
            SatelliteCode = AsvHelper.Sat2Code(SatelliteId, Prn);
            L1Code = AsvHelper.CODE_L1C;
            Frequency = 1602000000 + (((random.Next() % 16) - 7) * 562500);
            L1LockTime = 937;
        }
    }
}
