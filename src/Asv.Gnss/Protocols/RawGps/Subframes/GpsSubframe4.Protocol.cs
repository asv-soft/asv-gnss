using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class GpsSubframe4
{
    private static readonly Field DataIdField = GpsNavigationVisitor.Field(nameof(DataId), UInt8Type.Default);
    private static readonly Field PageIdField = GpsNavigationVisitor.Field(nameof(PageId), UInt8Type.Default);
    private static readonly Field RawPageDataField = GpsNavigationVisitor.Field(nameof(RawPageData), new ArrayType(UInt8Type.Default, 23));
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
    private static readonly Field AlmanacHealthField = GpsNavigationVisitor.Field(
        nameof(Health),
        new ArrayType(Int32Type.Default, 1)
    );
    private static readonly Field HealthField = GpsNavigationVisitor.Field(
        nameof(Health),
        new ArrayType(Int32Type.Default, 8)
    );
    private static readonly Field SvConfigField = GpsNavigationVisitor.Field(
        nameof(SvConfig),
        new ArrayType(Int32Type.Default, 32)
    );
    private static readonly Field IonoField = GpsNavigationVisitor.Field(
        nameof(Iono),
        new OptionalStructType(Ionospheric.Fields)
    );
    private static readonly Field UtcGpsField = GpsNavigationVisitor.Field(
        nameof(UtcGps),
        new OptionalStructType(UtcGpsRelationship.Fields)
    );

    private static readonly Field NmctAvailabilityIndicatorField = GpsNavigationVisitor.Field(nameof(NmctAvailabilityIndicator), UInt8Type.Default);
    private static readonly Field NmctEstimatedRangeDeviationsField = GpsNavigationVisitor.Field(nameof(NmctEstimatedRangeDeviations), new ArrayType(Int32Type.Default, 30));
    private static readonly Field SpecialMessageDataField = GpsNavigationVisitor.Field(nameof(SpecialMessageData), new ArrayType(UInt8Type.Default, 22));

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
        Health = null!;
        SvConfig = null!;
        Iono = null!;
        UtcGps = null!;
        NmctAvailabilityIndicator = 0;
        NmctEstimatedRangeDeviations = null!;
        SpecialMessageData = null!;
    }

    private void SerializeAlmanac(Span<byte> source)
    {
        if (SatelliteNumber is < 25 or > 32)
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
            case Subframe4Type.Almanac:
                SerializeAlmanac(source);
                break;
            case Subframe4Type.HealthAndConfigurations:
                GpsNavigationVisitor.RequireLength(SvConfig, 32, nameof(SvConfig));
                GpsNavigationVisitor.RequireLength(Health, 8, nameof(Health));
                RawGpsCodec.Unsigned(source, 48, 2, 1);
                RawGpsCodec.Unsigned(source, 50, 6, 63);
                for (var index = 0; index < 32; index++)
                    RawGpsCodec.Unsigned(source, 56 + index * 4, 4, SvConfig[index]);
                for (var index = 0; index < 8; index++)
                    RawGpsCodec.Unsigned(source, 186 + index * 6, 6, Health[index]);
                break;
            case Subframe4Type.IonoAndUtcData:
                ArgumentNullException.ThrowIfNull(Iono);
                ArgumentNullException.ThrowIfNull(UtcGps);
                RawGpsCodec.Unsigned(source, 48, 2, 1);
                RawGpsCodec.Unsigned(source, 50, 6, 56);
                RawGpsCodec.Signed(source, 56, 8, Iono.Alpha0, GpsRawHelper.P2_30);
                RawGpsCodec.Signed(source, 64, 8, Iono.Alpha1, GpsRawHelper.P2_27);
                RawGpsCodec.Signed(source, 72, 8, Iono.Alpha2, GpsRawHelper.P2_24);
                RawGpsCodec.Signed(source, 80, 8, Iono.Alpha3, GpsRawHelper.P2_24);
                RawGpsCodec.Signed(source, 88, 8, Iono.Betta0, 2048);
                RawGpsCodec.Signed(source, 96, 8, Iono.Betta1, 16384);
                RawGpsCodec.Signed(source, 104, 8, Iono.Betta2, 65536);
                RawGpsCodec.Signed(source, 112, 8, Iono.Betta3, 65536);
                RawGpsCodec.Signed(source, 120, 24, UtcGps.A1, GpsRawHelper.P2_50);
                RawGpsCodec.Signed(source, 144, 32, UtcGps.A0, GpsRawHelper.P2_30);
                RawGpsCodec.Signed(source, 176, 8, UtcGps.Tot, 4096);
                RawGpsCodec.Unsigned(source, 184, 8, UtcGps.WNt);
                RawGpsCodec.Unsigned(source, 192, 8, UtcGps.DeltaT_LS);
                RawGpsCodec.Unsigned(source, 200, 8, UtcGps.WN_LSF);
                RawGpsCodec.Unsigned(source, 208, 8, UtcGps.DN);
                RawGpsCodec.Signed(source, 216, 8, UtcGps.DeltaT_LSF);
                break;
            case Subframe4Type.Nmct:
                GpsNavigationVisitor.RequireLength(NmctEstimatedRangeDeviations, 30, nameof(NmctEstimatedRangeDeviations));
                RawGpsCodec.Unsigned(source, 48, 2, 1);
                RawGpsCodec.Unsigned(source, 50, 6, 52);
                RawGpsCodec.Unsigned(source, 56, 2, NmctAvailabilityIndicator);
                for (var index = 0; index < 30; index++)
                    RawGpsCodec.Signed(source, 58 + index * 6, 6, NmctEstimatedRangeDeviations[index]);
                break;
            case Subframe4Type.SpecialMessage:
                GpsNavigationVisitor.RequireLength(SpecialMessageData, 22, nameof(SpecialMessageData));
                RawGpsCodec.Unsigned(source, 48, 2, 1);
                RawGpsCodec.Unsigned(source, 50, 6, 55);
                SpecialMessageData.CopyTo(source[7..29]);
                break;
            case Subframe4Type.None:
                var page = PageId;
                if (DataId == 1 && (page is >= 25 and <= 32 or 52 or 55 or 56 or 63))
                    throw new InvalidOperationException("Changing a known GPS page to None requires a new unknown-page identity.");
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
        if (dataId != DataId) DataId = dataId;
        var pageId = PageId;
        UInt8Type.Accept(visitor, PageIdField, PageIdField.DataType, ref pageId);
        if (pageId != PageId) PageId = pageId;
        var type = (int)Type;
        Int32Type.Accept(visitor, TypeField, TypeField.DataType, ref type);
        if (type != (int)Type) Type = (Subframe4Type)type;
        switch (Type)
        {
            case Subframe4Type.Almanac:
                VisitAlmanac(visitor);
                break;
            case Subframe4Type.HealthAndConfigurations:
                GpsNavigationVisitor.Array(visitor, SvConfigField, _svConfig);
                GpsNavigationVisitor.Array(visitor, HealthField, _health);
                break;
            case Subframe4Type.IonoAndUtcData:
                OptionalStructType.Accept(visitor, IonoField, ref _iono);
                OptionalStructType.Accept(visitor, UtcGpsField, ref _utcGps);
                break;
            case Subframe4Type.Nmct:
                UInt8Type.Accept(visitor, NmctAvailabilityIndicatorField, NmctAvailabilityIndicatorField.DataType, ref _nmctAvailabilityIndicator);
                GpsNavigationVisitor.Array(visitor, NmctEstimatedRangeDeviationsField, _nmctEstimatedRangeDeviations);
                break;
            case Subframe4Type.SpecialMessage:
                GpsNavigationVisitor.Array(visitor, SpecialMessageDataField, _specialMessageData);
                break;
            case Subframe4Type.None:
                GpsNavigationVisitor.Array(visitor, RawPageDataField, _rawPageData);
                break;
        }
    }
}
