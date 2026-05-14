using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/091 Measured Height.
/// </summary>
public class AsterixFieldI010Frn018Type091 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 18;

    /// <inheritdoc />
    public override string Name => "Measured Height";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Measured height in flight level units.
    /// </summary>
    public double MeasuredHeight { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        MeasuredHeight = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(MeasuredHeight / 0.25));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}