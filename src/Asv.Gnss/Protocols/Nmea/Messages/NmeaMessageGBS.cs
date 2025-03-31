using System;

namespace Asv.Gnss
{
    /// <summary>
    /// GBS - GPS Satellite Fault Detection
    ///             1      2   3   4   5   6   7   8   9
    ///             |      |   |   |   |   |   |   |   |
    /// $--GBS,hhmmss.ss,x.x,x.x,x.x,x.x,x.x,x.x,x.x*hh<CR><LF>
    /// 
    /// 1. UTC time of the GGA or GNS fix associated with this sentence
    /// 2. Expected 1-sigma error in latitude (meters)
    /// 3. Expected 1-sigma error in longitude (meters)
    /// 4. Expected 1-sigma error in altitude (meters)
    /// 5. ID of most likely failed satellite (1 to 138)
    /// 6. Probability of missed detection for most likely failed satellite
    /// 7. Estimate of bias in meters on most likely failed satellite
    /// 8. Standard deviation of bias estimate
    /// 
    /// Source: https://gpsd.gitlab.io/gpsd/NMEA.html#MX521
    /// Example: $GPGBS,125027,23.43,M,13.91,M,34.01,M*07
    /// </summary>
    public class NmeaMessageGBS : NmeaMessage
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
            WriteDouble(ref buffer, LatitudeError, NmeaDoubleFormat.Double1X3);
            WriteDouble(ref buffer, LongitudeError, NmeaDoubleFormat.Double1X3);
            WriteDouble(ref buffer, AltitudeError, NmeaDoubleFormat.Double1X3);
            WriteInt(ref buffer, FailedSatelliteId, NmeaIntFormat.IntD3);
            WriteDouble(ref buffer, ProbabilityOfMissedDetection,NmeaDoubleFormat.Double1X3);
            WriteDouble(ref buffer, BiasEstimate,NmeaDoubleFormat.Double1X3);
            WriteDouble(ref buffer, BiasEstimateStandardDeviation, NmeaDoubleFormat.Double1X3);
        }

        protected override int InternalGetByteSize() =>
            SizeOfTime(TimeUtc)
            + SizeOfDouble(LatitudeError, NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(LongitudeError, NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(LongitudeError, NmeaDoubleFormat.Double1X3)
            + SizeOfInt(FailedSatelliteId, NmeaIntFormat.IntD3)
            + SizeOfDouble(ProbabilityOfMissedDetection, NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(BiasEstimate, NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(BiasEstimateStandardDeviation, NmeaDoubleFormat.Double1X3);

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