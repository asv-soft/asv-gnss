using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageGbs
{
    private static readonly Field NrTimeUtcField = NmeaRinexVisitor.Field(nameof(TimeUtc), TimeOnlyOptionalType.Default, readOnly: true);
    private static readonly Field NrLatitudeErrorField = NmeaRinexVisitor.Field(nameof(LatitudeError), DoubleType.Default);
    private static readonly Field NrLongitudeErrorField = NmeaRinexVisitor.Field(nameof(LongitudeError), DoubleType.Default);
    private static readonly Field NrAltitudeErrorField = NmeaRinexVisitor.Field(nameof(AltitudeError), DoubleType.Default);
    private static readonly Field NrFailedSatelliteIdField = NmeaRinexVisitor.Field(nameof(FailedSatelliteId), Int32OptionalType.Default);
    private static readonly Field NrProbabilityOfMissedDetectionField = NmeaRinexVisitor.Field(nameof(ProbabilityOfMissedDetection), DoubleType.Default);
    private static readonly Field NrBiasEstimateField = NmeaRinexVisitor.Field(nameof(BiasEstimate), DoubleType.Default);
    private static readonly Field NrBiasEstimateStandardDeviationField = NmeaRinexVisitor.Field(nameof(BiasEstimateStandardDeviation), DoubleType.Default);
    private static readonly Field NrSystemIdField = NmeaRinexVisitor.Field(nameof(SystemId), Int32OptionalType.Default);

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrTimeUtcField, NrLatitudeErrorField, NrLongitudeErrorField, NrAltitudeErrorField, NrFailedSatelliteIdField, NrProbabilityOfMissedDetectionField, NrBiasEstimateField, NrBiasEstimateStandardDeviationField, NrSystemIdField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueTimeUtc = TimeUtc;
        TimeOnlyOptionalType.Accept(visitor, NrTimeUtcField, NrTimeUtcField.DataType, ref valueTimeUtc);

        var valueLatitudeError = LatitudeError;
        DoubleType.Accept(visitor, NrLatitudeErrorField, NrLatitudeErrorField.DataType, ref valueLatitudeError);
        if (!EqualityComparer<double>.Default.Equals(valueLatitudeError, LatitudeError))
        {
            LatitudeError = valueLatitudeError;
        }

        var valueLongitudeError = LongitudeError;
        DoubleType.Accept(visitor, NrLongitudeErrorField, NrLongitudeErrorField.DataType, ref valueLongitudeError);
        if (!EqualityComparer<double>.Default.Equals(valueLongitudeError, LongitudeError))
        {
            LongitudeError = valueLongitudeError;
        }

        var valueAltitudeError = AltitudeError;
        DoubleType.Accept(visitor, NrAltitudeErrorField, NrAltitudeErrorField.DataType, ref valueAltitudeError);
        if (!EqualityComparer<double>.Default.Equals(valueAltitudeError, AltitudeError))
        {
            AltitudeError = valueAltitudeError;
        }

        var valueFailedSatelliteId = FailedSatelliteId;
        Int32OptionalType.Accept(visitor, NrFailedSatelliteIdField, NrFailedSatelliteIdField.DataType, ref valueFailedSatelliteId);
        if (!EqualityComparer<int?>.Default.Equals(valueFailedSatelliteId, FailedSatelliteId))
        {
            FailedSatelliteId = valueFailedSatelliteId;
        }

        var valueProbabilityOfMissedDetection = ProbabilityOfMissedDetection;
        DoubleType.Accept(visitor, NrProbabilityOfMissedDetectionField, NrProbabilityOfMissedDetectionField.DataType, ref valueProbabilityOfMissedDetection);
        if (!EqualityComparer<double>.Default.Equals(valueProbabilityOfMissedDetection, ProbabilityOfMissedDetection))
        {
            ProbabilityOfMissedDetection = valueProbabilityOfMissedDetection;
        }

        var valueBiasEstimate = BiasEstimate;
        DoubleType.Accept(visitor, NrBiasEstimateField, NrBiasEstimateField.DataType, ref valueBiasEstimate);
        if (!EqualityComparer<double>.Default.Equals(valueBiasEstimate, BiasEstimate))
        {
            BiasEstimate = valueBiasEstimate;
        }

        var valueBiasEstimateStandardDeviation = BiasEstimateStandardDeviation;
        DoubleType.Accept(visitor, NrBiasEstimateStandardDeviationField, NrBiasEstimateStandardDeviationField.DataType, ref valueBiasEstimateStandardDeviation);
        if (!EqualityComparer<double>.Default.Equals(valueBiasEstimateStandardDeviation, BiasEstimateStandardDeviation))
        {
            BiasEstimateStandardDeviation = valueBiasEstimateStandardDeviation;
        }

        var valueSystemId = SystemId;
        Int32OptionalType.Accept(visitor, NrSystemIdField, NrSystemIdField.DataType, ref valueSystemId);
        if (!EqualityComparer<int?>.Default.Equals(valueSystemId, SystemId))
        {
            SystemId = valueSystemId;
        }
    }
}
