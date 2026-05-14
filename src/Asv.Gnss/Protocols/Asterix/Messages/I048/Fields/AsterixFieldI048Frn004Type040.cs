using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I048/040 Measured Position in Polar Coordinates.
/// </summary>
public sealed class AsterixFieldI048Frn004Type040 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 4;

    /// <inheritdoc />
    public override string Name => "Measured Position in Polar Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Range in nautical miles.
    /// </summary>
    public double Rho { get; set; }

    /// <summary>
    /// Azimuth in degrees.
    /// </summary>
    public double Theta { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Rho = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 256.0;
        buffer = buffer[2..];
        Theta = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(Rho * 256.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(Theta * 65536.0 / 360.0));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}