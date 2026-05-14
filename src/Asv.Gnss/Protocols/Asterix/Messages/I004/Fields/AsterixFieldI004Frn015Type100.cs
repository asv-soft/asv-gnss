using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn015Type100 : AsterixI004CompoundField
{
    public const byte StaticFrn = 15;
    public override string Name => "Area Definition";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => 6,
        1 => 7,
        2 => 7,
        3 => 7,
        4 => 7,
        5 => 7,
        _ => 0
    };
}