using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn034Type020 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 30;
    public override string Name => "Emitter Category";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte EmitterCategory { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        EmitterCategory = buffer[0];
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = EmitterCategory;
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}