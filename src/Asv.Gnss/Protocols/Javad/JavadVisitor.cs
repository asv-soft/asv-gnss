using System;
using Asv.IO;

namespace Asv.Gnss;

internal static class JavadVisitor
{
    public static Field Field(
        string name,
        IFieldType type,
        string description,
        string? units = null
    )
    {
        var builder = new Field.Builder()
            .Name(name)
            .Title(name)
            .Description(description)
            .DataType(type);
        if (units is not null)
            builder.Units(units);
        return builder.Build();
    }

    public static Field ReadOnlyField(
        string name,
        IFieldType type,
        string description,
        string? units = null
    )
    {
        var builder = new Field.Builder()
            .Name(name)
            .Title(name)
            .Description(description)
            .DataType(type)
            .Metadata("readOnly", true);
        if (units is not null)
            builder.Units(units);
        return builder.Build();
    }

    public delegate void RefElementVisitor<T>(
        IVisitor visitor,
        Field field,
        IFieldType type,
        ref T value
    );

    public static void VisitArray<T>(
        IVisitor visitor,
        Field field,
        ref T[] source,
        Func<T> create,
        RefElementVisitor<T> visitElement
    )
    {
        if (field.DataType is ArrayType array)
        {
            if (visitor is not ArrayType.IVisitor fixedVisitor)
            {
                visitor.VisitUnknown(field, array);
                return;
            }
            fixedVisitor.BeginArray(field, array);
            for (var index = 0; index < array.Size; index++)
            {
                if (source is not null && index < source.Length)
                    visitElement(visitor, field, array.ElementType, ref source[index]);
                else
                    // Inspection must not repair invalid storage or create phantom nested records.
                    visitor.VisitUnknown(field, array.ElementType);
            }
            fixedVisitor.EndArray();
            return;
        }

        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }
        var list = (ListType)field.DataType;
        var oldSize = source?.Length ?? 0;
        var size = (uint)oldSize;
        listVisitor.BeginList(field, list, ref size);
        if (size < list.MinSize || size > list.MaxSize)
            throw new ArgumentOutOfRangeException(
                nameof(size),
                "The requested GREIS list size exceeds its field limits."
            );
        if (size != oldSize)
        {
            Array.Resize(ref source, checked((int)size));
            for (var index = oldSize; index < source.Length; index++)
                source[index] = create();
        }
        for (var index = 0; index < size; index++)
            visitElement(visitor, field, list.ElementType, ref source![index]);
        listVisitor.EndList();
    }
}
