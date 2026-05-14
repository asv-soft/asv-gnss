using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn012Type076 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 12;
    public override string Name => "Vertical Deviation";
    public override byte FieldReferenceNumber => StaticFrn;
    public double VerticalDeviationFt { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        VerticalDeviationFt = BinaryPrimitives.ReadInt16BigEndian(buffer) * 25.0;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(VerticalDeviationFt / 25.0));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}