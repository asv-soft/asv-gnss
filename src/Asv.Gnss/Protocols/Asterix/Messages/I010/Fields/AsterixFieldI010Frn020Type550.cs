using System;

namespace Asv.Gnss;

/// <summary>
/// I010/550 System Status.
/// </summary>
public class AsterixFieldI010Frn020Type550 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 20;

    /// <inheritdoc />
    public override string Name => "System Status";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Raw system status octet.
    /// </summary>
    public byte RawStatus { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        RawStatus = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = RawStatus;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}