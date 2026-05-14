using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn022Type230 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 20;
    public override string Name => "Roll Angle";
    public override byte FieldReferenceNumber => StaticFrn;
    public double RollAngleDeg { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        RollAngleDeg = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.01;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(RollAngleDeg / 0.01));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}