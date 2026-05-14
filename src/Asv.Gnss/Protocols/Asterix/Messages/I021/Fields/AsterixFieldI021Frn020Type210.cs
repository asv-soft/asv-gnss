using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn020Type210 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 18;
    public override string Name => "MOPS Version";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool Vns { get; set; }
    public byte Vn { get; set; }
    public byte Ltt { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        buffer = buffer[1..];
        Vns = (b & 0x40) != 0;
        Vn = (byte)((b >> 3) & 0x07);
        Ltt = (byte)(b & 0x07);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((Vns ? 0x40 : 0) | ((Vn & 0x07) << 3) | (Ltt & 0x07));
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}