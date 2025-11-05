using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn005Type042 : AsterixField
{
    
    private const double Multiply = 0.5;
    public const byte StaticFrn = 5;
    
    
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        XField,
        YField
    ]);
    
    public const string StaticName = "Position in Cartesian Coordinates";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private static readonly Field XField = new Field.Builder()
        .Name(nameof(X))
        .Title("Cartesian X Coordinate")
        .DataType(DoubleType.Default)
        .Build();
    private double _x;
    

    public double X
    {
        get => _x;
        set => _x = value;
    }

    private static readonly Field YField = new Field.Builder()
        .Name(nameof(Y))
        .Title("Cartesian Y Coordinate")
        .DataType(DoubleType.Default)
        .Build();
    
    private double _y;
    public double Y
    {
        get => _y;
        set => _y = value;
    }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var pos = 0;
        X = SpanBitHelper.GetBitS(buffer, ref pos, 24) * Multiply;
        Y = SpanBitHelper.GetBitS(buffer, ref pos, 24) * Multiply;
        buffer = buffer[6..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var pos = 0;
        SpanBitHelper.SetBitS(buffer, ref pos, 24, (int)(X / Multiply));
        SpanBitHelper.SetBitS(buffer, ref pos, 24, (int)(Y / Multiply));
        buffer = buffer[6..];
    }

    public override int GetByteSize() => 6;

    public override void Accept(IVisitor visitor)
    {
        DoubleType.Accept(visitor, XField, XField.DataType, ref _x);
        DoubleType.Accept(visitor, YField, YField.DataType, ref _y);
        
    }
}