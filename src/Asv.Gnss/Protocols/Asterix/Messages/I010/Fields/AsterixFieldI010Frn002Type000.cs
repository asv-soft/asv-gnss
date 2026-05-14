using System;

namespace Asv.Gnss;

/// <summary>
/// I010/000 Message Type.
/// </summary>
public class AsterixFieldI010Frn002Type000 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 2;

    /// <inheritdoc />
    public override string Name => "Message Type";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// CAT010 message type value.
    /// </summary>
    public byte MessageType { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        MessageType = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = MessageType;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}