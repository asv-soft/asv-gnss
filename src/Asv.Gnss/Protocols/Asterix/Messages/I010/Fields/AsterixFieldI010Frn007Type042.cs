using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/042 Position in Cartesian Coordinates.
/// </summary>
public class AsterixFieldI010Frn007Type042 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 7;

    /// <inheritdoc />
    public override string Name => "Position in Cartesian Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// X position in metres.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Y position in metres.
    /// </summary>
    public double Y { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        X = BinaryPrimitives.ReadInt16BigEndian(buffer);
        buffer = buffer[2..];
        Y = BinaryPrimitives.ReadInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(X));
        buffer = buffer[2..];
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(Y));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}