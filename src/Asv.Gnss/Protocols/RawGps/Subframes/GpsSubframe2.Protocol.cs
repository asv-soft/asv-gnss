using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class GpsSubframe2
{
    private static readonly Field AodoField = GpsNavigationVisitor.Field(
        nameof(Aodo),
        UInt8Type.Default
    );
    private static readonly Field iodeField = GpsNavigationVisitor.Field(
        nameof(iode),
        Int32Type.Default
    );
    private static readonly Field CrsField = GpsNavigationVisitor.Field(
        nameof(Crs),
        DoubleType.Default
    );
    private static readonly Field delnField = GpsNavigationVisitor.Field(
        nameof(deln),
        DoubleType.Default
    );
    private static readonly Field M0Field = GpsNavigationVisitor.Field(
        nameof(M0),
        DoubleType.Default
    );
    private static readonly Field CucField = GpsNavigationVisitor.Field(
        nameof(Cuc),
        DoubleType.Default
    );
    private static readonly Field eField = GpsNavigationVisitor.Field(
        nameof(e),
        DoubleType.Default
    );
    private static readonly Field CusField = GpsNavigationVisitor.Field(
        nameof(Cus),
        DoubleType.Default
    );
    private static readonly Field AField = GpsNavigationVisitor.Field(
        nameof(A),
        DoubleType.Default
    );
    private static readonly Field ToeSecField = GpsNavigationVisitor.Field(
        nameof(ToeSec),
        DoubleType.Default
    );
    private static readonly Field FitField = GpsNavigationVisitor.Field(
        nameof(Fit),
        DoubleType.Default
    );

    protected override void SerializeFields(Span<byte> source)
    {
        RawGpsCodec.Unsigned(source, 48, 8, iode, 1);
        RawGpsCodec.Signed(source, 56, 16, Crs, GpsRawHelper.P2_5);
        RawGpsCodec.Signed(source, 72, 16, deln, GpsRawHelper.P2_43 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Signed(source, 88, 32, M0, GpsRawHelper.P2_31 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Signed(source, 120, 16, Cuc, GpsRawHelper.P2_29);
        RawGpsCodec.Unsigned(source, 136, 32, e, GpsRawHelper.P2_33);
        RawGpsCodec.Signed(source, 168, 16, Cus, GpsRawHelper.P2_29);
        RawGpsCodec.Unsigned(source, 184, 32, Math.Sqrt(A), GpsRawHelper.P2_19);
        RawGpsCodec.Unsigned(source, 216, 16, ToeSec, 16);
        if (Fit is not (0 or 4))
            throw new ArgumentOutOfRangeException(nameof(Fit));
        RawGpsCodec.Unsigned(source, 232, 1, Fit == 0 ? 0 : 1);
        RawGpsCodec.Unsigned(source, 233, 5, Aodo);
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        Int32Type.Accept(visitor, iodeField, iodeField.DataType, ref _iode);
        DoubleType.Accept(visitor, CrsField, CrsField.DataType, ref _crs);
        DoubleType.Accept(visitor, delnField, delnField.DataType, ref _deln);
        DoubleType.Accept(visitor, M0Field, M0Field.DataType, ref _m0);
        DoubleType.Accept(visitor, CucField, CucField.DataType, ref _cuc);
        DoubleType.Accept(visitor, eField, eField.DataType, ref _e);
        DoubleType.Accept(visitor, CusField, CusField.DataType, ref _cus);
        DoubleType.Accept(visitor, AField, AField.DataType, ref _a);
        DoubleType.Accept(visitor, ToeSecField, ToeSecField.DataType, ref _toeSec);
        DoubleType.Accept(visitor, FitField, FitField.DataType, ref _fit);
        UInt8Type.Accept(visitor, AodoField, ref _aodo);
    }
}
