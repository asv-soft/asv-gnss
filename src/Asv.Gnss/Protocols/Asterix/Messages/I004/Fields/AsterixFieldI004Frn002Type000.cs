using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn002Type000 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 2;
    public override string Name => "Message Type";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte MessageType { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        MessageType = buffer[0];
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = MessageType;
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}