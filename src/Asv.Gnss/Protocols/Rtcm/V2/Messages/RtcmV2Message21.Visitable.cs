using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV2Message21
{
    private static readonly Field PayloadRawDataField = ProtocolPayloadVisitor.Field(nameof(RawData), new ListType(UInt8Type.Default, 0, int.MaxValue));

    internal new static readonly StructType PayloadVisitorType = new([..RtcmV2MessageBase.PayloadVisitorType.Fields, PayloadRawDataField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        byte[] valueRawData = RawData;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadRawDataField, PayloadRawDataField.DataType, ref valueRawData,
            static () => default(byte)!,
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!EqualityComparer<byte[]>.Default.Equals(valueRawData, RawData))
        {
            RawData = valueRawData;
        }
    }
}
