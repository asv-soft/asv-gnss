using System;

namespace Asv.Gnss;

/// <summary>
/// I010/020 Target Report Descriptor.
/// </summary>
public class AsterixFieldI010Frn003Type020 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 3;

    /// <inheritdoc />
    public override string Name => "Target Report Descriptor";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Type of detection.
    /// </summary>
    public byte Typ { get; set; }

    /// <summary>
    /// Differential correction indicator.
    /// </summary>
    public bool Dcr { get; set; }

    /// <summary>
    /// Chain indicator.
    /// </summary>
    public bool Chn { get; set; }

    /// <summary>
    /// Ground bit set indicator.
    /// </summary>
    public bool Gbs { get; set; }

    /// <summary>
    /// Corrupted reply in multilateration indicator.
    /// </summary>
    public bool Crt { get; set; }

    /// <summary>
    /// Simulation report indicator.
    /// </summary>
    public bool Sim { get; set; }

    /// <summary>
    /// Test target indicator.
    /// </summary>
    public bool Tst { get; set; }

    /// <summary>
    /// Report from field monitor indicator.
    /// </summary>
    public bool Rab { get; set; }

    /// <summary>
    /// Loop status.
    /// </summary>
    public byte Lop { get; set; }

    /// <summary>
    /// Type of target.
    /// </summary>
    public byte Tot { get; set; }

    /// <summary>
    /// Special position identification indicator.
    /// </summary>
    public bool Spi { get; set; }

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

        Typ = (byte)(first >> 5);
        Dcr = (first & 0x10) != 0;
        Chn = (first & 0x08) != 0;
        Gbs = (first & 0x04) != 0;
        Crt = (first & 0x02) != 0;
        HasFirstExtension = (first & 0x01) != 0;

        if (!HasFirstExtension) return;

        var second = buffer[0];
        buffer = buffer[1..];
        Sim = (second & 0x80) != 0;
        Tst = (second & 0x40) != 0;
        Rab = (second & 0x20) != 0;
        Lop = (byte)((second >> 3) & 0x03);
        Tot = (byte)((second >> 1) & 0x03);
        HasSecondExtension = (second & 0x01) != 0;

        if (!HasSecondExtension) return;

        var third = buffer[0];
        buffer = buffer[1..];
        Spi = (third & 0x80) != 0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var hasSecond = HasSecondExtension || Spi;
        var hasFirst = HasFirstExtension || hasSecond || Sim || Tst || Rab || Lop != 0 || Tot != 0;
        buffer[0] = (byte)((Typ << 5) |
                           (Dcr ? 0x10 : 0) |
                           (Chn ? 0x08 : 0) |
                           (Gbs ? 0x04 : 0) |
                           (Crt ? 0x02 : 0) |
                           (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];

        if (!hasFirst) return;

        buffer[0] = (byte)((Sim ? 0x80 : 0) |
                           (Tst ? 0x40 : 0) |
                           (Rab ? 0x20 : 0) |
                           ((Lop & 0x03) << 3) |
                           ((Tot & 0x03) << 1) |
                           (hasSecond ? 0x01 : 0));
        buffer = buffer[1..];

        if (!hasSecond) return;

        buffer[0] = (byte)(Spi ? 0x80 : 0);
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (HasFirstExtension || HasSecondExtension || Sim || Tst || Rab || Lop != 0 || Tot != 0 || Spi ? 1 : 0) + (HasSecondExtension || Spi ? 1 : 0);
}