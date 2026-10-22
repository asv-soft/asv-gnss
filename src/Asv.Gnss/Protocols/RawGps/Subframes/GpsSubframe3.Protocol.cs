using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class GpsSubframe3
{
    private static readonly Field CicField = GpsNavigationVisitor.Field(
        nameof(Cic),
        DoubleType.Default
    );
    private static readonly Field OMG0Field = GpsNavigationVisitor.Field(
        nameof(OMG0),
        DoubleType.Default
    );
    private static readonly Field CisField = GpsNavigationVisitor.Field(
        nameof(Cis),
        DoubleType.Default
    );
    private static readonly Field i0Field = GpsNavigationVisitor.Field(
        nameof(i0),
        DoubleType.Default
    );
    private static readonly Field CrcField = GpsNavigationVisitor.Field(
        nameof(Crc),
        DoubleType.Default
    );
    private static readonly Field omgField = GpsNavigationVisitor.Field(
        nameof(omg),
        DoubleType.Default
    );
    private static readonly Field OMGdField = GpsNavigationVisitor.Field(
        nameof(OMGd),
        DoubleType.Default
    );
    private static readonly Field iodeField = GpsNavigationVisitor.Field(
        nameof(iode),
        Int32Type.Default
    );
    private static readonly Field idotField = GpsNavigationVisitor.Field(
        nameof(idot),
        DoubleType.Default
    );

    protected override void SerializeFields(Span<byte> source)
    {
        RawGpsCodec.Signed(source, 48, 16, Cic, GpsRawHelper.P2_29);
        RawGpsCodec.Signed(source, 64, 32, OMG0, GpsRawHelper.P2_31 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Signed(source, 96, 16, Cis, GpsRawHelper.P2_29);
        RawGpsCodec.Signed(source, 112, 32, i0, GpsRawHelper.P2_31 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Signed(source, 144, 16, Crc, GpsRawHelper.P2_5);
        RawGpsCodec.Signed(source, 160, 32, omg, GpsRawHelper.P2_31 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Signed(source, 192, 24, OMGd, GpsRawHelper.P2_43 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Unsigned(source, 216, 8, iode, 1);
        RawGpsCodec.Signed(source, 224, 14, idot, GpsRawHelper.P2_43 * GpsRawHelper.SC2RAD);
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        DoubleType.Accept(visitor, CicField, CicField.DataType, ref _cic);
        DoubleType.Accept(visitor, OMG0Field, OMG0Field.DataType, ref _oMG0);
        DoubleType.Accept(visitor, CisField, CisField.DataType, ref _cis);
        DoubleType.Accept(visitor, i0Field, i0Field.DataType, ref _i0);
        DoubleType.Accept(visitor, CrcField, CrcField.DataType, ref _crc);
        DoubleType.Accept(visitor, omgField, omgField.DataType, ref _omg);
        DoubleType.Accept(visitor, OMGdField, OMGdField.DataType, ref _oMGd);
        Int32Type.Accept(visitor, iodeField, iodeField.DataType, ref _iode);
        DoubleType.Accept(visitor, idotField, idotField.DataType, ref _idot);
    }
}
