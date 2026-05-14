using System;

namespace Asv.Gnss;

/// <summary>
/// I048/020 Target Report Descriptor.
/// </summary>
public sealed class AsterixFieldI048Frn003Type020 : AsterixFieldI048
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
    /// Type of report.
    /// </summary>
    public byte Typ { get; set; }

    /// <summary>
    /// Simulated target report indicator.
    /// </summary>
    public bool Sim { get; set; }

    /// <summary>
    /// Reported detection probability indicator.
    /// </summary>
    public byte Rdp { get; set; }

    /// <summary>
    /// Special position identification indicator.
    /// </summary>
    public bool Spi { get; set; }

    /// <summary>
    /// Report from field monitor indicator.
    /// </summary>
    public bool Rab { get; set; }

    /// <summary>
    /// Test target report indicator.
    /// </summary>
    public bool Tst { get; set; }

    /// <summary>
    /// Extended range indicator.
    /// </summary>
    public bool Err { get; set; }

    /// <summary>
    /// X-pulse present indicator.
    /// </summary>
    public bool Xpp { get; set; }

    /// <summary>
    /// Military emergency indicator.
    /// </summary>
    public bool Me { get; set; }

    /// <summary>
    /// Military identification indicator.
    /// </summary>
    public bool Mi { get; set; }

    /// <summary>
    /// FOE/FRI status.
    /// </summary>
    public byte FoeFri { get; set; }

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
        Sim = (first & 0x10) != 0;
        Rdp = (byte)((first >> 3) & 0x03);
        Spi = (first & 0x04) != 0;
        Rab = (first & 0x02) != 0;
        HasFirstExtension = (first & 0x01) != 0;
        if (!HasFirstExtension)
        {
            return;
        }

        var second = buffer[0];
        buffer = buffer[1..];
        Tst = (second & 0x80) != 0;
        Err = (second & 0x40) != 0;
        Xpp = (second & 0x20) != 0;
        Me = (second & 0x10) != 0;
        Mi = (second & 0x08) != 0;
        FoeFri = (byte)((second >> 1) & 0x03);
        HasSecondExtension = (second & 0x01) != 0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var hasSecond = HasSecondExtension;
        var hasFirst = HasFirstExtension || hasSecond || Tst || Err || Xpp || Me || Mi || FoeFri != 0;
        buffer[0] = (byte)((Typ << 5) |
                           (Sim ? 0x10 : 0) |
                           ((Rdp & 0x03) << 3) |
                           (Spi ? 0x04 : 0) |
                           (Rab ? 0x02 : 0) |
                           (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];
        if (!hasFirst)
        {
            return;
        }

        buffer[0] = (byte)((Tst ? 0x80 : 0) |
                           (Err ? 0x40 : 0) |
                           (Xpp ? 0x20 : 0) |
                           (Me ? 0x10 : 0) |
                           (Mi ? 0x08 : 0) |
                           ((FoeFri & 0x03) << 1) |
                           (hasSecond ? 0x01 : 0));
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (HasFirstExtension || HasSecondExtension || Tst || Err || Xpp || Me || Mi || FoeFri != 0 ? 1 : 0);
}