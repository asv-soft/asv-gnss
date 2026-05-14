using System;

namespace Asv.Gnss;

/// <summary>
/// I010/210 Calculated Acceleration.
/// </summary>
public class AsterixFieldI010Frn025Type210 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 25;

    /// <inheritdoc />
    public override string Name => "Calculated Acceleration";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// X acceleration component.
    /// </summary>
    public double Ax { get; set; }

    /// <summary>
    /// Y acceleration component.
    /// </summary>
    public double Ay { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Ax = (sbyte)buffer[0] * 0.25;
        Ay = (sbyte)buffer[1] * 0.25;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(sbyte)Math.Round(Ax / 0.25);
        buffer[1] = (byte)(sbyte)Math.Round(Ay / 0.25);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}