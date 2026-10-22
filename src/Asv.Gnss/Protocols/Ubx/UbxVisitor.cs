using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Asv.Common;
using Asv.IO;
using Geodesy;

namespace Asv.Gnss;

internal static class UbxVisitor
{
    private static readonly Field LatitudeField = Field("Latitude", DoubleType.Default, "WGS84 geodetic latitude in degrees.", "deg");
    private static readonly Field LongitudeField = Field("Longitude", DoubleType.Default, "WGS84 geodetic longitude in degrees.", "deg");
    private static readonly Field AltitudeField = Field("Altitude", DoubleType.Default, "Height above the WGS84 ellipsoid in meters.", "m");
    private static readonly Field XField = Field("X", DoubleType.Default, "Earth-centered, Earth-fixed X coordinate in meters.", "m");
    private static readonly Field YField = Field("Y", DoubleType.Default, "Earth-centered, Earth-fixed Y coordinate in meters.", "m");
    private static readonly Field ZField = Field("Z", DoubleType.Default, "Earth-centered, Earth-fixed Z coordinate in meters.", "m");

    public static StructType GeoPointType { get; } = new([LatitudeField, LongitudeField, AltitudeField]);
    public static OptionalStructType LocationType { get; } = new(GeoPointType);
    public static StructType GlobalPositionType => GeoPointType;
    public static StructType EcefType { get; } = new([XField, YField, ZField]);

    public static Field Field(string name, IFieldType type, string description, string? units = null)
    {
        var builder = new Field.Builder().Name(name).Title(name).Description(description).DataType(type);
        if (units != null)
        {
            builder.Units(units);
        }
        return builder.Build();
    }

    public static Field Enum<T>(string name, IFieldType type, string description, params EnumValue<T>[] values)
        where T : notnull
    {
        return new Field.Builder().Name(name).Title(name).Description(description).DataType(type)
            .Metadata("enum", values.ToImmutableDictionary(value => value.Value)).Build();
    }

    public delegate void RefElementVisitor<T>(IVisitor visitor, Field field, IFieldType type, ref T value);

    public static void VisitArray<T>(IVisitor visitor, Field field, ref T[]? source, Func<T> create,
        RefElementVisitor<T> visitElement)
    {
        if (field.DataType is ArrayType arrayType)
        {
            if (visitor is not ArrayType.IVisitor arrayVisitor)
            {
                visitor.VisitUnknown(field, arrayType);
                return;
            }

            arrayVisitor.BeginArray(field, arrayType);
            for (var index = 0; index < arrayType.Size; index++)
            {
                if (source != null && source.Length >= arrayType.Size)
                {
                    visitElement(visitor, field, arrayType.ElementType, ref source[index]);
                }
                else
                {
                    // Preserve absent or short storage on reads; materialize it only after an edit.
                    var value = source != null && index < source.Length ? source[index] : create();
                    var before = value;
                    visitElement(visitor, field, arrayType.ElementType, ref value);
                    if (!EqualityComparer<T>.Default.Equals(before, value))
                    {
                        ResizeArray(ref source, arrayType.Size, create);
                        source![index] = value;
                    }
                }
            }
            arrayVisitor.EndArray();
            return;
        }

        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }

