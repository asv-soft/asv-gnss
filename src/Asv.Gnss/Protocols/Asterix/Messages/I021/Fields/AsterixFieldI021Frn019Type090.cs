using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn019Type090 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 17;
    public override string Name => "Quality Indicators";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte NucrOrNacv { get; set; }
    public byte NucpOrNic { get; set; }
    public bool NicBaro { get; set; }
    public byte Sil { get; set; }
    public byte Nacp { get; set; }
    public bool SilSupplement { get; set; }
    public byte Sda { get; set; }
    public byte Gva { get; set; }
    public byte Pic { get; set; }
    public bool HasFirstExtension { get; set; }
    public bool HasSecondExtension { get; set; }
    public bool HasThirdExtension { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        buffer = buffer[1..];
        NucrOrNacv = (byte)((b >> 5) & 0x07);
        NucpOrNic = (byte)((b >> 1) & 0x0F);
        HasFirstExtension = (b & 0x01) != 0;
        if (!HasFirstExtension) return;

        b = buffer[0];
        buffer = buffer[1..];
        NicBaro = (b & 0x80) != 0;
        Sil = (byte)((b >> 5) & 0x03);
        Nacp = (byte)((b >> 1) & 0x0F);
        HasSecondExtension = (b & 0x01) != 0;
        if (!HasSecondExtension) return;

        b = buffer[0];
        buffer = buffer[1..];
        SilSupplement = (b & 0x20) != 0;
        Sda = (byte)((b >> 3) & 0x03);
        Gva = (byte)((b >> 1) & 0x03);
        HasThirdExtension = (b & 0x01) != 0;
        if (!HasThirdExtension) return;

        b = buffer[0];
        buffer = buffer[1..];
        Pic = (byte)((b >> 4) & 0x0F);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var hasThird = HasThirdExtension || Pic != 0;
        var hasSecond = HasSecondExtension || hasThird || SilSupplement || Sda != 0 || Gva != 0;
        var hasFirst = HasFirstExtension || hasSecond || NicBaro || Sil != 0 || Nacp != 0;
        buffer[0] = (byte)((NucrOrNacv << 5) | ((NucpOrNic & 0x0F) << 1) | (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];
        if (!hasFirst) return;

        buffer[0] = (byte)((NicBaro ? 0x80 : 0) | ((Sil & 0x03) << 5) | ((Nacp & 0x0F) << 1) | (hasSecond ? 0x01 : 0));
        buffer = buffer[1..];
        if (!hasSecond) return;

        buffer[0] = (byte)((SilSupplement ? 0x20 : 0) | ((Sda & 0x03) << 3) | ((Gva & 0x03) << 1) | (hasThird ? 0x01 : 0));
        buffer = buffer[1..];
        if (!hasThird) return;

        buffer[0] = (byte)((Pic & 0x0F) << 4);
        buffer = buffer[1..];
    }

    public override int GetByteSize()
    {
        var hasThird = HasThirdExtension || Pic != 0;
        var hasSecond = HasSecondExtension || hasThird || SilSupplement || Sda != 0 || Gva != 0;
        var hasFirst = HasFirstExtension || hasSecond || NicBaro || Sil != 0 || Nacp != 0;
        return 1 + (hasFirst ? 1 : 0) + (hasSecond ? 1 : 0) + (hasThird ? 1 : 0);
    }
}