using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class GLONASSSatellite : IVisitable
{
    private static readonly Field RtcmSatellitePrnField = RtcmV3Visitor.Field(nameof(SatellitePrn), Int32Type.Default, "SatellitePrn");
    private static readonly Field RtcmSatelliteNumberField = RtcmV3Visitor.Field(nameof(SatelliteNumber), Int32Type.Default, "SatelliteNumber");
    private static readonly Field RtcmSatelliteCodeField = RtcmV3Visitor.Field(nameof(SatelliteCode), StringOptionalType.Utf8, "SatelliteCode");
    private static readonly Field RtcmL1CodeField = RtcmV3Visitor.Field(nameof(L1Code), UInt8Type.Default, "L1Code");
    private static readonly Field RtcmL1FrequencyField = RtcmV3Visitor.Field(nameof(L1Frequency), DoubleType.Default, "L1Frequency");
    private static readonly Field RtcmL1PseudoRangeField = RtcmV3Visitor.Field(nameof(L1PseudoRange), DoubleType.Default, "L1PseudoRange");
    private static readonly Field RtcmL1PhaseRangeField = RtcmV3Visitor.Field(nameof(L1PhaseRange), DoubleType.Default, "L1PhaseRange");
    private static readonly Field RtcmL1CarrierPhaseField = RtcmV3Visitor.Field(nameof(L1CarrierPhase), DoubleType.Default, "L1CarrierPhase");
    private static readonly Field RtcmL1LossOfLockIndicatorField = RtcmV3Visitor.Field(nameof(L1LossOfLockIndicator), BoolType.Default, "L1LossOfLockIndicator");
    private static readonly Field RtcmL1LockTimeField = RtcmV3Visitor.Field(nameof(L1LockTime), UInt8Type.Default, "L1LockTime");
    private static readonly Field RtcmL1CNRField = RtcmV3Visitor.Field(nameof(L1CNR), UInt16Type.Default, "L1CNR");
    private static readonly Field RtcmL2CodeField = RtcmV3Visitor.Field(nameof(L2Code), UInt8Type.Default, "L2Code");
    private static readonly Field RtcmL2FrequencyField = RtcmV3Visitor.Field(nameof(L2Frequency), DoubleType.Default, "L2Frequency");
    private static readonly Field RtcmL2PseudoRangeField = RtcmV3Visitor.Field(nameof(L2PseudoRange), DoubleType.Default, "L2PseudoRange");
    private static readonly Field RtcmL2PhaseRangeField = RtcmV3Visitor.Field(nameof(L2PhaseRange), DoubleType.Default, "L2PhaseRange");
    private static readonly Field RtcmL2CarrierPhaseField = RtcmV3Visitor.Field(nameof(L2CarrierPhase), DoubleType.Default, "L2CarrierPhase");
    private static readonly Field RtcmL2LossOfLockIndicatorField = RtcmV3Visitor.Field(nameof(L2LossOfLockIndicator), BoolType.Default, "L2LossOfLockIndicator");
    private static readonly Field RtcmL2LockTimeField = RtcmV3Visitor.Field(nameof(L2LockTime), UInt8Type.Default, "L2LockTime");
    private static readonly Field RtcmL2CNRField = RtcmV3Visitor.Field(nameof(L2CNR), UInt16Type.Default, "L2CNR");

    internal static readonly StructType RtcmVisitorType = new([RtcmSatellitePrnField, RtcmSatelliteNumberField, RtcmSatelliteCodeField, RtcmL1CodeField, RtcmL1FrequencyField, RtcmL1PseudoRangeField, RtcmL1PhaseRangeField, RtcmL1CarrierPhaseField, RtcmL1LossOfLockIndicatorField, RtcmL1LockTimeField, RtcmL1CNRField, RtcmL2CodeField, RtcmL2FrequencyField, RtcmL2PseudoRangeField, RtcmL2PhaseRangeField, RtcmL2CarrierPhaseField, RtcmL2LossOfLockIndicatorField, RtcmL2LockTimeField, RtcmL2CNRField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueSatellitePrn = SatellitePrn;
        Int32Type.Accept(visitor, RtcmSatellitePrnField, RtcmSatellitePrnField.DataType, ref valueSatellitePrn);
        if (!EqualityComparer<int>.Default.Equals(valueSatellitePrn, SatellitePrn))
        {
            SatellitePrn = valueSatellitePrn;
        }

        var valueSatelliteNumber = SatelliteNumber;
        Int32Type.Accept(visitor, RtcmSatelliteNumberField, RtcmSatelliteNumberField.DataType, ref valueSatelliteNumber);
        if (!EqualityComparer<int>.Default.Equals(valueSatelliteNumber, SatelliteNumber))
        {
            SatelliteNumber = valueSatelliteNumber;
        }

        string? valueSatelliteCode = SatelliteCode;
        StringOptionalType.Accept(visitor, RtcmSatelliteCodeField, RtcmSatelliteCodeField.DataType, ref valueSatelliteCode);
        if (!EqualityComparer<string>.Default.Equals(valueSatelliteCode, SatelliteCode))
        {
            SatelliteCode = valueSatelliteCode!;
        }

        var valueL1Code = L1Code;
        UInt8Type.Accept(visitor, RtcmL1CodeField, RtcmL1CodeField.DataType, ref valueL1Code);
        if (!EqualityComparer<byte>.Default.Equals(valueL1Code, L1Code))
        {
            L1Code = valueL1Code;
        }

        var valueL1Frequency = L1Frequency;
        DoubleType.Accept(visitor, RtcmL1FrequencyField, RtcmL1FrequencyField.DataType, ref valueL1Frequency);
        if (!EqualityComparer<double>.Default.Equals(valueL1Frequency, L1Frequency))
        {
            L1Frequency = valueL1Frequency;
        }

        var valueL1PseudoRange = L1PseudoRange;
        DoubleType.Accept(visitor, RtcmL1PseudoRangeField, RtcmL1PseudoRangeField.DataType, ref valueL1PseudoRange);
        if (!EqualityComparer<double>.Default.Equals(valueL1PseudoRange, L1PseudoRange))
        {
            L1PseudoRange = valueL1PseudoRange;
        }

        var valueL1PhaseRange = L1PhaseRange;
        DoubleType.Accept(visitor, RtcmL1PhaseRangeField, RtcmL1PhaseRangeField.DataType, ref valueL1PhaseRange);
        if (!EqualityComparer<double>.Default.Equals(valueL1PhaseRange, L1PhaseRange))
        {
            L1PhaseRange = valueL1PhaseRange;
        }

        var valueL1CarrierPhase = L1CarrierPhase;
        DoubleType.Accept(visitor, RtcmL1CarrierPhaseField, RtcmL1CarrierPhaseField.DataType, ref valueL1CarrierPhase);
        if (!EqualityComparer<double>.Default.Equals(valueL1CarrierPhase, L1CarrierPhase))
        {
            L1CarrierPhase = valueL1CarrierPhase;
        }

        var valueL1LossOfLockIndicator = L1LossOfLockIndicator;
        BoolType.Accept(visitor, RtcmL1LossOfLockIndicatorField, RtcmL1LossOfLockIndicatorField.DataType, ref valueL1LossOfLockIndicator);
        if (!EqualityComparer<bool>.Default.Equals(valueL1LossOfLockIndicator, L1LossOfLockIndicator))
        {
            L1LossOfLockIndicator = valueL1LossOfLockIndicator;
        }

        var valueL1LockTime = L1LockTime;
        UInt8Type.Accept(visitor, RtcmL1LockTimeField, RtcmL1LockTimeField.DataType, ref valueL1LockTime);
        if (!EqualityComparer<byte>.Default.Equals(valueL1LockTime, L1LockTime))
        {
            L1LockTime = valueL1LockTime;
        }

        var valueL1CNR = L1CNR;
        UInt16Type.Accept(visitor, RtcmL1CNRField, RtcmL1CNRField.DataType, ref valueL1CNR);
        if (!EqualityComparer<ushort>.Default.Equals(valueL1CNR, L1CNR))
        {
            L1CNR = valueL1CNR;
        }

        var valueL2Code = L2Code;
        UInt8Type.Accept(visitor, RtcmL2CodeField, RtcmL2CodeField.DataType, ref valueL2Code);
        if (!EqualityComparer<byte>.Default.Equals(valueL2Code, L2Code))
        {
            L2Code = valueL2Code;
        }

        var valueL2Frequency = L2Frequency;
        DoubleType.Accept(visitor, RtcmL2FrequencyField, RtcmL2FrequencyField.DataType, ref valueL2Frequency);
        if (!EqualityComparer<double>.Default.Equals(valueL2Frequency, L2Frequency))
        {
            L2Frequency = valueL2Frequency;
        }

        var valueL2PseudoRange = L2PseudoRange;
        DoubleType.Accept(visitor, RtcmL2PseudoRangeField, RtcmL2PseudoRangeField.DataType, ref valueL2PseudoRange);
        if (!EqualityComparer<double>.Default.Equals(valueL2PseudoRange, L2PseudoRange))
        {
            L2PseudoRange = valueL2PseudoRange;
        }

        var valueL2PhaseRange = L2PhaseRange;
        DoubleType.Accept(visitor, RtcmL2PhaseRangeField, RtcmL2PhaseRangeField.DataType, ref valueL2PhaseRange);
        if (!EqualityComparer<double>.Default.Equals(valueL2PhaseRange, L2PhaseRange))
        {
            L2PhaseRange = valueL2PhaseRange;
        }

        var valueL2CarrierPhase = L2CarrierPhase;
        DoubleType.Accept(visitor, RtcmL2CarrierPhaseField, RtcmL2CarrierPhaseField.DataType, ref valueL2CarrierPhase);
        if (!EqualityComparer<double>.Default.Equals(valueL2CarrierPhase, L2CarrierPhase))
        {
            L2CarrierPhase = valueL2CarrierPhase;
        }

        var valueL2LossOfLockIndicator = L2LossOfLockIndicator;
        BoolType.Accept(visitor, RtcmL2LossOfLockIndicatorField, RtcmL2LossOfLockIndicatorField.DataType, ref valueL2LossOfLockIndicator);
        if (!EqualityComparer<bool>.Default.Equals(valueL2LossOfLockIndicator, L2LossOfLockIndicator))
        {
            L2LossOfLockIndicator = valueL2LossOfLockIndicator;
        }

        var valueL2LockTime = L2LockTime;
        UInt8Type.Accept(visitor, RtcmL2LockTimeField, RtcmL2LockTimeField.DataType, ref valueL2LockTime);
        if (!EqualityComparer<byte>.Default.Equals(valueL2LockTime, L2LockTime))
        {
            L2LockTime = valueL2LockTime;
        }

        var valueL2CNR = L2CNR;
        UInt16Type.Accept(visitor, RtcmL2CNRField, RtcmL2CNRField.DataType, ref valueL2CNR);
        if (!EqualityComparer<ushort>.Default.Equals(valueL2CNR, L2CNR))
        {
            L2CNR = valueL2CNR;
        }
    }
}
