using System;

namespace Asv.Gnss;

public sealed class AsterixI021ModeSData
{
    public const int ByteSize = 8;
    public byte[] MbData { get; } = new byte[7];
    public byte Bds1 { get; set; }
    public byte Bds2 { get; set; }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer[..7].CopyTo(MbData);
        var address = buffer[7];
        Bds1 = (byte)((address >> 4) & 0x0F);
        Bds2 = (byte)(address & 0x0F);
        buffer = buffer[8..];
    }

    public void Serialize(ref Span<byte> buffer)
    {
        MbData.CopyTo(buffer);
        buffer[7] = (byte)((Bds1 << 4) | (Bds2 & 0x0F));
        buffer = buffer[8..];
    }
}