using System;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss
{
    public class AsvMessageGpsObservations : AsvMessageBase
    {
        public override ushort MessageId => 0x110;
        public override string Name => "GpsObservation";

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var tow = AsvHelper.GetBitU(buffer, ref bitIndex, 30) * 0.001;
            var week = AsvHelper.GetBitU(buffer, ref bitIndex, 10);
            var cycle = AsvHelper.GetBitU(buffer, ref bitIndex, 4);

            Tow = GpsRawHelper.Gps2Time((int)(cycle * 1024 + week), tow);
            TimeOffset = AsvHelper.GetBitS(buffer, ref bitIndex, 22) * GpsRawHelper.P2_30;
            var svNum = AsvHelper.GetBitU(buffer, ref bitIndex, 5);
            bitIndex += 1;
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);

            Observations = new AsvGpsObservation[svNum];
            for (var i = 0; i < svNum; i++)
            {
                var obs = new AsvGpsObservation();
                obs.Deserialize(ref buffer);
                Observations[i] = obs;
            }
        }

        public AsvGpsObservation[] Observations { get; set; }

        /// <summary>
        /// GPS Epoch Time
        /// </summary>
        public DateTime Tow { get; set; }

        /// <summary>
        /// GPS Receiver Time Offset
        /// </summary>
        public double TimeOffset { get; set; }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            var week = 0;
            double tow = 0;
            GpsRawHelper.Time2Gps(Tow, ref week, ref tow);
            var cycle = (uint)(week / 1024);
            week %= 1024;
            var bitIndex = 0;
            AsvHelper.SetBitU(buffer, (uint)Math.Round(tow * 1000.0), ref bitIndex, 30);
            AsvHelper.SetBitU(buffer, (uint)week, ref bitIndex, 10);
            AsvHelper.SetBitU(buffer, cycle, ref bitIndex, 4);
            AsvHelper.SetBitS(buffer, (int)Math.Round(TimeOffset / GpsRawHelper.P2_30), ref bitIndex, 22);
            AsvHelper.SetBitU(buffer, (uint)(Observations?.Length ?? 0), ref bitIndex, 5);
            bitIndex += 1;
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
            Tow = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc);
            var length = (random.Next() % 6) + 4;
            var randomPrn = new int[length];
            var index = 0;
            while (index < length)
            {
                var prn = random.Next() % 32 + 1;
                if (randomPrn.Any(_ => _ == prn)) continue;
                randomPrn[index] = prn;
                index++;
            }
            Observations = new AsvGpsObservation[length];
            
            for (var i = 0; i < length; i++)
            {
                var obs = new AsvGpsObservation();
                obs.Randomize(random, randomPrn[i]);
                Observations[i] = obs;
            }
        }
    }

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


            if (ppr1 != -524288) // (0xFFF80000)
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
            AsvHelper.SetBitU(buffer, L1Code == AsvHelper.CODE_L1C ? (uint)0 : 1, ref bitIndex, 1);
            var amb = (uint)(L1PseudoRange / AsvHelper.PRUNIT_GPS);
            var pr1 = (uint)Math.Round((L1PseudoRange % AsvHelper.PRUNIT_GPS) * 50.0);
            AsvHelper.SetBitU(buffer, pr1, ref bitIndex, 24);
            var ppr1 = double.IsNaN(L1CarrierPhase)
                ? -524288
                : (int)Math.Round(L1CarrierPhase * 20000 * AsvHelper.CLIGHT / 1.57542E9);
            AsvHelper.SetBitS(buffer, ppr1, ref bitIndex, 20);
            AsvHelper.SetBitU(buffer, AsvHelper.GetLockTimeIndicator(L1LockTime), ref bitIndex, 7);
            AsvHelper.SetBitU(buffer, amb, ref bitIndex, 8);
            AsvHelper.SetBitU(buffer, ParticipationIndicator ? (uint)1 : 0, ref bitIndex, 1);
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

    }
}