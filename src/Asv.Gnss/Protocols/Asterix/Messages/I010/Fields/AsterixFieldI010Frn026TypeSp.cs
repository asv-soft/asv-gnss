using System;

namespace Asv.Gnss;

/// <summary>
/// I010/SP Special Purpose Field.
/// </summary>
public class AsterixFieldI010Frn026TypeSp : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 26;

    /// <inheritdoc />
    public override string Name => "Special Purpose Field";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Raw special purpose payload without the length octet.
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