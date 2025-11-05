using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn015Type105 : AsterixField
{
    
    public const byte StaticFrn = 15;
    public const string StaticName = "Geometric Height (WGS-84)";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        EllipsoidAltitudeFt = BinaryPrimitives.ReadInt16BigEndian(buffer) * 6.25;
        buffer = buffer[GetByteSize()..]; 
    }

    

    public override void Serialize(ref Span<byte> buffer)
    {
        // Convert altitude from feet to the wire format
        // Geometric height is typically stored as a signed 16-bit value in 6.25 ft resolution
        var altitudeValue = (short)Math.Round(_ellipsoidAltitudeFt / 6.25);
        
        BinaryPrimitives.WriteInt16BigEndian(buffer, altitudeValue);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;

    public override void Accept(IVisitor visitor)
    {
        DoubleType.Accept(visitor, EllipsoidAltitudeFtField, ref _ellipsoidAltitudeFt);
    }
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        EllipsoidAltitudeFtField
    ]);
    
    private static readonly Field EllipsoidAltitudeFtField = new Field.Builder()
        .Name(nameof(EllipsoidAltitudeFt))
        .Title("Ellipsoid Altitude (Feet)")
        .Description("Geometric height (WGS-84) in feet")
        .DataType(DoubleType.Default)
        .Build();

    private double _ellipsoidAltitudeFt;
    public double EllipsoidAltitudeFt
    {
        get => _ellipsoidAltitudeFt;
        set => _ellipsoidAltitudeFt = value;
    }

    public double EllipsoidAltitudeM
    {
        get => _ellipsoidAltitudeFt * 0.3048;
        set => _ellipsoidAltitudeFt = value / 0.3048;
    }
}