namespace Asv.Gnss;

/// <summary>
/// I048/055 Mode-1 Code in Octal Representation.
/// </summary>
public sealed class AsterixFieldI048Frn023Type055 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 23;
    public override string Name => "Mode-1 Code in Octal Representation";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 1;
}