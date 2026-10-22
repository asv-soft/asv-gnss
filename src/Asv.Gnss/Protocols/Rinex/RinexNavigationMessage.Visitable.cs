using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RinexNavigationMessage
{
    private static readonly Field NrSatelliteField = NmeaRinexVisitor.Field(nameof(Satellite), StringOptionalType.Utf8);
    private static readonly Field NrEpochField = NmeaRinexVisitor.Field(nameof(Epoch), RinexEpoch.NrVisitorType);
    private static readonly Field NrRecordTypeField = NmeaRinexVisitor.Field(nameof(RecordType), StringOptionalType.Utf8);
    private static readonly Field NrNavigationMessageTypeField = NmeaRinexVisitor.Field(nameof(NavigationMessageType), StringOptionalType.Utf8);
    private static readonly Field NrNavigationMessageSubtypeField = NmeaRinexVisitor.Field(nameof(NavigationMessageSubtype), StringOptionalType.Utf8);
    private static readonly Field NrTimeSystemCorrectionCodeField = NmeaRinexVisitor.Field(nameof(TimeSystemCorrectionCode), StringOptionalType.Utf8);
    private static readonly Field NrSbasIdentifierField = NmeaRinexVisitor.Field(nameof(SbasIdentifier), StringOptionalType.Utf8);
    private static readonly Field NrUtcIdentifierField = NmeaRinexVisitor.Field(nameof(UtcIdentifier), StringOptionalType.Utf8);
    private static readonly Field NrValuesField = NmeaRinexVisitor.Field(nameof(Values), new ListType(DoubleOptionalType.Default, 0, 32768), readOnly: true);
    private static readonly Field NrValueFieldCountsField = NmeaRinexVisitor.Field(nameof(ValueFieldCounts), new ListType(Int32Type.Default, 0, 32768), readOnly: true);

    internal new static readonly StructType NrVisitorType = new([..RinexMessageBase.NrVisitorType.Fields, NrSatelliteField, NrEpochField, NrRecordTypeField, NrNavigationMessageTypeField, NrNavigationMessageSubtypeField, NrTimeSystemCorrectionCodeField, NrSbasIdentifierField, NrUtcIdentifierField, NrValuesField, NrValueFieldCountsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        string? valueSatellite = Satellite;
        StringOptionalType.Accept(visitor, NrSatelliteField, NrSatelliteField.DataType, ref valueSatellite);
        if (!EqualityComparer<string>.Default.Equals(valueSatellite, Satellite))
        {
            Satellite = valueSatellite!;
        }

        var valueEpoch = Epoch;
        StructType.Accept(visitor, NrEpochField, NrEpochField.DataType, valueEpoch);

        string? valueRecordType = RecordType;
        StringOptionalType.Accept(visitor, NrRecordTypeField, NrRecordTypeField.DataType, ref valueRecordType);
        if (!EqualityComparer<string>.Default.Equals(valueRecordType, RecordType))
        {
            RecordType = valueRecordType!;
        }

        string? valueNavigationMessageType = NavigationMessageType;
        StringOptionalType.Accept(visitor, NrNavigationMessageTypeField, NrNavigationMessageTypeField.DataType, ref valueNavigationMessageType);
        if (!EqualityComparer<string>.Default.Equals(valueNavigationMessageType, NavigationMessageType))
        {
            NavigationMessageType = valueNavigationMessageType!;
        }

        string? valueNavigationMessageSubtype = NavigationMessageSubtype;
        StringOptionalType.Accept(visitor, NrNavigationMessageSubtypeField, NrNavigationMessageSubtypeField.DataType, ref valueNavigationMessageSubtype);
        if (!EqualityComparer<string>.Default.Equals(valueNavigationMessageSubtype, NavigationMessageSubtype))
        {
            NavigationMessageSubtype = valueNavigationMessageSubtype!;
        }

        string? valueTimeSystemCorrectionCode = TimeSystemCorrectionCode;
        StringOptionalType.Accept(visitor, NrTimeSystemCorrectionCodeField, NrTimeSystemCorrectionCodeField.DataType, ref valueTimeSystemCorrectionCode);
        if (!EqualityComparer<string>.Default.Equals(valueTimeSystemCorrectionCode, TimeSystemCorrectionCode))
        {
            TimeSystemCorrectionCode = valueTimeSystemCorrectionCode!;
        }

        string? valueSbasIdentifier = SbasIdentifier;
        StringOptionalType.Accept(visitor, NrSbasIdentifierField, NrSbasIdentifierField.DataType, ref valueSbasIdentifier);
        if (!EqualityComparer<string>.Default.Equals(valueSbasIdentifier, SbasIdentifier))
        {
            SbasIdentifier = valueSbasIdentifier!;
        }

        string? valueUtcIdentifier = UtcIdentifier;
        StringOptionalType.Accept(visitor, NrUtcIdentifierField, NrUtcIdentifierField.DataType, ref valueUtcIdentifier);
        if (!EqualityComparer<string>.Default.Equals(valueUtcIdentifier, UtcIdentifier))
        {
            UtcIdentifier = valueUtcIdentifier!;
        }

        var valueValues = Values;
        NmeaRinexVisitor.VisitList(visitor, NrValuesField, NrValuesField.DataType, valueValues,
            static (IVisitor visitor, Field field, IFieldType type, ref double? item) =>
            {
                DoubleOptionalType.Accept(visitor, field, type, ref item);
            });

        var valueValueFieldCounts = ValueFieldCounts;
        NmeaRinexVisitor.VisitList(visitor, NrValueFieldCountsField, NrValueFieldCountsField.DataType, valueValueFieldCounts,
            static (IVisitor visitor, Field field, IFieldType type, ref int item) =>
            {
                Int32Type.Accept(visitor, field, type, ref item);
            });
    }
}
