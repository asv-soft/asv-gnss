using System;
using System.Runtime.CompilerServices;
using Asv.IO;

namespace Asv.Gnss;

public partial class GpsSubFrame5
{
    private static readonly Field DataIdField = GpsNavigationVisitor.Field(
        nameof(DataId),
        UInt8Type.Default
    );
    private static readonly Field PageIdField = GpsNavigationVisitor.Field(
        nameof(PageId),
        UInt8Type.Default
    );
    private static readonly Field RawPageDataField = GpsNavigationVisitor.Field(
        nameof(RawPageData),
        new ArrayType(UInt8Type.Default, 23)
    );
    private static readonly Field TypeField = GpsNavigationVisitor.Field(
        nameof(Type),
        Int32Type.Default
    );
    private static readonly Field SatelliteNumberField = GpsNavigationVisitor.Field(
        nameof(SatelliteNumber),
        Int32Type.Default
    );
    private static readonly Field AField = GpsNavigationVisitor.Field(
        nameof(A),
        DoubleType.Default
    );
    private static readonly Field eField = GpsNavigationVisitor.Field(
        nameof(e),
        DoubleType.Default
    );
    private static readonly Field i0Field = GpsNavigationVisitor.Field(
        nameof(i0),
        DoubleType.Default
    );
    private static readonly Field OMG0Field = GpsNavigationVisitor.Field(
        nameof(OMG0),
        DoubleType.Default
    );
    private static readonly Field omgField = GpsNavigationVisitor.Field(
        nameof(omg),
        DoubleType.Default
    );
    private static readonly Field M0Field = GpsNavigationVisitor.Field(
        nameof(M0),
        DoubleType.Default
    );
    private static readonly Field OMGdField = GpsNavigationVisitor.Field(
        nameof(OMGd),
        DoubleType.Default
    );
    private static readonly Field ToaSecField = GpsNavigationVisitor.Field(
        nameof(ToaSec),
        DoubleType.Default
    );
    private static readonly Field Af0Field = GpsNavigationVisitor.Field(
        nameof(Af0),
        DoubleType.Default
    );
    private static readonly Field Af1Field = GpsNavigationVisitor.Field(
        nameof(Af1),
        DoubleType.Default
    );
    private static readonly Field WeekField = GpsNavigationVisitor.Field(
        nameof(Week),
        Int32Type.Default
    );
    private static readonly Field AlmanacHealthField = GpsNavigationVisitor.Field(
        nameof(Health),
        new ArrayType(Int32Type.Default, 1)
    );
    private static readonly Field HealthField = GpsNavigationVisitor.Field(
        nameof(Health),
        new ArrayType(Int32Type.Default, 24)
    );

    private void ResetPage()
    {
        SatelliteNumber = 0;
        A = 0;
        e = 0;
        i0 = 0;
        OMG0 = 0;
        omg = 0;
        M0 = 0;
        OMGd = 0;
        ToaSec = 0;
        Af0 = 0;
        Af1 = 0;
        Week = 0;
        Health = null!;
    }

