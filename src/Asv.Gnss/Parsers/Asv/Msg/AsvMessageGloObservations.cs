using System;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss
{
    public class AsvMessageGloObservations : AsvMessageBase
    {
        public override ushort MessageId => 0x111;
        public override string Name => "GloObservation";

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var tod = AsvHelper.GetBitU(buffer, ref bitIndex, 27) * 0.001;
            var day = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 11);
            var cycle = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 5);

            Tod = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddYears((cycle - 1) * 4)
                .AddDays(day - 1)
                .AddSeconds(tod)
                .AddHours(-3);
            TimeOffset = AsvHelper.GetBitS(buffer, ref bitIndex, 22) * GpsRawHelper.P2_30;
            var svNum = AsvHelper.GetBitU(buffer, ref bitIndex, 5);
            bitIndex += 2;
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);

            Observations = new AsvGloObservation[svNum];
            for (var i = 0; i < svNum; i++)
            {
                var obs = new AsvGloObservation();
                obs.Deserialize(ref buffer);
                Observations[i] = obs;
            }
        }

        public AsvGloObservation[] Observations { get; set; }

        /// <summary>
        /// GPS Epoch Time
        /// </summary>
        public DateTime Tod { get; set; }

        /// <summary>
        /// GPS Receiver Time Offset
        /// </summary>
        public double TimeOffset { get; set; }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            var time = Tod.AddHours(3);
            var datum = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var cycle = (int)((time - datum).TotalDays / 1461) + 1;
            var day = (uint)((time - datum.AddYears((cycle - 1) * 4)).TotalDays) + 1;
            var tod = (time - datum.AddYears((cycle - 1) * 4).AddDays(day - 1)).TotalSeconds;
            
            var bitIndex = 0;
            AsvHelper.SetBitU(buffer, (uint)Math.Round(tod * 1000.0), ref bitIndex, 27);
            AsvHelper.SetBitU(buffer, day, ref bitIndex, 11);
            AsvHelper.SetBitU(buffer, (uint)cycle, ref bitIndex, 5);

            AsvHelper.SetBitS(buffer, (int)Math.Round(TimeOffset / GpsRawHelper.P2_30), ref bitIndex, 22);
            AsvHelper.SetBitU(buffer, (uint)(Observations?.Length ?? 0), ref bitIndex, 5);
            bitIndex += 2;
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);

            if (Observations == null) return;
            foreach (var obs in Observations)
            {
                obs.Serialize(ref buffer);
            }
        }

        protected override int InternalGetContentByteSize()
        {
            return 9 + (Observations?.Sum(_ => _.GetByteSize()) ?? 0);
        }

        public override void Randomize(Random random)
        {
            Tod = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc);
            var length = (random.Next() % 6) + 4;
            var randomPrn = new int[length];
            var index = 0;
            while (index < length)
            {
                var prn = random.Next() % 24 + 1;
                if (randomPrn.Any(_ => _ == prn)) continue;
                randomPrn[index] = prn;
                index++;
            }
            Observations = new AsvGloObservation[length];
            
            for (var i = 0; i < length; i++)
            {
                var obs = new AsvGloObservation();
                obs.Randomize(random, randomPrn[i]);
                Observations[i] = obs;
            }
        }
    }

    public class AsvGloObservation : ISizedSpanSerializable
    {
        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var sys = NavigationSystemEnum.SYS_GLO;
            Prn = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 6);
            var code1 = AsvHelper.GetBitU(buffer, ref bitIndex, 1);
            Frequency = 1602000000 + (AsvHelper.GetBitU(buffer, ref bitIndex, 5) - 7) * 562500;
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

            pr1 = pr1 * 0.02 + amb * AsvHelper.PRUNIT_GLO;
            L1PseudoRange = pr1;

            if (ppr1 != -524288) // (0xFFF80000)
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
            var sys = NavigationSystemEnum.SYS_GLO;
            
            AsvHelper.SetBitU(buffer, (uint)(Prn >= 40 ? Prn - 80 : Prn), ref bitIndex, 6);
            AsvHelper.SetBitU(buffer, (uint)(L1Code == AsvHelper.CODE_L1C ? 0 : 1), ref bitIndex, 1);
            AsvHelper.SetBitU(buffer, (uint)((Frequency - 1602000000) / 562500 + 7), ref bitIndex, 5);
            
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
        /// 
        /// </summary>
        public int Prn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SatelliteId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SatelliteCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte L1Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Frequency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double L1PseudoRange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double L1CarrierPhase { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort L1LockTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ParticipationIndicator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ReasonForException ReasonForException { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double Elevation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double L1CNR { get; set; }

        public void Randomize(Random random, int prn)
        {
            Prn = prn;
            SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GLO, Prn);
            SatelliteCode = AsvHelper.Sat2Code(SatelliteId, Prn);
            L1Code = AsvHelper.CODE_L1C;
            Frequency = 1602000000 + (random.Next() % 16 - 7) * 562500;
            L1LockTime = 937;
        }
    }
}

