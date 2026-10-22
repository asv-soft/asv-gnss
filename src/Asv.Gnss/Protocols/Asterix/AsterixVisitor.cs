using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

internal static class AsterixVisitor
{
    internal delegate void ElementVisitor<T>(IVisitor visitor, Field field, IFieldType type, ref T value);

    internal static void VisitList<T>(IVisitor visitor, Field field, ref List<T> source,
        Func<T> create, ElementVisitor<T> visit)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }
        var type = (ListType)field.DataType;
        var size = (uint)source.Count;
        listVisitor.BeginList(field, type, ref size);
        if (size > type.MaxSize || size < type.MinSize) throw new ArgumentOutOfRangeException(nameof(size));
        while (source.Count > size) source.RemoveAt(source.Count - 1);
        while (source.Count < size) source.Add(create());
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            visit(visitor, field, type.ElementType, ref item);
            if (!EqualityComparer<T>.Default.Equals(item, source[i])) source[i] = item;
        }
        listVisitor.EndList();
    }

    internal static void VisitArray<T>(IVisitor visitor, Field field, ref T[]? source,
        Func<T> create, ElementVisitor<T> visit, bool canResize = true)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }
        var type = (ListType)field.DataType;
        var previous = source?.Length ?? 0;
        var size = (uint)previous;
        listVisitor.BeginList(field, type, ref size);
        if (size != previous)
        {
            if (!canResize) throw new ArgumentOutOfRangeException(nameof(size), "Read-only array storage cannot be resized.");
            if (size > type.MaxSize || size < type.MinSize) throw new ArgumentOutOfRangeException(nameof(size));
            Array.Resize(ref source, checked((int)size));
            for (var i = previous; i < size; i++) source[i] = create();
        }
        for (var i = 0; i < size; i++) visit(visitor, field, type.ElementType, ref source![i]);
        listVisitor.EndList();
    }
}
