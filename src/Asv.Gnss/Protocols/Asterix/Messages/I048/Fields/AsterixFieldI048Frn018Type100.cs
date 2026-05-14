namespace Asv.Gnss;

/// <summary>
/// I048/100 Mode-C Code and Confidence Indicator.
/// </summary>
public sealed class AsterixFieldI048Frn018Type100 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 18;
    public override string Name => "Mode-C Code and Confidence Indicator";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 4;
}