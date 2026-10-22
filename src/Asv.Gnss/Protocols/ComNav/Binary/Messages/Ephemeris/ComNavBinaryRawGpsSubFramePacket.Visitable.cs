using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavBinaryRawGpsSubFramePacket
{
    private static readonly Field PayloadSvIdField = SbfComNavVisitor.Field(nameof(SvId), UInt16Type.Default, "SvId");
    private static readonly Field PayloadSatPrnField = SbfComNavVisitor.Field(nameof(SatPrn), Int32Type.Default, "SatPrn");
    private static readonly Field PayloadRinexSatCodeField = SbfComNavVisitor.Field(nameof(RinexSatCode), StringOptionalType.Utf8, "RinexSatCode");
    private static readonly Field PayloadSignalTypeField = SbfComNavVisitor.Field(nameof(SignalType), Int32Type.Default, "SignalType", enumType: typeof(GnssSignalTypeEnum));
    private static readonly Field PayloadRindexSignalCodeField = SbfComNavVisitor.Field(nameof(RindexSignalCode), StringOptionalType.Utf8, "RindexSignalCode");
    private static readonly Field PayloadFrequencyField = SbfComNavVisitor.Field(nameof(Frequency), DoubleType.Default, "Frequency");
    private static readonly Field PayloadSubFrameIdField = SbfComNavVisitor.Field(nameof(SubFrameId), UInt32Type.Default, "SubFrameId");
    private static readonly Field PayloadRawDataField = SbfComNavVisitor.Field(nameof(RawData), new ListType(UInt32Type.Default, 0, ushort.MaxValue), "RawData");
    private static readonly Field PayloadGpsSubFrameField = SbfComNavVisitor.Field(nameof(GpsSubFrame), new StructType([]), "GpsSubFrame");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavBinaryMessageBase.PayloadVisitorType.Fields, PayloadSvIdField, PayloadSatPrnField, PayloadRinexSatCodeField, PayloadSignalTypeField, PayloadRindexSignalCodeField, PayloadFrequencyField, PayloadSubFrameIdField, PayloadRawDataField, PayloadGpsSubFrameField]);

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

        var valueSubFrameId = SubFrameId;
        UInt32Type.Accept(visitor, PayloadSubFrameIdField, PayloadSubFrameIdField.DataType, ref valueSubFrameId);
        if (!EqualityComparer<uint>.Default.Equals(valueSubFrameId, SubFrameId))
        {
            SubFrameId = valueSubFrameId;
        }

        var valueRawData = RawData;
        SbfComNavVisitor.VisitArray(visitor, PayloadRawDataField, PayloadRawDataField.DataType, ref valueRawData, static () => default(uint),
            static (IVisitor v, Field f, IFieldType t, ref uint item) =>
            {
                UInt32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueRawData, RawData))
        {
            RawData = valueRawData!;
        }

        var valueGpsSubFrame = GpsSubFrame;
        if (valueGpsSubFrame is not null)
        {
            StructType.Accept(visitor, PayloadGpsSubFrameField, PayloadGpsSubFrameField.DataType, valueGpsSubFrame);
        }
        else
        {
            visitor.VisitUnknown(PayloadGpsSubFrameField, PayloadGpsSubFrameField.DataType);
        }
    }
}
