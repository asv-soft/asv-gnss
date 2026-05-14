using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn012Type080 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 11;
    public override string Name => "Target Address";
    public override byte FieldReferenceNumber => StaticFrn;
    public uint TargetAddress { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TargetAddress = AsterixI021Binary.ReadUInt24(ref buffer);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI021Binary.WriteUInt24(ref buffer, TargetAddress);
    }

    public override int GetByteSize() => 3;
}