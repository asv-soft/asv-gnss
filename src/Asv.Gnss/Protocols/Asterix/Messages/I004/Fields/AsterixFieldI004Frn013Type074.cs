using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn013Type074 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 13;
    public override string Name => "Longitudinal Deviation";
    public override byte FieldReferenceNumber => StaticFrn;
    public double LongitudinalDeviationM { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        LongitudinalDeviationM = BinaryPrimitives.ReadInt16BigEndian(buffer) * 32.0;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(LongitudinalDeviationM / 32.0));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}