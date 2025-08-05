using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn014Type110 : AsterixField
{
    private const string StaticName = "Measured Height (Cartesian Coordinates)";
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        LocalCartesianAltitudeFtField
    ]);
    public const byte StaticFrn = 14;
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        LocalCartesianAltitudeFt = BinaryPrimitives.ReadInt16BigEndian(buffer) * 6.25;
        buffer = buffer[GetByteSize()..]; 
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var rawValue = (short)(_localCartesianAltitudeFt / 6.25);
        BinaryPrimitives.WriteInt16BigEndian(buffer, rawValue);
        buffer = buffer[GetByteSize()..];   
    }
    public override int GetByteSize() => 2;
    public override void Accept(IVisitor visitor)
    {
        var altitudeFt = LocalCartesianAltitudeFt;
        DoubleType.Accept(visitor, LocalCartesianAltitudeFtField, ref altitudeFt);
        LocalCartesianAltitudeFt = altitudeFt;

    }
    private static readonly Field LocalCartesianAltitudeFtField = new Field.Builder()
        .Name(nameof(LocalCartesianAltitudeFt))
        .Title("Local Cartesian Altitude (Feet)")
        .Description("Measured height in Cartesian coordinates expressed in feet")
        .DataType(DoubleType.Default)
        .Build();

    
    private double _localCartesianAltitudeFt;
    public double LocalCartesianAltitudeFt
    {
        get => _localCartesianAltitudeFt;
        set => _localCartesianAltitudeFt = value;
    }

    public double LocalCartesianAltitudeM
    {
        get =>
            // Convert feet to meters (1 foot = 0.3048 meters)
            _localCartesianAltitudeFt * 0.3048;
        set =>
            // Convert meters to feet (1 meter = 3.28084 feet)
            _localCartesianAltitudeFt = value / 0.3048;
    }
}