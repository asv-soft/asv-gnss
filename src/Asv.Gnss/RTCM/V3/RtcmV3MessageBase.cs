using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class RtcmV3MessageBase: GnssMessageBase<ushort>
    {
        public override string ProtocolId => RtcmV3Parser.GnssProtocolId;

        public byte Reserved { get; set; }

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var preamble = (byte)SpanBitHelper.GetBitU(buffer,ref bitIndex, 8);
            if (preamble != RtcmV3Helper.SyncByte)
            {
                throw new Exception($"Deserialization RTCMv3 message failed: want {RtcmV3Helper.SyncByte:X}. Read {preamble:X}");
            }
            Reserved = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6); 
            var messageLength = (byte)SpanBitHelper.GetBitU(buffer,ref bitIndex, 10);
            if (messageLength > (buffer.Length - 3 /* crc 24 bit*/))
            {
                throw new Exception($"Deserialization RTCMv3 message failed: length too small. Want '{messageLength}'. Read = '{buffer.Length}'");
            }
            var msgId = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            if (msgId != MessageId)
            {
                throw new Exception($"Deserialization RTCMv3 message failed: want message number '{MessageId}'. Read = '{msgId}'");
            }
            DeserializeContent(buffer,ref bitIndex,messageLength);
            bitIndex += 3 * 8; // skip crc
            buffer = bitIndex % 8.0 == 0 ? buffer.Slice(bitIndex / 8) : buffer.Slice(bitIndex / 8 + 1);
        }

        protected abstract void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength);

        public override void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override int GetByteSize()
        {
            throw new NotImplementedException();
        }
    }
}