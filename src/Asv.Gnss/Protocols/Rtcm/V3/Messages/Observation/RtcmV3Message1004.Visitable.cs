using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1004
{
    private static readonly Field RtcmSatellitesField = RtcmV3Visitor.Field(nameof(Satellites), new ListType(GPSSatellite.RtcmVisitorType, 0, RtcmV3Visitor.MaximumElements), "Satellites");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3RTKObservableMessagesBase.RtcmVisitorType.Fields, RtcmSatellitesField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSatellites = Satellites;
        RtcmV3Visitor.VisitArray(visitor, RtcmSatellitesField, RtcmSatellitesField.DataType, ref valueSatellites,
            static () => new GPSSatellite(),
            static (IVisitor v, Field f, IFieldType t, ref GPSSatellite item) =>
            {
                if (item != null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valueSatellites, Satellites))
        {
            Satellites = valueSatellites!;
        }
    }
}
