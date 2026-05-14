using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I034/041 Antenna Rotation Speed.
/// </summary>
public sealed class AsterixFieldI034Frn005Type041 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 5;

    /// <inheritdoc />
    public override string Name => "Antenna Rotation Speed";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Antenna rotation period in seconds.
    /// </summary>
    public double AntennaRotationPeriod { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AntennaRotationPeriod = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 128.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(AntennaRotationPeriod * 128.0));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}