using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI002Frn005Type041 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 5;
    public override string Name => "Antenna Rotation Speed";
    public override byte FieldReferenceNumber => StaticFrn;
    public double AntennaRotationPeriod { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AntennaRotationPeriod = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 128.0;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(AntennaRotationPeriod * 128.0));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}