using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MessageBlockType11
{
    private static readonly Field PayloadModifiedZCountField = ProtocolPayloadVisitor.Field(nameof(ModifiedZCount), DoubleType.Default);
    private static readonly Field PayloadAdditionalMessageFlagField = ProtocolPayloadVisitor.Field(nameof(AdditionalMessageFlag), Int32Type.Default, enumType: typeof(AdditionalMessageEnum));
    private static readonly Field PayloadMeasurementTypeField = ProtocolPayloadVisitor.Field(nameof(MeasurementType), UInt8Type.Default, enumType: typeof(MeasurementTypeEnum));
    private static readonly Field PayloadPDField = ProtocolPayloadVisitor.Field(nameof(PD), DoubleType.Default);
    private static readonly Field PayloadMeasurementsField = ProtocolPayloadVisitor.Field(nameof(Measurements), new ListType(MeasurementBlockD.PayloadVisitorType, 0, int.MaxValue), readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..GbasMessageBlockBase.PayloadVisitorType.Fields, PayloadModifiedZCountField, PayloadAdditionalMessageFlagField, PayloadMeasurementTypeField, PayloadPDField, PayloadMeasurementsField]);

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

        double valuePD = PD;
        DoubleType.Accept(visitor, PayloadPDField, PayloadPDField.DataType, ref valuePD);
        if (!EqualityComparer<double>.Default.Equals(valuePD, PD))
        {
            PD = valuePD;
        }

        List<MeasurementBlockD> valueMeasurements = Measurements;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadMeasurementsField, PayloadMeasurementsField.DataType, ref valueMeasurements,
            static () => new MeasurementBlockD(),
            static (IVisitor v, Field f, IFieldType t, ref MeasurementBlockD item) =>
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
