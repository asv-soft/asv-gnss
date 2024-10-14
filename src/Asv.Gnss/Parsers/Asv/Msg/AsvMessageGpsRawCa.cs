using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class AsvMessageGpsRawCa : AsvMessageBase
    {
        private const int NavBitsU32Length = 10;
        
        public override ushort MessageId => 0x112;
        public override string Name => "GpsRawCa";
        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var tow = AsvHelper.GetBitU(buffer, ref bitIndex, 30) * 0.001;
            var week = AsvHelper.GetBitU(buffer, ref bitIndex, 10);
            var cycle = AsvHelper.GetBitU(buffer, ref bitIndex, 4);
            var gpsTime = GpsRawHelper.Gps2Time((int)(cycle * 1024 + week), tow);
            UtcTime = AsvHelper.Gps2Utc(gpsTime);
            Prn = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 6);
            CrcPassed = AsvHelper.GetBitU(buffer, ref bitIndex, 1) != 0;
            var code1 = AsvHelper.GetBitU(buffer, ref bitIndex, 1); bitIndex += 4;
            
            SignalType = GnssSignalTypeEnum.L1CA;
            RindexSignalCode = "1C";
            RinexSatCode = $"G{Prn}";
            
            SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GPS, Prn);
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);
            NAVBitsU32 = new uint[NavBitsU32Length];
            for (var i = 0; i < NavBitsU32Length; i++)
            {
                NAVBitsU32[i] = BinSerialize.ReadUInt(ref buffer);
            }
            GpsSubFrame = GpsSubFrameFactory.Create(NAVBitsU32);
            L1Code = code1 != 0 ? AsvHelper.CODE_L1P : AsvHelper.CODE_L1C;
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            var week = 0;
            double tow = 0;
            GpsRawHelper.Time2Gps(AsvHelper.Utc2Gps(UtcTime), ref week, ref tow);
            var cycle = (uint)(week / 1024);
            week %= 1024;
            var bitIndex = 0;
            AsvHelper.SetBitU(buffer, (uint)Math.Round(tow * 1000.0), ref bitIndex, 30);
            AsvHelper.SetBitU(buffer, (uint)week, ref bitIndex, 10);
            AsvHelper.SetBitU(buffer, cycle, ref bitIndex, 4);
            AsvHelper.SetBitU(buffer, (uint)Prn, ref bitIndex, 6);
            AsvHelper.SetBitU(buffer, (uint)(CrcPassed ? 1 : 0), ref bitIndex, 1);
            AsvHelper.SetBitU(buffer, (uint)(L1Code == AsvHelper.CODE_L1C ? 0 : 1), ref bitIndex, 1);
            bitIndex += 4;
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);
            if (NAVBitsU32 == null) return;
            for (var i = 0; i < NavBitsU32Length; i++)
            {
                BinSerialize.WriteUInt(ref buffer, NAVBitsU32[i]);
            }
            
        }

        protected override int InternalGetContentByteSize()
        {
            return 7 + (NAVBitsU32?.Length ?? 0) * sizeof(uint);
        }

        public override void Randomize(Random random)
        {
            UtcTime = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc);
            Prn = random.Next() % 32 + 1;
            SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GPS, Prn);
            L1Code = AsvHelper.CODE_L1C;
            SignalType = GnssSignalTypeEnum.L1CA;
            RindexSignalCode = "1C";
            RinexSatCode = $"G{Prn}";
            NAVBitsU32 = new uint[NavBitsU32Length];
            NAVBitsU32[0] = (uint)GpsRawHelper.GpsSubframePreamble << 22;
            NAVBitsU32[1] = (uint)(random.Next() % 5 + 1) << 8;
            GpsSubFrame = GpsSubFrameFactory.Create(NAVBitsU32);
        }
        
        /// <summary>
        /// GPS Epoch Time
        /// </summary>
        public DateTime UtcTime { get; set; }
        public int Prn { get; set; }
        
        public int SatelliteId { get; set; }
        public bool CrcPassed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte L1Code { get; set; }
        public ushort SvId { get; set; }
        public string RinexSatCode { get; set; }
        public GnssSignalTypeEnum SignalType { get; set; }
        public string RindexSignalCode { get; set; }
        public double Frequency { get; set; }
        public uint SubFrameId { get; set; }
        public uint[] NAVBitsU32 { get; set; }
        public GpsSubframeBase GpsSubFrame { get; set; }

        public GpsRawCa GetGnssRawNavMsg()
        {
            var msg = new GpsRawCa
            {
                NavSystem = NavSysEnum.GPS,
                CarrierFreq = Frequency,
                SignalType = SignalType,
                UtcTime = UtcTime,
                RawData = new uint[NAVBitsU32.Length],
                SatId = SvId,
                SatPrn = Prn,
                RinexSatCode = RinexSatCode,
                RindexSignalCode = RindexSignalCode,
                GpsSubFrame = GpsSubFrame
            };

            Array.Copy(NAVBitsU32, msg.RawData, NAVBitsU32.Length);

            return msg;
        }
    }
}