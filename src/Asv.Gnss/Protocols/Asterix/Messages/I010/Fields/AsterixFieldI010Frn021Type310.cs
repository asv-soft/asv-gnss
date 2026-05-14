using System;

namespace Asv.Gnss;

/// <summary>
/// I010/310 Pre-programmed Message.
/// </summary>
public class AsterixFieldI010Frn021Type310 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 21;

    /// <inheritdoc />
    public override string Name => "Pre-programmed Message";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Trouble or emergency indicator.
    /// </summary>
    public bool Trb { get; set; }

    /// <summary>
    /// Message identifier.
    /// </summary>
    public byte Message { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Trb = (buffer[0] & 0x80) != 0;
        Message = (byte)(buffer[0] & 0x7F);
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((Trb ? 0x80 : 0) | (Message & 0x7F));
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}