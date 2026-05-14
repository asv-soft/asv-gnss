using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn042Type271 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 37;
    public override string Name => "Surface Capabilities and Characteristics";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool Poa { get; set; }
    public bool CdtiS { get; set; }
    public bool B2Low { get; set; }
    public bool Ras { get; set; }
    public bool Ident { get; set; }
    public byte LengthAndWidth { get; set; }
    public bool HasExtension { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        buffer = buffer[1..];
        Poa = (b & 0x20) != 0;
        CdtiS = (b & 0x10) != 0;
        B2Low = (b & 0x08) != 0;
        Ras = (b & 0x04) != 0;
        Ident = (b & 0x02) != 0;
        HasExtension = (b & 0x01) != 0;
        if (HasExtension)
        {
            LengthAndWidth = (byte)(buffer[0] & 0x0F);
            buffer = buffer[1..];
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var hasExtension = HasExtension || LengthAndWidth != 0;
        buffer[0] = (byte)((Poa ? 0x20 : 0) | (CdtiS ? 0x10 : 0) | (B2Low ? 0x08 : 0) | (Ras ? 0x04 : 0) |
                           (Ident ? 0x02 : 0) | (hasExtension ? 0x01 : 0));
        buffer = buffer[1..];
        if (hasExtension)
        {
            buffer[0] = (byte)(LengthAndWidth & 0x0F);
            buffer = buffer[1..];
        }
    }

    public override int GetByteSize() => 1 + (HasExtension || LengthAndWidth != 0 ? 1 : 0);
}