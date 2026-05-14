namespace Asv.Gnss;

/// <summary>
/// I048/210 Track Quality.
/// </summary>
public sealed class AsterixFieldI048Frn015Type210 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 15;
    public override string Name => "Track Quality";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 4;
}