using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class SbfPacketGnssRawNavMsgBase
{
    private static readonly Field PayloadNavSystemField = SbfComNavVisitor.Field(nameof(NavSystem), Int32Type.Default, "NavSystem", enumType: typeof(SbfNavSysEnum));
    private static readonly Field PayloadNAVBitsU32Field = SbfComNavVisitor.Field(nameof(NAVBitsU32), new ListType(UInt32Type.Default, 0, ushort.MaxValue), "NAVBitsU32");
    private static readonly Field PayloadRxChannelField = SbfComNavVisitor.Field(nameof(RxChannel), UInt8Type.Default, "Receiver channel (see 4.1.11)");
    private static readonly Field PayloadFreqNrField = SbfComNavVisitor.Field(nameof(FreqNr), UInt8Type.Default, "GLONASS frequency number, with an offset of 8. It ranges from 1 (corresponding to an actual frequency number of -7) to 21 (corresponding to an actual frequency number of 13). For non-GLONASS satellites, FreqNr is reserved and must be ignored by the decoding software.");
    private static readonly Field PayloadSourceField = SbfComNavVisitor.Field(nameof(Source), UInt8Type.Default, "Bit field: Bits 0-4: Signal type from which the bits have been received, as defined in 4.1.10 Bits 5-7: Reserved");
    private static readonly Field PayloadViterbiCntField = SbfComNavVisitor.Field(nameof(ViterbiCnt), UInt8Type.Default, "Not applicable");
    private static readonly Field PayloadCrcPassedField = SbfComNavVisitor.Field(nameof(CrcPassed), BoolType.Default, "Status of the CRC or parity check: 0: CRC or parity check failed 1: CRC or parity check passed");
    private static readonly Field PayloadSvIdField = SbfComNavVisitor.Field(nameof(SvId), UInt8Type.Default, "Satellite ID, see 4.1.9");
    private static readonly Field PayloadRinexSatCodeField = SbfComNavVisitor.Field(nameof(RinexSatCode), StringOptionalType.Utf8, "RINEX satellite code");
    private static readonly Field PayloadSignalTypeField = SbfComNavVisitor.Field(nameof(SignalType), Int32Type.Default, "SignalType", enumType: typeof(SbfSignalTypeEnum));
    private static readonly Field PayloadRindexSignalCodeField = SbfComNavVisitor.Field(nameof(RindexSignalCode), StringOptionalType.Utf8, "RindexSignalCode");
    private static readonly Field PayloadCarrierFreqField = SbfComNavVisitor.Field(nameof(CarrierFreq), DoubleType.Default, "CarrierFreq");
    private static readonly Field PayloadSatPrnField = SbfComNavVisitor.Field(nameof(SatPrn), Int32Type.Default, "SatPrn");

    internal new static readonly StructType PayloadVisitorType = new([..SbfMessageBase.PayloadVisitorType.Fields, PayloadNavSystemField, PayloadNAVBitsU32Field, PayloadRxChannelField, PayloadFreqNrField, PayloadSourceField, PayloadViterbiCntField, PayloadCrcPassedField, PayloadSvIdField, PayloadRinexSatCodeField, PayloadSignalTypeField, PayloadRindexSignalCodeField, PayloadCarrierFreqField, PayloadSatPrnField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueNavSystem = NavSystem;
        var numericvalueNavSystem = (int)valueNavSystem;
        Int32Type.Accept(visitor, PayloadNavSystemField, PayloadNavSystemField.DataType, ref numericvalueNavSystem);
        valueNavSystem = (SbfNavSysEnum)numericvalueNavSystem;
        if (!EqualityComparer<SbfNavSysEnum>.Default.Equals(valueNavSystem, NavSystem))
        {
            NavSystem = valueNavSystem;
        }

        var valueNAVBitsU32 = NAVBitsU32;
        SbfComNavVisitor.VisitArray(visitor, PayloadNAVBitsU32Field, PayloadNAVBitsU32Field.DataType, ref valueNAVBitsU32, static () => default(uint),
            static (IVisitor v, Field f, IFieldType t, ref uint item) =>
            {
                UInt32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueNAVBitsU32, NAVBitsU32))
        {
            NAVBitsU32 = valueNAVBitsU32!;
        }

        var valueRxChannel = RxChannel;
        UInt8Type.Accept(visitor, PayloadRxChannelField, PayloadRxChannelField.DataType, ref valueRxChannel);
        if (!EqualityComparer<byte>.Default.Equals(valueRxChannel, RxChannel))
        {
            RxChannel = valueRxChannel;
        }

        var valueFreqNr = FreqNr;
        UInt8Type.Accept(visitor, PayloadFreqNrField, PayloadFreqNrField.DataType, ref valueFreqNr);
        if (!EqualityComparer<byte>.Default.Equals(valueFreqNr, FreqNr))
        {
            FreqNr = valueFreqNr;
        }

        var valueSource = Source;
        UInt8Type.Accept(visitor, PayloadSourceField, PayloadSourceField.DataType, ref valueSource);
        if (!EqualityComparer<byte>.Default.Equals(valueSource, Source))
        {
            Source = valueSource;
        }

        var valueViterbiCnt = ViterbiCnt;
        UInt8Type.Accept(visitor, PayloadViterbiCntField, PayloadViterbiCntField.DataType, ref valueViterbiCnt);
        if (!EqualityComparer<byte>.Default.Equals(valueViterbiCnt, ViterbiCnt))
        {
            ViterbiCnt = valueViterbiCnt;
        }

        var valueCrcPassed = CrcPassed;
        BoolType.Accept(visitor, PayloadCrcPassedField, PayloadCrcPassedField.DataType, ref valueCrcPassed);
        if (!EqualityComparer<bool>.Default.Equals(valueCrcPassed, CrcPassed))
        {
            CrcPassed = valueCrcPassed;
        }

        var valueSvId = SvId;
        UInt8Type.Accept(visitor, PayloadSvIdField, PayloadSvIdField.DataType, ref valueSvId);
        if (!EqualityComparer<byte>.Default.Equals(valueSvId, SvId))
        {
            SvId = valueSvId;
        }

        string? valueRinexSatCode = RinexSatCode;
        StringOptionalType.Accept(visitor, PayloadRinexSatCodeField, PayloadRinexSatCodeField.DataType, ref valueRinexSatCode);
        if (!EqualityComparer<string>.Default.Equals(valueRinexSatCode, RinexSatCode))
        {
            RinexSatCode = valueRinexSatCode!;
        }

        var valueSignalType = SignalType;
        var numericvalueSignalType = (int)valueSignalType;
        Int32Type.Accept(visitor, PayloadSignalTypeField, PayloadSignalTypeField.DataType, ref numericvalueSignalType);
        valueSignalType = (SbfSignalTypeEnum)numericvalueSignalType;
        if (!EqualityComparer<SbfSignalTypeEnum>.Default.Equals(valueSignalType, SignalType))
        {
            SignalType = valueSignalType;
        }

        string? valueRindexSignalCode = RindexSignalCode;
        StringOptionalType.Accept(visitor, PayloadRindexSignalCodeField, PayloadRindexSignalCodeField.DataType, ref valueRindexSignalCode);
        if (!EqualityComparer<string>.Default.Equals(valueRindexSignalCode, RindexSignalCode))
        {
            RindexSignalCode = valueRindexSignalCode!;
        }

        var valueCarrierFreq = CarrierFreq;
        DoubleType.Accept(visitor, PayloadCarrierFreqField, PayloadCarrierFreqField.DataType, ref valueCarrierFreq);
        if (!EqualityComparer<double>.Default.Equals(valueCarrierFreq, CarrierFreq))
        {
            CarrierFreq = valueCarrierFreq;
        }

        var valueSatPrn = SatPrn;
        Int32Type.Accept(visitor, PayloadSatPrnField, PayloadSatPrnField.DataType, ref valueSatPrn);
        if (!EqualityComparer<int>.Default.Equals(valueSatPrn, SatPrn))
        {
            SatPrn = valueSatPrn;
        }
    }
}
