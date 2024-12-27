using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class SystemMessage
    {
        /// <summary>
        /// Gets or sets each announcement lists the Message ID as transmitted by the reference station.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Gets or sets 0 - Asynchronous – not transmitted on a regular basis;
        /// 1 - Synchronous – scheduled for transmission at regular intervals.
        /// </summary>
        public byte SyncFlag { get; set; }

        /// <summary>
        /// Gets or sets each announcement lists the Message Transmission Interval as
        /// transmitted by the reference station. If asynchronous, the transmission
        /// interval is approximate.
        /// </summary>
        public double TransmissionInterval { get; set; }

        public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
        {
            Id = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            SyncFlag = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            TransmissionInterval = SpanBitHelper.GetBitU(buffer, ref bitIndex, 16) * 0.1;
        }
    }
}
