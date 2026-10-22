using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

internal static class NmeaRinexVisitor
{
    internal delegate void ElementVisitor<T>(IVisitor visitor, Field field, IFieldType type, ref T value);

    internal static Field Field(string name, IFieldType type, bool readOnly = false, Type? sourceType = null)
    {
        var builder = new Field.Builder().Name(name).Title(name).Description(name).DataType(type);
        if (readOnly) builder.Metadata("readOnly", true);
        if (sourceType != null) builder.Metadata("sourceType", sourceType.FullName);
        return builder.Build();
    }

    internal static void VisitList<T>(IVisitor visitor, Field field, IFieldType type, List<T> values,
        ElementVisitor<T> accept)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, type);
            return;
        }
        var listType = (ListType)type;
        var size = (uint)values.Count;
        listVisitor.BeginList(field, listType, ref size);
        if (size > listType.MaxSize || size < listType.MinSize)
            throw new ArgumentOutOfRangeException(nameof(size));
        // These lists can contain immutable records without parameterless constructors.
        // Existing entries are editable; growing requires a caller-provided valid record.
        if (size > values.Count) throw new InvalidOperationException("The visitor cannot create new RINEX or NMEA records.");
        if (size < values.Count) values.RemoveRange((int)size, values.Count - (int)size);
        for (var i = 0; i < values.Count; i++)
        {
            var value = values[i];
            accept(visitor, field, listType.ElementType, ref value);
            if (!EqualityComparer<T>.Default.Equals(value, values[i])) values[i] = value;
        }
        listVisitor.EndList();
    }

    private static readonly Field SystemField = Field("System", CharType.Utf8, readOnly: true);
    private static readonly Field CodesField = Field("Codes", new ListType(StringOptionalType.Utf8, 0, 32768));
    internal static readonly StructType ObservationEntryType = new([SystemField, CodesField]);

    internal static void VisitObservationTypes(IVisitor visitor, Field field, Dictionary<char, List<string>> values)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }
        var size = (uint)values.Count;
        listVisitor.BeginList(field, (ListType)field.DataType, ref size);
        if (size != values.Count) throw new InvalidOperationException("Observation systems cannot be added or removed by a visitor.");
        foreach (var pair in values)
        {
            if (visitor is StructType.IVisitor structVisitor)
            {
                structVisitor.BeginStruct(field, ObservationEntryType);
                var system = pair.Key;
                CharType.Accept(visitor, SystemField, SystemField.DataType, ref system);
                VisitList(visitor, CodesField, CodesField.DataType, pair.Value,
                    static (IVisitor v, Field f, IFieldType t, ref string code) =>
                    {
                        string? value = code;
                        StringOptionalType.Accept(v, f, t, ref value);
                        code = value!;
                    });
                structVisitor.EndStruct();
            }
            else visitor.VisitUnknown(field, ObservationEntryType);
        }
        listVisitor.EndList();
    }
}
