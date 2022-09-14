using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Table 3.5-1 Contents of the Message Header, Types 1001, 1002, 1003, 1004: GPS RTK Messages and
    /// Table 3.5-10 Contents of the Message Header, Types 1009 through 1012: GLONASS RTK Messages
    /// </summary>
    public abstract class RtcmV3RTKObservableMessagesBase : RtcmV3MessageBase
    {
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            ReferenceStationID = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);

            if (MessageId is >= 1001 and <= 1004)
            {
                EpochTimeTOW = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 30);
            }

            if (MessageId is >= 1009 and <= 1012)
            {
                EpochTimeTOW = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 27);
            }

            SynchronousGNSSFlag = (byte)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 1); 

            SatelliteCount = (byte)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 5);

            SmoothingIndicator = (byte)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 1);

            SmoothingInterval = (byte)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 3);
        }

        /// <summary>
        /// The Reference Station ID is determined by the service provider. Its 
        /// primary purpose is to link all message data to their unique source. It is 
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

        /// <summary>
        /// GPS Epoch Time is provided in milliseconds from the beginning of the GPS week, which begins at midnight GMT on Saturday night/Sunday morning, measured in GPS time (as opposed to UTC).
        /// GLONASS Epoch Time of measurement is defined by the GLONASS
        /// ICD as UTC(SU) + 3.0 hours. It rolls over at 86,400 seconds for
        /// GLONASS, except for the leap second, where it rolls over at 86,401. 
        /// </summary>
        public uint EpochTimeTOW { get; set; }

        /// <summary>
        /// 0 - No further GNSS observables referenced to the same Epoch Time
        /// will be transmitted. This enables the receiver to begin processing
        /// the data immediately after decoding the message.
        /// 1 - The next message will contain observables of another GNSS
        /// source referenced to the same Epoch Time.
        /// Note: “Synchronous" here means that the measurements are taken
        /// within one microsecond of each other
        /// </summary>
        public byte SynchronousGNSSFlag { get; set; }

        /// <summary>
        /// The Number of GPS/GLONASS Satellite Signals Processed refers to the number
        /// of satellites in the message. It does not necessarily equal the number
        /// of satellites visible to the Reference Station.
        /// </summary>
        public byte SatelliteCount { get; set; }

        /// <summary>
        /// 0 - Divergence-free smoothing not used
        /// 1 - Divergence-free smoothing used 
        /// </summary>
        public byte SmoothingIndicator { get; set; }

        /// <summary>
        /// The GPS/GLONASS Smoothing Interval is the integration period over which
        /// reference station pseudorange code phase measurements are averaged
        /// using carrier phase information. Divergence-free smoothing may be
        /// continuous over the entire period the satellite is visible.
        /// </summary>
        public byte SmoothingInterval { get; set; }
    }
}