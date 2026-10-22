using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MessageBlockType101
{
    private static readonly Field PayloadModifiedZCountField = ProtocolPayloadVisitor.Field(nameof(ModifiedZCount), DoubleType.Default);
    private static readonly Field PayloadAdditionalMessageFlagField = ProtocolPayloadVisitor.Field(nameof(AdditionalMessageFlag), Int32Type.Default, enumType: typeof(AdditionalMessageEnum));
    private static readonly Field PayloadMeasurementTypeField = ProtocolPayloadVisitor.Field(nameof(MeasurementType), UInt8Type.Default, enumType: typeof(MeasurementTypeEnum));
    private static readonly Field PayloadPField = ProtocolPayloadVisitor.Field(nameof(P), DoubleType.Default);
    private static readonly Field PayloadEphemerisCrcField = ProtocolPayloadVisitor.Field(nameof(EphemerisCrc), UInt16Type.Default);
    private static readonly Field PayloadSourceAvailabilityDurationField = ProtocolPayloadVisitor.Field(nameof(SourceAvailabilityDuration), UInt16OptionalType.Default);
    private static readonly Field PayloadMeasurementsField = ProtocolPayloadVisitor.Field(nameof(Measurements), new ListType(MeasurementBlockMsg101.PayloadVisitorType, 0, int.MaxValue), readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..GbasMessageBlockBase.PayloadVisitorType.Fields, PayloadModifiedZCountField, PayloadAdditionalMessageFlagField, PayloadMeasurementTypeField, PayloadPField, PayloadEphemerisCrcField, PayloadSourceAvailabilityDurationField, PayloadMeasurementsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        double valueModifiedZCount = ModifiedZCount;
        DoubleType.Accept(visitor, PayloadModifiedZCountField, PayloadModifiedZCountField.DataType, ref valueModifiedZCount);
        if (!EqualityComparer<double>.Default.Equals(valueModifiedZCount, ModifiedZCount))
        {
            ModifiedZCount = valueModifiedZCount;
        }

        AdditionalMessageEnum valueAdditionalMessageFlag = AdditionalMessageFlag;
        var numericAdditionalMessageFlag = (int)valueAdditionalMessageFlag;
        Int32Type.Accept(visitor, PayloadAdditionalMessageFlagField, PayloadAdditionalMessageFlagField.DataType, ref numericAdditionalMessageFlag);
        valueAdditionalMessageFlag = (AdditionalMessageEnum)numericAdditionalMessageFlag;
        if (!EqualityComparer<AdditionalMessageEnum>.Default.Equals(valueAdditionalMessageFlag, AdditionalMessageFlag))
        {
            AdditionalMessageFlag = valueAdditionalMessageFlag;
        }

        MeasurementTypeEnum valueMeasurementType = MeasurementType;
        var numericMeasurementType = (byte)valueMeasurementType;
        UInt8Type.Accept(visitor, PayloadMeasurementTypeField, PayloadMeasurementTypeField.DataType, ref numericMeasurementType);
        valueMeasurementType = (MeasurementTypeEnum)numericMeasurementType;
        if (!EqualityComparer<MeasurementTypeEnum>.Default.Equals(valueMeasurementType, MeasurementType))
        {
            MeasurementType = valueMeasurementType;
        }

        double valueP = P;
        DoubleType.Accept(visitor, PayloadPField, PayloadPField.DataType, ref valueP);
        if (!EqualityComparer<double>.Default.Equals(valueP, P))
        {
            P = valueP;
        }

        ushort valueEphemerisCrc = EphemerisCrc;
        UInt16Type.Accept(visitor, PayloadEphemerisCrcField, PayloadEphemerisCrcField.DataType, ref valueEphemerisCrc);
        if (!EqualityComparer<ushort>.Default.Equals(valueEphemerisCrc, EphemerisCrc))
        {
            EphemerisCrc = valueEphemerisCrc;
        }

        ushort? valueSourceAvailabilityDuration = SourceAvailabilityDuration;
        UInt16OptionalType.Accept(visitor, PayloadSourceAvailabilityDurationField, PayloadSourceAvailabilityDurationField.DataType, ref valueSourceAvailabilityDuration);
        if (!EqualityComparer<ushort?>.Default.Equals(valueSourceAvailabilityDuration, SourceAvailabilityDuration))
        {
            SourceAvailabilityDuration = valueSourceAvailabilityDuration;
        }

        List<MeasurementBlockMsg101> valueMeasurements = Measurements;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadMeasurementsField, PayloadMeasurementsField.DataType, ref valueMeasurements,
            static () => new MeasurementBlockMsg101(),
            static (IVisitor v, Field f, IFieldType t, ref MeasurementBlockMsg101 item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
    }
}
