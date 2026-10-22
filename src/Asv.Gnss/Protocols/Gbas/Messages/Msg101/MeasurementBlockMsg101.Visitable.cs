using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MeasurementBlockMsg101 : IVisitable
{
    private static readonly Field PayloadRangingSourceIdField = ProtocolPayloadVisitor.Field(nameof(RangingSourceId), UInt8Type.Default);
    private static readonly Field PayloadIodField = ProtocolPayloadVisitor.Field(nameof(Iod), UInt8Type.Default);
    private static readonly Field PayloadPrcField = ProtocolPayloadVisitor.Field(nameof(Prc), DoubleType.Default);
    private static readonly Field PayloadRrcField = ProtocolPayloadVisitor.Field(nameof(Rrc), DoubleType.Default);
    private static readonly Field PayloadSigmaPrGndField = ProtocolPayloadVisitor.Field(nameof(SigmaPrGnd), DoubleType.Default);
    private static readonly Field PayloadBField = ProtocolPayloadVisitor.Field(nameof(B), DoubleOptionalType.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadRangingSourceIdField, PayloadIodField, PayloadPrcField, PayloadRrcField, PayloadSigmaPrGndField, PayloadBField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        byte valueRangingSourceId = RangingSourceId;
        UInt8Type.Accept(visitor, PayloadRangingSourceIdField, PayloadRangingSourceIdField.DataType, ref valueRangingSourceId);
        if (!EqualityComparer<byte>.Default.Equals(valueRangingSourceId, RangingSourceId))
        {
            RangingSourceId = valueRangingSourceId;
        }

        byte valueIod = Iod;
        UInt8Type.Accept(visitor, PayloadIodField, PayloadIodField.DataType, ref valueIod);
        if (!EqualityComparer<byte>.Default.Equals(valueIod, Iod))
        {
            Iod = valueIod;
        }

        double valuePrc = Prc;
        DoubleType.Accept(visitor, PayloadPrcField, PayloadPrcField.DataType, ref valuePrc);
        if (!EqualityComparer<double>.Default.Equals(valuePrc, Prc))
        {
            Prc = valuePrc;
        }

        double valueRrc = Rrc;
        DoubleType.Accept(visitor, PayloadRrcField, PayloadRrcField.DataType, ref valueRrc);
        if (!EqualityComparer<double>.Default.Equals(valueRrc, Rrc))
        {
            Rrc = valueRrc;
        }

        double valueSigmaPrGnd = SigmaPrGnd;
        DoubleType.Accept(visitor, PayloadSigmaPrGndField, PayloadSigmaPrGndField.DataType, ref valueSigmaPrGnd);
        if (!EqualityComparer<double>.Default.Equals(valueSigmaPrGnd, SigmaPrGnd))
        {
            SigmaPrGnd = valueSigmaPrGnd;
        }

        double? valueB = B;
        DoubleOptionalType.Accept(visitor, PayloadBField, PayloadBField.DataType, ref valueB);
        if (!EqualityComparer<double?>.Default.Equals(valueB, B))
        {
            B = valueB;
        }
    }
}
