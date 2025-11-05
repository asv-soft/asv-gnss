using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn004Type041 : AsterixField
{
    private const double Multiply = 5.36441802978515625e-6;
    public const byte StaticFrn = 4;
    
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        LatitudeField,
        LongitudeField,
    ]);
    
    public const string StaticName = "Position in WGS-84 Coordinates";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    
    private static readonly Field LatitudeField = new Field.Builder()
        .Name(nameof(Latitude))
        .Title(nameof(Latitude))
        .DataType(DoubleType.Default)
        .Build();
    private double _latitude;
    public double Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    private static readonly Field LongitudeField = new Field.Builder()
        .Name(nameof(Longitude))
        .Title(nameof(Longitude))
        .DataType(DoubleType.Default)
        .Build();
    private double _longitude;
    public double Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Latitude = BinaryPrimitives.ReadInt32BigEndian(buffer) * Multiply;
        buffer = buffer[4..];
        Longitude = BinaryPrimitives.ReadInt32BigEndian(buffer) * Multiply;
        buffer = buffer[4..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)(Latitude / Multiply));
        buffer = buffer[4..];
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)(Longitude / Multiply));
        buffer = buffer[4..];
    }

    public override int GetByteSize() => 8;

    public override void Accept(IVisitor visitor)
    {
        DoubleType.Accept(visitor, LatitudeField, LatitudeField.DataType, ref _latitude);
        DoubleType.Accept(visitor, LongitudeField, LongitudeField.DataType, ref _longitude);
    }
}