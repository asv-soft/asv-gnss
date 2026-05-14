using System;

namespace Asv.Gnss;

/// <summary>
/// I034/110 Data Filter.
/// </summary>
public sealed class AsterixFieldI034Frn010Type110 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 10;

    /// <inheritdoc />
    public override string Name => "Data Filter";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Data filter code.
    /// </summary>
    public byte Filter { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Filter = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = Filter;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}