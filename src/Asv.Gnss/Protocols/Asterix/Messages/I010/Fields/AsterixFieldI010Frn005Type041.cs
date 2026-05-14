using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/041 Position in WGS-84 Coordinates.
/// </summary>
public class AsterixFieldI010Frn005Type041 : AsterixFieldI010
{
    private const double Scale = 180.0 / 2147483648.0;

    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 5;

    /// <inheritdoc />
    public override string Name => "Position in WGS-84 Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Latitude in degrees.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in degrees.
    /// </summary>
    public double Longitude { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Latitude = BinaryPrimitives.ReadInt32BigEndian(buffer) * Scale;
        buffer = buffer[4..];
        Longitude = BinaryPrimitives.ReadInt32BigEndian(buffer) * Scale;
        buffer = buffer[4..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)Math.Round(Latitude / Scale));
        buffer = buffer[4..];
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)Math.Round(Longitude / Scale));
        buffer = buffer[4..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 8;
}