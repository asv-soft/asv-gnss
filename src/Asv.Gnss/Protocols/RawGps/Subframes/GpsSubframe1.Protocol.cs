using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class GpsSubframe1
{
    private static readonly Field WeekNumberField = GpsNavigationVisitor.Field(
        nameof(WeekNumber),
        Int32Type.Default
    );
    private static readonly Field CodeField = GpsNavigationVisitor.Field(
        nameof(Code),
        Int32Type.Default
    );
    private static readonly Field SatelliteAccuracyField = GpsNavigationVisitor.Field(
        nameof(SatelliteAccuracy),
        UInt8Type.Default
    );
    private static readonly Field SatelliteHealthField = GpsNavigationVisitor.Field(
        nameof(SatelliteHealth),
        UInt8Type.Default
    );
    private static readonly Field FlagField = GpsNavigationVisitor.Field(
        nameof(Flag),
        Int32Type.Default
    );
    private static readonly Field TocSecField = GpsNavigationVisitor.Field(
        nameof(TocSec),
        DoubleType.Default
    );
    private static readonly Field Af2Field = GpsNavigationVisitor.Field(
        nameof(Af2),
        DoubleType.Default
    );
    private static readonly Field Af1Field = GpsNavigationVisitor.Field(
        nameof(Af1),
        DoubleType.Default
    );
    private static readonly Field Af0Field = GpsNavigationVisitor.Field(
        nameof(Af0),
        DoubleType.Default
    );
    private static readonly Field iodcField = GpsNavigationVisitor.Field(
        nameof(iodc),
        Int32Type.Default
    );
    private static readonly Field TgdField = GpsNavigationVisitor.Field(
        nameof(Tgd),
        new ArrayType(DoubleType.Default, 1)
    );

    protected override void SerializeFields(Span<byte> source)
    {
        RawGpsCodec.Unsigned(source, 48, 10, WeekNumber, 1);
        RawGpsCodec.Unsigned(source, 58, 2, Code, 1);
        RawGpsCodec.Unsigned(source, 60, 4, SatelliteAccuracy, 1);
        RawGpsCodec.Unsigned(source, 64, 6, SatelliteHealth, 1);
        RawGpsCodec.Unsigned(source, 72, 1, Flag, 1);
        RawGpsCodec.Unsigned(source, 176, 16, TocSec, 16);
        RawGpsCodec.Signed(source, 192, 8, Af2, GpsRawHelper.P2_55);
        RawGpsCodec.Signed(source, 200, 16, Af1, GpsRawHelper.P2_43);
        RawGpsCodec.Signed(source, 216, 22, Af0, GpsRawHelper.P2_31);
        if (iodc is < 0 or > 1023)
            throw new ArgumentOutOfRangeException(nameof(iodc));
        RawGpsCodec.Unsigned(source, 70, 2, iodc >> 8);
        RawGpsCodec.Unsigned(source, 168, 8, iodc & 255);
        GpsNavigationVisitor.RequireLength(Tgd, 1, nameof(Tgd));
        if (!(Tgd[0] == 0 && RawGpsCodec.Read(source, 160, 8) == 128))
            RawGpsCodec.Signed(source, 160, 8, Tgd[0], GpsRawHelper.P2_31);
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        Int32Type.Accept(visitor, WeekNumberField, WeekNumberField.DataType, ref _weekNumber);
        Int32Type.Accept(visitor, CodeField, CodeField.DataType, ref _code);
        UInt8Type.Accept(
            visitor,
            SatelliteAccuracyField,
            SatelliteAccuracyField.DataType,
            ref _satelliteAccuracy
        );
        UInt8Type.Accept(
            visitor,
            SatelliteHealthField,
            SatelliteHealthField.DataType,
            ref _satelliteHealth
        );
        Int32Type.Accept(visitor, FlagField, FlagField.DataType, ref _flag);
        DoubleType.Accept(visitor, TocSecField, TocSecField.DataType, ref _tocSec);
        DoubleType.Accept(visitor, Af2Field, Af2Field.DataType, ref _af2);
        DoubleType.Accept(visitor, Af1Field, Af1Field.DataType, ref _af1);
        DoubleType.Accept(visitor, Af0Field, Af0Field.DataType, ref _af0);
        Int32Type.Accept(visitor, iodcField, iodcField.DataType, ref _iodc);
        if (visitor is ArrayType.IVisitor arrayVisitor)
        {
            var arrayType = (ArrayType)TgdField.DataType;
            arrayVisitor.BeginArray(TgdField, arrayType);
            if (_tgd is { Length: > 0 })
                DoubleType.Accept(visitor, TgdField, arrayType.ElementType, ref _tgd[0]);
            else
                visitor.VisitUnknown(TgdField, arrayType.ElementType);
            arrayVisitor.EndArray();
        }
        else
            visitor.VisitUnknown(TgdField, TgdField.DataType);
    }
}
