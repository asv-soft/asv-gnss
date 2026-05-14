using System;

namespace Asv.Gnss;

/// <summary>
/// I010/270 Target Size and Orientation.
/// </summary>
public class AsterixFieldI010Frn019Type270 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 19;

    /// <inheritdoc />
    public override string Name => "Target Size and Orientation";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Target length in metres.
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Target orientation in degrees.
    /// </summary>
    public double Orientation { get; set; }

    /// <summary>
    /// Target width in metres.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Indicates that orientation octet is present.
    /// </summary>
    public bool HasOrientation { get; set; }

    /// <summary>
    /// Indicates that width octet is present.
    /// </summary>
    public bool HasWidth { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var first = buffer[0];
        buffer = buffer[1..];
        Length = first >> 1;
        HasOrientation = (first & 0x01) != 0;
        if (!HasOrientation) return;

        var second = buffer[0];
        buffer = buffer[1..];
        Orientation = (second >> 1) * 360.0 / 128.0;
        HasWidth = (second & 0x01) != 0;
        if (!HasWidth) return;

        var third = buffer[0];
        buffer = buffer[1..];
        Width = third >> 1;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(((int)Math.Round(Length) << 1) | (HasOrientation ? 1 : 0));
        buffer = buffer[1..];
        if (!HasOrientation) return;

        buffer[0] = (byte)(((int)Math.Round(Orientation * 128.0 / 360.0) << 1) | (HasWidth ? 1 : 0));
        buffer = buffer[1..];
        if (!HasWidth) return;

        buffer[0] = (byte)((int)Math.Round(Width) << 1);
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (HasOrientation ? 1 : 0) + (HasWidth ? 1 : 0);
}