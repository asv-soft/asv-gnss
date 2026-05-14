namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn020TypeRe : AsterixI004LengthPrefixedRawField
{
    public const byte StaticFrn = 20;
    public override string Name => "Reserved Expansion Field";
    public override byte FieldReferenceNumber => StaticFrn;
}