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
    /// 8. Standard deviation of bias estimate.
    /// </summary>
    public class Nmea0183MessageGBS : Nmea0183MessageBase
    {
        /// <summary>
        /// Represents the GNSS message ID.
        /// </summary>
        public const string GnssMessageId = "GBS";

        // Gets the message ID associated with this message.
        // @return The message ID as a string.
        // /
        public override string MessageId => GnssMessageId;

        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            TimeUtc = Nmea0183Helper.ParseTime(items[1]);
            LatitudeError = Nmea0183Helper.ParseDouble(items[2]);
            LongitudeError = Nmea0183Helper.ParseDouble(items[3]);
            AltitudeError = Nmea0183Helper.ParseDouble(items[4]);
            FailedSatelliteId = Nmea0183Helper.ParseInt(items[5]);
            if (FailedSatelliteId.HasValue)
            {
                ProbabilityOfMissedDetection = Nmea0183Helper.ParseDouble(items[6]);
                BiasEstimate = Nmea0183Helper.ParseDouble(items[7]);
                BiasEstimateStandardDeviation = Nmea0183Helper.ParseDouble(items[8]);
            }
            else
            {
                ProbabilityOfMissedDetection = double.NaN;
                BiasEstimate = double.NaN;
                BiasEstimateStandardDeviation = double.NaN;
            }
        }

        /// <summary>
        /// Gets or sets uTC time of the GGA or GNS fix associated with this sentence.
        /// </summary>
        public DateTime? TimeUtc { get; set; }

        /// <summary>
        /// Gets or sets expected 1-sigma error in latitude (meters).
        /// </summary>
        public double LatitudeError { get; set; }

        /// <summary>
        /// Gets or sets expected 1-sigma error in longitude (meters).
        /// </summary>
        public double LongitudeError { get; set; }

        /// <summary>
        /// Gets or sets expected 1-sigma error in altitude (meters).
        /// </summary>
        public double AltitudeError { get; set; }

        /// <summary>
        /// Gets or sets iD of most likely failed satellite (1 to 138).
        /// </summary>
        public int? FailedSatelliteId { get; set; }

        /// <summary>
        /// Gets or sets probability of missed detection for most likely failed satellite.
        /// </summary>
        public double ProbabilityOfMissedDetection { get; set; }

        /// <summary>
        /// Gets or sets estimate of bias in meters on most likely failed satellite.
        /// </summary>
        public double BiasEstimate { get; set; }

        /// <summary>
        /// Gets or sets standard deviation of bias estimate.
        /// </summary>
        public double BiasEstimateStandardDeviation { get; set; }
    }
}
