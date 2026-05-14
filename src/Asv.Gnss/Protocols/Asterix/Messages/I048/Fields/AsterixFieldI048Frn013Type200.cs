using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I048/200 Calculated Track Velocity in Polar Coordinates.
/// </summary>
public sealed class AsterixFieldI048Frn013Type200 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 13;

    /// <inheritdoc />
    public override string Name => "Calculated Track Velocity in Polar Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Ground speed in knots.
    /// </summary>
    public double GroundSpeedKt { get; set; }

    /// <summary>
    /// Heading in degrees.
    /// </summary>
    public double HeadingDeg { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        GroundSpeedKt = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 16384.0 * 3600.0;
        buffer = buffer[2..];
        HeadingDeg = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(GroundSpeedKt / 3600.0 * 16384.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(HeadingDeg * 65536.0 / 360.0));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}