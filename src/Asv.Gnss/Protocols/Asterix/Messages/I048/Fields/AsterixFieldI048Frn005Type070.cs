using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I048/070 Mode-3/A Code in Octal Representation.
/// </summary>
public sealed class AsterixFieldI048Frn005Type070 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 5;

    /// <inheritdoc />
    public override string Name => "Mode-3/A Code in Octal Representation";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Validation flag.
    /// </summary>
    public bool V { get; set; }

    /// <summary>
    /// Garbled flag.
    /// </summary>
    public bool G { get; set; }

    /// <summary>
    /// Last reply flag.
    /// </summary>
    public bool L { get; set; }

    /// <summary>
    /// Mode-3/A squawk code.
    /// </summary>
    public ushort Mode3AReply { get; set; }

    private static ushort DecodeMode3ACode(ushort raw)
    {
        var a = (raw >> 9) & 0x07;
        var b = (raw >> 6) & 0x07;
        var c = (raw >> 3) & 0x07;
        var d = raw & 0x07;
        return (ushort)(a * 1000 + b * 100 + c * 10 + d);
    }

    private static ushort EncodeMode3ACode(ushort code)
    {
        var a = (code / 1000) % 10;
        var b = (code / 100) % 10;
        var c = (code / 10) % 10;
        var d = code % 10;
        if (a > 7 || b > 7 || c > 7 || d > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(code), code, "Mode-3/A code digits must be octal.");
        }

        return (ushort)((a << 9) | (b << 6) | (c << 3) | d);
    }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        V = (raw & 0x8000) != 0;
        G = (raw & 0x4000) != 0;
        L = (raw & 0x2000) != 0;
        Mode3AReply = DecodeMode3ACode((ushort)(raw & 0x0FFF));
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var raw = EncodeMode3ACode(Mode3AReply);
        if (V) raw |= 0x8000;
        if (G) raw |= 0x4000;
        if (L) raw |= 0x2000;
        BinaryPrimitives.WriteUInt16BigEndian(buffer, raw);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}