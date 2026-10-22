using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MeasurementBlockD : IVisitable
{
    private static readonly Field PayloadRangingSourceIdField = ProtocolPayloadVisitor.Field(nameof(RangingSourceId), UInt8Type.Default);
    private static readonly Field PayloadPrc30Field = ProtocolPayloadVisitor.Field(nameof(Prc30), DoubleType.Default);
    private static readonly Field PayloadRrc30Field = ProtocolPayloadVisitor.Field(nameof(Rrc30), DoubleType.Default);
    private static readonly Field PayloadSigmaPrGndDField = ProtocolPayloadVisitor.Field(nameof(SigmaPrGndD), DoubleType.Default);
    private static readonly Field PayloadSigmaPrGndD30Field = ProtocolPayloadVisitor.Field(nameof(SigmaPrGndD30), DoubleType.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadRangingSourceIdField, PayloadPrc30Field, PayloadRrc30Field, PayloadSigmaPrGndDField, PayloadSigmaPrGndD30Field]);

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

        double valuePrc30 = Prc30;
        DoubleType.Accept(visitor, PayloadPrc30Field, PayloadPrc30Field.DataType, ref valuePrc30);
        if (!EqualityComparer<double>.Default.Equals(valuePrc30, Prc30))
        {
            Prc30 = valuePrc30;
        }

        double valueRrc30 = Rrc30;
        DoubleType.Accept(visitor, PayloadRrc30Field, PayloadRrc30Field.DataType, ref valueRrc30);
        if (!EqualityComparer<double>.Default.Equals(valueRrc30, Rrc30))
        {
            Rrc30 = valueRrc30;
        }

        double valueSigmaPrGndD = SigmaPrGndD;
        DoubleType.Accept(visitor, PayloadSigmaPrGndDField, PayloadSigmaPrGndDField.DataType, ref valueSigmaPrGndD);
        if (!EqualityComparer<double>.Default.Equals(valueSigmaPrGndD, SigmaPrGndD))
        {
            SigmaPrGndD = valueSigmaPrGndD;
        }

        double valueSigmaPrGndD30 = SigmaPrGndD30;
        DoubleType.Accept(visitor, PayloadSigmaPrGndD30Field, PayloadSigmaPrGndD30Field.DataType, ref valueSigmaPrGndD30);
        if (!EqualityComparer<double>.Default.Equals(valueSigmaPrGndD30, SigmaPrGndD30))
        {
            SigmaPrGndD30 = valueSigmaPrGndD30;
        }
    }
}
