using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RinexHeader : IVisitable
{
    private static readonly Field NrVersionField = NmeaRinexVisitor.Field(nameof(Version), DoubleType.Default, readOnly: true, sourceType: typeof(decimal));
    private static readonly Field NrFileTypeField = NmeaRinexVisitor.Field(nameof(FileType), CharType.Utf8);
    private static readonly Field NrSatelliteSystemField = NmeaRinexVisitor.Field(nameof(SatelliteSystem), CharType.Utf8);
    private static readonly Field NrTimeSystemField = NmeaRinexVisitor.Field(nameof(TimeSystem), StringOptionalType.Utf8);
    private static readonly Field NrObservationTypesField = NmeaRinexVisitor.Field(nameof(ObservationTypes), new ListType(NmeaRinexVisitor.ObservationEntryType, 0, 256), readOnly: true);
    private static readonly Field NrLegacyObservationTypesField = NmeaRinexVisitor.Field(nameof(LegacyObservationTypes), new ListType(StringOptionalType.Utf8, 0, 32768), readOnly: true);
    private static readonly Field NrHeaderLinesField = NmeaRinexVisitor.Field(nameof(HeaderLines), new ListType(StringOptionalType.Utf8, 0, 32768), readOnly: true);
    private static readonly Field NrMarkerNameField = NmeaRinexVisitor.Field(nameof(MarkerName), StringOptionalType.Utf8, readOnly: true);

    internal static readonly StructType NrVisitorType = new([NrVersionField, NrFileTypeField, NrSatelliteSystemField, NrTimeSystemField, NrObservationTypesField, NrLegacyObservationTypesField, NrHeaderLinesField, NrMarkerNameField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueVersion = (double)Version;
        DoubleType.Accept(visitor, NrVersionField, NrVersionField.DataType, ref valueVersion);

        var valueFileType = FileType;
        CharType.Accept(visitor, NrFileTypeField, NrFileTypeField.DataType, ref valueFileType);
        if (!EqualityComparer<char>.Default.Equals(valueFileType, FileType))
        {
            FileType = valueFileType;
        }

        var valueSatelliteSystem = SatelliteSystem;
        CharType.Accept(visitor, NrSatelliteSystemField, NrSatelliteSystemField.DataType, ref valueSatelliteSystem);
        if (!EqualityComparer<char>.Default.Equals(valueSatelliteSystem, SatelliteSystem))
        {
            SatelliteSystem = valueSatelliteSystem;
        }

        string? valueTimeSystem = TimeSystem;
        StringOptionalType.Accept(visitor, NrTimeSystemField, NrTimeSystemField.DataType, ref valueTimeSystem);
        if (!EqualityComparer<string>.Default.Equals(valueTimeSystem, TimeSystem))
        {
            TimeSystem = valueTimeSystem!;
        }

        var valueObservationTypes = ObservationTypes;
        NmeaRinexVisitor.VisitObservationTypes(visitor, NrObservationTypesField, valueObservationTypes);

        var valueLegacyObservationTypes = LegacyObservationTypes;
        NmeaRinexVisitor.VisitList(visitor, NrLegacyObservationTypesField, NrLegacyObservationTypesField.DataType, valueLegacyObservationTypes,
            static (IVisitor visitor, Field field, IFieldType type, ref string item) =>
            {
                string? value = item;
                        StringOptionalType.Accept(visitor, field, type, ref value);
                        item = value!;
            });

        var valueHeaderLines = HeaderLines;
        NmeaRinexVisitor.VisitList(visitor, NrHeaderLinesField, NrHeaderLinesField.DataType, valueHeaderLines,
            static (IVisitor visitor, Field field, IFieldType type, ref string item) =>
            {
                string? value = item;
                        StringOptionalType.Accept(visitor, field, type, ref value);
                        item = value!;
            });

        string? valueMarkerName = MarkerName;
        StringOptionalType.Accept(visitor, NrMarkerNameField, NrMarkerNameField.DataType, ref valueMarkerName);
    }
}
