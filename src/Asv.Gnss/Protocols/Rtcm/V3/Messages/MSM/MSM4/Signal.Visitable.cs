using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class Signal : IVisitable
{
    private static readonly Field RtcmRinexCodeField = RtcmV3Visitor.Field(nameof(RinexCode), StringOptionalType.Utf8, "RinexCode");
    private static readonly Field RtcmPseudoRangeField = RtcmV3Visitor.Field(nameof(PseudoRange), DoubleType.Default, "Observation data PseudoRange (m)");
    private static readonly Field RtcmCarrierPhaseField = RtcmV3Visitor.Field(nameof(CarrierPhase), DoubleType.Default, "Observation data carrier-phase (m)");
    private static readonly Field RtcmPhaseRangeRateField = RtcmV3Visitor.Field(nameof(PhaseRangeRate), DoubleType.Default, "Observation data PhaseRangeRate (hz)");
    private static readonly Field RtcmCnrField = RtcmV3Visitor.Field(nameof(Cnr), DoubleType.Default, "Signal strength (0.001 dBHz)");
    private static readonly Field RtcmLockTimeField = RtcmV3Visitor.Field(nameof(LockTime), UInt16Type.Default, "");
    private static readonly Field RtcmMinLockTimeField = RtcmV3Visitor.Field(nameof(MinLockTime), DoubleType.Default, "Min lock time in min");
    private static readonly Field RtcmHalfCycleField = RtcmV3Visitor.Field(nameof(HalfCycle), UInt8Type.Default, "");
    private static readonly Field RtcmObservationCodeField = RtcmV3Visitor.Field(nameof(ObservationCode), UInt8Type.Default, "");

    internal static readonly StructType RtcmVisitorType = new([RtcmRinexCodeField, RtcmPseudoRangeField, RtcmCarrierPhaseField, RtcmPhaseRangeRateField, RtcmCnrField, RtcmLockTimeField, RtcmMinLockTimeField, RtcmHalfCycleField, RtcmObservationCodeField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        string? valueRinexCode = RinexCode;
        StringOptionalType.Accept(visitor, RtcmRinexCodeField, RtcmRinexCodeField.DataType, ref valueRinexCode);
        if (!EqualityComparer<string>.Default.Equals(valueRinexCode, RinexCode))
        {
            RinexCode = valueRinexCode!;
        }

        var valuePseudoRange = PseudoRange;
        DoubleType.Accept(visitor, RtcmPseudoRangeField, RtcmPseudoRangeField.DataType, ref valuePseudoRange);
        if (!EqualityComparer<double>.Default.Equals(valuePseudoRange, PseudoRange))
        {
            PseudoRange = valuePseudoRange;
        }

        var valueCarrierPhase = CarrierPhase;
        DoubleType.Accept(visitor, RtcmCarrierPhaseField, RtcmCarrierPhaseField.DataType, ref valueCarrierPhase);
        if (!EqualityComparer<double>.Default.Equals(valueCarrierPhase, CarrierPhase))
        {
            CarrierPhase = valueCarrierPhase;
        }

        var valuePhaseRangeRate = PhaseRangeRate;
        DoubleType.Accept(visitor, RtcmPhaseRangeRateField, RtcmPhaseRangeRateField.DataType, ref valuePhaseRangeRate);
        if (!EqualityComparer<double>.Default.Equals(valuePhaseRangeRate, PhaseRangeRate))
        {
            PhaseRangeRate = valuePhaseRangeRate;
        }

        var valueCnr = Cnr;
        DoubleType.Accept(visitor, RtcmCnrField, RtcmCnrField.DataType, ref valueCnr);
        if (!EqualityComparer<double>.Default.Equals(valueCnr, Cnr))
        {
            Cnr = valueCnr;
        }

        var valueLockTime = LockTime;
        UInt16Type.Accept(visitor, RtcmLockTimeField, RtcmLockTimeField.DataType, ref valueLockTime);
        if (!EqualityComparer<ushort>.Default.Equals(valueLockTime, LockTime))
        {
            LockTime = valueLockTime;
        }

        var valueMinLockTime = MinLockTime;
        DoubleType.Accept(visitor, RtcmMinLockTimeField, RtcmMinLockTimeField.DataType, ref valueMinLockTime);
        if (!EqualityComparer<double>.Default.Equals(valueMinLockTime, MinLockTime))
        {
            MinLockTime = valueMinLockTime;
        }

        var valueHalfCycle = HalfCycle;
        UInt8Type.Accept(visitor, RtcmHalfCycleField, RtcmHalfCycleField.DataType, ref valueHalfCycle);
        if (!EqualityComparer<byte>.Default.Equals(valueHalfCycle, HalfCycle))
        {
            HalfCycle = valueHalfCycle;
        }

        var valueObservationCode = ObservationCode;
        UInt8Type.Accept(visitor, RtcmObservationCodeField, RtcmObservationCodeField.DataType, ref valueObservationCode);
        if (!EqualityComparer<byte>.Default.Equals(valueObservationCode, ObservationCode))
        {
            ObservationCode = valueObservationCode;
        }
    }
}
