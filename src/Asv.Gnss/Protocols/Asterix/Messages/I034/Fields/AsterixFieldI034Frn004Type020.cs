using System;

namespace Asv.Gnss;

/// <summary>
/// I034/020 Sector Number.
/// </summary>
public sealed class AsterixFieldI034Frn004Type020 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 4;

    /// <inheritdoc />
    public override string Name => "Sector Number";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Sector angle in degrees.
    /// </summary>
    public double SectorNumberDeg { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        SectorNumberDeg = buffer[0] * 360.0 / 256.0;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Math.Round(SectorNumberDeg / (360.0 / 256.0));
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}