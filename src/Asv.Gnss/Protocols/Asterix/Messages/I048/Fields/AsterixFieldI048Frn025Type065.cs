namespace Asv.Gnss;

/// <summary>
/// I048/065 Mode-1 Code Confidence Indicator.
/// </summary>
public sealed class AsterixFieldI048Frn025Type065 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 25;
    public override string Name => "Mode-1 Code Confidence Indicator";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 1;
}