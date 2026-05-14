using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn038Type110 : AsterixRawCompoundField
{
    public const byte StaticFrn = 34;
    public override string Name => "Trajectory Intent";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer)
    {
        return dataBitIndex switch
        {
            0 => 1,
            1 => 1 + buffer[0] * 15,
            _ => 0
        };
    }
}