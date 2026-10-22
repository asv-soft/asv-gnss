using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessageGloObservations
{
    private static readonly Field PayloadObservationsField = ProtocolPayloadVisitor.Field(nameof(Observations), new ListType(AsvGloObservation.PayloadVisitorType, 0, int.MaxValue));
    private static readonly Field PayloadTodField = ProtocolPayloadVisitor.Field(nameof(Tod), DateTimeType.Default);
    private static readonly Field PayloadTimeOffsetField = ProtocolPayloadVisitor.Field(nameof(TimeOffset), DoubleType.Default);

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadObservationsField, PayloadTodField, PayloadTimeOffsetField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        AsvGloObservation[] valueObservations = Observations;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadObservationsField, PayloadObservationsField.DataType, ref valueObservations,
            static () => new AsvGloObservation(),
            static (IVisitor v, Field f, IFieldType t, ref AsvGloObservation item) =>
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
        if (!EqualityComparer<AsvGloObservation[]>.Default.Equals(valueObservations, Observations))
        {
            Observations = valueObservations;
        }

        DateTime valueTod = Tod;
        DateTimeType.Accept(visitor, PayloadTodField, PayloadTodField.DataType, ref valueTod);
        if (!EqualityComparer<DateTime>.Default.Equals(valueTod, Tod))
        {
            Tod = valueTod;
        }

        double valueTimeOffset = TimeOffset;
        DoubleType.Accept(visitor, PayloadTimeOffsetField, PayloadTimeOffsetField.DataType, ref valueTimeOffset);
        if (!EqualityComparer<double>.Default.Equals(valueTimeOffset, TimeOffset))
        {
            TimeOffset = valueTimeOffset;
        }
    }
}
