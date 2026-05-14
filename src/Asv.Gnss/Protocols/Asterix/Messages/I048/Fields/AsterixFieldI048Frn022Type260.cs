namespace Asv.Gnss;

/// <summary>
/// I048/260 ACAS Resolution Advisory Report.
/// </summary>
public sealed class AsterixFieldI048Frn022Type260 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 22;
    public override string Name => "ACAS Resolution Advisory Report";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 7;
}