using System;

namespace Asv.Gnss;

/// <summary>
/// I048/230 Communications/ACAS Capability and Flight Status.
/// </summary>
public sealed class AsterixFieldI048Frn021Type230 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 21;

    /// <inheritdoc />
    public override string Name => "Communications/ACAS Capability and Flight Status";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Communications capability.
    /// </summary>
    public byte Com { get; set; }

    /// <summary>
    /// Flight status.
    /// </summary>
    public byte Stat { get; set; }

    /// <summary>
    /// Mode-S specific service capability.
    /// </summary>
    public bool Mssc { get; set; }

    /// <summary>
    /// Altitude reporting capability.
    /// </summary>
    public bool Arc { get; set; }

    /// <summary>
    /// Aircraft identification capability.
    /// </summary>
    public bool Aic { get; set; }

    /// <summary>
    /// BDS 1,0 bit 16.
    /// </summary>
    public bool B1A { get; set; }

    /// <summary>
    /// BDS 1,0 bits 37/40.
    /// </summary>
    public byte B1B { get; set; }

    /// <summary>
    /// Surveillance identifier flag used by newer CAT048 editions.
    /// </summary>
    public bool Si { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Com = (byte)(buffer[0] >> 5);
        Stat = (byte)((buffer[0] >> 2) & 0x07);
        Si = (buffer[0] & 0x01) != 0;
        Mssc = (buffer[1] & 0x80) != 0;
        Arc = (buffer[1] & 0x40) != 0;
        Aic = (buffer[1] & 0x20) != 0;
        B1A = (buffer[1] & 0x10) != 0;
        B1B = (byte)(buffer[1] & 0x0F);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((Com << 5) | ((Stat & 0x07) << 2) | (Si ? 0x01 : 0));
        buffer[1] = (byte)((Mssc ? 0x80 : 0) |
                           (Arc ? 0x40 : 0) |
                           (Aic ? 0x20 : 0) |
                           (B1A ? 0x10 : 0) |
                           (B1B & 0x0F));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}