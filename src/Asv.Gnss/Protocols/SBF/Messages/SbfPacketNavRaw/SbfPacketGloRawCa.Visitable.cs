using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketGloRawCa
{
    private static readonly Field PayloadGlonassWordField = SbfComNavVisitor.Field(nameof(GlonassWord), new StructType([]), "GlonassWord");

    internal new static readonly StructType PayloadVisitorType = new([..SbfPacketGnssRawNavMsgBase.PayloadVisitorType.Fields, PayloadGlonassWordField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueGlonassWord = GlonassWord;
        if (valueGlonassWord is not null)
        {
            StructType.Accept(visitor, PayloadGlonassWordField, PayloadGlonassWordField.DataType, valueGlonassWord);
        }
        else
        {
            visitor.VisitUnknown(PayloadGlonassWordField, PayloadGlonassWordField.DataType);
        }
    }
}
