namespace Asv.Gnss;

/// <summary>
/// I048/110 Height Measured by a 3D Radar.
/// </summary>
public sealed class AsterixFieldI048Frn019Type110 : AsterixFieldI048FixedRaw
{
    public const byte StaticFrn = 19;
    public override string Name => "Height Measured by a 3D Radar";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int ByteSize => 2;
}