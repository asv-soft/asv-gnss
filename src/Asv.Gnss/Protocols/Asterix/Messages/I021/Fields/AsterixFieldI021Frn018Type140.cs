using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn018Type140 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 16;
    public override string Name => "Geometric Height";
    public override byte FieldReferenceNumber => StaticFrn;
    public double GeometricHeightFt { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        GeometricHeightFt = BinaryPrimitives.ReadInt16BigEndian(buffer) * 6.25;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(GeometricHeightFt / 6.25));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}