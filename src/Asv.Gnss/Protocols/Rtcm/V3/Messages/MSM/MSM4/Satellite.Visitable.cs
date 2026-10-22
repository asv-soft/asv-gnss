using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class Satellite : IVisitable
{
    private static readonly Field RtcmSatellitePrnField = RtcmV3Visitor.Field(nameof(SatellitePrn), UInt8Type.Default, "SatellitePrn");
    private static readonly Field RtcmSignalsField = RtcmV3Visitor.Field(nameof(Signals), new ListType(Signal.RtcmVisitorType, 0, RtcmV3Visitor.MaximumElements), "Signals");
    private static readonly Field RtcmSatelliteCodeField = RtcmV3Visitor.Field(nameof(SatelliteCode), StringOptionalType.Utf8, "SatelliteCode");

    internal static readonly StructType RtcmVisitorType = new([RtcmSatellitePrnField, RtcmSignalsField, RtcmSatelliteCodeField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueSatellitePrn = SatellitePrn;
        UInt8Type.Accept(visitor, RtcmSatellitePrnField, RtcmSatellitePrnField.DataType, ref valueSatellitePrn);
        if (!EqualityComparer<byte>.Default.Equals(valueSatellitePrn, SatellitePrn))
        {
            SatellitePrn = valueSatellitePrn;
        }

        var valueSignals = Signals;
        RtcmV3Visitor.VisitArray(visitor, RtcmSignalsField, RtcmSignalsField.DataType, ref valueSignals,
            static () => new Signal(),
            static (IVisitor v, Field f, IFieldType t, ref Signal item) =>
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
        if (!ReferenceEquals(valueSignals, Signals))
        {
            Signals = valueSignals!;
        }

        string? valueSatelliteCode = SatelliteCode;
        StringOptionalType.Accept(visitor, RtcmSatelliteCodeField, RtcmSatelliteCodeField.DataType, ref valueSatelliteCode);
        if (!EqualityComparer<string>.Default.Equals(valueSatelliteCode, SatelliteCode))
        {
            SatelliteCode = valueSatelliteCode!;
        }
    }
}
