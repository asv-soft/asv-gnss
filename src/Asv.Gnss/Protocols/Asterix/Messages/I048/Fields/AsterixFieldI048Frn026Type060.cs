namespace Asv.Gnss;

/// <summary>
/// I048/060 Mode-2 Code Confidence Indicator.
/// </summary>
public sealed class AsterixFieldI048Frn026Type060 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 26;
    public override string Name => "Mode-2 Code Confidence Indicator";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 2;
}