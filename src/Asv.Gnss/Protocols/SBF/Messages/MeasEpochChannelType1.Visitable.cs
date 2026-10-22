using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MeasEpochChannelType1 : IVisitable
{
    private static readonly Field PayloadIsSmoothedField = SbfComNavVisitor.Field(nameof(IsSmoothed), BoolType.Default, "IsSmoothed");
    private static readonly Field PayloadIsHalfCycleAmbiguityField = SbfComNavVisitor.Field(nameof(IsHalfCycleAmbiguity), BoolType.Default, "IsHalfCycleAmbiguity");
    private static readonly Field PayloadSatSysField = SbfComNavVisitor.Field(nameof(SatSys), Int32Type.Default, "SatSys", enumType: typeof(SbfNavSysEnum));
    private static readonly Field PayloadRinexCodeField = SbfComNavVisitor.Field(nameof(RinexCode), StringOptionalType.Utf8, "RinexCode");
    private static readonly Field PayloadFrequencyField = SbfComNavVisitor.Field(nameof(Frequency), DoubleType.Default, "Frequency");
    private static readonly Field PayloadLockTimeField = SbfComNavVisitor.Field(nameof(LockTime), UInt16Type.Default, "Duration of continuous carrier phase. The lock-time is reset at the initial lock of the phase-locked-loop, and whenever a loss of lock condition occurs.");
    private static readonly Field PayloadCN0Field = SbfComNavVisitor.Field(nameof(CN0), DoubleType.Default, "The C/N0 in dB-Hz");
    private static readonly Field PayloadAntennaField = SbfComNavVisitor.Field(nameof(Antenna), Int32Type.Default, "Antenna", enumType: typeof(AntennaId));
    private static readonly Field PayloadCarrierPhaseField = SbfComNavVisitor.Field(nameof(CarrierPhase), DoubleType.Default, "MSB of the carrier phase relative to the pseudorange. The full carrier phase can be computed by: L[cycles] = PRtype1[m]/\u03bb +(CarrierMSB*65536+CarrierLSB)*0.001 where \u03bb is the carrier wavelength corresponding to the frequency of the signal type in the Type field above: \u03bb=299792458/fL m, with fL the carrier frequency as listed in section 4.1.10.");
    private static readonly Field PayloadDopplerHzField = SbfComNavVisitor.Field(nameof(DopplerHz), DoubleType.Default, "DopplerHz");
    private static readonly Field PayloadPRField = SbfComNavVisitor.Field(nameof(PR), DoubleType.Default, "SB of the pseudorange. The pseudorange expressed in meters is computed as follows: PRtype1[m] = (CodeMSB*4294967296+CodeLSB)*0.001 where CodeMSB is part of the Misc field");
    private static readonly Field PayloadSVIDField = SbfComNavVisitor.Field(nameof(SVID), UInt8Type.Default, "SVID");
    private static readonly Field PayloadRxChannelField = SbfComNavVisitor.Field(nameof(RxChannel), UInt8Type.Default, "RxChannel");

    internal static readonly StructType PayloadVisitorType = new([PayloadIsSmoothedField, PayloadIsHalfCycleAmbiguityField, PayloadSatSysField, PayloadRinexCodeField, PayloadFrequencyField, PayloadLockTimeField, PayloadCN0Field, PayloadAntennaField, PayloadCarrierPhaseField, PayloadDopplerHzField, PayloadPRField, PayloadSVIDField, PayloadRxChannelField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueIsSmoothed = IsSmoothed;
        BoolType.Accept(visitor, PayloadIsSmoothedField, PayloadIsSmoothedField.DataType, ref valueIsSmoothed);
        if (!EqualityComparer<bool>.Default.Equals(valueIsSmoothed, IsSmoothed))
        {
            IsSmoothed = valueIsSmoothed;
        }

        var valueIsHalfCycleAmbiguity = IsHalfCycleAmbiguity;
        BoolType.Accept(visitor, PayloadIsHalfCycleAmbiguityField, PayloadIsHalfCycleAmbiguityField.DataType, ref valueIsHalfCycleAmbiguity);
        if (!EqualityComparer<bool>.Default.Equals(valueIsHalfCycleAmbiguity, IsHalfCycleAmbiguity))
        {
            IsHalfCycleAmbiguity = valueIsHalfCycleAmbiguity;
        }

        var valueSatSys = SatSys;
        var numericvalueSatSys = (int)valueSatSys;
        Int32Type.Accept(visitor, PayloadSatSysField, PayloadSatSysField.DataType, ref numericvalueSatSys);
        valueSatSys = (SbfNavSysEnum)numericvalueSatSys;
        if (!EqualityComparer<SbfNavSysEnum>.Default.Equals(valueSatSys, SatSys))
        {
            SatSys = valueSatSys;
        }

        string? valueRinexCode = RinexCode;
        StringOptionalType.Accept(visitor, PayloadRinexCodeField, PayloadRinexCodeField.DataType, ref valueRinexCode);
        if (!EqualityComparer<string>.Default.Equals(valueRinexCode, RinexCode))
        {
            RinexCode = valueRinexCode!;
        }

        var valueFrequency = Frequency;
        DoubleType.Accept(visitor, PayloadFrequencyField, PayloadFrequencyField.DataType, ref valueFrequency);
        if (!EqualityComparer<double>.Default.Equals(valueFrequency, Frequency))
        {
            Frequency = valueFrequency;
        }

        var valueLockTime = LockTime;
        UInt16Type.Accept(visitor, PayloadLockTimeField, PayloadLockTimeField.DataType, ref valueLockTime);
        if (!EqualityComparer<ushort>.Default.Equals(valueLockTime, LockTime))
        {
            LockTime = valueLockTime;
        }

        var valueCN0 = CN0;
        DoubleType.Accept(visitor, PayloadCN0Field, PayloadCN0Field.DataType, ref valueCN0);
        if (!EqualityComparer<double>.Default.Equals(valueCN0, CN0))
        {
            CN0 = valueCN0;
        }

        var valueAntenna = Antenna;
        var numericvalueAntenna = (int)valueAntenna;
        Int32Type.Accept(visitor, PayloadAntennaField, PayloadAntennaField.DataType, ref numericvalueAntenna);
        valueAntenna = (AntennaId)numericvalueAntenna;
        if (!EqualityComparer<AntennaId>.Default.Equals(valueAntenna, Antenna))
        {
            Antenna = valueAntenna;
        }

        var valueCarrierPhase = CarrierPhase;
        DoubleType.Accept(visitor, PayloadCarrierPhaseField, PayloadCarrierPhaseField.DataType, ref valueCarrierPhase);
        if (!EqualityComparer<double>.Default.Equals(valueCarrierPhase, CarrierPhase))
        {
            CarrierPhase = valueCarrierPhase;
        }

        var valueDopplerHz = DopplerHz;
        DoubleType.Accept(visitor, PayloadDopplerHzField, PayloadDopplerHzField.DataType, ref valueDopplerHz);
        if (!EqualityComparer<double>.Default.Equals(valueDopplerHz, DopplerHz))
        {
            DopplerHz = valueDopplerHz;
        }

        var valuePR = PR;
        DoubleType.Accept(visitor, PayloadPRField, PayloadPRField.DataType, ref valuePR);
        if (!EqualityComparer<double>.Default.Equals(valuePR, PR))
        {
            PR = valuePR;
        }

        var valueSVID = SVID;
        UInt8Type.Accept(visitor, PayloadSVIDField, PayloadSVIDField.DataType, ref valueSVID);
        if (!EqualityComparer<byte>.Default.Equals(valueSVID, SVID))
        {
            SVID = valueSVID;
        }

        var valueRxChannel = RxChannel;
        UInt8Type.Accept(visitor, PayloadRxChannelField, PayloadRxChannelField.DataType, ref valueRxChannel);
        if (!EqualityComparer<byte>.Default.Equals(valueRxChannel, RxChannel))
        {
            RxChannel = valueRxChannel;
        }
    }
}
