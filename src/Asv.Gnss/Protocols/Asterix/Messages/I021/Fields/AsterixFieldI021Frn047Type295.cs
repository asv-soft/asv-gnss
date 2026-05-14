using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn047Type295 : AsterixRawCompoundField
{
    public const byte StaticFrn = 42;
    public override string Name => "Data Ages";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => 1;
}