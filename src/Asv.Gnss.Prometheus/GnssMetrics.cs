using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Asv.Common;
using Prometheus;

namespace Asv.Gnss
{
    public static class GnssMetrics
    {
        private const string SourceMetricLabel = "src";
        private const string ProtocolMetricLabel = "protocol";
        private const string MessageIdMetricLabel = "id";
        private const string MessageNameMetricLabel = "name";

        private static readonly Counter MetricGnssRxBytesCounter = Metrics.CreateCounter("gnss_rx_bytes",
            "Input bytes counter for GNSS connection",
            new CounterConfiguration
            {
                LabelNames = new[] { SourceMetricLabel }
            });

        private static readonly Counter MetricGnssTxBytesCounter = Metrics.CreateCounter("gnss_tx_bytes",
            "Input bytes counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[] { SourceMetricLabel }
            });

        private static readonly Counter MetricGnssRxMessageCounter = Metrics.CreateCounter("gnss_rx_message_count",
            "Input message counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[] { SourceMetricLabel, ProtocolMetricLabel,MessageIdMetricLabel, MessageNameMetricLabel }
            });

        private static readonly Counter MetricGnssTxMessageCounter = Metrics.CreateCounter("gnss_tx_message_count",
            "Output message counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel, MessageIdMetricLabel, MessageNameMetricLabel }
            });

        private static readonly Counter MetricGnssErrorCounter = Metrics.CreateCounter("gnss_rx_message_errors_count",
            "Any errors counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel }
            });

        private static readonly Counter MetricGnssCrcErrorCounter = Metrics.CreateCounter(
            "gnss_rx_message_crc_error_count", "Crc error counter for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel }
            });

        private static readonly Counter MetricGnssReadNotAllDataWhenDeserializePacketError = Metrics.CreateCounter(
            "gnss_rx_message_read_not_all_data_error_count",
            "Read not all data after deserialization message errors for GNSS connection", new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel,MessageIdMetricLabel  }
            });

        private static readonly Counter MetricGnssUnknownMessageCounter = Metrics.CreateCounter(
            "gnss_rx_message_unknown_counter", "Unknown message ID counter for GNSS connection",
            new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel, MessageIdMetricLabel }
            });

        private static readonly Counter MetricGnssDeserializationErrorCounter = Metrics.CreateCounter(
            "gnss_rx_message_deserialization_error_count", "Deserialization error counter for GNSS connection",
            new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel, MessageIdMetricLabel, MessageNameMetricLabel }
            });

        private static readonly Counter MetricGnssPublicationErrorCounter = Metrics.CreateCounter(
            "gnss_rx_message_publication_error_count", "Publication error counter for GNSS connection",
            new CounterConfiguration
            {
                LabelNames = new[]
                    { SourceMetricLabel, ProtocolMetricLabel, MessageIdMetricLabel, MessageNameMetricLabel }
            });

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


        public const string SbfStatusRxErrorMetricLabel = "reason";

        private static readonly Gauge MetricGnssSbfStatusCpuGauge = Metrics.CreateGauge("gnss_sbf_status_cpu_percent",
            "Load on the receiver’s CPU",
            new GaugeConfiguration
            {
                LabelNames = new[] { SourceMetricLabel, }
            });

        private static readonly Gauge MetricGnssSbfStatusTemperatureGauge = Metrics.CreateGauge("gnss_sbf_status_temp",
            "Receiver temperature  in degree Celsius",
            new GaugeConfiguration
            {
                LabelNames = new[] { SourceMetricLabel }
            });
        private static readonly Gauge MetricGnssSbfStatusStartUpTimeGauge = Metrics.CreateGauge("gnss_sbf_status_uptime",
            "Receiver temperature  in degree Celsius",
            new GaugeConfiguration
            {
                LabelNames = new[] { SourceMetricLabel }
            });
        private static readonly Counter MetricGnssSbfStatusRxErrorCounter = Metrics.CreateCounter(
            "gnss_sbf_status_rx_error", "Bit field indicating whether an error occurred previously. ", new CounterConfiguration
            {
                LabelNames = new[] { SourceMetricLabel,SbfStatusRxErrorMetricLabel  }
            });


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
