using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn009Type170 : AsterixI004CompoundField
{
    public const byte StaticFrn = 9;
    public override string Name => "Aircraft Identification & Characteristics 1";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => 7,
        1 => 2,
        2 => 10,
        3 => 8,
        4 => 3,
        5 => 2,
        6 => (buffer[0] & 0x01) != 0 ? 2 : 1,
        8 => 6,
        9 => 4,
        10 => 2,
        _ => 0
    };
}