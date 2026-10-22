using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RinexSatelliteObservation : IVisitable
{
    private static readonly Field NrSatelliteField = NmeaRinexVisitor.Field(nameof(Satellite), StringOptionalType.Utf8);
    private static readonly Field NrObservationsField = NmeaRinexVisitor.Field(nameof(Observations), new ListType(RinexObservationValue.NrVisitorType, 0, 32768), readOnly: true);

    internal static readonly StructType NrVisitorType = new([NrSatelliteField, NrObservationsField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        string? valueSatellite = Satellite;
        StringOptionalType.Accept(visitor, NrSatelliteField, NrSatelliteField.DataType, ref valueSatellite);
        if (!EqualityComparer<string>.Default.Equals(valueSatellite, Satellite))
        {
            Satellite = valueSatellite!;
        }

        var valueObservations = Observations;
        NmeaRinexVisitor.VisitList(visitor, NrObservationsField, NrObservationsField.DataType, valueObservations,
            static (IVisitor visitor, Field field, IFieldType type, ref RinexObservationValue item) =>
            {
                if (item != null) StructType.Accept(visitor, field, type, item);
                else visitor.VisitUnknown(field, type);
            });
    }
}