        var listType = (ListType)field.DataType;
        var size = (uint)(source?.Length ?? 0);
        listVisitor.BeginList(field, listType, ref size);
        ValidateListSize(listType, size);
        if (size != (source?.Length ?? 0))
        {
            ResizeArray(ref source, (int)size, create);
        }
        for (var index = 0; index < size; index++)
        {
            visitElement(visitor, field, listType.ElementType, ref source![index]);
        }
        listVisitor.EndList();
    }

    public static void VisitFixedArray<T>(IVisitor visitor, Field field, T[] source,
        RefElementVisitor<T> visitElement)
    {
        if (visitor is not ArrayType.IVisitor arrayVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }

        var type = (ArrayType)field.DataType;
        arrayVisitor.BeginArray(field, type);
        for (var index = 0; index < type.Size; index++)
        {
            visitElement(visitor, field, type.ElementType, ref source[index]);
        }
        arrayVisitor.EndArray();
    }

    public static void VisitList<T>(IVisitor visitor, Field field, ref List<T>? source, Func<T> create,
        RefElementVisitor<T> visitElement)
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }

        var type = (ListType)field.DataType;
        var size = (uint)(source?.Count ?? 0);
        listVisitor.BeginList(field, type, ref size);
        ValidateListSize(type, size);
        if (source == null && size > 0)
        {
            source = new List<T>((int)size);
        }
        if (source != null)
        {
            while (source.Count < size)
            {
                source.Add(create());
            }
            if (source.Count > size)
            {
                source.RemoveRange((int)size, source.Count - (int)size);
            }
            // The list size is set by BeginList and stays fixed during element visits.
            var values = CollectionsMarshal.AsSpan(source);
            for (var index = 0; index < values.Length; index++)
            {
                visitElement(visitor, field, type.ElementType, ref values[index]);
            }
        }
        listVisitor.EndList();
    }

    private static void ResizeArray<T>(ref T[]? source, int size, Func<T> create)
    {
        var oldSize = source?.Length ?? 0;
        Array.Resize(ref source, size);
        for (var index = oldSize; index < size; index++)
        {
            source[index] = create();
        }
    }

    private static void ValidateListSize(ListType type, uint size)
    {
        if (size < type.MinSize || size > type.MaxSize)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "The requested list size is outside the field limits.");
        }
    }

    public static void VisitLocation(IVisitor visitor, Field field, ref GeoPoint? value)
    {
        if (visitor is not OptionalStructType.IVisitor optionalVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }

        optionalVisitor.BeginOptionalStruct(field, (OptionalStructType)field.DataType, value.HasValue, out var createNew);
        if (createNew)
        {
            value = new GeoPoint(0, 0, 0);
        }
        if (value.HasValue)
        {
            var latitude = value?.Latitude ?? 0;
            var longitude = value?.Longitude ?? 0;
            var altitude = value?.Altitude ?? 0;
            VisitCoordinates(visitor, ref latitude, ref longitude, ref altitude);
            if (!value.HasValue || !latitude.Equals(value.Value.Latitude) ||
                !longitude.Equals(value.Value.Longitude) || !altitude.Equals(value.Value.Altitude))
            {
                value = new GeoPoint(latitude, longitude, altitude);
            }
        }
        optionalVisitor.EndOptionalStruct(value.HasValue);
    }

    public static void VisitGlobalPosition(IVisitor visitor, Field field, ref GlobalPosition value)
    {
        if (visitor is not StructType.IVisitor structVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }

        structVisitor.BeginStruct(field, (StructType)field.DataType);
        var latitude = value.Latitude.Degrees;
        var longitude = value.Longitude.Degrees;
        var altitude = value.Elevation;
        VisitCoordinates(visitor, ref latitude, ref longitude, ref altitude);
        if (!latitude.Equals(value.Latitude.Degrees) || !longitude.Equals(value.Longitude.Degrees) ||
            !altitude.Equals(value.Elevation))
        {
            value = new GlobalPosition(new GlobalCoordinates(latitude, longitude), altitude);
        }
        structVisitor.EndStruct();
    }

    private static void VisitCoordinates(IVisitor visitor, ref double latitude, ref double longitude, ref double altitude)
    {
        DoubleType.Accept(visitor, LatitudeField, ref latitude);
        DoubleType.Accept(visitor, LongitudeField, ref longitude);
        DoubleType.Accept(visitor, AltitudeField, ref altitude);
    }

    public static void VisitEcef(IVisitor visitor, Field field, ref (double X, double Y, double Z) value)
    {
        if (visitor is not StructType.IVisitor structVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }

        structVisitor.BeginStruct(field, (StructType)field.DataType);
        DoubleType.Accept(visitor, XField, ref value.X);
        DoubleType.Accept(visitor, YField, ref value.Y);
        DoubleType.Accept(visitor, ZField, ref value.Z);
        structVisitor.EndStruct();
    }
}
