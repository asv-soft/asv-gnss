using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Asv.Common;
using Prometheus;

namespace Asv.Gnss
{
    /// <summary>
    /// Utility class for tracking various metrics related to GNSS connection.
    /// </summary>
    public static class GnssMetrics
    {
        /// <summary>
        /// Represents the label for the source metric.
        /// </summary>
        /// <remarks>
        /// This variable is a constant of type string and has the value "src".
        /// It is used to label the source metric in the application.
        /// </remarks>
        private const string SourceMetricLabel = "src";

        /// <summary>
        /// The label used for protocol metric.
        /// </summary>
        private const string ProtocolMetricLabel = "protocol";

        /// <summary>
        /// Constant variable representing the metric label for message ID.
        /// </summary>
        private const string MessageIdMetricLabel = "id";

        /// <summary>
        /// The constant variable represents the label for the metric name in a message.
        /// </summary>
        /// <value>
        /// The label for the metric name in a message.
        /// </value>
        private const string MessageNameMetricLabel = "name";

        /// <summary>
        /// Input bytes counter for GNSS connection
        /// </summary>
        private static readonly Counter MetricGnssRxBytesCounter = Metrics.CreateCounter("gnss_rx_bytes",
            "Input bytes counter for GNSS connection",
            new CounterConfiguration
            {
                LabelNames = new[] { SourceMetricLabel }
            });

        /// <summary>
        /// The counter for measuring input bytes for GNSS connection.
        /// </summary>
        private static readonly Counter MetricGnssTxBytesCounter = Metrics.CreateCounter("gnss_tx_bytes",
            "Input bytes counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[] { SourceMetricLabel }
            });

