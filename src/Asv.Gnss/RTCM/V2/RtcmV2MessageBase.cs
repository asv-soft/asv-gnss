using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class RtcmV2MessageBase : GnssMessageBase<ushort>
    {
        public override string ProtocolId => RtcmV2Parser.GnssProtocolId;

        public double Udre { get; set; }

        public byte SequenceNumber { get; set; }

        public DateTime GpsTime { get; set; }

        public double ZCount { get; set; }

        public ushort ReferenceStationId { get; set; }

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var preamble = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (preamble != RtcmV2Parser.SyncByte)
            {
                throw new Exception($"Deserialization RTCMv2 message failed: want {RtcmV2Parser.SyncByte:X}. Read {preamble:X}");
            }
            var msgType = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            if (msgType != MessageId)
            {
                throw new Exception($"Deserialization RTCMv2 message failed: want message number '{MessageId}'. Read = '{msgType}'");
            }
            ReferenceStationId = (ushort)SpanBitHelper.GetBitU(buffer,ref bitIndex, 10);
            var zCountRaw = SpanBitHelper.GetBitU(buffer,ref bitIndex, 13);
            ZCount = zCountRaw * 0.6;

            if (ZCount >= 3600.0)
            {
                throw new Exception($"RTCMv2 Modified Z-count error: zcnt={ZCount}");
            }
            GpsTime = Adjhour(ZCount);

            SequenceNumber = (byte)SpanBitHelper.GetBitU(buffer,ref bitIndex, 3);

            var payloadLength = (byte)(SpanBitHelper.GetBitU(buffer,ref bitIndex, 5) * 3);
            if (payloadLength > (buffer.Length - 6 /* header 48 bit*/))
            {
                throw new Exception($"Deserialization RTCMv2 message failed: length too small. Want '{payloadLength}'. Read = '{buffer.Length - 6}'");
            }
            Udre = GetUdre((byte)SpanBitHelper.GetBitU(buffer,ref bitIndex, 3));
            DeserializeContent(buffer, ref bitIndex, payloadLength);
            buffer = bitIndex % 8.0 == 0 ? buffer.Slice(bitIndex / 8) : buffer.Slice(bitIndex / 8 + 1);
        }

        protected abstract void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength);

        public override void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override int GetByteSize()
        {
            throw new NotImplementedException();
        }

        protected virtual DateTime Adjhour(double zcnt)
        {
            var utc = DateTime.UtcNow;
            double tow = 0;
            var week = 0;

            /* if no time, get cpu time */
            var time = RtcmV3Helper.Utc2Gps(utc);

            RtcmV3Helper.GetFromTime(time, ref week, ref tow);

            var hour = Math.Floor(tow / 3600.0);
            var sec = tow - hour * 3600.0;
            if (zcnt < sec - 1800.0) zcnt += 3600.0;
            else if (zcnt > sec + 1800.0) zcnt -= 3600.0;

            return RtcmV3Helper.GetFromGps(week, hour * 3600 + zcnt);
        }

        private double GetUdre(byte rsHealth)
        {
            switch (rsHealth)
            {
                case 0:
                    return 1.0;
                case 1:
                    return 0.75;
                case 2:
                    return 0.5;
                case 3:
                    return 0.3;
                case 4:
                    return 0.2;
                case 5:
                    return 0.1;
                case 6:
                    return double.NaN;
                case 7:
                    return 0.0;
                default:
                    return double.NaN;
            }
        }


    }
}