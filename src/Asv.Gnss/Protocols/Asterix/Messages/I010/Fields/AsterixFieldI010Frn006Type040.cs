using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/040 Measured Position in Polar Coordinates.
/// </summary>
public class AsterixFieldI010Frn006Type040 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 6;

    /// <inheritdoc />
    public override string Name => "Measured Position in Polar Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Range in metres.
    /// </summary>
    public double Rho { get; set; }

    /// <summary>
    /// Azimuth in degrees.
    /// </summary>
    public double Theta { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Rho = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        Theta = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(Rho));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(Theta * 65536.0 / 360.0));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}