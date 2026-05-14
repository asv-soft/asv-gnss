using System;

namespace Asv.Gnss;

/// <summary>
/// I010/131 Amplitude of Primary Plot.
/// </summary>
public class AsterixFieldI010Frn024Type131 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 24;

    /// <inheritdoc />
    public override string Name => "Amplitude of Primary Plot";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Primary plot amplitude.
    /// </summary>
    public sbyte Amplitude { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Amplitude = (sbyte)buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Amplitude;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}