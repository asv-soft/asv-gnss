using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

internal static class SbfComNavVisitor
{
    // Both binary frame formats encode their payload length in at most 16 bits.
    public const int MaximumElements = ushort.MaxValue;

    public static Field Field(string name, IFieldType type, string description,
        bool readOnly = false, Type? enumType = null)
    {
        var builder = new Field.Builder().Name(name).Title(name).Description(string.IsNullOrWhiteSpace(description) ? name : description).DataType(type);
        if (readOnly)
            builder.Metadata("readOnly", true);
        if (enumType != null)
            builder.Metadata("enumType", enumType.FullName);
        return builder.Build();
    }

    public delegate void ElementVisitor<T>(IVisitor visitor, Field field, IFieldType type, ref T value);

    public static void VisitArray<T>(IVisitor visitor, Field field, IFieldType type, ref T[]? source,
        Func<T> create, ElementVisitor<T> visit)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, type);
            return;
        }
        var list = (ListType)type;
        var previous = source?.Length ?? 0;
        var size = (uint)previous;
        listVisitor.BeginList(field, list, ref size);
        // Unchanged storage, including null, must survive read-only traversal verbatim.
        if (size != previous)
        {
            if (size > list.MaxSize || size < list.MinSize)
                throw new ArgumentOutOfRangeException(nameof(size));
            Array.Resize(ref source, checked((int)size));
            for (var index = previous; index < source.Length; index++)
                source[index] = create();
        }
        for (var index = 0; index < size; index++)
            visit(visitor, field, list.ElementType, ref source![index]);
        listVisitor.EndList();
    }
    public static void VisitFixedArray<T>(IVisitor visitor, Field field, T[]? source, ElementVisitor<T> visit)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }
        var type = (ListType)field.DataType;
        var original = (uint)(source?.Length ?? 0);
        var size = original;
        listVisitor.BeginList(field, type, ref size);
        if (size != original)
            throw new InvalidOperationException("This collection has fixed storage.");
        for (var index = 0; index < size; index++)
            visit(visitor, field, type.ElementType, ref source![index]);
        listVisitor.EndList();
    }

    public static void VisitStrings(IVisitor visitor, Field field, IList<string> source)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }
        var type = (ListType)field.DataType;
        var size = (uint)source.Count;
        listVisitor.BeginList(field, type, ref size);
        if (size > type.MaxSize || size < type.MinSize)
            throw new ArgumentOutOfRangeException(nameof(size));
        if (size != source.Count && source.IsReadOnly)
            throw new InvalidOperationException("This collection has fixed storage.");
        while (source.Count > size) source.RemoveAt(source.Count - 1);
        while (source.Count < size) source.Add(string.Empty);
        for (var index = 0; index < size; index++)
        {
            string? value = source[index];
            StringOptionalType.Accept(visitor, field, type.ElementType, ref value);
            if (value != source[index]) source[index] = value!;
        }
        listVisitor.EndList();
    }
}
