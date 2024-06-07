using System;
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

            EpochTime = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddYears((cycle - 1) * 4)
                .AddDays(day - 1)
                .AddSeconds(tod);
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
        public DateTime EpochTime { get; set; }

        /// <summary>
        /// GPS Receiver Time Offset
        /// </summary>
        public double TimeOffset { get; set; }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override int InternalGetContentByteSize()
        {
            throw new NotImplementedException();
        }

        public override void Randomize(Random random)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public int GetByteSize()
        {
            return 13;
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
        public int L1LockTime { get; set; }

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

    }
}

