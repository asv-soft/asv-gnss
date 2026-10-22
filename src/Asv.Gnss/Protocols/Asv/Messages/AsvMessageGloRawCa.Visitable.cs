using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessageGloRawCa
{
    private static readonly Field PayloadEpochTimeField = ProtocolPayloadVisitor.Field(nameof(EpochTime), DateTimeType.Default);
    private static readonly Field PayloadPrnField = ProtocolPayloadVisitor.Field(nameof(Prn), Int32Type.Default);
    private static readonly Field PayloadSatelliteIdField = ProtocolPayloadVisitor.Field(nameof(SatelliteId), Int32Type.Default);
    private static readonly Field PayloadCrcPassedField = ProtocolPayloadVisitor.Field(nameof(CrcPassed), BoolType.Default);
    private static readonly Field PayloadL1CodeField = ProtocolPayloadVisitor.Field(nameof(L1Code), UInt8Type.Default);
    private static readonly Field PayloadFrequencyField = ProtocolPayloadVisitor.Field(nameof(Frequency), Int64Type.Default);
    private static readonly Field PayloadNAVBitsU32Field = ProtocolPayloadVisitor.Field(nameof(NAVBitsU32), new ListType(new ListType(UInt32Type.Default, 0, int.MaxValue), 0, int.MaxValue));
    private static readonly Field PayloadRindexSignalCodeField = ProtocolPayloadVisitor.Field(nameof(RindexSignalCode), StringOptionalType.Utf8);
    private static readonly Field PayloadRinexSatCodeField = ProtocolPayloadVisitor.Field(nameof(RinexSatCode), StringOptionalType.Utf8);
    private static readonly Field PayloadSignalTypeField = ProtocolPayloadVisitor.Field(nameof(SignalType), Int32Type.Default, enumType: typeof(GnssSignalTypeEnum));
    private static readonly Field PayloadGloWordsField = ProtocolPayloadVisitor.Field(nameof(GloWords), new ListType(ProtocolPayloadVisitor.PolymorphicType, 0, int.MaxValue));

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadEpochTimeField, PayloadPrnField, PayloadSatelliteIdField, PayloadCrcPassedField, PayloadL1CodeField, PayloadFrequencyField, PayloadNAVBitsU32Field, PayloadRindexSignalCodeField, PayloadRinexSatCodeField, PayloadSignalTypeField, PayloadGloWordsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        DateTime valueEpochTime = EpochTime;
        DateTimeType.Accept(visitor, PayloadEpochTimeField, PayloadEpochTimeField.DataType, ref valueEpochTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueEpochTime, EpochTime))
        {
            EpochTime = valueEpochTime;
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

        long valueFrequency = Frequency;
        Int64Type.Accept(visitor, PayloadFrequencyField, PayloadFrequencyField.DataType, ref valueFrequency);
        if (!EqualityComparer<long>.Default.Equals(valueFrequency, Frequency))
        {
            Frequency = valueFrequency;
        }

        uint[][] valueNAVBitsU32 = NAVBitsU32;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadNAVBitsU32Field, PayloadNAVBitsU32Field.DataType, ref valueNAVBitsU32,
            static () => Array.Empty<uint>(),
            static (IVisitor v, Field f, IFieldType t, ref uint[] item) =>
            {
                ProtocolPayloadVisitor.VisitArray(v, f, t, ref item,
                    static () => default(uint)!,
                    static (IVisitor v, Field f, IFieldType t, ref uint item) =>
                    {
                        UInt32Type.Accept(v, f, t, ref item);
                    });
            });
        if (!EqualityComparer<uint[][]>.Default.Equals(valueNAVBitsU32, NAVBitsU32))
        {
            NAVBitsU32 = valueNAVBitsU32;
        }

        string? valueRindexSignalCode = RindexSignalCode;
        StringOptionalType.Accept(visitor, PayloadRindexSignalCodeField, PayloadRindexSignalCodeField.DataType, ref valueRindexSignalCode);
        if (!EqualityComparer<string>.Default.Equals(valueRindexSignalCode, RindexSignalCode))
        {
            RindexSignalCode = valueRindexSignalCode!;
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

        GlonassWordBase[] valueGloWords = GloWords;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadGloWordsField, PayloadGloWordsField.DataType, ref valueGloWords,
            static () => default(GlonassWordBase)!,
            static (IVisitor v, Field f, IFieldType t, ref GlonassWordBase item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!EqualityComparer<GlonassWordBase[]>.Default.Equals(valueGloWords, GloWords))
        {
            GloWords = valueGloWords;
        }
    }
}
