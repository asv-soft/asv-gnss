using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn046Type400 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 41;
    public override string Name => "Receiver ID";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte ReceiverId { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        ReceiverId = buffer[0];
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = ReceiverId;
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}