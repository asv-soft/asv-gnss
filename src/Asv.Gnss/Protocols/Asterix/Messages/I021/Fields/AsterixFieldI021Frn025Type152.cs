using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn025Type152 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 22;
    public override string Name => "Magnetic Heading";
    public override byte FieldReferenceNumber => StaticFrn;
    public double MagneticHeadingDeg { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        MagneticHeadingDeg = BinaryPrimitives.ReadUInt16BigEndian(buffer) * (360.0 / 65536.0);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(MagneticHeadingDeg / (360.0 / 65536.0)));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}