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
        }

        public string RindexSignalCode { get; set; }

        public string RinexSatCode { get; set; }
        
        public GnssSignalTypeEnum SignalType { get; set; }
        
        public GlonassWordBase[] GloWords { get; set; }

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
        
        /// <summary>
        /// GLO Epoch Time
        /// </summary>
        public DateTime EpochTime { get; set; }
        
        public int Prn { get; set; }
        
        public int SatelliteId { get; set; }
        public bool CrcPassed { get; set; }
        
        public uint Frequency { get; set; }
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