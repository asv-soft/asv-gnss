using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketGpsRawCa
{
    private static readonly Field PayloadGpsSubFrameField = SbfComNavVisitor.Field(nameof(GpsSubFrame), new StructType([]), "GpsSubFrame");

    internal new static readonly StructType PayloadVisitorType = new([..SbfPacketGnssRawNavMsgBase.PayloadVisitorType.Fields, PayloadGpsSubFrameField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueGpsSubFrame = GpsSubFrame;
        if (valueGpsSubFrame is not null)
        {
            StructType.Accept(visitor, PayloadGpsSubFrameField, PayloadGpsSubFrameField.DataType, valueGpsSubFrame);
        }
        else
        {
            visitor.VisitUnknown(PayloadGpsSubFrameField, PayloadGpsSubFrameField.DataType);
        }
    }
}
