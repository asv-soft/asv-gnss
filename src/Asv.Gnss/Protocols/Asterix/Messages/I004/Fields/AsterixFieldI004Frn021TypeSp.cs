namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn021TypeSp : AsterixI004LengthPrefixedRawField
{
    public const byte StaticFrn = 21;
    public override string Name => "Special Purpose Field";
    public override byte FieldReferenceNumber => StaticFrn;
}