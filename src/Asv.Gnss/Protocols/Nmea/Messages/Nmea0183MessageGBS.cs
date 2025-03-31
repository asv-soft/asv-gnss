using System;

namespace Asv.Gnss
{
    /// <summary>
    /// 1. UTC time of the GGA or GNS fix associated with this sentence
    /// 2. Expected 1-sigma error in latitude (meters)
    /// 3. Expected 1-sigma error in longitude (meters)
    /// 4. Expected 1-sigma error in altitude (meters)
    /// 5. ID of most likely failed satellite (1 to 138)
    /// 6. Probability of missed detection for most likely failed satellite
    /// 7. Estimate of bias in meters on most likely failed satellite
    /// 8. Standard deviation of bias estimate
    /// </summary>
    public class Nmea0183MessageGBS : NmeaMessage
    {
        public const string MessageName = "GBS";
        public static readonly NmeaMessageId MessageId = new(MessageName);
        public override string Name => MessageName;
        public override NmeaMessageId Id => MessageId;
        
        protected override void InternalDeserialize(ReadOnlySpan<char> charBufferSpan)
        {
            TimeUtc = ReadTime(ref charBufferSpan);
            LatitudeError = ReadDouble(ref charBufferSpan);
            LongitudeError = ReadDouble(ref charBufferSpan);
            AltitudeError = ReadDouble(ref charBufferSpan);
            FailedSatelliteId = ReadInt(ref charBufferSpan);
            if (FailedSatelliteId.HasValue)
            {
                ProbabilityOfMissedDetection = ReadDouble(ref charBufferSpan);
                BiasEstimate = ReadDouble(ref charBufferSpan);
                BiasEstimateStandardDeviation = ReadDouble(ref charBufferSpan);
            }
            else
            {
                ProbabilityOfMissedDetection = double.NaN;
                BiasEstimate = double.NaN;
                BiasEstimateStandardDeviation = double.NaN;
            }
        }

        protected override void InternalSerialize(ref Span<byte> buffer)
        {
            WriteTime(ref buffer, TimeUtc);
            WriteDouble(ref buffer, LatitudeError, "0.000");
            WriteDouble(ref buffer, LongitudeError, "0.000");
            WriteDouble(ref buffer, AltitudeError, "0.000");
            WriteInt(ref buffer, FailedSatelliteId, "000");
            WriteDouble(ref buffer, ProbabilityOfMissedDetection,"0.000");
            WriteDouble(ref buffer, BiasEstimate,"0.000");
            WriteDouble(ref buffer, BiasEstimateStandardDeviation, "0.000");
        }

        protected override int InternalGetByteSize() =>
            SizeOfTime(TimeUtc)
            + SizeOfDouble(LatitudeError, "0.000")
            + SizeOfDouble(LongitudeError, "0.000")
            + SizeOfDouble(LongitudeError, "0.000")
            + SizeOfInt(FailedSatelliteId, "000")
            + SizeOfDouble(ProbabilityOfMissedDetection, "0.000")
            + SizeOfDouble(BiasEstimate, "0.000")
            + SizeOfDouble(BiasEstimateStandardDeviation, "0.000");

        /// <summary>
        /// UTC time of the GGA or GNS fix associated with this sentence
        /// </summary>
        public TimeSpan? TimeUtc { get; set; }

        /// <summary>
        /// Expected 1-sigma error in latitude (meters)
        /// </summary>
        public double LatitudeError { get; set; }

        /// <summary>
        /// Expected 1-sigma error in longitude (meters)
        /// </summary>
        public double LongitudeError { get; set; }

        /// <summary>
        /// Expected 1-sigma error in altitude (meters)
        /// </summary>
        public double AltitudeError { get; set; }

        /// <summary>
        /// ID of most likely failed satellite (1 to 138)
        /// </summary>
        public int? FailedSatelliteId { get; set; }

        /// <summary>
        /// Probability of missed detection for most likely failed satellite
        /// </summary>
        public double ProbabilityOfMissedDetection { get; set; }

        /// <summary>
        /// Estimate of bias in meters on most likely failed satellite
        /// </summary>
        public double BiasEstimate { get; set; }

        /// <summary>
        /// Standard deviation of bias estimate
        /// </summary>
        public double BiasEstimateStandardDeviation { get; set; }

       

        
    }
}