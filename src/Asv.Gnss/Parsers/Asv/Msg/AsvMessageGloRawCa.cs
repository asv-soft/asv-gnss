using System;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss
{
    public class AsvMessageGloRawCa : AsvMessageBase
    {
        private const int NavBitsU32Length = 3;
        
        public override ushort MessageId => 0x113;
        public override string Name => "GloRawCa";
        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var tod = AsvHelper.GetBitU(buffer, ref bitIndex, 27) * 0.001;
            var day = AsvHelper.GetBitU(buffer, ref bitIndex, 11);
            var cycle = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 5);
            EpochTime = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddYears((cycle - 1) * 4)
                .AddDays(day - 1)
                .AddSeconds(tod).AddHours(-3);
            Prn = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 6);
            CrcPassed = AsvHelper.GetBitU(buffer, ref bitIndex, 1) != 0;
            var code1 = AsvHelper.GetBitU(buffer, ref bitIndex, 1);
            Frequency = 1602000000 + (AsvHelper.GetBitU(buffer, ref bitIndex, 5) - 7) * 562500;
            
            SignalType = GnssSignalTypeEnum.L1CA;
            RindexSignalCode = "1C";
            RinexSatCode = $"R{Prn}";
            
            var recNum = AsvHelper.GetBitU(buffer, ref bitIndex, 4); bitIndex += 4;
            
            SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GLO, Prn);
            
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);
            
            NAVBitsU32 = new uint[recNum][];
            GloWords = new GlonassWordBase[recNum];
            for (var i = 0; i < recNum; i++)
            {
                NAVBitsU32[i] = new uint[NavBitsU32Length];
                for (var j = 0; j < NavBitsU32Length; j++)
                {
                    NAVBitsU32[i][j] = BinSerialize.ReadUInt(ref buffer);    
                }
                GloWords[i] = GlonassWordFactory.Create(NAVBitsU32[i]);
            }
            L1Code = code1 != 0 ? AsvHelper.CODE_L1P : AsvHelper.CODE_L1C;
        }

        public string RindexSignalCode { get; set; }

        public string RinexSatCode { get; set; }
        
        public GnssSignalTypeEnum SignalType { get; set; }
        
        public GlonassWordBase[] GloWords { get; set; }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            var time = EpochTime.AddHours(3);
            var datum = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var cycle = (int)((time - datum).TotalDays / 1461) + 1;
            var day = (uint)((time - datum.AddYears((cycle - 1) * 4)).TotalDays) + 1;
            var tod = (time - datum.AddYears((cycle - 1) * 4).AddDays(day - 1)).TotalSeconds;
            
            var bitIndex = 0;
            AsvHelper.SetBitU(buffer, (uint)Math.Round(tod * 1000.0), ref bitIndex, 27);
            AsvHelper.SetBitU(buffer, day, ref bitIndex, 11);
            AsvHelper.SetBitU(buffer, (uint)cycle, ref bitIndex, 5);
            
            AsvHelper.SetBitU(buffer, (uint)Prn, ref bitIndex, 6);
            AsvHelper.SetBitU(buffer, (uint)(CrcPassed ? 1 : 0), ref bitIndex, 1);
            AsvHelper.SetBitU(buffer, (uint)(L1Code == AsvHelper.CODE_L1C ? 0 : 1), ref bitIndex, 1);
            AsvHelper.SetBitU(buffer, (uint)((Frequency - 1602000000) / 562500 + 7), ref bitIndex, 5);
            AsvHelper.SetBitU(buffer, (uint)(NAVBitsU32?.Length ?? 0), ref bitIndex, 4); bitIndex += 4;
            
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);

            if (NAVBitsU32 == null) return;
            foreach (var nav in NAVBitsU32)
            {
                if (nav == null) continue;
                for (var j = 0; j < NavBitsU32Length; j++)
                {
                    BinSerialize.WriteUInt(ref buffer, nav[j]);    
                }
            }
        }

        protected override int InternalGetContentByteSize()
        {
            return 8 + (NAVBitsU32?.Length ?? 0) * NavBitsU32Length * sizeof(uint);
        }

        public override void Randomize(Random random)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// GLO Epoch Time
        /// </summary>
        public DateTime EpochTime { get; set; }
        
        public int Prn { get; set; }
        
        public int SatelliteId { get; set; }
        public bool CrcPassed { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public byte L1Code { get; set; }
        public long Frequency { get; set; }
        public uint[][] NAVBitsU32 { get; private set; }
        
        public GloRawCa[] GetGnssRawNavMsg()
        {
            if (NAVBitsU32 == null || !NAVBitsU32.Any()) return Array.Empty<GloRawCa>();
            
            var result = new GloRawCa[NAVBitsU32.Length];
            for (var i = 0; i < NAVBitsU32.Length; i++)
            {
                result[i] = new GloRawCa
                {
                    NavSystem = NavSysEnum.GLONASS,
                    CarrierFreq = Frequency,
                    SignalType = SignalType,
                    UtcTime = EpochTime,
                    RawData = new uint[NAVBitsU32[i].Length],
                    SatId = SatelliteId,
                    SatPrn = Prn,
                    RinexSatCode = RinexSatCode,
                    RindexSignalCode = RindexSignalCode,
                    GlonassWord = GloWords[i]
                };
                Array.Copy(NAVBitsU32[i], result[i].RawData, NAVBitsU32[i].Length);
            }
            return result;
        }
    }
}