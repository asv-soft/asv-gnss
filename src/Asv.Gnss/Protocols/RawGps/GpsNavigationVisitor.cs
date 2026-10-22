using System;
using Asv.IO;

namespace Asv.Gnss;

internal static class GpsNavigationVisitor
{
    internal static Field Field(string name, IFieldType type) =>
        new Field.Builder().Name(name).Title(name).DataType(type).Build();

    internal static void Array(IVisitor visitor, Field field, int[] values)
    {
        var type = (ArrayType)field.DataType;
        if (visitor is not ArrayType.IVisitor arrayVisitor)
        {
            visitor.VisitUnknown(field, type);
            return;
        }
        arrayVisitor.BeginArray(field, type);
        for (var index = 0; index < type.Size; index++)
            if (values is not null && index < values.Length)
                Int32Type.Accept(visitor, field, type.ElementType, ref values[index]);
            else
                visitor.VisitUnknown(field, type.ElementType);
        arrayVisitor.EndArray();
    }

    internal static void RequireLength<T>(T[] values, int length, string name)
    {
        if (values is null || values.Length != length)
            throw new ArgumentException($"{name} must contain {length} elements.", name);
    }

    internal static void Array(IVisitor visitor, Field field, byte[] values)
    {
        var type = (ArrayType)field.DataType;
        if (visitor is not ArrayType.IVisitor arrayVisitor)
        {
            visitor.VisitUnknown(field, type);
            return;
        }
        arrayVisitor.BeginArray(field, type);
        for (var index = 0; index < type.Size; index++)
            if (values is not null && index < values.Length)
                UInt8Type.Accept(visitor, field, type.ElementType, ref values[index]);
            else
                visitor.VisitUnknown(field, type.ElementType);
        arrayVisitor.EndArray();
    }
}
