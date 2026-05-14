using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I048/130 Radar Plot Characteristics.
/// </summary>
public sealed class AsterixFieldI048Frn007Type130 : AsterixFieldI048
{
    private byte _flags;

    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 7;

    /// <inheritdoc />
    public override string Name => "Radar Plot Characteristics";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Raw primary subfield specification octet.
    /// </summary>
    public byte Flags => _flags;

    /// <summary>
    /// Slant range correction.
    /// </summary>
    public byte? Srl { get; set; }

    /// <summary>
    /// Slant range rate correction.
    /// </summary>
    public byte? Srr { get; set; }

    /// <summary>
    /// Amplitude of SSR reply in dBm.
    /// </summary>
    public sbyte? Sam { get; set; }

    /// <summary>
    /// PSR range difference.
    /// </summary>
    public short? Prl { get; set; }

    /// <summary>
    /// PSR amplitude.
    /// </summary>
    public sbyte? Pam { get; set; }

    /// <summary>
    /// Range difference.
    /// </summary>
    public sbyte? Rpd { get; set; }

    /// <summary>
    /// Azimuth difference.
    /// </summary>
    public sbyte? Apd { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _flags = buffer[0];
        buffer = buffer[1..];
        Srl = (_flags & 0x80) != 0 ? buffer[0] : null;
        if (Srl.HasValue) buffer = buffer[1..];
        Srr = (_flags & 0x40) != 0 ? buffer[0] : null;
        if (Srr.HasValue) buffer = buffer[1..];
        Sam = (_flags & 0x20) != 0 ? (sbyte)buffer[0] : null;
        if (Sam.HasValue) buffer = buffer[1..];
        Prl = (_flags & 0x10) != 0 ? BinaryPrimitives.ReadInt16BigEndian(buffer) : null;
        if (Prl.HasValue) buffer = buffer[2..];
        Pam = (_flags & 0x08) != 0 ? (sbyte)buffer[0] : null;
        if (Pam.HasValue) buffer = buffer[1..];
        Rpd = (_flags & 0x04) != 0 ? (sbyte)buffer[0] : null;
        if (Rpd.HasValue) buffer = buffer[1..];
        Apd = (_flags & 0x02) != 0 ? (sbyte)buffer[0] : null;
        if (Apd.HasValue) buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((Srl.HasValue ? 0x80 : 0) |
                           (Srr.HasValue ? 0x40 : 0) |
                           (Sam.HasValue ? 0x20 : 0) |
                           (Prl.HasValue ? 0x10 : 0) |
                           (Pam.HasValue ? 0x08 : 0) |
                           (Rpd.HasValue ? 0x04 : 0) |
                           (Apd.HasValue ? 0x02 : 0));
        buffer = buffer[1..];
        if (Srl.HasValue) buffer[0] = Srl.Value;
        if (Srl.HasValue) buffer = buffer[1..];
        if (Srr.HasValue) buffer[0] = Srr.Value;
        if (Srr.HasValue) buffer = buffer[1..];
        if (Sam.HasValue) buffer[0] = (byte)Sam.Value;
        if (Sam.HasValue) buffer = buffer[1..];
        if (Prl.HasValue) BinaryPrimitives.WriteInt16BigEndian(buffer, Prl.Value);
        if (Prl.HasValue) buffer = buffer[2..];
        if (Pam.HasValue) buffer[0] = (byte)Pam.Value;
        if (Pam.HasValue) buffer = buffer[1..];
        if (Rpd.HasValue) buffer[0] = (byte)Rpd.Value;
        if (Rpd.HasValue) buffer = buffer[1..];
        if (Apd.HasValue) buffer[0] = (byte)Apd.Value;
        if (Apd.HasValue) buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 +
                                         (Srl.HasValue ? 1 : 0) +
                                         (Srr.HasValue ? 1 : 0) +
                                         (Sam.HasValue ? 1 : 0) +
                                         (Prl.HasValue ? 2 : 0) +
                                         (Pam.HasValue ? 1 : 0) +
                                         (Rpd.HasValue ? 1 : 0) +
                                         (Apd.HasValue ? 1 : 0);
}