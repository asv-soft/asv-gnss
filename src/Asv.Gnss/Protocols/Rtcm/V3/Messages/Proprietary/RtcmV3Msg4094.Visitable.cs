using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Msg4094
{
    private static readonly Field RtcmDataField = RtcmV3Visitor.Field(nameof(Data), new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), "Data");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmDataField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueData = Data;
        RtcmV3Visitor.VisitArray(visitor, RtcmDataField, RtcmDataField.DataType, ref valueData,
            static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueData, Data))
        {
            Data = valueData!;
        }
    }
}
