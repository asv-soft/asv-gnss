using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Base class for RTCMv2 messages.
    /// </summary>
    public abstract class RtcmV2MessageBase : GnssMessageBase<ushort>
    {
        /// <summary>
        /// Gets the protocol identifier for the RTCM V2 parser.
        /// </summary>
        /// <value>
        /// The protocol identifier.
        /// </value>
        public override string ProtocolId => RtcmV2Parser.GnssProtocolId;

        /// <summary>
        /// Gets or sets the value of Udre.
        /// </summary>
        /// <value>
        /// The value of Udre.
        /// </value>
        public double Udre { get; set; }

        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <value>
        /// The sequence number.
        /// </value>
        public byte SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the GPS time.
        /// </summary>
        public DateTime GpsTime { get; set; }

        /// <summary>
        /// Gets or sets the value of the ZCount property.
        /// </summary>
        /// <value>
        /// The value of the ZCount property.
        /// </value>
        public double ZCount { get; set; }

        /// <summary>
        /// Gets or sets the reference station ID.
        /// </summary>
        /// <remarks>
        /// The reference station ID is a 16-bit unsigned integer that uniquely identifies a reference station.
        /// It is used to establish a link between the current station and a specific reference station.
        /// </remarks>
        /// <value>
        /// The reference station ID.
        /// </value>
        public ushort ReferenceStationId { get; set; }

        /// <summary>
        /// Deserialize the buffer into the current object.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to be deserialized.</param>
        /// <exception cref="System.Exception">Thrown when deserialization fails or when the buffer length is too small.</exception>
        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var preamble = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (preamble != RtcmV2Parser.SyncByte)
            {
                throw new Exception(
                    $"Deserialization RTCMv2 message failed: want {RtcmV2Parser.SyncByte:X}. Read {preamble:X}"
                );
            }

            var msgType = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            if (msgType != MessageId)
            {
                throw new Exception(
                    $"Deserialization RTCMv2 message failed: want message number '{MessageId}'. Read = '{msgType}'"
                );
            }

            ReferenceStationId = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
            var zCountRaw = SpanBitHelper.GetBitU(buffer, ref bitIndex, 13);
            ZCount = zCountRaw * 0.6;

            if (ZCount >= 3600.0)
            {
                throw new Exception($"RTCMv2 Modified Z-count error: zcnt={ZCount}");
            }

            GpsTime = Adjhour(ZCount);

            SequenceNumber = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);

            var payloadLength = (byte)(SpanBitHelper.GetBitU(buffer, ref bitIndex, 5) * 3);
            if (
                payloadLength
                > (
                    buffer.Length - 6 /* header 48 bit*/
                )
            )
            {
                throw new Exception(
                    $"Deserialization RTCMv2 message failed: length too small. Want '{payloadLength}'. Read = '{buffer.Length - 6}'"
                );
            }

            Udre = RtcmV2MessageBase.GetUdre((byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 3));
            DeserializeContent(buffer, ref bitIndex, payloadLength);
            buffer =
                bitIndex % 8.0 == 0 ? buffer.Slice(bitIndex / 8) : buffer.Slice((bitIndex / 8) + 1);
        }

        /// <summary>
        /// Deserialize the content of a buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing the content to be deserialized.</param>
        /// <param name="bitIndex">The starting bit index from where to begin deserialization.</param>
        /// <param name="payloadLength">The length of the content payload.</param>
        protected abstract void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            byte payloadLength
        );

        /// <summary>
        /// Serializes the object and writes it to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the serialized object to. The length of the buffer must be sufficient to hold the serialized object.</param>
        /// <exception cref="System.NotImplementedException">Thrown when the method is not implemented.</exception>
        public override void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the byte size of the object.
        /// </summary>
        /// <returns>The byte size of the object.</returns>
        public override int GetByteSize()
        {
            throw new NotImplementedException();
        }

        // Adjusts the hour of the current UTC time based on the given zcnt value.
        // @param zcnt The value used to adjust the hour of the current UTC time.
        // @return The adjusted DateTime value.
        // /
        protected virtual DateTime Adjhour(double zcnt)
        {
            var utc = DateTime.UtcNow;
            double tow = 0;
            var week = 0;

            /* if no time, get cpu time */
            var time = RtcmV3Helper.Utc2Gps(utc);

            RtcmV3Helper.GetFromTime(time, ref week, ref tow);

            var hour = Math.Floor(tow / 3600.0);
            var sec = tow - (hour * 3600.0);
            if (zcnt < sec - 1800.0)
            {
                zcnt += 3600.0;
            }
            else if (zcnt > sec + 1800.0)
            {
                zcnt -= 3600.0;
            }

            return RtcmV3Helper.GetFromGps(week, (hour * 3600) + zcnt);
        }

        /// <summary>
        /// Returns the Udre (Uncertainty to Define a Reference Element) value based on the given rsHealth value.
        /// </summary>
        /// <param name="rsHealth">The health value of a satellite receiver. Should be a byte value between 0 and 7.</param>
        /// <returns>
        /// The Udre value based on the rsHealth value.
        /// If rsHealth is 0, the Udre value is 1.0.
        /// If rsHealth is 1, the Udre value is 0.75.
        /// If rsHealth is 2, the Udre value is 0.5.
        /// If rsHealth is 3, the Udre value is 0.3.
        /// If rsHealth is 4, the Udre value is 0.2.
        /// If rsHealth is 5, the Udre value is 0.1.
        /// If rsHealth is 6, the Udre value is NaN (Not a Number).
        /// If rsHealth is 7, the Udre value is 0.0.
        /// If rsHealth is not within the valid range of 0 to 7, the Udre value is NaN (Not a Number).
        /// </returns>
        private static double GetUdre(byte rsHealth)
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
