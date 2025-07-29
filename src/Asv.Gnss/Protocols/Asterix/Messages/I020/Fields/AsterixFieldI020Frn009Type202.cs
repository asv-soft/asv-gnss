using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn009Type202 : AsterixField
{
    public const byte StaticFrn = 9;
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
       VxField,
       VyField
    ]);
    
    public const string StaticName = "Calculated Track Velocity in Cartesian Coord.";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private static readonly Field VxField = new Field.Builder()
        .Name(nameof(Vx))
        .Title("Vx")
        .Description("X Component of Velocity")
        .DataType(DoubleType.Default)
        .Build();
    private double _vx;
    public double Vx
    {
        get => _vx;
        set => _vx = value;
    }

    private static readonly Field VyField = new Field.Builder()
        .Name(nameof(Vy))
        .Title("Vy")
        .Description("Y Component of Velocity")
        .DataType(DoubleType.Default)
        .Build();
    private double _vy;
    public double Vy
    {
        get => _vy;
        set => _vy = value;
    }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Vx = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
        Vy = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)(Vx / 0.25));
        buffer = buffer[2..];
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)(Vy / 0.25));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 4;

    public override void Accept(IVisitor visitor)
    {
        DoubleType.Accept(visitor, VxField, VxField.DataType, ref _vx);
        DoubleType.Accept(visitor, VyField, VyField.DataType, ref _vy);
    }
}