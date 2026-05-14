using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/090 Flight Level in Binary Representation.
/// </summary>
public class AsterixFieldI010Frn017Type090 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 17;

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
        var value = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        V = (value & 0x8000) != 0;
        G = (value & 0x4000) != 0;
        FlightLevel = AsterixI010Binary.SignExtend(value & 0x3FFF, 14) * 0.25;
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