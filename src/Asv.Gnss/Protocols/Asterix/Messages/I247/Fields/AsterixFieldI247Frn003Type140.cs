using System;

namespace Asv.Gnss;

/// <summary>
/// I247/140 Time of Day.
/// </summary>
public class AsterixFieldI247Frn003Type140 : AsterixFieldI247
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
    /// Absolute UTC time in seconds since midnight.
    /// </summary>
    public double Seconds { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Seconds = AsterixI247Binary.ReadUInt24(ref buffer) / 128.0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI247Binary.WriteUInt24(ref buffer, (int)Math.Round(Seconds * 128.0));
    }

    /// <inheritdoc />
    public override int GetByteSize() => 3;
}