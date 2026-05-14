using System;

namespace Asv.Gnss;

/// <summary>
/// Base class for fixed-length raw CAT034 fields.
/// </summary>
public abstract class AsterixFieldI034FixedRaw : AsterixFieldI034
{
    /// <summary>
    /// Raw field bytes.
    /// </summary>
    public byte[] Data { get; private set; } = [];

    /// <summary>
    /// Fixed byte size.
    /// </summary>
    protected abstract int ByteSize { get; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Data = buffer[..ByteSize].ToArray();
        buffer = buffer[ByteSize..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        Data.CopyTo(buffer);
        buffer = buffer[Data.Length..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => Data.Length == 0 ? ByteSize : Data.Length;
}