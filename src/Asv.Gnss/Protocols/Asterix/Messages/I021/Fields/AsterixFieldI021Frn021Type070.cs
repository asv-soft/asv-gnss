using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn021Type070 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 19;
    public override string Name => "Mode 3/A Code in Octal Representation";
    public override byte FieldReferenceNumber => StaticFrn;
    public ushort Mode3ACode { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer) & 0x0FFF;
        Mode3ACode = (ushort)((((raw >> 9) & 0x07) * 1000) +
                              (((raw >> 6) & 0x07) * 100) +
                              (((raw >> 3) & 0x07) * 10) +
                              (raw & 0x07));
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var a = (Mode3ACode / 1000) % 10;
        var b = (Mode3ACode / 100) % 10;
        var c = (Mode3ACode / 10) % 10;
        var d = Mode3ACode % 10;
        if (a > 7 || b > 7 || c > 7 || d > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(Mode3ACode), "Mode 3/A digits must be octal.");
        }

        var raw = (ushort)((a << 9) | (b << 6) | (c << 3) | d);
        BinaryPrimitives.WriteUInt16BigEndian(buffer, raw);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}