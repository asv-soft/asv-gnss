using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I048/090 Flight Level in Binary Representation.
/// </summary>
public sealed class AsterixFieldI048Frn006Type090 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 6;

    /// <inheritdoc />
    public override string Name => "Flight Level in Binary Representation";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Validation flag.
    /// </summary>
    public bool V { get; set; }

    /// <summary>
    /// Garbled flag.
    /// </summary>
    public bool G { get; set; }

    /// <summary>
    /// Flight level in FL units.
    /// </summary>
    public double FlightLevel { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        V = (raw & 0x8000) != 0;
        G = (raw & 0x4000) != 0;
        FlightLevel = AsterixI048Binary.SignExtend(raw & 0x3FFF, 14) * 0.25;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var raw = ((int)Math.Round(FlightLevel / 0.25)) & 0x3FFF;
        if (V) raw |= 0x8000;
        if (G) raw |= 0x4000;
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)raw);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}