using System;
using System.Buffers;

namespace Asv.Gnss
{
    /// <summary>
    /// [GBS] GPS Satellite Fault Detection
    /// https://gpsd.gitlab.io/gpsd/NMEA.html#MX521
    /// https://receiverhelp.trimble.com/alloy-gnss/en-us/NMEA-0183messages_GBS.html
    /// </summary>
    public class NmeaMessageGbs : NmeaMessageBase
    {
        public const string MessageName = "GBS";
        public static readonly NmeaMessageId MessageId = new(MessageName);
        private TimeSpan? _timeUtc;
        private double _latitudeError;
        private double _longitudeError;
        private double _altitudeError;
        private int? _failedSatelliteId;
        private double _probabilityOfMissedDetection = double.NaN;
        private double _biasEstimate = double.NaN;
        private double _biasEstimateStandardDeviation = double.NaN;
        public override string Name => MessageName;
        public override NmeaMessageId Id => MessageId;
        
        protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
        {
            ReadTime(ref buffer,out _timeUtc);
            ReadDouble(ref buffer, out _latitudeError);
            ReadDouble(ref buffer, out _longitudeError);
            ReadDouble(ref buffer, out _altitudeError);
            ReadInt(ref buffer, out _failedSatelliteId, false);
            ReadDouble(ref buffer, out _probabilityOfMissedDetection, false);
            ReadDouble(ref buffer, out _biasEstimate, false);
            ReadDouble(ref buffer, out _biasEstimateStandardDeviation, false);
        }

        protected override void InternalSerialize(ref Span<byte> buffer)
        {
            WriteTime(ref buffer, TimeUtc);
            WriteDouble(ref buffer, in _latitudeError, NmeaDoubleFormat.Double1X3);
            WriteDouble(ref buffer, in _longitudeError, NmeaDoubleFormat.Double1X3);
            WriteDouble(ref buffer, in _altitudeError, NmeaDoubleFormat.Double1X3);
            WriteInt(ref buffer, in _failedSatelliteId, NmeaIntFormat.IntD1);
            WriteDouble(ref buffer, in _probabilityOfMissedDetection,NmeaDoubleFormat.Double1X3);
            WriteDouble(ref buffer, in _biasEstimate,NmeaDoubleFormat.Double1X3);
            WriteDouble(ref buffer, in _biasEstimateStandardDeviation, NmeaDoubleFormat.Double1X3);
        }

        protected override int InternalGetByteSize() =>
            SizeOfTime(in _timeUtc)
            + SizeOfDouble(in _latitudeError,in NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(in _longitudeError,in NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(in _altitudeError,in NmeaDoubleFormat.Double1X3)
            + SizeOfInt(in _failedSatelliteId,in NmeaIntFormat.IntD1)
            + SizeOfDouble(in _probabilityOfMissedDetection,in NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(in _biasEstimate,in NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(in _biasEstimateStandardDeviation,in NmeaDoubleFormat.Double1X3);

        /// <summary>
        /// UTC time of the GGA or GNS fix associated with this sentence
        /// </summary>
        public TimeSpan? TimeUtc
        {
            get => _timeUtc;
            init => _timeUtc = value;
        }

        /// <summary>
        /// Expected 1-sigma error in latitude (meters)
        /// </summary>
        public double LatitudeError
        {
            get => _latitudeError;
            set => _latitudeError = value;
        }

        /// <summary>
        /// Expected 1-sigma error in longitude (meters)
        /// </summary>
        public double LongitudeError
        {
            get => _longitudeError;
            set => _longitudeError = value;
        }

        /// <summary>
        /// Expected 1-sigma error in altitude (meters)
        /// </summary>
        public double AltitudeError
        {
            get => _altitudeError;
            set => _altitudeError = value;
        }

        /// <summary>
        /// ID of most likely failed satellite (1 to 138)
        /// </summary>
        public int? FailedSatelliteId
        {
            get => _failedSatelliteId;
            set => _failedSatelliteId = value;
        }

        /// <summary>
        /// Probability of missed detection for most likely failed satellite
        /// </summary>
        public double ProbabilityOfMissedDetection
        {
            get => _probabilityOfMissedDetection;
            set => _probabilityOfMissedDetection = value;
        }

        /// <summary>
        /// Estimate of bias in meters on most likely failed satellite
        /// </summary>
        public double BiasEstimate
        {
            get => _biasEstimate;
            set => _biasEstimate = value;
        }

        /// <summary>
        /// Standard deviation of bias estimate
        /// </summary>
        public double BiasEstimateStandardDeviation
        {
            get => _biasEstimateStandardDeviation;
            set => _biasEstimateStandardDeviation = value;
        }

        
    }
}