namespace Asv.Gnss;

/// <summary>
/// I048/042 Calculated Position in Cartesian Coordinates.
/// </summary>
public sealed class AsterixFieldI048Frn012Type042 : AsterixFieldI048FixedRaw
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 12;

    /// <inheritdoc />
    public override string Name => "Calculated Position in Cartesian Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <inheritdoc />
    protected override int ByteSize => 4;
}