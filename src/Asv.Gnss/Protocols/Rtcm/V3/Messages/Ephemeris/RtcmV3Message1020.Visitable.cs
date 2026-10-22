using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1020
{
    private static readonly Field RtcmMLFifthStringWordField = RtcmV3Visitor.Field(nameof(MLFifthStringWord), UInt32Type.Default, "MLFifthStringWord");
    private static readonly Field RtcmMTgpsField = RtcmV3Visitor.Field(nameof(MTgps), Int32Type.Default, "MTgps");
    private static readonly Field RtcmMN4Field = RtcmV3Visitor.Field(nameof(MN4), UInt32Type.Default, "MN4");
    private static readonly Field RtcmTcField = RtcmV3Visitor.Field(nameof(Tc), Int32Type.Default, "Tc");
    private static readonly Field RtcmNaField = RtcmV3Visitor.Field(nameof(Na), UInt32Type.Default, "Na");
    private static readonly Field RtcmAvailabilityOfAdditionalDataField = RtcmV3Visitor.Field(nameof(AvailabilityOfAdditionalData), UInt32Type.Default, "AvailabilityOfAdditionalData");
    private static readonly Field RtcmMMWordField = RtcmV3Visitor.Field(nameof(MMWord), UInt32Type.Default, "MMWord");
    private static readonly Field RtcmMNtWordField = RtcmV3Visitor.Field(nameof(MNtWord), UInt32Type.Default, "MNtWord");
    private static readonly Field RtcmMFtWordField = RtcmV3Visitor.Field(nameof(MFtWord), UInt32Type.Default, "MFtWord");
    private static readonly Field RtcmMP4WordField = RtcmV3Visitor.Field(nameof(MP4Word), UInt32Type.Default, "MP4Word");
    private static readonly Field RtcmMLThirdStringWordField = RtcmV3Visitor.Field(nameof(MLThirdStringWord), UInt32Type.Default, "MLThirdStringWord");
    private static readonly Field RtcmMPWordField = RtcmV3Visitor.Field(nameof(MPWord), UInt32Type.Default, "MPWord");
    private static readonly Field RtcmP3WordField = RtcmV3Visitor.Field(nameof(P3Word), UInt32Type.Default, "P3Word");
    private static readonly Field RtcmP2WordField = RtcmV3Visitor.Field(nameof(P2Word), UInt32Type.Default, "P2Word");
    private static readonly Field RtcmP1WordField = RtcmV3Visitor.Field(nameof(P1Word), UInt32Type.Default, "P1Word");
    private static readonly Field RtcmAlmanacHealthAvailabilityIndicatorField = RtcmV3Visitor.Field(nameof(AlmanacHealthAvailabilityIndicator), UInt32Type.Default, "AlmanacHealthAvailabilityIndicator");
    private static readonly Field RtcmAlmanacHealthField = RtcmV3Visitor.Field(nameof(AlmanacHealth), UInt32Type.Default, "AlmanacHealth");
    private static readonly Field RtcmSatelliteNumberField = RtcmV3Visitor.Field(nameof(SatelliteNumber), Int32Type.Default, "satellite number");
    private static readonly Field RtcmSatellitePrnField = RtcmV3Visitor.Field(nameof(SatellitePrn), Int32Type.Default, "SatellitePrn");
    private static readonly Field RtcmSatelliteCodeField = RtcmV3Visitor.Field(nameof(SatelliteCode), StringOptionalType.Utf8, "SatelliteCode");
    private static readonly Field RtcmIodeField = RtcmV3Visitor.Field(nameof(Iode), Int32Type.Default, "Iode");
    private static readonly Field RtcmFrequencyNumberField = RtcmV3Visitor.Field(nameof(FrequencyNumber), Int32Type.Default, "satellite frequency number");
    private static readonly Field RtcmOperationHealthField = RtcmV3Visitor.Field(nameof(OperationHealth), Int32Type.Default, "satellite health of operation");
    private static readonly Field RtcmOperationAccuracyField = RtcmV3Visitor.Field(nameof(OperationAccuracy), Int32Type.Default, "satellite accuracy of operation");
    private static readonly Field RtcmOperationAgeField = RtcmV3Visitor.Field(nameof(OperationAge), Int32Type.Default, "satellite age of operation");
    private static readonly Field RtcmEphemerisEpochField = RtcmV3Visitor.Field(nameof(EphemerisEpoch), DateTimeType.Default, "EphemerisEpoch");
    private static readonly Field RtcmFrameTimeField = RtcmV3Visitor.Field(nameof(FrameTime), DateTimeType.Default, "FrameTime");
    private static readonly Field RtcmPositionField = RtcmV3Visitor.Field(nameof(Position), EcefPoint.RtcmVisitorType, "Position", readOnly: true);
    private static readonly Field RtcmVelocityField = RtcmV3Visitor.Field(nameof(Velocity), EcefPoint.RtcmVisitorType, "Velocity", readOnly: true);
    private static readonly Field RtcmAccelerationField = RtcmV3Visitor.Field(nameof(Acceleration), EcefPoint.RtcmVisitorType, "Acceleration", readOnly: true);
    private static readonly Field RtcmSvClockBiasField = RtcmV3Visitor.Field(nameof(SvClockBias), DoubleType.Default, "SvClockBias");
    private static readonly Field RtcmRelativeFreqBiasField = RtcmV3Visitor.Field(nameof(RelativeFreqBias), DoubleType.Default, "RelativeFreqBias");
    private static readonly Field RtcmDelayL1ToL2Field = RtcmV3Visitor.Field(nameof(DelayL1ToL2), DoubleType.Default, "DelayL1ToL2");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmMLFifthStringWordField, RtcmMTgpsField, RtcmMN4Field, RtcmTcField, RtcmNaField, RtcmAvailabilityOfAdditionalDataField, RtcmMMWordField, RtcmMNtWordField, RtcmMFtWordField, RtcmMP4WordField, RtcmMLThirdStringWordField, RtcmMPWordField, RtcmP3WordField, RtcmP2WordField, RtcmP1WordField, RtcmAlmanacHealthAvailabilityIndicatorField, RtcmAlmanacHealthField, RtcmSatelliteNumberField, RtcmSatellitePrnField, RtcmSatelliteCodeField, RtcmIodeField, RtcmFrequencyNumberField, RtcmOperationHealthField, RtcmOperationAccuracyField, RtcmOperationAgeField, RtcmEphemerisEpochField, RtcmFrameTimeField, RtcmPositionField, RtcmVelocityField, RtcmAccelerationField, RtcmSvClockBiasField, RtcmRelativeFreqBiasField, RtcmDelayL1ToL2Field]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueMLFifthStringWord = MLFifthStringWord;
        UInt32Type.Accept(visitor, RtcmMLFifthStringWordField, RtcmMLFifthStringWordField.DataType, ref valueMLFifthStringWord);
        if (!EqualityComparer<uint>.Default.Equals(valueMLFifthStringWord, MLFifthStringWord))
        {
            MLFifthStringWord = valueMLFifthStringWord;
        }

        var valueMTgps = MTgps;
        Int32Type.Accept(visitor, RtcmMTgpsField, RtcmMTgpsField.DataType, ref valueMTgps);
        if (!EqualityComparer<int>.Default.Equals(valueMTgps, MTgps))
        {
            MTgps = valueMTgps;
        }

        var valueMN4 = MN4;
        UInt32Type.Accept(visitor, RtcmMN4Field, RtcmMN4Field.DataType, ref valueMN4);
        if (!EqualityComparer<uint>.Default.Equals(valueMN4, MN4))
        {
            MN4 = valueMN4;
        }

        var valueTc = Tc;
        Int32Type.Accept(visitor, RtcmTcField, RtcmTcField.DataType, ref valueTc);
        if (!EqualityComparer<int>.Default.Equals(valueTc, Tc))
        {
            Tc = valueTc;
        }

        var valueNa = Na;
        UInt32Type.Accept(visitor, RtcmNaField, RtcmNaField.DataType, ref valueNa);
        if (!EqualityComparer<uint>.Default.Equals(valueNa, Na))
        {
            Na = valueNa;
        }

        var valueAvailabilityOfAdditionalData = AvailabilityOfAdditionalData;
        UInt32Type.Accept(visitor, RtcmAvailabilityOfAdditionalDataField, RtcmAvailabilityOfAdditionalDataField.DataType, ref valueAvailabilityOfAdditionalData);
        if (!EqualityComparer<uint>.Default.Equals(valueAvailabilityOfAdditionalData, AvailabilityOfAdditionalData))
        {
            AvailabilityOfAdditionalData = valueAvailabilityOfAdditionalData;
        }

        var valueMMWord = MMWord;
        UInt32Type.Accept(visitor, RtcmMMWordField, RtcmMMWordField.DataType, ref valueMMWord);
        if (!EqualityComparer<uint>.Default.Equals(valueMMWord, MMWord))
        {
            MMWord = valueMMWord;
        }

        var valueMNtWord = MNtWord;
        UInt32Type.Accept(visitor, RtcmMNtWordField, RtcmMNtWordField.DataType, ref valueMNtWord);
        if (!EqualityComparer<uint>.Default.Equals(valueMNtWord, MNtWord))
        {
            MNtWord = valueMNtWord;
        }

        var valueMFtWord = MFtWord;
        UInt32Type.Accept(visitor, RtcmMFtWordField, RtcmMFtWordField.DataType, ref valueMFtWord);
        if (!EqualityComparer<uint>.Default.Equals(valueMFtWord, MFtWord))
        {
            MFtWord = valueMFtWord;
        }

        var valueMP4Word = MP4Word;
        UInt32Type.Accept(visitor, RtcmMP4WordField, RtcmMP4WordField.DataType, ref valueMP4Word);
        if (!EqualityComparer<uint>.Default.Equals(valueMP4Word, MP4Word))
        {
            MP4Word = valueMP4Word;
        }

        var valueMLThirdStringWord = MLThirdStringWord;
        UInt32Type.Accept(visitor, RtcmMLThirdStringWordField, RtcmMLThirdStringWordField.DataType, ref valueMLThirdStringWord);
        if (!EqualityComparer<uint>.Default.Equals(valueMLThirdStringWord, MLThirdStringWord))
        {
            MLThirdStringWord = valueMLThirdStringWord;
        }

        var valueMPWord = MPWord;
        UInt32Type.Accept(visitor, RtcmMPWordField, RtcmMPWordField.DataType, ref valueMPWord);
        if (!EqualityComparer<uint>.Default.Equals(valueMPWord, MPWord))
        {
            MPWord = valueMPWord;
        }

        var valueP3Word = P3Word;
        UInt32Type.Accept(visitor, RtcmP3WordField, RtcmP3WordField.DataType, ref valueP3Word);
        if (!EqualityComparer<uint>.Default.Equals(valueP3Word, P3Word))
        {
            P3Word = valueP3Word;
        }

        var valueP2Word = P2Word;
        UInt32Type.Accept(visitor, RtcmP2WordField, RtcmP2WordField.DataType, ref valueP2Word);
        if (!EqualityComparer<uint>.Default.Equals(valueP2Word, P2Word))
        {
            P2Word = valueP2Word;
        }

        var valueP1Word = P1Word;
        UInt32Type.Accept(visitor, RtcmP1WordField, RtcmP1WordField.DataType, ref valueP1Word);
        if (!EqualityComparer<uint>.Default.Equals(valueP1Word, P1Word))
        {
            P1Word = valueP1Word;
        }

        var valueAlmanacHealthAvailabilityIndicator = AlmanacHealthAvailabilityIndicator;
        UInt32Type.Accept(visitor, RtcmAlmanacHealthAvailabilityIndicatorField, RtcmAlmanacHealthAvailabilityIndicatorField.DataType, ref valueAlmanacHealthAvailabilityIndicator);
        if (!EqualityComparer<uint>.Default.Equals(valueAlmanacHealthAvailabilityIndicator, AlmanacHealthAvailabilityIndicator))
        {
            AlmanacHealthAvailabilityIndicator = valueAlmanacHealthAvailabilityIndicator;
        }

        var valueAlmanacHealth = AlmanacHealth;
        UInt32Type.Accept(visitor, RtcmAlmanacHealthField, RtcmAlmanacHealthField.DataType, ref valueAlmanacHealth);
        if (!EqualityComparer<uint>.Default.Equals(valueAlmanacHealth, AlmanacHealth))
        {
            AlmanacHealth = valueAlmanacHealth;
        }

        var valueSatelliteNumber = SatelliteNumber;
        Int32Type.Accept(visitor, RtcmSatelliteNumberField, RtcmSatelliteNumberField.DataType, ref valueSatelliteNumber);
        if (!EqualityComparer<int>.Default.Equals(valueSatelliteNumber, SatelliteNumber))
        {
            SatelliteNumber = valueSatelliteNumber;
        }

        var valueSatellitePrn = SatellitePrn;
        Int32Type.Accept(visitor, RtcmSatellitePrnField, RtcmSatellitePrnField.DataType, ref valueSatellitePrn);
        if (!EqualityComparer<int>.Default.Equals(valueSatellitePrn, SatellitePrn))
        {
            SatellitePrn = valueSatellitePrn;
        }

        string? valueSatelliteCode = SatelliteCode;
        StringOptionalType.Accept(visitor, RtcmSatelliteCodeField, RtcmSatelliteCodeField.DataType, ref valueSatelliteCode);
        if (!EqualityComparer<string>.Default.Equals(valueSatelliteCode, SatelliteCode))
        {
            SatelliteCode = valueSatelliteCode!;
        }

        var valueIode = Iode;
        Int32Type.Accept(visitor, RtcmIodeField, RtcmIodeField.DataType, ref valueIode);
        if (!EqualityComparer<int>.Default.Equals(valueIode, Iode))
        {
            Iode = valueIode;
        }

        var valueFrequencyNumber = FrequencyNumber;
        Int32Type.Accept(visitor, RtcmFrequencyNumberField, RtcmFrequencyNumberField.DataType, ref valueFrequencyNumber);
        if (!EqualityComparer<int>.Default.Equals(valueFrequencyNumber, FrequencyNumber))
        {
            FrequencyNumber = valueFrequencyNumber;
        }

        var valueOperationHealth = OperationHealth;
        Int32Type.Accept(visitor, RtcmOperationHealthField, RtcmOperationHealthField.DataType, ref valueOperationHealth);
        if (!EqualityComparer<int>.Default.Equals(valueOperationHealth, OperationHealth))
        {
            OperationHealth = valueOperationHealth;
        }

        var valueOperationAccuracy = OperationAccuracy;
        Int32Type.Accept(visitor, RtcmOperationAccuracyField, RtcmOperationAccuracyField.DataType, ref valueOperationAccuracy);
        if (!EqualityComparer<int>.Default.Equals(valueOperationAccuracy, OperationAccuracy))
        {
            OperationAccuracy = valueOperationAccuracy;
        }

        var valueOperationAge = OperationAge;
        Int32Type.Accept(visitor, RtcmOperationAgeField, RtcmOperationAgeField.DataType, ref valueOperationAge);
        if (!EqualityComparer<int>.Default.Equals(valueOperationAge, OperationAge))
        {
            OperationAge = valueOperationAge;
        }

        var valueEphemerisEpoch = EphemerisEpoch;
        DateTimeType.Accept(visitor, RtcmEphemerisEpochField, RtcmEphemerisEpochField.DataType, ref valueEphemerisEpoch);
        if (!EqualityComparer<DateTime>.Default.Equals(valueEphemerisEpoch, EphemerisEpoch))
        {
            EphemerisEpoch = valueEphemerisEpoch;
        }

        var valueFrameTime = FrameTime;
        DateTimeType.Accept(visitor, RtcmFrameTimeField, RtcmFrameTimeField.DataType, ref valueFrameTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueFrameTime, FrameTime))
        {
            FrameTime = valueFrameTime;
        }

        var valuePosition = Position;
        StructType.Accept(visitor, RtcmPositionField, valuePosition);

        var valueVelocity = Velocity;
        StructType.Accept(visitor, RtcmVelocityField, valueVelocity);

        var valueAcceleration = Acceleration;
        StructType.Accept(visitor, RtcmAccelerationField, valueAcceleration);

        var valueSvClockBias = SvClockBias;
        DoubleType.Accept(visitor, RtcmSvClockBiasField, RtcmSvClockBiasField.DataType, ref valueSvClockBias);
        if (!EqualityComparer<double>.Default.Equals(valueSvClockBias, SvClockBias))
        {
            SvClockBias = valueSvClockBias;
        }

        var valueRelativeFreqBias = RelativeFreqBias;
        DoubleType.Accept(visitor, RtcmRelativeFreqBiasField, RtcmRelativeFreqBiasField.DataType, ref valueRelativeFreqBias);
        if (!EqualityComparer<double>.Default.Equals(valueRelativeFreqBias, RelativeFreqBias))
        {
            RelativeFreqBias = valueRelativeFreqBias;
        }

        var valueDelayL1ToL2 = DelayL1ToL2;
        DoubleType.Accept(visitor, RtcmDelayL1ToL2Field, RtcmDelayL1ToL2Field.DataType, ref valueDelayL1ToL2);
        if (!EqualityComparer<double>.Default.Equals(valueDelayL1ToL2, DelayL1ToL2))
        {
            DelayL1ToL2 = valueDelayL1ToL2;
        }
    }
}
