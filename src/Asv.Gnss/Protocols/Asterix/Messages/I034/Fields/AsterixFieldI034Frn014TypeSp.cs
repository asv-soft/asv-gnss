using System;

namespace Asv.Gnss;

/// <summary>
/// I034/SP Special Purpose Field.
/// </summary>
public sealed class AsterixFieldI034Frn014TypeSp : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 14;

    /// <inheritdoc />
    public override string Name => "Special Purpose Field";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Special purpose payload without the length octet.
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
        buffer[0] = checked((byte)(Data.Length + 1));
        Data.CopyTo(buffer[1..]);
        buffer = buffer[(Data.Length + 1)..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => Data.Length + 1;
}