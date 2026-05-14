using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn026Type200 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 23;
    public override string Name => "Target Status";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool Icf { get; set; }
    public bool Lnav { get; set; }
    public byte Ps { get; set; }
    public byte Ss { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        buffer = buffer[1..];
        Icf = (b & 0x80) != 0;
        Lnav = (b & 0x40) != 0;
        Ps = (byte)((b >> 3) & 0x07);
        Ss = (byte)(b & 0x07);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((Icf ? 0x80 : 0) | (Lnav ? 0x40 : 0) | ((Ps & 0x07) << 3) | (Ss & 0x07));
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}