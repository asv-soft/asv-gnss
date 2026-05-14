using System;

namespace Asv.Gnss;

/// <summary>
/// I048/170 Track Status.
/// </summary>
public sealed class AsterixFieldI048Frn014Type170 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 14;

    /// <inheritdoc />
    public override string Name => "Track Status";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Confirmed track indicator.
    /// </summary>
    public bool Cnf { get; set; }

    /// <summary>
    /// Radar source status.
    /// </summary>
    public byte Rad { get; set; }

    /// <summary>
    /// Doubtful track indicator.
    /// </summary>
    public bool Dou { get; set; }

    /// <summary>
    /// Horizontal manoeuvre indicator.
    /// </summary>
    public bool Mah { get; set; }

    /// <summary>
    /// Climb/descent mode.
    /// </summary>
    public byte Cdm { get; set; }

    /// <summary>
    /// Indicates that an extension octet was present.
    /// </summary>
    public bool HasExtension { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = buffer[0];
        buffer = buffer[1..];
        Cnf = (raw & 0x80) != 0;
        Rad = (byte)((raw >> 5) & 0x03);
        Dou = (raw & 0x10) != 0;
        Mah = (raw & 0x08) != 0;
        Cdm = (byte)((raw >> 1) & 0x03);
        HasExtension = (raw & 0x01) != 0;
        if (HasExtension)
        {
            // Extension bits are currently not interpreted, but preserved by consuming the octets.
            do
            {
                raw = buffer[0];
                buffer = buffer[1..];
            }
            while ((raw & 0x01) != 0);
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((Cnf ? 0x80 : 0) |
                           ((Rad & 0x03) << 5) |
                           (Dou ? 0x10 : 0) |
                           (Mah ? 0x08 : 0) |
                           ((Cdm & 0x03) << 1));
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}