    private void SerializeAlmanac(Span<byte> source)
    {
        if (SatelliteNumber is < 1 or > 24)
            throw new ArgumentOutOfRangeException(nameof(SatelliteNumber));
        GpsNavigationVisitor.RequireLength(Health, 1, nameof(Health));
        RawGpsCodec.Unsigned(source, 48, 2, 1);
        RawGpsCodec.Unsigned(source, 50, 6, SatelliteNumber);
        RawGpsCodec.Signed(source, 56, 16, e, GpsRawHelper.P2_21);
        RawGpsCodec.Unsigned(source, 72, 8, ToaSec, 4096);
        RawGpsCodec.Signed(source, 80, 16, i0 / GpsRawHelper.SC2RAD - 0.3, GpsRawHelper.P2_19);
        RawGpsCodec.Signed(source, 96, 16, OMGd, GpsRawHelper.P2_38 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Unsigned(source, 112, 8, Health[0]);
        RawGpsCodec.Unsigned(source, 120, 24, Math.Sqrt(A), GpsRawHelper.P2_11);
        RawGpsCodec.Signed(source, 144, 24, OMG0, GpsRawHelper.P2_23 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Signed(source, 168, 24, omg, GpsRawHelper.P2_23 * GpsRawHelper.SC2RAD);
        RawGpsCodec.Signed(source, 192, 24, M0, GpsRawHelper.P2_23 * GpsRawHelper.SC2RAD);
        RawGpsCodec.AlmanacClock(source, Af0, Af1);
    }

    private void VisitAlmanac(IVisitor visitor)
    {
        Int32Type.Accept(
            visitor,
            SatelliteNumberField,
            SatelliteNumberField.DataType,
            ref _satelliteNumber
        );
        DoubleType.Accept(visitor, AField, AField.DataType, ref _a);
        DoubleType.Accept(visitor, eField, eField.DataType, ref _e);
        DoubleType.Accept(visitor, i0Field, i0Field.DataType, ref _i0);
        DoubleType.Accept(visitor, OMG0Field, OMG0Field.DataType, ref _oMG0);
        DoubleType.Accept(visitor, omgField, omgField.DataType, ref _omg);
        DoubleType.Accept(visitor, M0Field, M0Field.DataType, ref _m0);
        DoubleType.Accept(visitor, OMGdField, OMGdField.DataType, ref _oMGd);
        DoubleType.Accept(visitor, ToaSecField, ToaSecField.DataType, ref _toaSec);
        DoubleType.Accept(visitor, Af0Field, Af0Field.DataType, ref _af0);
        DoubleType.Accept(visitor, Af1Field, Af1Field.DataType, ref _af1);
        GpsNavigationVisitor.Array(visitor, AlmanacHealthField, _health);
    }

    protected override void SerializeFields(Span<byte> source)
    {
        switch (Type)
        {
            case Subframe5Type.Almanac:
                SerializeAlmanac(source);
                break;
            case Subframe5Type.ToaAndHealth:
                GpsNavigationVisitor.RequireLength(Health, 24, nameof(Health));
                RawGpsCodec.Unsigned(source, 48, 2, 1);
                RawGpsCodec.Unsigned(source, 50, 6, 51);
                RawGpsCodec.Unsigned(source, 56, 8, ToaSec, 4096);
                RawGpsCodec.Unsigned(source, 64, 8, Week);
                for (var index = 0; index < 24; index++)
                    RawGpsCodec.Unsigned(source, 72 + index * 6, 6, Health[index]);
                break;
            case Subframe5Type.None:
                var page = PageId;
                if (DataId == 1 && (page is >= 1 and <= 24 or 51))
                    throw new InvalidOperationException(
                        "Changing a known GPS page to None requires a new unknown-page identity."
                    );
                GpsNavigationVisitor.RequireLength(_rawPageData, 23, nameof(RawPageData));
                RawGpsCodec.Unsigned(source, 48, 2, DataId);
                RawGpsCodec.Unsigned(source, 50, 6, PageId);
                _rawPageData.CopyTo(source[7..]);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Type));
        }
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var dataId = DataId;
        UInt8Type.Accept(visitor, DataIdField, DataIdField.DataType, ref dataId);
        if (dataId != DataId)
            DataId = dataId;
        var pageId = PageId;
        UInt8Type.Accept(visitor, PageIdField, PageIdField.DataType, ref pageId);
        if (pageId != PageId)
            PageId = pageId;
        var previousType = _type;
        Int32Type.Accept(
            visitor,
            TypeField,
            TypeField.DataType,
            ref Unsafe.As<Subframe5Type, int>(ref _type)
        );
        if (_type != previousType)
        {
            var editedType = _type;
            _type = previousType;
            Type = editedType;
        }
        switch (Type)
        {
            case Subframe5Type.Almanac:
                VisitAlmanac(visitor);
                break;
            case Subframe5Type.ToaAndHealth:
                DoubleType.Accept(visitor, ToaSecField, ToaSecField.DataType, ref _toaSec);
                Int32Type.Accept(visitor, WeekField, WeekField.DataType, ref _week);
                GpsNavigationVisitor.Array(visitor, HealthField, _health);
                break;
            case Subframe5Type.None:
                GpsNavigationVisitor.Array(visitor, RawPageDataField, _rawPageData);
                break;
        }
    }
}
