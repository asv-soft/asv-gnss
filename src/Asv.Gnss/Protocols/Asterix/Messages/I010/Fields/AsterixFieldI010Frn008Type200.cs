using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/200 Calculated Track Velocity in Polar Coordinates.
/// </summary>
public class AsterixFieldI010Frn008Type200 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 8;

    /// <inheritdoc />
    public override string Name => "Calculated Track Velocity in Polar Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Ground speed in nautical miles per second.
    /// </summary>
    public double GroundSpeed { get; set; }

    /// <summary>
    /// Track angle in degrees.
    /// </summary>
    public double TrackAngle { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        GroundSpeed = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 16384.0;
        buffer = buffer[2..];
        TrackAngle = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(GroundSpeed * 16384.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(TrackAngle * 65536.0 / 360.0));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}