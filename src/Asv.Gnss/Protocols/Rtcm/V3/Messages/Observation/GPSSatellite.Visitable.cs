using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class GPSSatellite : IVisitable
{
    private static readonly Field RtcmSatellitePrnField = RtcmV3Visitor.Field(nameof(SatellitePrn), Int32Type.Default, "A GPS Satellite ID number from 1 to 32 refers to the PRN code of the GPS satellite. Satellite ID’s higher than 32 are reserved for satellite signals from Satellite-Based Augmentation Systems (SBAS’s) such as the FAA’s Wide-Area Augmentation System (WAAS). SBAS PRN codes cover the range 120-138. The Satellite ID’s reserved for SBAS satellites are 40-58, so that the SBAS PRN codes are derived from the Version 3 Satellite ID codes by adding 80.");
    private static readonly Field RtcmSatelliteNumberField = RtcmV3Visitor.Field(nameof(SatelliteNumber), Int32Type.Default, "SatelliteNumber");
    private static readonly Field RtcmSatelliteCodeField = RtcmV3Visitor.Field(nameof(SatelliteCode), StringOptionalType.Utf8, "SatelliteCode");
    private static readonly Field RtcmL1CodeField = RtcmV3Visitor.Field(nameof(L1Code), UInt8Type.Default, "The GPS L1 Code Indicator identifies the code being tracked by the reference station. Civil receivers can track the C/A code, and optionally the P code, while military receivers can track C/A, and can also track P and Y code, whichever is broadcast by the satellite 0 - C/A Code 1 - P(Y) Code Direct");
    private static readonly Field RtcmL1PseudoRangeField = RtcmV3Visitor.Field(nameof(L1PseudoRange), DoubleType.Default, "The GPS L1 Pseudorange field provides the raw L1 pseudorange measurement at the reference station in meters, modulo one lightmillisecond (299,792.458 meters). The GPS L1 pseudorange measurement is reconstructed by the user receiver from the L1 pseudorange field by: (GPS L1 pseudorange measurement) = (GPS L1 pseudorange field) modulo (299,792.458 m) + integer as determined from the user receiver's estimate of the reference station range, or as provided by the extended data set. 80000h - invalid L1 pseudorange; used only in the calculation of L2 measurements.");
    private static readonly Field RtcmL1PhaseRangeField = RtcmV3Visitor.Field(nameof(L1PhaseRange), DoubleType.Default, "The GPS L1 PhaseRange – L1 Pseudorange field provides the information necessary to determine the L1 phase measurement. Note that the PhaseRange defined here has the same sign as the pseudorange. The PhaseRange has much higher resolution than the pseudorange, so that providing this field is just a numerical technique to reduce the length of the message. At start up and after each cycle slip, the initial ambiguity is reset and chosen so that the L1 PhaseRange should match the L1 Pseudorange as closely as possible (i.e., within 1/2 L1 cycle) while not destroying the integer nature of the original carrier phase observation. The Full GPS L1 PhaseRange is constructed as follows (all quantities in units of meters): (Full L1 PhaseRange) = (L1 pseudorange as reconstructed from L1 pseudorange field) + (GPS L1 PhaseRange – L1 Pseudorange field) Certain ionospheric conditions might cause the GPS L1 PhaseRange – L1 Pseudorange to diverge over time across the range limits defined. Under these circumstances the computed value needs to be adjusted (rolled over) by the equivalent of 1500 cycles in order to bring the value back within the range. See also comments in sections 3.1.6 and 3.5.1 for correction of antenna phase center variations in Network RTK applications. 80000h - L1 phase is invalid; used only in the calculation of L2 measurements.");
    private static readonly Field RtcmL1CarrierPhaseField = RtcmV3Visitor.Field(nameof(L1CarrierPhase), DoubleType.Default, "L1CarrierPhase");
    private static readonly Field RtcmL1LossOfLockIndicatorField = RtcmV3Visitor.Field(nameof(L1LossOfLockIndicator), BoolType.Default, "The GPS L1 Lock Time Indicator provides a measure of the amount of time that has elapsed during which the Reference Station receiver has maintained continuous lock on that satellite signal. If a cycle slip occurs during the previous measurement cycle, the lock indicator will be reset to zero.");
    private static readonly Field RtcmL1LockTimeField = RtcmV3Visitor.Field(nameof(L1LockTime), UInt8Type.Default, "L1LockTime");
    private static readonly Field RtcmL1CNRField = RtcmV3Visitor.Field(nameof(L1CNR), UInt16Type.Default, "The GPS L1 CNR measurements provide the reference station's estimate of the carrier-to-noise ratio of the satellite’s signal in dB-Hz. 0 - the CNR measurement is not computed.");
    private static readonly Field RtcmL2CodeField = RtcmV3Visitor.Field(nameof(L2Code), UInt8Type.Default, "The GPS L2 Code Indicator depicts which L2 code is processed by the reference station, and how it is processed. 0 - C/A or L2C code 1 - P(Y) code direct 2 - P(Y) code cross-correlated 3 - Correlated P/Y The GPS L2 Code Indicator refers to the method used by the GPS reference station receiver to recover the L2 pseudorange. The GPS L2 Code Indicator should be set to “0” (C/A or L2C code) for any of the L2 civil codes. It is assumed here that a satellite will not transmit both C/A code and L2C code signals on L2 simultaneously, so that the reference station and user receivers will always utilize the same signal. The code indicator should be set to “1” if the satellite’s signal is correlated directly, i.e., either P code or Y code depending whether anti-spoofing (AS) is switched off or on. The code indicator should be set to “2” when the reference station receiver L2 pseudorange measurement is derived by adding a cross-correlated pseudorange measurement (Y2-Y1) to the measured L1 C/A code. The code indicator should be set to 3 when the GPS reference station receiver is using a proprietary method that uses only the L2 P(Y) code signal to derive L2 pseudorange.");
    private static readonly Field RtcmL2PseudoRangeField = RtcmV3Visitor.Field(nameof(L2PseudoRange), DoubleType.Default, "The GPS L2-L1 Pseudorange Difference field is utilized, rather than the full L2 pseudorange, in order to reduce the message length. The receiver must reconstruct the L2 code phase pseudorange by using the following formula: (GPS L2 pseudorange measurement) = (GPS L1 pseudorange as reconstructed from L1 pseudorange field) + (GPS L2-L1 pseudorange field) 2000h (-163.84m) - no valid L2 code available, or that the value exceeds the allowed range.");
    private static readonly Field RtcmL2PhaseRangeField = RtcmV3Visitor.Field(nameof(L2PhaseRange), DoubleType.Default, "The GPS L2 PhaseRange - L1 Pseudorange field provides the information necessary to determine the L2 phase measurement. Note that the PhaseRange defined here has the same sign as the pseudorange. The PhaseRange has much higher resolution than the pseudorange, so that providing this field is just a numerical technique to reduce the length of the message. At start up and after each cycle slip, the initial ambiguity is reset and chosen so that the L2 PhaseRange should match the L1 Pseudorange as closely as possible (i.e., within 1/2 L2 cycle) while not destroying the integer nature of the original carrier phase observation. The Full GPS L2 PhaseRange is constructed as follows (all quantities in units of meters): (Full L2 PhaseRange) = (L1 pseudorange as reconstructed from L1 pseudorange field) + (GPS L2 PhaseRange – L1 Pseudorange field) Certain ionospheric conditions might cause the GPS L2 PhaseRange – L1 Pseudorange to diverge over time across the range limits defined. Under these circumstances the computed value needs to be adjusted (rolled over) by the equivalent of 1500 cycles in order to bring the value back within the range. Note: A bit pattern equivalent to 80000h in this field indicates an invalid carrier phase measurement that should not be processed by the mobile receiver. This indication may be used at low signal levels where carrier tracking is temporarily lost, but code tracking is still possible. See also comments in sections 3.1.6 and 3.5.1 for correction of antenna phase center variations in Network RTK applications.");
    private static readonly Field RtcmL2CarrierPhaseField = RtcmV3Visitor.Field(nameof(L2CarrierPhase), DoubleType.Default, "L2CarrierPhase");
    private static readonly Field RtcmL2LossOfLockIndicatorField = RtcmV3Visitor.Field(nameof(L2LossOfLockIndicator), BoolType.Default, "The GPS L2 Lock Time Indicator provides a measure of the amount of time that has elapsed during which the Reference Station receiver has maintained continuous lock on that satellite signal. If a cycle slip occurs during the previous measurement cycle, the lock indicator will be reset to zero.");
    private static readonly Field RtcmL2LockTimeField = RtcmV3Visitor.Field(nameof(L2LockTime), UInt8Type.Default, "L2LockTime");
    private static readonly Field RtcmL2CNRField = RtcmV3Visitor.Field(nameof(L2CNR), UInt16Type.Default, "The GPS L2 CNR measurements provide the reference station's estimate of the carrier-to-noise ratio of the satellite’s signal in dB-Hz. 0 - the CNR measurement is not computed");

    internal static readonly StructType RtcmVisitorType = new([RtcmSatellitePrnField, RtcmSatelliteNumberField, RtcmSatelliteCodeField, RtcmL1CodeField, RtcmL1PseudoRangeField, RtcmL1PhaseRangeField, RtcmL1CarrierPhaseField, RtcmL1LossOfLockIndicatorField, RtcmL1LockTimeField, RtcmL1CNRField, RtcmL2CodeField, RtcmL2PseudoRangeField, RtcmL2PhaseRangeField, RtcmL2CarrierPhaseField, RtcmL2LossOfLockIndicatorField, RtcmL2LockTimeField, RtcmL2CNRField]);

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
