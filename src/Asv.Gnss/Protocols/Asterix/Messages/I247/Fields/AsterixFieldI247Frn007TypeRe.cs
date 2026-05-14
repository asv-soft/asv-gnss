using System;

namespace Asv.Gnss;

/// <summary>
/// I247/RE Reserved Expansion Field.
/// </summary>
public class AsterixFieldI247Frn007TypeRe : AsterixFieldI247
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 7;

    /// <inheritdoc />
    public override string Name => "Reserved Expansion Field";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Raw reserved expansion payload without the length octet.
    /// </summary>
    public byte[] Data { get; set; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var length = buffer[0];
        Data = buffer.Slice(1, length - 1).ToArray();
        buffer = buffer[length..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(Data.Length + 1);
        Data.CopyTo(buffer[1..]);
        buffer = buffer[(Data.Length + 1)..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Data.Length;
}