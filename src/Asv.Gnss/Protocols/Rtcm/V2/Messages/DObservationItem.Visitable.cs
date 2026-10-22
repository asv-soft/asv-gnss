using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class DObservationItem : IVisitable
{
    private static readonly Field PayloadSatelliteIdField = ProtocolPayloadVisitor.Field(nameof(SatelliteId), Int32Type.Default);
    private static readonly Field PayloadPrnField = ProtocolPayloadVisitor.Field(nameof(Prn), UInt8Type.Default);
    private static readonly Field PayloadPrcField = ProtocolPayloadVisitor.Field(nameof(Prc), DoubleType.Default);
    private static readonly Field PayloadRrcField = ProtocolPayloadVisitor.Field(nameof(Rrc), DoubleType.Default);
    private static readonly Field PayloadIodField = ProtocolPayloadVisitor.Field(nameof(Iod), UInt8Type.Default);
    private static readonly Field PayloadUdreField = ProtocolPayloadVisitor.Field(nameof(Udre), Int32Type.Default, enumType: typeof(SatUdreEnum));
    private static readonly Field PayloadTkField = ProtocolPayloadVisitor.Field(nameof(Tk), DateTimeType.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadSatelliteIdField, PayloadPrnField, PayloadPrcField, PayloadRrcField, PayloadIodField, PayloadUdreField, PayloadTkField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        int valueSatelliteId = SatelliteId;
        Int32Type.Accept(visitor, PayloadSatelliteIdField, PayloadSatelliteIdField.DataType, ref valueSatelliteId);
        if (!EqualityComparer<int>.Default.Equals(valueSatelliteId, SatelliteId))
        {
            SatelliteId = valueSatelliteId;
        }

        byte valuePrn = Prn;
        UInt8Type.Accept(visitor, PayloadPrnField, PayloadPrnField.DataType, ref valuePrn);
        if (!EqualityComparer<byte>.Default.Equals(valuePrn, Prn))
        {
            Prn = valuePrn;
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

        byte valueIod = Iod;
        UInt8Type.Accept(visitor, PayloadIodField, PayloadIodField.DataType, ref valueIod);
        if (!EqualityComparer<byte>.Default.Equals(valueIod, Iod))
        {
            Iod = valueIod;
        }

        SatUdreEnum valueUdre = Udre;
        var numericUdre = (int)valueUdre;
        Int32Type.Accept(visitor, PayloadUdreField, PayloadUdreField.DataType, ref numericUdre);
        valueUdre = (SatUdreEnum)numericUdre;
        if (!EqualityComparer<SatUdreEnum>.Default.Equals(valueUdre, Udre))
        {
            Udre = valueUdre;
        }

        DateTime valueTk = Tk;
        DateTimeType.Accept(visitor, PayloadTkField, PayloadTkField.DataType, ref valueTk);
        if (!EqualityComparer<DateTime>.Default.Equals(valueTk, Tk))
        {
            Tk = valueTk;
        }
    }
}
