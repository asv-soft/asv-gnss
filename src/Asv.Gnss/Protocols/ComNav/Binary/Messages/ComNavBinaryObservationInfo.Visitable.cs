using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavBinaryObservationInfo
{
    private static readonly Field PayloadObservationsField = SbfComNavVisitor.Field(nameof(Observations), new ListType(Observation.PayloadVisitorType, 0, ushort.MaxValue), "Observations");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavBinaryMessageBase.PayloadVisitorType.Fields, PayloadObservationsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueObservations = Observations;
        SbfComNavVisitor.VisitArray(visitor, PayloadObservationsField, PayloadObservationsField.DataType, ref valueObservations, static () => new Observation(),
            static (IVisitor v, Field f, IFieldType t, ref Observation item) =>
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
        if (!ReferenceEquals(valueObservations, Observations))
        {
            Observations = valueObservations!;
        }
    }
}
