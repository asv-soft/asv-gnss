using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV3Message1013 : RtcmV3MessageBase
    {
        public const int RtcmMessageId = 1013;

        public override string Name => "System Parameters";

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            ReferenceStationID = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 12);
            ModifiedJulianDay = (ushort)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 16);
            var secondsOfDDay = (uint)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 17);
            var msgCount = (byte)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 5);
            var leapSeconds = (byte)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 8);
            var dateTime = RtcmV3Helper.GetUtc(DateTime.UtcNow, secondsOfDDay);
            EpochTime = dateTime.AddSeconds(leapSeconds);
            RtcmV3Helper.AdjustWeekly(DateTime.UtcNow, secondsOfDDay);
            SystemMessages = new SystemMessage[msgCount];
            for (var i = 0; i < msgCount; i++)
            {
                var message = new SystemMessage();
                message.Deserialize(buffer, ref bitIndex);
                SystemMessages[i] = message;
            }
        }

        public ushort ModifiedJulianDay { get; set; }

        public DateTime EpochTime { get; set; }

        public override ushort MessageId => RtcmMessageId;
        

        /// <summary>
        /// The Reference Station ID is determined by the service provider. Its 
        /// primary purpose is to link all message data to their unique sourceName. It is 
        /// useful in distinguishing between desired and undesired data in cases 
        /// where more than one service may be using the same data link 
        /// frequency. It is also useful in accommodating multiple reference 
        /// stations within a single data link transmission. 
        /// In reference network applications the Reference Station ID plays an 
        /// important role, because it is the link between the observation messages 
        /// of a specific reference station and its auxiliary information contained in 
        /// other messages for proper operation. Thus the Service Provider should 
        /// ensure that the Reference Station ID is unique within the whole 
        /// network, and that ID’s should be reassigned only when absolutely 
        /// necessary. 
        /// Service Providers may need to coordinate their Reference Station ID
        /// assignments with other Service Providers in their region in order to 
        /// avoid conflicts. This may be especially critical for equipment 
        /// accessing multiple services, depending on their services and means of 
        /// information distribution.
        /// </summary>
        public uint ReferenceStationID { get; set; }

        public SystemMessage[] SystemMessages { get; set; }
        
    }

    public class SystemMessage
    {
        /// <summary>
        /// Each announcement lists the Message ID as transmitted by the reference station.
        /// </summary>
        public ushort Id { get; set; }
        /// <summary>
        /// 0 - Asynchronous – not transmitted on a regular basis;
        /// 1 - Synchronous – scheduled for transmission at regular intervals
        /// </summary>
        public byte SyncFlag { get; set; }

        /// <summary>
        /// Each announcement lists the Message Transmission Interval as
        /// transmitted by the reference station. If asynchronous, the transmission
        /// interval is approximate. 
        /// </summary>
        public double TransmissionInterval { get; set; }

        public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
        {
            Id = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            SyncFlag = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            TransmissionInterval = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 16) * 0.1;
        }
    }
}
