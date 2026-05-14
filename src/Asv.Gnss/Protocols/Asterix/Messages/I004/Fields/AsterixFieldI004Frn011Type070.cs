using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn011Type070 : AsterixI004CompoundField
{
    public const byte StaticFrn = 11;
    public override string Name => "Conflict Timing and Separation";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => 3,
        1 => 3,
        2 => 3,
        3 => 2,
        4 => 2,
        5 => 2,
        _ => 0
    };
}