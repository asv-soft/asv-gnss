using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3NetworkResidual : IVisitable
{
    private static readonly Field RtcmSatelliteIdField = RtcmV3Visitor.Field(nameof(SatelliteId), UInt8Type.Default, "SatelliteId");
    private static readonly Field RtcmSOcField = RtcmV3Visitor.Field(nameof(SOc), DoubleType.Default, "SOc");
    private static readonly Field RtcmSOdField = RtcmV3Visitor.Field(nameof(SOd), DoubleType.Default, "SOd");
    private static readonly Field RtcmSOhField = RtcmV3Visitor.Field(nameof(SOh), DoubleType.Default, "SOh");
    private static readonly Field RtcmSIcField = RtcmV3Visitor.Field(nameof(SIc), DoubleType.Default, "SIc");
    private static readonly Field RtcmSIdField = RtcmV3Visitor.Field(nameof(SId), DoubleType.Default, "SId");

    internal static readonly StructType RtcmVisitorType = new([RtcmSatelliteIdField, RtcmSOcField, RtcmSOdField, RtcmSOhField, RtcmSIcField, RtcmSIdField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueSatelliteId = SatelliteId;
        UInt8Type.Accept(visitor, RtcmSatelliteIdField, RtcmSatelliteIdField.DataType, ref valueSatelliteId);
        if (!EqualityComparer<byte>.Default.Equals(valueSatelliteId, SatelliteId))
        {
            SatelliteId = valueSatelliteId;
        }

        var valueSOc = SOc;
        DoubleType.Accept(visitor, RtcmSOcField, RtcmSOcField.DataType, ref valueSOc);
        if (!EqualityComparer<double>.Default.Equals(valueSOc, SOc))
        {
            SOc = valueSOc;
        }

        var valueSOd = SOd;
        DoubleType.Accept(visitor, RtcmSOdField, RtcmSOdField.DataType, ref valueSOd);
        if (!EqualityComparer<double>.Default.Equals(valueSOd, SOd))
        {
            SOd = valueSOd;
        }

        var valueSOh = SOh;
        DoubleType.Accept(visitor, RtcmSOhField, RtcmSOhField.DataType, ref valueSOh);
        if (!EqualityComparer<double>.Default.Equals(valueSOh, SOh))
        {
            SOh = valueSOh;
        }

        var valueSIc = SIc;
        DoubleType.Accept(visitor, RtcmSIcField, RtcmSIcField.DataType, ref valueSIc);
        if (!EqualityComparer<double>.Default.Equals(valueSIc, SIc))
        {
            SIc = valueSIc;
        }

        var valueSId = SId;
        DoubleType.Accept(visitor, RtcmSIdField, RtcmSIdField.DataType, ref valueSId);
        if (!EqualityComparer<double>.Default.Equals(valueSId, SId))
        {
            SId = valueSId;
        }
    }
}
