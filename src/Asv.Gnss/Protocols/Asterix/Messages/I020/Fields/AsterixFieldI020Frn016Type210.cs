using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Data Item I020/210, Calculated Acceleration
/// Definition: Calculated Acceleration of the target, in two's complement form.
/// Format: Two-Octet fixed length data item.
/// Structure:
/// Octet no. 1
/// 16 15 14 13 12 11 10 9
/// Ax LSB
/// Octet no. 2
/// 8 7 6 5 4 3 2 1
/// Ay LSB
/// bits 9 & 1 (LSB) = 0.25 m/s²
/// Max. range ± 31 m/s²
/// Encoding Rule: This item is optional
/// NOTE: Maximum value means "maximum value or above"
/// </summary>
public class AsterixFieldI020Frn016Type210 : AsterixField
{
    
    public const byte StaticFrn = 16;
    public const string StaticName = "Calculated Acceleration";
    public override string Name => StaticName;

    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Ax = (sbyte)buffer[0] * 0.25; // Ax in m/s^2
        Ay = (sbyte)buffer[1] * 0.25; // Ay
        buffer = buffer[GetByteSize()..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((sbyte)(Ax / 0.25)); // Convert Ax back to signed byte
        buffer[1] = (byte)((sbyte)(Ay / 0.25)); // Convert Ay back to signed byte
        buffer = buffer[GetByteSize()..];
    }

    public override int GetByteSize() => 2;

    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        AxField,
        AyField
    ]);
    
    public override void Accept(IVisitor visitor)
    {
        DoubleType.Accept(visitor, AxField, AxField.DataType, ref _ax);
        DoubleType.Accept(visitor, AyField, AyField.DataType, ref _ay);
    }
    
    private static readonly Field AxField = new Field.Builder()
        .Name(nameof(Ax))
        .Title("Ax")
        .Description("Calculated acceleration along x-axis in m/s²")
        .DataType(DoubleType.Default)
        .Build();
    private double _ax;
    public double Ax
    {
        get => _ax;
        set => _ax = value;
    }
    
    private static readonly Field AyField = new Field.Builder()
        .Name(nameof(Ay))
        .Title("Ay")
        .Description("Calculated acceleration along y-axis in m/s²")
        .DataType(DoubleType.Default)
        .Build();
    private double _ay;
    public double Ay
    {
        get => _ay;
        set => _ay = value;
    }
}