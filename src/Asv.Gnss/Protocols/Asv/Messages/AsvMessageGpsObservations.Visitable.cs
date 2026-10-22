using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessageGpsObservations
{
    private static readonly Field PayloadObservationsField = ProtocolPayloadVisitor.Field(nameof(Observations), new ListType(AsvGpsObservation.PayloadVisitorType, 0, int.MaxValue));
    private static readonly Field PayloadTowField = ProtocolPayloadVisitor.Field(nameof(Tow), DateTimeType.Default);
    private static readonly Field PayloadTimeOffsetField = ProtocolPayloadVisitor.Field(nameof(TimeOffset), DoubleType.Default);

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadObservationsField, PayloadTowField, PayloadTimeOffsetField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        AsvGpsObservation[] valueObservations = Observations;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadObservationsField, PayloadObservationsField.DataType, ref valueObservations,
            static () => new AsvGpsObservation(),
            static (IVisitor v, Field f, IFieldType t, ref AsvGpsObservation item) =>
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
        if (!EqualityComparer<AsvGpsObservation[]>.Default.Equals(valueObservations, Observations))
        {
            Observations = valueObservations;
        }

        DateTime valueTow = Tow;
        DateTimeType.Accept(visitor, PayloadTowField, PayloadTowField.DataType, ref valueTow);
        if (!EqualityComparer<DateTime>.Default.Equals(valueTow, Tow))
        {
            Tow = valueTow;
        }

        double valueTimeOffset = TimeOffset;
        DoubleType.Accept(visitor, PayloadTimeOffsetField, PayloadTimeOffsetField.DataType, ref valueTimeOffset);
        if (!EqualityComparer<double>.Default.Equals(valueTimeOffset, TimeOffset))
        {
            TimeOffset = valueTimeOffset;
        }
    }
}
