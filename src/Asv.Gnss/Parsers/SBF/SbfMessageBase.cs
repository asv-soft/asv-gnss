using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class SbfMessageBase : GnssMessageBase<ushort>
    {
        public override string ProtocolId => SbfBinaryParser.GnssProtocolId;
        public override ushort MessageId =>
            (ushort)((MessageType & 0x1fff) + (MessageRevision << 13));
        public abstract ushort MessageRevision { get; }
        public abstract ushort MessageType { get; }

        public DateTime UtcTime { get; set; }

        /// <summary>
        /// Gets or sets time-Of-Week : Time-tag, expressed in whole milliseconds from the beginning of the current GPS week.
        /// </summary>
        public uint TOW { get; set; }

        /// <summary>
        /// Gets or sets the GPS week number associated with the TOW. WNc is a continuous week count(hence the "c").
        /// It is not affected by GPS week rollovers, which occur every 1024 weeks.
        /// By definition of the Galileo system time, WNc is also the Galileo week number plus 1024.
        /// </summary>
        public ushort WNc { get; set; }

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            // The Sync field is a 2-byte array always set to {0x24, 0x40}. The first byte of every SBF
            // block has hexadecimal value 24(decimal 36, ASCII „$‟).The second byte of every SBF
            // block has hexadecimal value 40(decimal 64, ASCII „@‟).These two bytes identify the
            // beginning of any SBF block and can be used for synchronization.
            var sync1 = BinSerialize.ReadByte(ref buffer);
            var sync2 = BinSerialize.ReadByte(ref buffer);

            // The CRC field is the 16-bit CRC of all the bytes in an SBF block from and including the
            // ID field to the last byte of the block. The generator polynomial for this CRC is the socalled CRC - CCITT
            // polynomial: x16 + x12 + x5 + x0.The CRC is computed in the forward
            // direction using a seed of 0, no reverse and no final XOR
            var crc = BinSerialize.ReadUShort(ref buffer);

            // The ID field is a 2-byte block ID, which uniquely identifies the block type and its
            // contents.It is a bit field with the following definition:
            // bits 0 - 12: block number;
            // bits 13 - 15: block revision number, starting from 0 at the initial block definition, and
            // incrementing each time backwards - compatible changes are performed to the
            // block(see section 2.6).
            var msgId = BinSerialize.ReadUShort(ref buffer);
            var messageType = (ushort)(msgId & 0x1fff << 0);
            var messageRevision = (ushort)((msgId >> 13) & 0x7);

            if (messageType != MessageType)
            {
                throw new GnssParserException(
                    SbfBinaryParser.GnssProtocolId,
                    $"Error to deserialize SBF packet message. MessageType not equal (want [{MessageType}] read [{messageType}])"
                );
            }

            if (messageRevision != MessageRevision)
            {
                throw new GnssParserException(
                    SbfBinaryParser.GnssProtocolId,
                    $"Error to deserialize SBF packet message. Id not equal (want [{MessageRevision}] read [{messageRevision}])"
                );
            }

            if (msgId != MessageId)
            {
                throw new GnssParserException(
                    SbfBinaryParser.GnssProtocolId,
                    $"Error to deserialize SBF packet message. Id not equal (want [{MessageId}] read [{msgId}])"
                );
            }

            // The Length field is a 2-byte unsigned integer containing the size of the SBF block. It is
            // the total number of bytes in the SBF block including the header. It is always a multiple of 4
            var msgLength = BinSerialize.ReadUShort(ref buffer);

            TOW = BinSerialize.ReadUInt(ref buffer);
            WNc = BinSerialize.ReadUShort(ref buffer);

            UtcTime = GpsRawHelper.Gps2Utc(
                new DateTime(1980, 1, 06, 0, 0, 0, DateTimeKind.Utc)
                    .AddDays(7 * WNc)
                    .AddMilliseconds(
                        TOW /* ms in 1 week */
                    )
            );
            DeserializeContent(ref buffer);
        }

        protected abstract void DeserializeContent(ref ReadOnlySpan<byte> buffer);

        public override int GetByteSize()
        {
            throw new NotImplementedException();
        }

        public override void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}
