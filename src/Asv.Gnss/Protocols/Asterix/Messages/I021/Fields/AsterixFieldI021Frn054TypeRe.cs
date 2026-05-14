namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn054TypeRe : AsterixLengthPrefixedRawField
{
    public const byte StaticFrn = 48;
    public override string Name => "Reserved Expansion Field";
    public override byte FieldReferenceNumber => StaticFrn;
}