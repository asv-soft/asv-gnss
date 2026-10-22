using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavBinaryPsrPosPacket
{
    private static readonly Field PayloadPsrPosReserved4Field = SbfComNavVisitor.Field(nameof(PsrPosReserved4), UInt8Type.Default, "PsrPosReserved4");
    private static readonly Field PayloadPsrPosReserved3Field = SbfComNavVisitor.Field(nameof(PsrPosReserved3), UInt8Type.Default, "PsrPosReserved3");
    private static readonly Field PayloadPsrPosReserved2Field = SbfComNavVisitor.Field(nameof(PsrPosReserved2), UInt8Type.Default, "PsrPosReserved2");
    private static readonly Field PayloadPsrPosReserved1Field = SbfComNavVisitor.Field(nameof(PsrPosReserved1), UInt8Type.Default, "PsrPosReserved1");
    private static readonly Field PayloadSignalMaskField = SbfComNavVisitor.Field(nameof(SignalMask), UInt8Type.Default, "Signals used mask - if 0, signals used in solution are unknown. See Table 33.");
    private static readonly Field PayloadExtSolutionStatusField = SbfComNavVisitor.Field(nameof(ExtSolutionStatus), UInt8Type.Default, "Extended solution status (default: 0)");
    private static readonly Field PayloadSolutionSatsField = SbfComNavVisitor.Field(nameof(SolutionSats), UInt8Type.Default, "Number of satellite vehicles used in solution");
    private static readonly Field PayloadTracketSatsField = SbfComNavVisitor.Field(nameof(TracketSats), UInt8Type.Default, "Number of satellite vehicles tracked");
    private static readonly Field PayloadSolutionAgeSecField = SbfComNavVisitor.Field(nameof(SolutionAgeSec), FloatType.Default, "Solution age in seconds");
    private static readonly Field PayloadDifferentialAgeSecField = SbfComNavVisitor.Field(nameof(DifferentialAgeSec), FloatType.Default, "Differential age in seconds");
    private static readonly Field PayloadBaseStationIdField = SbfComNavVisitor.Field(nameof(BaseStationId), StringOptionalType.Utf8, "This is station ID of the station, who sending DGPS corrections (Not Current station id!)");
    private static readonly Field PayloadHeightMslSdField = SbfComNavVisitor.Field(nameof(HeightMslSd), FloatType.Default, "Height standard deviation");
    private static readonly Field PayloadLongitudeSdField = SbfComNavVisitor.Field(nameof(LongitudeSd), FloatType.Default, "Longitude standard deviation");
    private static readonly Field PayloadLatitudeSdField = SbfComNavVisitor.Field(nameof(LatitudeSd), FloatType.Default, "Latitude standard deviation");
    private static readonly Field PayloadDatumField = SbfComNavVisitor.Field(nameof(Datum), Int32Type.Default, "Datum", enumType: typeof(ComNavDatum));
    private static readonly Field PayloadUndulationField = SbfComNavVisitor.Field(nameof(Undulation), FloatType.Default, "Undulation - the relationship between the geoids and the WGS84 ellipsoid (m)");
    private static readonly Field PayloadHeightMslField = SbfComNavVisitor.Field(nameof(HeightMsl), DoubleType.Default, "Height above mean sea level");
    private static readonly Field PayloadLongitudeField = SbfComNavVisitor.Field(nameof(Longitude), DoubleType.Default, "Longitude");
    private static readonly Field PayloadLatitudeField = SbfComNavVisitor.Field(nameof(Latitude), DoubleType.Default, "Latitude");
    private static readonly Field PayloadPositionTypeField = SbfComNavVisitor.Field(nameof(PositionType), Int32Type.Default, "PositionType", enumType: typeof(ComNavPositionType));
    private static readonly Field PayloadSolutionStatusField = SbfComNavVisitor.Field(nameof(SolutionStatus), Int32Type.Default, "SolutionStatus", enumType: typeof(ComNavSolutionStatus));

    internal new static readonly StructType PayloadVisitorType = new([..ComNavBinaryMessageBase.PayloadVisitorType.Fields, PayloadPsrPosReserved4Field, PayloadPsrPosReserved3Field, PayloadPsrPosReserved2Field, PayloadPsrPosReserved1Field, PayloadSignalMaskField, PayloadExtSolutionStatusField, PayloadSolutionSatsField, PayloadTracketSatsField, PayloadSolutionAgeSecField, PayloadDifferentialAgeSecField, PayloadBaseStationIdField, PayloadHeightMslSdField, PayloadLongitudeSdField, PayloadLatitudeSdField, PayloadDatumField, PayloadUndulationField, PayloadHeightMslField, PayloadLongitudeField, PayloadLatitudeField, PayloadPositionTypeField, PayloadSolutionStatusField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valuePsrPosReserved4 = PsrPosReserved4;
        UInt8Type.Accept(visitor, PayloadPsrPosReserved4Field, PayloadPsrPosReserved4Field.DataType, ref valuePsrPosReserved4);
        if (!EqualityComparer<byte>.Default.Equals(valuePsrPosReserved4, PsrPosReserved4))
        {
            PsrPosReserved4 = valuePsrPosReserved4;
        }

        var valuePsrPosReserved3 = PsrPosReserved3;
        UInt8Type.Accept(visitor, PayloadPsrPosReserved3Field, PayloadPsrPosReserved3Field.DataType, ref valuePsrPosReserved3);
        if (!EqualityComparer<byte>.Default.Equals(valuePsrPosReserved3, PsrPosReserved3))
        {
            PsrPosReserved3 = valuePsrPosReserved3;
        }

        var valuePsrPosReserved2 = PsrPosReserved2;
        UInt8Type.Accept(visitor, PayloadPsrPosReserved2Field, PayloadPsrPosReserved2Field.DataType, ref valuePsrPosReserved2);
        if (!EqualityComparer<byte>.Default.Equals(valuePsrPosReserved2, PsrPosReserved2))
        {
            PsrPosReserved2 = valuePsrPosReserved2;
        }

        var valuePsrPosReserved1 = PsrPosReserved1;
        UInt8Type.Accept(visitor, PayloadPsrPosReserved1Field, PayloadPsrPosReserved1Field.DataType, ref valuePsrPosReserved1);
        if (!EqualityComparer<byte>.Default.Equals(valuePsrPosReserved1, PsrPosReserved1))
        {
            PsrPosReserved1 = valuePsrPosReserved1;
        }

        var valueSignalMask = SignalMask;
        UInt8Type.Accept(visitor, PayloadSignalMaskField, PayloadSignalMaskField.DataType, ref valueSignalMask);
        if (!EqualityComparer<byte>.Default.Equals(valueSignalMask, SignalMask))
        {
            SignalMask = valueSignalMask;
        }

        var valueExtSolutionStatus = ExtSolutionStatus;
        UInt8Type.Accept(visitor, PayloadExtSolutionStatusField, PayloadExtSolutionStatusField.DataType, ref valueExtSolutionStatus);
        if (!EqualityComparer<byte>.Default.Equals(valueExtSolutionStatus, ExtSolutionStatus))
        {
            ExtSolutionStatus = valueExtSolutionStatus;
        }

        var valueSolutionSats = SolutionSats;
        UInt8Type.Accept(visitor, PayloadSolutionSatsField, PayloadSolutionSatsField.DataType, ref valueSolutionSats);
        if (!EqualityComparer<byte>.Default.Equals(valueSolutionSats, SolutionSats))
        {
            SolutionSats = valueSolutionSats;
        }

        var valueTracketSats = TracketSats;
        UInt8Type.Accept(visitor, PayloadTracketSatsField, PayloadTracketSatsField.DataType, ref valueTracketSats);
        if (!EqualityComparer<byte>.Default.Equals(valueTracketSats, TracketSats))
        {
            TracketSats = valueTracketSats;
        }

        var valueSolutionAgeSec = SolutionAgeSec;
        FloatType.Accept(visitor, PayloadSolutionAgeSecField, PayloadSolutionAgeSecField.DataType, ref valueSolutionAgeSec);
        if (!EqualityComparer<float>.Default.Equals(valueSolutionAgeSec, SolutionAgeSec))
        {
            SolutionAgeSec = valueSolutionAgeSec;
        }

        var valueDifferentialAgeSec = DifferentialAgeSec;
        FloatType.Accept(visitor, PayloadDifferentialAgeSecField, PayloadDifferentialAgeSecField.DataType, ref valueDifferentialAgeSec);
        if (!EqualityComparer<float>.Default.Equals(valueDifferentialAgeSec, DifferentialAgeSec))
        {
            DifferentialAgeSec = valueDifferentialAgeSec;
        }

        string? valueBaseStationId = BaseStationId;
        StringOptionalType.Accept(visitor, PayloadBaseStationIdField, PayloadBaseStationIdField.DataType, ref valueBaseStationId);
        if (!EqualityComparer<string>.Default.Equals(valueBaseStationId, BaseStationId))
        {
            BaseStationId = valueBaseStationId!;
        }

        var valueHeightMslSd = HeightMslSd;
        FloatType.Accept(visitor, PayloadHeightMslSdField, PayloadHeightMslSdField.DataType, ref valueHeightMslSd);
        if (!EqualityComparer<float>.Default.Equals(valueHeightMslSd, HeightMslSd))
        {
            HeightMslSd = valueHeightMslSd;
        }

        var valueLongitudeSd = LongitudeSd;
        FloatType.Accept(visitor, PayloadLongitudeSdField, PayloadLongitudeSdField.DataType, ref valueLongitudeSd);
        if (!EqualityComparer<float>.Default.Equals(valueLongitudeSd, LongitudeSd))
        {
            LongitudeSd = valueLongitudeSd;
        }

        var valueLatitudeSd = LatitudeSd;
        FloatType.Accept(visitor, PayloadLatitudeSdField, PayloadLatitudeSdField.DataType, ref valueLatitudeSd);
        if (!EqualityComparer<float>.Default.Equals(valueLatitudeSd, LatitudeSd))
        {
            LatitudeSd = valueLatitudeSd;
        }

        var valueDatum = Datum;
        var numericvalueDatum = (int)valueDatum;
        Int32Type.Accept(visitor, PayloadDatumField, PayloadDatumField.DataType, ref numericvalueDatum);
        valueDatum = (ComNavDatum)numericvalueDatum;
        if (!EqualityComparer<ComNavDatum>.Default.Equals(valueDatum, Datum))
        {
            Datum = valueDatum;
        }

        var valueUndulation = Undulation;
        FloatType.Accept(visitor, PayloadUndulationField, PayloadUndulationField.DataType, ref valueUndulation);
        if (!EqualityComparer<float>.Default.Equals(valueUndulation, Undulation))
        {
            Undulation = valueUndulation;
        }

        var valueHeightMsl = HeightMsl;
        DoubleType.Accept(visitor, PayloadHeightMslField, PayloadHeightMslField.DataType, ref valueHeightMsl);
        if (!EqualityComparer<double>.Default.Equals(valueHeightMsl, HeightMsl))
        {
            HeightMsl = valueHeightMsl;
        }

        var valueLongitude = Longitude;
        DoubleType.Accept(visitor, PayloadLongitudeField, PayloadLongitudeField.DataType, ref valueLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLongitude, Longitude))
        {
            Longitude = valueLongitude;
        }

        var valueLatitude = Latitude;
        DoubleType.Accept(visitor, PayloadLatitudeField, PayloadLatitudeField.DataType, ref valueLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLatitude, Latitude))
        {
            Latitude = valueLatitude;
        }

        var valuePositionType = PositionType;
        var numericvaluePositionType = (int)valuePositionType;
        Int32Type.Accept(visitor, PayloadPositionTypeField, PayloadPositionTypeField.DataType, ref numericvaluePositionType);
        valuePositionType = (ComNavPositionType)numericvaluePositionType;
        if (!EqualityComparer<ComNavPositionType>.Default.Equals(valuePositionType, PositionType))
        {
            PositionType = valuePositionType;
        }

        var valueSolutionStatus = SolutionStatus;
        var numericvalueSolutionStatus = (int)valueSolutionStatus;
        Int32Type.Accept(visitor, PayloadSolutionStatusField, PayloadSolutionStatusField.DataType, ref numericvalueSolutionStatus);
        valueSolutionStatus = (ComNavSolutionStatus)numericvalueSolutionStatus;
        if (!EqualityComparer<ComNavSolutionStatus>.Default.Equals(valueSolutionStatus, SolutionStatus))
        {
            SolutionStatus = valueSolutionStatus;
        }
    }
}
