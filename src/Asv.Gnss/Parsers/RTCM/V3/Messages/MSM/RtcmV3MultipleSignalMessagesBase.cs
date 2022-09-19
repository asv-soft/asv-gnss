using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class RtcmV3MultipleSignalMessagesBase : RtcmV3MessageBase
    {
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            var utc = DateTime.UtcNow;
            ReferenceStationId = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);

            var sys = NavigationSystem = RtcmV3Helper.GetNavigationSystem(MessageId);

            switch (sys)
            {
                case NavigationSystemEnum.SYS_GLO:
                {
                    var dow = SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);
                    var tod = SpanBitHelper.GetBitU(buffer, ref bitIndex, 27);
                    EpochTimeTow = dow * 86400000 + tod;
                    EpochTime = RtcmV3Helper.AdjustDailyRoverGlonassTime(utc, tod * 0.001);
                    break;
                }
                case NavigationSystemEnum.SYS_CMP:
                {
                    EpochTimeTow = SpanBitHelper.GetBitU(buffer, ref bitIndex, 30);
                    var tow = EpochTimeTow * 0.001;
                    tow += 14.0; /* BDT -> GPS Time */
                    EpochTime = RtcmV3Helper.AdjustWeekly(utc, tow);
                    break;
                }
                default:
                {
                    EpochTimeTow = SpanBitHelper.GetBitU(buffer, ref bitIndex, 30);
                    var tow = EpochTimeTow * 0.001;
                    EpochTime = RtcmV3Helper.AdjustWeekly(utc, tow);
                    break;
                }
            }
            

            MultipleMessageBit = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            ObservableDataIsComplete = MultipleMessageBit == 0 ? true : false;

            Iods = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);

            SessionTime = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);

            ClockSteeringIndicator = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            ExternalClockIndicator = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);

            SmoothingIndicator = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            SmoothingInterval = SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);


            var satellites = new List<byte>();
            var signals = new List<byte>();

            for (byte i = 1; i <= 64; i++)
            {
                var mask = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
                if (mask > 0) satellites.Add(i);
            }

            for (byte i = 1; i <= 32; i++)
            {
                var mask = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
                if (mask > 0) signals.Add(i);
            }

            SatelliteIds = satellites.ToArray();
            SignalIds = signals.ToArray();
            var cellMaskCount = SatelliteIds.Length * SignalIds.Length;

            if (cellMaskCount > 64)
            {
                throw new Exception($"RtcmV3 {MessageId} number of Satellite and Signals error: Satellite={SatelliteIds.Length} Signals={SignalIds.Length}");
            }

            // CellMask = new byte[cellMaskCount];
            CellMask = new byte[SatelliteIds.Length][];
            for (var i = 0; i < SatelliteIds.Length; i++)
            {
                CellMask[i] = new byte[SignalIds.Length];
                for (var j = 0; j < SignalIds.Length; j++)
                {
                    CellMask[i][j] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
                }
            }
        }

       

        public NavigationSystemEnum NavigationSystem { get; set; }

        /// <summary>
        /// Observable data complete flag (1:ok, 0:not complete)
        /// </summary>
        public bool ObservableDataIsComplete { get; set; }

        public DateTime EpochTime { get; set; }

        // protected byte[] CellMask { get; set; }
        protected byte[][] CellMask { get; set; }

        protected byte[] SatelliteIds { get; set; }

        protected byte[] SignalIds { get; set; }

        
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
        public uint ReferenceStationId { get; set; }

        /// <summary>
        /// GNSS Epoch Time, specific for each GNSS
        /// GPS: Full seconds since the beginning of the GPS week
        /// GLONASS: Full seconds since the beginning of GLONASS day
        /// </summary>
        public uint EpochTimeTow { get; set; }

        /// <summary>
        /// 
        /// 1 - indicates that more MSMs follow for given physical time and reference station ID.
        /// 0 - indicates that it is the last MSM for given physical time and reference station ID
        /// </summary>
        public byte MultipleMessageBit { get; set; }

        /// <summary>
        /// Issue of Data Station.
        /// This field is reserved to be used to link MSM with future site- description (receiver, antenna description, etc.) messages. A value of “0” indicates that this field is not utilized.
        /// </summary>
        public byte Iods { get; set; }

        /// <summary>
        /// Cumulative session transmitting time
        /// </summary>
        public byte SessionTime { get; set; }

        /// <summary>
        /// 0 – clock steering is not applied In this case receiver clock must be kept in the range of ± 1 ms (approximately ± 300 km).
        /// 1 – clock steering has been applied In this case receiver clock must be kept in the range of ± 1 microsecond (approximately ± 300 meters).
        /// 2 – unknown clock steering status. 
        /// 3 – reserved
        /// </summary>
        public uint ClockSteeringIndicator { get; set; }

        /// <summary>
        /// 0 – internal clock is used.
        /// 1 – external clock is used, clock status is “locked”.
        /// 2 – external clock is used, clock status is “not locked”, which may indicate external clock failure and that the transmitted data may not be reliable.
        /// 3 – unknown clock is used
        /// </summary>
        public uint ExternalClockIndicator { get; set; }

        /// <summary>
        /// GNSS Smoothing Type Indicator:
        /// 1 – Divergence-free smoothing is used.
        /// 0 – Other type of smoothing is used 
        /// </summary>
        public uint SmoothingIndicator { get; set; }

        /// <summary>
        /// The GNSS Smoothing Interval is the integration period over which the
        /// pseudorange code phase measurements are averaged using carrier phase
        /// information.
        /// Divergence-free smoothing may be continuous over the entire period
        /// for which the satellite is visible.
        /// Notice: A value of zero indicates no smoothing is used. 
        /// </summary>
        public uint SmoothingInterval { get; set; }

    }
}