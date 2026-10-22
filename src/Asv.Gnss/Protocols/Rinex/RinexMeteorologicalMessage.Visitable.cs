using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RinexMeteorologicalMessage
{
    private static readonly Field NrEpochField = NmeaRinexVisitor.Field(nameof(Epoch), RinexEpoch.NrVisitorType);
    private static readonly Field NrObservationsField = NmeaRinexVisitor.Field(nameof(Observations), new ListType(RinexMeteorologicalValue.NrVisitorType, 0, 32768), readOnly: true);

    internal new static readonly StructType NrVisitorType = new([..RinexMessageBase.NrVisitorType.Fields, NrEpochField, NrObservationsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueEpoch = Epoch;
        StructType.Accept(visitor, NrEpochField, NrEpochField.DataType, valueEpoch);

        var valueObservations = Observations;
        NmeaRinexVisitor.VisitList(visitor, NrObservationsField, NrObservationsField.DataType, valueObservations,
            static (IVisitor visitor, Field field, IFieldType type, ref RinexMeteorologicalValue item) =>
            {
                if (item != null) StructType.Accept(visitor, field, type, item);
                else visitor.VisitUnknown(field, type);
            });
    }
}
