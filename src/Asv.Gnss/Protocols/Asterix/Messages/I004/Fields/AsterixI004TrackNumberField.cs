using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public abstract class AsterixI004TrackNumberField : AsterixFieldI004Fixed
{
    public ushort TrackNumber { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TrackNumber = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, TrackNumber);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}