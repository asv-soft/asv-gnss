using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/060 Mode-3/A Code in Octal Representation.
/// </summary>
public class AsterixFieldI010Frn012Type060 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 12;

    /// <inheritdoc />
    public override string Name => "Mode-3/A Code in Octal Representation";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Mode-3/A code validation flag.
    /// </summary>
    public bool V { get; set; }

    /// <summary>
    /// Garbled code flag.
    /// </summary>
    public bool G { get; set; }

    /// <summary>
    /// Last reply flag.
    /// </summary>
    public bool L { get; set; }

    /// <summary>
    /// Squawk code in octal representation.
    /// </summary>
    public ushort Squawk { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var value = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        V = (value & 0x8000) != 0;
        G = (value & 0x4000) != 0;
        L = (value & 0x2000) != 0;
        Squawk = AsterixProtocol.GetSquawk((ushort)(value & 0x1FFF));
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var value = AsterixProtocol.SetSquawk(Squawk);
        if (V) value |= 0x8000;
        if (G) value |= 0x4000;
        if (L) value |= 0x2000;
        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}