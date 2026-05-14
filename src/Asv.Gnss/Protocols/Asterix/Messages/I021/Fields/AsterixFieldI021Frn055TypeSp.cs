namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn055TypeSp : AsterixLengthPrefixedRawField
{
    public const byte StaticFrn = 49;
    public override string Name => "Special Purpose Field";
    public override byte FieldReferenceNumber => StaticFrn;
}