using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn005Type040 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 5;
    public override string Name => "Alert Identifier";
    public override byte FieldReferenceNumber => StaticFrn;
    public ushort AlertIdentifier { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AlertIdentifier = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, AlertIdentifier);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}