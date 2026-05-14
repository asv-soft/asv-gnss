using System;

namespace Asv.Gnss;

/// <summary>
/// I010/140 Time of Day.
/// </summary>
public class AsterixFieldI010Frn004Type140 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 4;

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
        Seconds = AsterixI010Binary.ReadUInt24(ref buffer) / 128.0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI010Binary.WriteUInt24(ref buffer, (int)Math.Round(Seconds * 128.0));
    }

    /// <inheritdoc />
    public override int GetByteSize() => 3;
}