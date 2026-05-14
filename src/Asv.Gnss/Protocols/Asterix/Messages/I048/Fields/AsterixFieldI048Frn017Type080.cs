namespace Asv.Gnss;

/// <summary>
/// I048/080 Mode-3/A Code Confidence Indicator.
/// </summary>
public sealed class AsterixFieldI048Frn017Type080 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 17;
    public override string Name => "Mode-3/A Code Confidence Indicator";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 2;
}