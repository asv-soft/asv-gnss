using System;

namespace Asv.Gnss;

/// <summary>
/// I034/030 Time of Day.
/// </summary>
public sealed class AsterixFieldI034Frn003Type030 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 3;

    /// <inheritdoc />
    public override string Name => "Time of Day";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Time of day in seconds since midnight UTC.
    /// </summary>
    public double Seconds { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Seconds = AsterixI034Binary.ReadUInt24(ref buffer) / 128.0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI034Binary.WriteUInt24(ref buffer, (uint)Math.Round(Seconds * 128.0));
    }

    /// <inheritdoc />
    public override int GetByteSize() => 3;
}