namespace Asv.Gnss;

/// <summary>
/// I048/050 Mode-2 Code in Octal Representation.
/// </summary>
public sealed class AsterixFieldI048Frn024Type050 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 24;
    public override string Name => "Mode-2 Code in Octal Representation";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 2;
}