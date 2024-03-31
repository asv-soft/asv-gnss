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
            
            SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GPS, Prn);
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);
            NAVBitsU32 = new uint[NavBitsU32Length];
            for (var i = 0; i < NavBitsU32Length; i++)
            {
                NAVBitsU32[i] = BinSerialize.ReadUInt(ref buffer);
            }
            GpsSubFrame = GpsSubFrameFactory.Create(NAVBitsU32);
        }

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
        /// GPS Epoch Time
        /// </summary>
        public DateTime UtcTime { get; set; }
        public int Prn { get; set; }
        
        public int SatelliteId { get; set; }
        public bool CrcPassed { get; set; }
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