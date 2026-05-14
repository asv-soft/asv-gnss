using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn010Type120 : AsterixI004CompoundField
{
    public const byte StaticFrn = 10;
    public override string Name => "Conflict Characteristics";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => (buffer[0] & 0x01) != 0 ? 2 : 1,
        1 => 1,
        2 => 1,
        3 => 3,
        _ => 0
    };
}