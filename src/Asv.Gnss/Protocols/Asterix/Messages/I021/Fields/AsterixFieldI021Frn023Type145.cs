using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn023Type145 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 21;
    public override string Name => "Flight Level";
    public override byte FieldReferenceNumber => StaticFrn;
    public double FlightLevel { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        FlightLevel = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(FlightLevel / 0.25));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}