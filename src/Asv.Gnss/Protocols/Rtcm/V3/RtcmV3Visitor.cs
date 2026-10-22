using System;
using Asv.IO;

namespace Asv.Gnss;

internal static class RtcmV3Visitor
{
    // Even a one-bit-per-element collection cannot exceed the RTCM3 payload bit count.
    public const int MaximumElements = 1023 * 8;

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
}
