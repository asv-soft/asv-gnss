namespace Asv.Gnss;

/// <summary>
/// I034/120 3D-Position of Data Source.
/// </summary>
public sealed class AsterixFieldI034Frn011Type120 : AsterixFieldI034FixedRaw
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 11;

    /// <inheritdoc />
    public override string Name => "3D-Position of Data Source";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <inheritdoc />
    protected override int ByteSize => 8;
}