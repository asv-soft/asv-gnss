using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavBinaryRawGloEphemPacket
{
    private static readonly Field PayloadSvIdField = SbfComNavVisitor.Field(nameof(SvId), UInt16Type.Default, "SvId");
    private static readonly Field PayloadSatPrnField = SbfComNavVisitor.Field(nameof(SatPrn), Int32Type.Default, "SatPrn");
    private static readonly Field PayloadRinexSatCodeField = SbfComNavVisitor.Field(nameof(RinexSatCode), StringOptionalType.Utf8, "RinexSatCode");
    private static readonly Field PayloadSignalTypeField = SbfComNavVisitor.Field(nameof(SignalType), Int32Type.Default, "SignalType", enumType: typeof(GnssSignalTypeEnum));
    private static readonly Field PayloadRindexSignalCodeField = SbfComNavVisitor.Field(nameof(RindexSignalCode), StringOptionalType.Utf8, "RindexSignalCode");
    private static readonly Field PayloadFrequencyField = SbfComNavVisitor.Field(nameof(Frequency), DoubleType.Default, "Frequency");
    private static readonly Field PayloadGpsEphTimeField = SbfComNavVisitor.Field(nameof(GpsEphTime), DateTimeType.Default, "GpsEphTime");
    private static readonly Field PayloadRawDataField = SbfComNavVisitor.Field(nameof(RawData), new ListType(new ListType(UInt32Type.Default, 0, ushort.MaxValue), 0, ushort.MaxValue), "RawData");
    private static readonly Field PayloadGlonassWordsField = SbfComNavVisitor.Field(nameof(GlonassWords), new ListType(new StructType([]), 0, ushort.MaxValue), "GlonassWords");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavBinaryMessageBase.PayloadVisitorType.Fields, PayloadSvIdField, PayloadSatPrnField, PayloadRinexSatCodeField, PayloadSignalTypeField, PayloadRindexSignalCodeField, PayloadFrequencyField, PayloadGpsEphTimeField, PayloadRawDataField, PayloadGlonassWordsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSvId = SvId;
        UInt16Type.Accept(visitor, PayloadSvIdField, PayloadSvIdField.DataType, ref valueSvId);
        if (!EqualityComparer<ushort>.Default.Equals(valueSvId, SvId))
        {
            SvId = valueSvId;
        }

        var valueSatPrn = SatPrn;
        Int32Type.Accept(visitor, PayloadSatPrnField, PayloadSatPrnField.DataType, ref valueSatPrn);
        if (!EqualityComparer<int>.Default.Equals(valueSatPrn, SatPrn))
        {
            SatPrn = valueSatPrn;
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
        valueSignalType = (GnssSignalTypeEnum)numericvalueSignalType;
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

        var valueFrequency = Frequency;
        DoubleType.Accept(visitor, PayloadFrequencyField, PayloadFrequencyField.DataType, ref valueFrequency);
        if (!EqualityComparer<double>.Default.Equals(valueFrequency, Frequency))
        {
            Frequency = valueFrequency;
        }

        var valueGpsEphTime = GpsEphTime;
        DateTimeType.Accept(visitor, PayloadGpsEphTimeField, PayloadGpsEphTimeField.DataType, ref valueGpsEphTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueGpsEphTime, GpsEphTime))
        {
            GpsEphTime = valueGpsEphTime;
        }

        var valueRawData = RawData;
        SbfComNavVisitor.VisitArray(visitor, PayloadRawDataField, PayloadRawDataField.DataType, ref valueRawData, static () => Array.Empty<uint>(),
            static (IVisitor v, Field f, IFieldType t, ref uint[] item) =>
            {
                uint[]? itemRow = item;
                SbfComNavVisitor.VisitArray(v, f, t, ref itemRow, static () => default(uint),
                    static (IVisitor v2, Field f2, IFieldType t2, ref uint item2) =>
                    {
                        UInt32Type.Accept(v2, f2, t2, ref item2);
                    });
                item = itemRow!;
            });
        if (!ReferenceEquals(valueRawData, RawData))
        {
            RawData = valueRawData!;
        }

        var valueGlonassWords = GlonassWords;
        SbfComNavVisitor.VisitArray(visitor, PayloadGlonassWordsField, PayloadGlonassWordsField.DataType, ref valueGlonassWords, static () => null!,
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
        if (!ReferenceEquals(valueGlonassWords, GlonassWords))
        {
            GlonassWords = valueGlonassWords!;
        }
    }
}
