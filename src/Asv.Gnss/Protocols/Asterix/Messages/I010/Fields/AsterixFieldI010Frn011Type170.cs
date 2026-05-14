using System;

namespace Asv.Gnss;

/// <summary>
/// I010/170 Track Status.
/// </summary>
public class AsterixFieldI010Frn011Type170 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 11;

    /// <inheritdoc />
    public override string Name => "Track Status";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Confirmed track indicator.
    /// </summary>
    public bool Cnf { get; set; }

    /// <summary>
    /// Last report for a track indicator.
    /// </summary>
    public bool Tre { get; set; }

    /// <summary>
    /// Track type or correlation status.
    /// </summary>
    public bool Cst { get; set; }

    /// <summary>
    /// Manoeuvre horizontal sense indicator.
    /// </summary>
    public bool Mah { get; set; }

    /// <summary>
    /// Tracking coordination status.
    /// </summary>
    public bool Tcc { get; set; }

    /// <summary>
    /// Smoothed track indicator.
    /// </summary>
    public bool Sth { get; set; }

    /// <summary>
    /// Type of movement.
    /// </summary>
    public byte Tom { get; set; }

    /// <summary>
    /// Doubtful track indicator.
    /// </summary>
    public bool Dou { get; set; }

    /// <summary>
    /// Merge or split status.
    /// </summary>
    public byte Mrs { get; set; }

    /// <summary>
    /// Ghost target indicator.
    /// </summary>
    public bool Gho { get; set; }

    /// <summary>
    /// Indicates that the first extension octet was present.
    /// </summary>
    public bool HasFirstExtension { get; set; }

    /// <summary>
    /// Indicates that the second extension octet was present.
    /// </summary>
    public bool HasSecondExtension { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var first = buffer[0];
        buffer = buffer[1..];
        Cnf = (first & 0x80) != 0;
        Tre = (first & 0x40) != 0;
        Cst = (first & 0x20) != 0;
        Mah = (first & 0x10) != 0;
        Tcc = (first & 0x08) != 0;
        Sth = (first & 0x02) != 0;
        HasFirstExtension = (first & 0x01) != 0;

        if (!HasFirstExtension) return;

        var second = buffer[0];
        buffer = buffer[1..];
        Tom = (byte)(second >> 6);
        Dou = (second & 0x20) != 0;
        Mrs = (byte)((second >> 1) & 0x0F);
        HasSecondExtension = (second & 0x01) != 0;

        if (!HasSecondExtension) return;

        var third = buffer[0];
        buffer = buffer[1..];
        Gho = (third & 0x80) != 0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var hasSecond = HasSecondExtension || Gho;
        var hasFirst = HasFirstExtension || hasSecond || Tom != 0 || Dou || Mrs != 0;
        buffer[0] = (byte)((Cnf ? 0x80 : 0) |
                           (Tre ? 0x40 : 0) |
                           (Cst ? 0x20 : 0) |
                           (Mah ? 0x10 : 0) |
                           (Tcc ? 0x08 : 0) |
                           (Sth ? 0x02 : 0) |
                           (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];

        if (!hasFirst) return;

        buffer[0] = (byte)(((Tom & 0x03) << 6) |
                           (Dou ? 0x20 : 0) |
                           ((Mrs & 0x0F) << 1) |
                           (hasSecond ? 0x01 : 0));
        buffer = buffer[1..];

        if (!hasSecond) return;

        buffer[0] = (byte)(Gho ? 0x80 : 0);
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (HasFirstExtension || HasSecondExtension || Tom != 0 || Dou || Mrs != 0 || Gho ? 1 : 0) + (HasSecondExtension || Gho ? 1 : 0);
}