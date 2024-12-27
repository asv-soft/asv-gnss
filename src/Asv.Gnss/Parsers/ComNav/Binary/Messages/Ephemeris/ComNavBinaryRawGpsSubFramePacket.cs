using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class ComNavBinaryRawGpsSubFramePacket : ComNavBinaryMessageBase
    {
        public const ushort ComNavMessageId = 25;
        public override ushort MessageId => ComNavMessageId;
        public override string Name => "RAWGPSSUBFRAME";

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var frameDecoderNum = BinSerialize.ReadInt(ref buffer);
            SvId = (ushort)BinSerialize.ReadUInt(ref buffer);
            Frequency = 1.57542E9;
            SatPrn = ComNavBinaryHelper.GetPnrAndRinexCode(
                ComNavSatelliteSystemEnum.GPS,
                SvId,
                out var rCore
            );
            SignalType = GnssSignalTypeEnum.L1CA;
            RindexSignalCode = "1C";
            RinexSatCode = rCore;
            SubFrameId = BinSerialize.ReadUInt(ref buffer);

            var st = buffer;
            var rawData = new uint[8];
            for (var i = 0; i < 8; i++)
            {
                rawData[i] = BinSerialize.ReadUInt(ref st);
            }

            RawData = new uint[10];
            for (var i = 0; i < 10; i++)
            {
                var byte1 = BinSerialize.ReadByte(ref buffer);
                var byte2 = BinSerialize.ReadByte(ref buffer);
                var byte3 = BinSerialize.ReadByte(ref buffer);
                RawData[i] = (uint)((byte1 << 22) | (byte2 << 14) | (byte3 << 6));
            }

            var offsetByte = BinSerialize.ReadUShort(ref buffer);

            GpsSubFrame = GpsSubFrameFactory.Create(RawData);

            var signalChNum = BinSerialize.ReadUInt(ref buffer);
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override int InternalGetContentByteSize()
        {
            return 48;
        }

        public ushort SvId { get; set; }
        public int SatPrn { get; set; }
        public string RinexSatCode { get; set; }
        public GnssSignalTypeEnum SignalType { get; set; }
        public string RindexSignalCode { get; set; }
        public double Frequency { get; set; }
        public uint SubFrameId { get; set; }
        public uint[] RawData { get; set; }
        public GpsSubframeBase GpsSubFrame { get; set; }

        public GpsRawCa GetGnssRawNavMsg()
        {
            var msg = new GpsRawCa
            {
                NavSystem = NavSysEnum.GPS,
                CarrierFreq = Frequency,
                SignalType = SignalType,
                UtcTime = UtcTime,
                RawData = new uint[RawData.Length],
                SatId = SvId,
                SatPrn = SatPrn,
                RinexSatCode = RinexSatCode,
                RindexSignalCode = RindexSignalCode,
                GpsSubFrame = GpsSubFrame,
            };

            Array.Copy(RawData, msg.RawData, RawData.Length);

            return msg;
        }
    }
}
