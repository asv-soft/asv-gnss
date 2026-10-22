using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavBinaryBestPosPacket
{
    private static readonly Field PayloadSolutionStatusField = SbfComNavVisitor.Field(nameof(SolutionStatus), Int32Type.Default, "Solution status.", enumType: typeof(ComNavSolutionStatus));
    private static readonly Field PayloadPositionTypeField = SbfComNavVisitor.Field(nameof(PositionType), Int32Type.Default, "Position type.", enumType: typeof(ComNavPositionType));
    private static readonly Field PayloadLatitudeField = SbfComNavVisitor.Field(nameof(Latitude), DoubleType.Default, "Latitude, degrees.");
    private static readonly Field PayloadLongitudeField = SbfComNavVisitor.Field(nameof(Longitude), DoubleType.Default, "Longitude, degrees.");
    private static readonly Field PayloadHeightMslField = SbfComNavVisitor.Field(nameof(HeightMsl), DoubleType.Default, "Height above mean sea level, metres.");
    private static readonly Field PayloadUndulationField = SbfComNavVisitor.Field(nameof(Undulation), FloatType.Default, "Geoid undulation, metres.");
    private static readonly Field PayloadDatumField = SbfComNavVisitor.Field(nameof(Datum), Int32Type.Default, "Datum identifier.", enumType: typeof(ComNavDatum));
    private static readonly Field PayloadLatitudeSdField = SbfComNavVisitor.Field(nameof(LatitudeSd), FloatType.Default, "Latitude standard deviation, metres.");
    private static readonly Field PayloadLongitudeSdField = SbfComNavVisitor.Field(nameof(LongitudeSd), FloatType.Default, "Longitude standard deviation, metres.");
    private static readonly Field PayloadHeightMslSdField = SbfComNavVisitor.Field(nameof(HeightMslSd), FloatType.Default, "Height standard deviation, metres.");
    private static readonly Field PayloadBaseStationIdField = SbfComNavVisitor.Field(nameof(BaseStationId), StringOptionalType.Utf8, "Base station identifier.");
    private static readonly Field PayloadDifferentialAgeSecField = SbfComNavVisitor.Field(nameof(DifferentialAgeSec), FloatType.Default, "Differential age, seconds.");
    private static readonly Field PayloadSolutionAgeSecField = SbfComNavVisitor.Field(nameof(SolutionAgeSec), FloatType.Default, "Solution age, seconds.");
    private static readonly Field PayloadTrackedSatsField = SbfComNavVisitor.Field(nameof(TrackedSats), UInt8Type.Default, "Number of tracked satellites.");
    private static readonly Field PayloadSolutionSatsField = SbfComNavVisitor.Field(nameof(SolutionSats), UInt8Type.Default, "Number of satellites used in solution.");
    private static readonly Field PayloadL1CarrierPhaseSatsField = SbfComNavVisitor.Field(nameof(L1CarrierPhaseSats), UInt8Type.Default, "Number of L1 carrier phases used.");
    private static readonly Field PayloadL2CarrierPhaseSatsField = SbfComNavVisitor.Field(nameof(L2CarrierPhaseSats), UInt8Type.Default, "Number of L2 carrier phases used.");
    private static readonly Field PayloadReserved1Field = SbfComNavVisitor.Field(nameof(Reserved1), UInt8Type.Default, "Reserved1");
    private static readonly Field PayloadExtSolutionStatusField = SbfComNavVisitor.Field(nameof(ExtSolutionStatus), UInt8Type.Default, "Extended solution status.");
    private static readonly Field PayloadReserved2Field = SbfComNavVisitor.Field(nameof(Reserved2), UInt8Type.Default, "Reserved2");
    private static readonly Field PayloadSignalMaskField = SbfComNavVisitor.Field(nameof(SignalMask), UInt8Type.Default, "Signals used mask.");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavBinaryMessageBase.PayloadVisitorType.Fields, PayloadSolutionStatusField, PayloadPositionTypeField, PayloadLatitudeField, PayloadLongitudeField, PayloadHeightMslField, PayloadUndulationField, PayloadDatumField, PayloadLatitudeSdField, PayloadLongitudeSdField, PayloadHeightMslSdField, PayloadBaseStationIdField, PayloadDifferentialAgeSecField, PayloadSolutionAgeSecField, PayloadTrackedSatsField, PayloadSolutionSatsField, PayloadL1CarrierPhaseSatsField, PayloadL2CarrierPhaseSatsField, PayloadReserved1Field, PayloadExtSolutionStatusField, PayloadReserved2Field, PayloadSignalMaskField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSolutionStatus = SolutionStatus;
        var numericvalueSolutionStatus = (int)valueSolutionStatus;
        Int32Type.Accept(visitor, PayloadSolutionStatusField, PayloadSolutionStatusField.DataType, ref numericvalueSolutionStatus);
        valueSolutionStatus = (ComNavSolutionStatus)numericvalueSolutionStatus;
        if (!EqualityComparer<ComNavSolutionStatus>.Default.Equals(valueSolutionStatus, SolutionStatus))
        {
            SolutionStatus = valueSolutionStatus;
        }

        var valuePositionType = PositionType;
        var numericvaluePositionType = (int)valuePositionType;
        Int32Type.Accept(visitor, PayloadPositionTypeField, PayloadPositionTypeField.DataType, ref numericvaluePositionType);
        valuePositionType = (ComNavPositionType)numericvaluePositionType;
        if (!EqualityComparer<ComNavPositionType>.Default.Equals(valuePositionType, PositionType))
        {
            PositionType = valuePositionType;
        }

        var valueLatitude = Latitude;
        DoubleType.Accept(visitor, PayloadLatitudeField, PayloadLatitudeField.DataType, ref valueLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLatitude, Latitude))
        {
            Latitude = valueLatitude;
        }

        var valueLongitude = Longitude;
        DoubleType.Accept(visitor, PayloadLongitudeField, PayloadLongitudeField.DataType, ref valueLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLongitude, Longitude))
        {
            Longitude = valueLongitude;
        }

        var valueHeightMsl = HeightMsl;
        DoubleType.Accept(visitor, PayloadHeightMslField, PayloadHeightMslField.DataType, ref valueHeightMsl);
        if (!EqualityComparer<double>.Default.Equals(valueHeightMsl, HeightMsl))
        {
            HeightMsl = valueHeightMsl;
        }

        var valueUndulation = Undulation;
        FloatType.Accept(visitor, PayloadUndulationField, PayloadUndulationField.DataType, ref valueUndulation);
        if (!EqualityComparer<float>.Default.Equals(valueUndulation, Undulation))
        {
            Undulation = valueUndulation;
        }

        var valueDatum = Datum;
        var numericvalueDatum = (int)valueDatum;
        Int32Type.Accept(visitor, PayloadDatumField, PayloadDatumField.DataType, ref numericvalueDatum);
        valueDatum = (ComNavDatum)numericvalueDatum;
        if (!EqualityComparer<ComNavDatum>.Default.Equals(valueDatum, Datum))
        {
            Datum = valueDatum;
        }

        var valueLatitudeSd = LatitudeSd;
        FloatType.Accept(visitor, PayloadLatitudeSdField, PayloadLatitudeSdField.DataType, ref valueLatitudeSd);
        if (!EqualityComparer<float>.Default.Equals(valueLatitudeSd, LatitudeSd))
        {
            LatitudeSd = valueLatitudeSd;
        }

        var valueLongitudeSd = LongitudeSd;
        FloatType.Accept(visitor, PayloadLongitudeSdField, PayloadLongitudeSdField.DataType, ref valueLongitudeSd);
        if (!EqualityComparer<float>.Default.Equals(valueLongitudeSd, LongitudeSd))
        {
            LongitudeSd = valueLongitudeSd;
        }

        var valueHeightMslSd = HeightMslSd;
        FloatType.Accept(visitor, PayloadHeightMslSdField, PayloadHeightMslSdField.DataType, ref valueHeightMslSd);
        if (!EqualityComparer<float>.Default.Equals(valueHeightMslSd, HeightMslSd))
        {
            HeightMslSd = valueHeightMslSd;
        }

        string? valueBaseStationId = BaseStationId;
        StringOptionalType.Accept(visitor, PayloadBaseStationIdField, PayloadBaseStationIdField.DataType, ref valueBaseStationId);
        if (!EqualityComparer<string>.Default.Equals(valueBaseStationId, BaseStationId))
        {
            BaseStationId = valueBaseStationId!;
        }

        var valueDifferentialAgeSec = DifferentialAgeSec;
        FloatType.Accept(visitor, PayloadDifferentialAgeSecField, PayloadDifferentialAgeSecField.DataType, ref valueDifferentialAgeSec);
        if (!EqualityComparer<float>.Default.Equals(valueDifferentialAgeSec, DifferentialAgeSec))
        {
            DifferentialAgeSec = valueDifferentialAgeSec;
        }

        var valueSolutionAgeSec = SolutionAgeSec;
        FloatType.Accept(visitor, PayloadSolutionAgeSecField, PayloadSolutionAgeSecField.DataType, ref valueSolutionAgeSec);
        if (!EqualityComparer<float>.Default.Equals(valueSolutionAgeSec, SolutionAgeSec))
        {
            SolutionAgeSec = valueSolutionAgeSec;
        }

        var valueTrackedSats = TrackedSats;
        UInt8Type.Accept(visitor, PayloadTrackedSatsField, PayloadTrackedSatsField.DataType, ref valueTrackedSats);
        if (!EqualityComparer<byte>.Default.Equals(valueTrackedSats, TrackedSats))
        {
            TrackedSats = valueTrackedSats;
        }

        var valueSolutionSats = SolutionSats;
        UInt8Type.Accept(visitor, PayloadSolutionSatsField, PayloadSolutionSatsField.DataType, ref valueSolutionSats);
        if (!EqualityComparer<byte>.Default.Equals(valueSolutionSats, SolutionSats))
        {
            SolutionSats = valueSolutionSats;
        }

        var valueL1CarrierPhaseSats = L1CarrierPhaseSats;
        UInt8Type.Accept(visitor, PayloadL1CarrierPhaseSatsField, PayloadL1CarrierPhaseSatsField.DataType, ref valueL1CarrierPhaseSats);
        if (!EqualityComparer<byte>.Default.Equals(valueL1CarrierPhaseSats, L1CarrierPhaseSats))
        {
            L1CarrierPhaseSats = valueL1CarrierPhaseSats;
        }

        var valueL2CarrierPhaseSats = L2CarrierPhaseSats;
        UInt8Type.Accept(visitor, PayloadL2CarrierPhaseSatsField, PayloadL2CarrierPhaseSatsField.DataType, ref valueL2CarrierPhaseSats);
        if (!EqualityComparer<byte>.Default.Equals(valueL2CarrierPhaseSats, L2CarrierPhaseSats))
        {
            L2CarrierPhaseSats = valueL2CarrierPhaseSats;
        }

        var valueReserved1 = Reserved1;
        UInt8Type.Accept(visitor, PayloadReserved1Field, PayloadReserved1Field.DataType, ref valueReserved1);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved1, Reserved1))
        {
            Reserved1 = valueReserved1;
        }

        var valueExtSolutionStatus = ExtSolutionStatus;
        UInt8Type.Accept(visitor, PayloadExtSolutionStatusField, PayloadExtSolutionStatusField.DataType, ref valueExtSolutionStatus);
        if (!EqualityComparer<byte>.Default.Equals(valueExtSolutionStatus, ExtSolutionStatus))
        {
            ExtSolutionStatus = valueExtSolutionStatus;
        }

        var valueReserved2 = Reserved2;
        UInt8Type.Accept(visitor, PayloadReserved2Field, PayloadReserved2Field.DataType, ref valueReserved2);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved2, Reserved2))
        {
            Reserved2 = valueReserved2;
        }

        var valueSignalMask = SignalMask;
        UInt8Type.Accept(visitor, PayloadSignalMaskField, PayloadSignalMaskField.DataType, ref valueSignalMask);
        if (!EqualityComparer<byte>.Default.Equals(valueSignalMask, SignalMask))
        {
            SignalMask = valueSignalMask;
        }
    }
}