        /// <summary>
        /// Input message counter for GNSS connection.
        /// </summary>
        private static readonly Counter MetricGnssRxMessageCounter = Metrics.CreateCounter("gnss_rx_message_count",
            "Input message counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[] { SourceMetricLabel, ProtocolMetricLabel,MessageIdMetricLabel, MessageNameMetricLabel }
            });

        /// <summary>
        /// MetricGnssTxMessageCounter represents a counter for output GNSS messages.
        /// </summary>
        /// <remarks>
        /// This counter keeps track of the number of GNSS messages transmitted through a connection.
        /// </remarks>
        private static readonly Counter MetricGnssTxMessageCounter = Metrics.CreateCounter("gnss_tx_message_count",
            "Output message counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel, MessageIdMetricLabel, MessageNameMetricLabel }
            });

        /// <summary>
        /// MetricGnssErrorCounter is a counter metric that keeps track of any errors in the GNSS connection.
        /// </summary>
        private static readonly Counter MetricGnssErrorCounter = Metrics.CreateCounter("gnss_rx_message_errors_count",
            "Any errors counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel }
            });

        /// <summary>
        /// Represents a counter for CRC error in GNSS connection.
        /// </summary>
        private static readonly Counter MetricGnssCrcErrorCounter = Metrics.CreateCounter(
            "gnss_rx_message_crc_error_count", "Crc error counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel }
            });

        /// <summary>
        /// Represents a counter for the number of times when not all data is read after deserialization message errors for a GNSS connection.
        /// </summary>
        private static readonly Counter MetricGnssReadNotAllDataWhenDeserializePacketError = Metrics.CreateCounter(
            "gnss_rx_message_read_not_all_data_error_count",
            "Read not all data after deserialization message errors for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel,MessageIdMetricLabel  }
            });

        /// <summary>
        /// A counter for tracking the number of unknown message IDs received from a GNSS connection.
        /// </summary>
        private static readonly Counter MetricGnssUnknownMessageCounter = Metrics.CreateCounter(
            "gnss_rx_message_unknown_counter", "Unknown message ID counter for GNSS connection",
            new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel, MessageIdMetricLabel }
            });

        /// <summary>
        /// The counter for tracking the number of deserialization errors that occur while processing GNSS messages.
        /// </summary>
        private static readonly Counter MetricGnssDeserializationErrorCounter = Metrics.CreateCounter(
            "gnss_rx_message_deserialization_error_count", "Deserialization error counter for GNSS connection",
            new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel, MessageIdMetricLabel, MessageNameMetricLabel }
            });

        /// <summary>
        /// Represents a counter for tracking publication errors of GNSS connection messages.
        /// </summary>
        private static readonly Counter MetricGnssPublicationErrorCounter = Metrics.CreateCounter(
            "gnss_rx_message_publication_error_count", "Publication error counter for GNSS connection",
            new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel, MessageIdMetricLabel, MessageNameMetricLabel }
            });

        /// <summary>
        /// Adds metric tracking to an IGnssConnection object.
        /// </summary>
        /// <param name="src">The IGnssConnection object.</param>
        /// <param name="sourceName">The name of the data source.</param>
        /// <returns>A disposable object that can be used to remove the metric tracking.</returns>
        public static IDisposable AddMetrics(this IGnssConnection src, string sourceName)
        {
            return new CompositeDisposable(
                src.StatisticRxBytes.Subscribe(_=> MetricGnssRxBytesCounter.WithLabels(sourceName).Inc(_)),
                src.StatisticTxBytes.Subscribe(_ => MetricGnssTxBytesCounter.WithLabels(sourceName).Inc(_)),
                src.OnMessage.Subscribe(_ => MetricGnssRxMessageCounter.WithLabels(sourceName, _.ProtocolId, _.MessageStringId, _.Name).Inc()),
                src.OnTxMessage.Subscribe(_ => MetricGnssTxMessageCounter.WithLabels(sourceName, _.ProtocolId, _.MessageStringId, _.Name).Inc()),
                src.OnError.Subscribe(_ => MetricGnssErrorCounter.WithLabels(sourceName, _.ProtocolId).Inc()),
                src.OnError.Where(_ => _ is GnssCrcErrorException).Subscribe(_ => MetricGnssCrcErrorCounter.WithLabels(sourceName, _.ProtocolId).Inc()),
                src.OnError.Where(_ => _ is GnssReadNotAllDataWhenDeserializePacketErrorException)
                    .Cast<GnssReadNotAllDataWhenDeserializePacketErrorException>()
                    .Subscribe(_ =>
                        MetricGnssReadNotAllDataWhenDeserializePacketError
                            .WithLabels(sourceName, _.ProtocolId, _.MessageId).Inc()),
                src.OnError.Where(_ => _ is GnssUnknownMessageException)
                    .Cast<GnssUnknownMessageException>()
                    .Subscribe(_ =>
                        MetricGnssUnknownMessageCounter.WithLabels(sourceName, _.ProtocolId, _.MessageId).Inc()),
                src.OnError.Where(_ => _ is GnssDeserializeMessageException)
                    .Cast<GnssDeserializeMessageException>()
                    .Subscribe(_ =>
                        MetricGnssDeserializationErrorCounter.WithLabels(sourceName, _.ProtocolId, _.MessageId, _.MessageName)
                            .Inc()),
            src.OnError.Where(_ => _ is GnssPublishMessageException)
                .Cast<GnssPublishMessageException>()
                .Subscribe(_ =>
                    MetricGnssPublicationErrorCounter.WithLabels(sourceName, _.ProtocolId, _.MessageId, _.MessageName)
                        .Inc())

            );
        }


        /// Represents the label used to identify a reason for a receive error in the SBF status.
        /// This constant is used to provide a meaningful label when reporting a receive error in the SBF status.
        /// The value of this constant is "reason".
        /// @since 1.0.0
        /// @see SbfStatus
        /// /
        public const string SbfStatusRxErrorMetricLabel = "reason";

        /// <summary>
        /// Represents the gauge for the load on the receiver's CPU.
        /// </summary>
        private static readonly Gauge MetricGnssSbfStatusCpuGauge = Metrics.CreateGauge("gnss_sbf_status_cpu_percent",
            "Load on the receiver’s CPU",
            new GaugeConfiguration
            {
                LabelNames = new[] { SourceMetricLabel, }
            });

        /// <summary>
        /// A gauge metric for representing receiver temperature in degree Celsius.
        /// </summary>
        private static readonly Gauge MetricGnssSbfStatusTemperatureGauge = Metrics.CreateGauge("gnss_sbf_status_temp",
            "Receiver temperature  in degree Celsius",
            new GaugeConfiguration
            {
                LabelNames = new[] { SourceMetricLabel }
            });

        /// <summary>
        /// The gauge metric for measuring the start-up time of the GNSS SBF status.
        /// </summary>
        private static readonly Gauge MetricGnssSbfStatusStartUpTimeGauge = Metrics.CreateGauge("gnss_sbf_status_uptime",
            "Receiver temperature  in degree Celsius",
            new GaugeConfiguration
            {
                LabelNames = new[] { SourceMetricLabel }
            });

        /// Variable: MetricGnssSbfStatusRxErrorCounter
        /// Description: Represents a counter metric that indicates whether an error occurred previously
        /// Type: Counter
        /// Labels:
        /// - SourceMetricLabel (string): The label indicating the source metric
        /// - SbfStatusRxErrorMetricLabel (string): The label indicating the gnss sbf status rx error
        /// Usage:
        /// - Increment(): Increments the counter by 1
        /// - Increment(double count): Increments the counter by the specified count
        /// - IncrementTo(double targetValue, TimeSpan duration): Increments the counter to the targetValue over the specified duration
        /// - Decrement(): Decrements the counter by 1
        /// - Decrement(double count): Decrements the counter by the specified count
        /// - Reset(): Resets the counter to 0
        /// Example Usage:
        /// MetricGnssSbfStatusRxErrorCounter.Increment();
        /// MetricGnssSbfStatusRxErrorCounter.Increment(5);
        /// MetricGnssSbfStatusRxErrorCounter.Decrement();
        /// MetricGnssSbfStatusRxErrorCounter.Reset();
        /// /
        private static readonly Counter MetricGnssSbfStatusRxErrorCounter = Metrics.CreateCounter(
            "gnss_sbf_status_rx_error", "Bit field indicating whether an error occurred previously. ", new CounterConfiguration
            {
                LabelNames = new[] { SourceMetricLabel,SbfStatusRxErrorMetricLabel  }
            });


        /// <summary>
        /// Adds SBF metrics for a given GNSS connection.
        /// </summary>
        /// <param name="src">The GNSS connection to add metrics to.</param>
        /// <param name="sourceName">The name of the GNSS data source.</param>
        /// <returns>An IDisposable object that can be used to dispose the added metrics.</returns>
        public static IDisposable AddSbfMetrics(this IGnssConnection src, string sourceName)
        {
            return new CompositeDisposable(src.Filter<SbfPacketReceiverStatusRev1>().Subscribe(_ =>
            {
                MetricGnssSbfStatusCpuGauge.WithLabels(sourceName).Set(_.CPULoad);
                MetricGnssSbfStatusTemperatureGauge.WithLabels(sourceName).Set(_.Temperature - 100);
                MetricGnssSbfStatusStartUpTimeGauge.Set(_.UpTime.TotalSeconds);
                foreach (var flag in _.RxError.GetFlags())
                {
                    MetricGnssSbfStatusRxErrorCounter.WithLabels(sourceName,flag.ToString("G")).Inc();
                }

            }));
        }

    }
}
