using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

internal static class ProtocolPayloadVisitor
{
    // The actual fields of polymorphic navigation records are supplied by their Accept override.
    internal static readonly StructType PolymorphicType = new([]);

    internal static Field Field(string name, IFieldType type, bool readOnly = false, Type? enumType = null)
    {
        var builder = new Field.Builder().Name(name).Title(name).Description(name).DataType(type);
        if (readOnly) builder.Metadata("readOnly", true);
        if (enumType is not null) builder.Metadata("enumType", enumType.FullName);
        return builder.Build();
    }

    internal delegate void ElementVisitor<T>(IVisitor visitor, Field field, IFieldType type, ref T value);

    internal static void VisitArray<T>(IVisitor visitor, Field field, IFieldType type, ref T[] source,
        Func<T> create, ElementVisitor<T> visit)
    {
        if (type is ArrayType array)
        {
            if (visitor is not ArrayType.IVisitor arrayVisitor)
            {
                visitor.VisitUnknown(field, type);
                return;
            }
            arrayVisitor.BeginArray(field, array);
            for (var i = 0; i < array.Size; i++)
            {
                if (source is not null && i < source.Length) visit(visitor, field, array.ElementType, ref source[i]);
                else visitor.VisitUnknown(field, array.ElementType);
            }
            arrayVisitor.EndArray();
            return;
        }
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, type);
            return;
        }
        var list = (ListType)type;
        var previousSize = source?.Length ?? 0;
        var size = (uint)previousSize;
        listVisitor.BeginList(field, list, ref size);
        ValidateSize(list, size);
        if (size != previousSize)
        {
            Array.Resize(ref source, checked((int)size));
            for (var i = previousSize; i < source.Length; i++) source[i] = create();
        }
        for (var i = 0; i < size; i++) visit(visitor, field, list.ElementType, ref source![i]);
        listVisitor.EndList();
    }

    internal static void VisitList<T>(IVisitor visitor, Field field, IFieldType type, ref List<T> source,
        Func<T> create, ElementVisitor<T> visit)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, type);
            return;
        }
        var list = (ListType)type;
        var previousSize = source?.Count ?? 0;
        var size = (uint)previousSize;
        listVisitor.BeginList(field, list, ref size);
        ValidateSize(list, size);
        if (size != previousSize)
        {
            source ??= [];
            if (size < previousSize) source.RemoveRange((int)size, previousSize - (int)size);
            else while (source.Count < size) source.Add(create());
        }
        for (var i = 0; i < size; i++)
        {
            var item = source![i];
            visit(visitor, field, list.ElementType, ref item);
            if (!EqualityComparer<T>.Default.Equals(item, source[i])) source[i] = item;
        }
        listVisitor.EndList();
    }

    private static void ValidateSize(ListType type, uint size)
    {
        if (size < type.MinSize || size > type.MaxSize) throw new ArgumentOutOfRangeException(nameof(size));
    }
}
