using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvGpsObservation : IVisitable
{
    private static readonly Field PayloadPrnField = ProtocolPayloadVisitor.Field(nameof(Prn), Int32Type.Default);
    private static readonly Field PayloadSatelliteIdField = ProtocolPayloadVisitor.Field(nameof(SatelliteId), Int32Type.Default);
    private static readonly Field PayloadSatelliteCodeField = ProtocolPayloadVisitor.Field(nameof(SatelliteCode), StringOptionalType.Utf8);
    private static readonly Field PayloadL1CodeField = ProtocolPayloadVisitor.Field(nameof(L1Code), UInt8Type.Default);
    private static readonly Field PayloadL1PseudoRangeField = ProtocolPayloadVisitor.Field(nameof(L1PseudoRange), DoubleType.Default);
    private static readonly Field PayloadL1CarrierPhaseField = ProtocolPayloadVisitor.Field(nameof(L1CarrierPhase), DoubleType.Default);
    private static readonly Field PayloadL1LockTimeField = ProtocolPayloadVisitor.Field(nameof(L1LockTime), UInt16Type.Default);
    private static readonly Field PayloadParticipationIndicatorField = ProtocolPayloadVisitor.Field(nameof(ParticipationIndicator), BoolType.Default);
    private static readonly Field PayloadReasonForExceptionField = ProtocolPayloadVisitor.Field(nameof(ReasonForException), Int32Type.Default, enumType: typeof(ReasonForException));
    private static readonly Field PayloadElevationField = ProtocolPayloadVisitor.Field(nameof(Elevation), DoubleType.Default);
    private static readonly Field PayloadAzimuthField = ProtocolPayloadVisitor.Field(nameof(Azimuth), DoubleType.Default);
    private static readonly Field PayloadL1CNRField = ProtocolPayloadVisitor.Field(nameof(L1CNR), DoubleType.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadPrnField, PayloadSatelliteIdField, PayloadSatelliteCodeField, PayloadL1CodeField, PayloadL1PseudoRangeField, PayloadL1CarrierPhaseField, PayloadL1LockTimeField, PayloadParticipationIndicatorField, PayloadReasonForExceptionField, PayloadElevationField, PayloadAzimuthField, PayloadL1CNRField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        int valuePrn = Prn;
        Int32Type.Accept(visitor, PayloadPrnField, PayloadPrnField.DataType, ref valuePrn);
        if (!EqualityComparer<int>.Default.Equals(valuePrn, Prn))
        {
            Prn = valuePrn;
        }

        int valueSatelliteId = SatelliteId;
        Int32Type.Accept(visitor, PayloadSatelliteIdField, PayloadSatelliteIdField.DataType, ref valueSatelliteId);
        if (!EqualityComparer<int>.Default.Equals(valueSatelliteId, SatelliteId))
        {
            SatelliteId = valueSatelliteId;
        }

        string? valueSatelliteCode = SatelliteCode;
        StringOptionalType.Accept(visitor, PayloadSatelliteCodeField, PayloadSatelliteCodeField.DataType, ref valueSatelliteCode);
        if (!EqualityComparer<string>.Default.Equals(valueSatelliteCode, SatelliteCode))
        {
            SatelliteCode = valueSatelliteCode!;
        }

        byte valueL1Code = L1Code;
        UInt8Type.Accept(visitor, PayloadL1CodeField, PayloadL1CodeField.DataType, ref valueL1Code);
        if (!EqualityComparer<byte>.Default.Equals(valueL1Code, L1Code))
        {
            L1Code = valueL1Code;
        }

        double valueL1PseudoRange = L1PseudoRange;
        DoubleType.Accept(visitor, PayloadL1PseudoRangeField, PayloadL1PseudoRangeField.DataType, ref valueL1PseudoRange);
        if (!EqualityComparer<double>.Default.Equals(valueL1PseudoRange, L1PseudoRange))
        {
            L1PseudoRange = valueL1PseudoRange;
        }

        double valueL1CarrierPhase = L1CarrierPhase;
        DoubleType.Accept(visitor, PayloadL1CarrierPhaseField, PayloadL1CarrierPhaseField.DataType, ref valueL1CarrierPhase);
        if (!EqualityComparer<double>.Default.Equals(valueL1CarrierPhase, L1CarrierPhase))
        {
            L1CarrierPhase = valueL1CarrierPhase;
        }

        ushort valueL1LockTime = L1LockTime;
        UInt16Type.Accept(visitor, PayloadL1LockTimeField, PayloadL1LockTimeField.DataType, ref valueL1LockTime);
        if (!EqualityComparer<ushort>.Default.Equals(valueL1LockTime, L1LockTime))
        {
            L1LockTime = valueL1LockTime;
        }

        bool valueParticipationIndicator = ParticipationIndicator;
        BoolType.Accept(visitor, PayloadParticipationIndicatorField, PayloadParticipationIndicatorField.DataType, ref valueParticipationIndicator);
        if (!EqualityComparer<bool>.Default.Equals(valueParticipationIndicator, ParticipationIndicator))
        {
            ParticipationIndicator = valueParticipationIndicator;
        }

        ReasonForException valueReasonForException = ReasonForException;
        var numericReasonForException = (int)valueReasonForException;
        Int32Type.Accept(visitor, PayloadReasonForExceptionField, PayloadReasonForExceptionField.DataType, ref numericReasonForException);
        valueReasonForException = (ReasonForException)numericReasonForException;
        if (!EqualityComparer<ReasonForException>.Default.Equals(valueReasonForException, ReasonForException))
        {
            ReasonForException = valueReasonForException;
        }

        double valueElevation = Elevation;
        DoubleType.Accept(visitor, PayloadElevationField, PayloadElevationField.DataType, ref valueElevation);
        if (!EqualityComparer<double>.Default.Equals(valueElevation, Elevation))
        {
            Elevation = valueElevation;
        }

        double valueAzimuth = Azimuth;
        DoubleType.Accept(visitor, PayloadAzimuthField, PayloadAzimuthField.DataType, ref valueAzimuth);
        if (!EqualityComparer<double>.Default.Equals(valueAzimuth, Azimuth))
        {
            Azimuth = valueAzimuth;
        }

        double valueL1CNR = L1CNR;
        DoubleType.Accept(visitor, PayloadL1CNRField, PayloadL1CNRField.DataType, ref valueL1CNR);
        if (!EqualityComparer<double>.Default.Equals(valueL1CNR, L1CNR))
        {
            L1CNR = valueL1CNR;
        }
    }
}
