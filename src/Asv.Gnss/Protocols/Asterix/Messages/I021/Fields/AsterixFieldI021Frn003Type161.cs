using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn003Type161 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 3;
    public override string Name => "Track Number";
    public override byte FieldReferenceNumber => StaticFrn;
    public ushort TrackNumber { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TrackNumber = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer) & 0x0FFF);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(TrackNumber & 0x0FFF));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}