using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessageGpsRawCa
{
    private static readonly Field PayloadUtcTimeField = ProtocolPayloadVisitor.Field(nameof(UtcTime), DateTimeType.Default);
    private static readonly Field PayloadPrnField = ProtocolPayloadVisitor.Field(nameof(Prn), Int32Type.Default);
    private static readonly Field PayloadSatelliteIdField = ProtocolPayloadVisitor.Field(nameof(SatelliteId), Int32Type.Default);
    private static readonly Field PayloadCrcPassedField = ProtocolPayloadVisitor.Field(nameof(CrcPassed), BoolType.Default);
    private static readonly Field PayloadL1CodeField = ProtocolPayloadVisitor.Field(nameof(L1Code), UInt8Type.Default);
    private static readonly Field PayloadSvIdField = ProtocolPayloadVisitor.Field(nameof(SvId), UInt16Type.Default);
    private static readonly Field PayloadRinexSatCodeField = ProtocolPayloadVisitor.Field(nameof(RinexSatCode), StringOptionalType.Utf8);
    private static readonly Field PayloadSignalTypeField = ProtocolPayloadVisitor.Field(nameof(SignalType), Int32Type.Default, enumType: typeof(GnssSignalTypeEnum));
    private static readonly Field PayloadRindexSignalCodeField = ProtocolPayloadVisitor.Field(nameof(RindexSignalCode), StringOptionalType.Utf8);
    private static readonly Field PayloadFrequencyField = ProtocolPayloadVisitor.Field(nameof(Frequency), DoubleType.Default);
    private static readonly Field PayloadSubFrameIdField = ProtocolPayloadVisitor.Field(nameof(SubFrameId), UInt32Type.Default);
    private static readonly Field PayloadNAVBitsU32Field = ProtocolPayloadVisitor.Field(nameof(NAVBitsU32), new ListType(UInt32Type.Default, 0, int.MaxValue));
    private static readonly Field PayloadGpsSubFrameField = ProtocolPayloadVisitor.Field(nameof(GpsSubFrame), ProtocolPayloadVisitor.PolymorphicType);

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadUtcTimeField, PayloadPrnField, PayloadSatelliteIdField, PayloadCrcPassedField, PayloadL1CodeField, PayloadSvIdField, PayloadRinexSatCodeField, PayloadSignalTypeField, PayloadRindexSignalCodeField, PayloadFrequencyField, PayloadSubFrameIdField, PayloadNAVBitsU32Field, PayloadGpsSubFrameField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        DateTime valueUtcTime = UtcTime;
        DateTimeType.Accept(visitor, PayloadUtcTimeField, PayloadUtcTimeField.DataType, ref valueUtcTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueUtcTime, UtcTime))
        {
            UtcTime = valueUtcTime;
        }

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

        bool valueCrcPassed = CrcPassed;
        BoolType.Accept(visitor, PayloadCrcPassedField, PayloadCrcPassedField.DataType, ref valueCrcPassed);
        if (!EqualityComparer<bool>.Default.Equals(valueCrcPassed, CrcPassed))
        {
            CrcPassed = valueCrcPassed;
        }

        byte valueL1Code = L1Code;
        UInt8Type.Accept(visitor, PayloadL1CodeField, PayloadL1CodeField.DataType, ref valueL1Code);
        if (!EqualityComparer<byte>.Default.Equals(valueL1Code, L1Code))
        {
            L1Code = valueL1Code;
        }

        ushort valueSvId = SvId;
        UInt16Type.Accept(visitor, PayloadSvIdField, PayloadSvIdField.DataType, ref valueSvId);
        if (!EqualityComparer<ushort>.Default.Equals(valueSvId, SvId))
        {
            SvId = valueSvId;
        }

        string? valueRinexSatCode = RinexSatCode;
        StringOptionalType.Accept(visitor, PayloadRinexSatCodeField, PayloadRinexSatCodeField.DataType, ref valueRinexSatCode);
        if (!EqualityComparer<string>.Default.Equals(valueRinexSatCode, RinexSatCode))
        {
            RinexSatCode = valueRinexSatCode!;
        }

        GnssSignalTypeEnum valueSignalType = SignalType;
        var numericSignalType = (int)valueSignalType;
        Int32Type.Accept(visitor, PayloadSignalTypeField, PayloadSignalTypeField.DataType, ref numericSignalType);
        valueSignalType = (GnssSignalTypeEnum)numericSignalType;
        if (!EqualityComparer<GnssSignalTypeEnum>.Default.Equals(valueSignalType, SignalType))
        {
            SignalType = valueSignalType;
        }

        string? valueRindexSignalCode = RindexSignalCode;
        StringOptionalType.Accept(visitor, PayloadRindexSignalCodeField, PayloadRindexSignalCodeField.DataType, ref valueRindexSignalCode);
        if (!EqualityComparer<string>.Default.Equals(valueRindexSignalCode, RindexSignalCode))
        {
            RindexSignalCode = valueRindexSignalCode!;
        }

        double valueFrequency = Frequency;
        DoubleType.Accept(visitor, PayloadFrequencyField, PayloadFrequencyField.DataType, ref valueFrequency);
        if (!EqualityComparer<double>.Default.Equals(valueFrequency, Frequency))
        {
            Frequency = valueFrequency;
        }

        uint valueSubFrameId = SubFrameId;
        UInt32Type.Accept(visitor, PayloadSubFrameIdField, PayloadSubFrameIdField.DataType, ref valueSubFrameId);
        if (!EqualityComparer<uint>.Default.Equals(valueSubFrameId, SubFrameId))
        {
            SubFrameId = valueSubFrameId;
        }

        uint[] valueNAVBitsU32 = NAVBitsU32;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadNAVBitsU32Field, PayloadNAVBitsU32Field.DataType, ref valueNAVBitsU32,
            static () => default(uint)!,
            static (IVisitor v, Field f, IFieldType t, ref uint item) =>
            {
                UInt32Type.Accept(v, f, t, ref item);
            });
        if (!EqualityComparer<uint[]>.Default.Equals(valueNAVBitsU32, NAVBitsU32))
        {
            NAVBitsU32 = valueNAVBitsU32;
        }

        GpsSubframeBase? valueGpsSubFrame = GpsSubFrame;
        if (valueGpsSubFrame is not null)
        {
            StructType.Accept(visitor, PayloadGpsSubFrameField, PayloadGpsSubFrameField.DataType, valueGpsSubFrame);
        }
        else
        {
            visitor.VisitUnknown(PayloadGpsSubFrameField, PayloadGpsSubFrameField.DataType);
        }
        if (!EqualityComparer<GpsSubframeBase?>.Default.Equals(valueGpsSubFrame, GpsSubFrame))
        {
            GpsSubFrame = valueGpsSubFrame;
        }
    }
}
