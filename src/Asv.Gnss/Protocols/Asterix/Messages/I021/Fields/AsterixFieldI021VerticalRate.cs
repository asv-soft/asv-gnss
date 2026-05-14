using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public abstract class AsterixFieldI021VerticalRate : AsterixFieldI021Fixed
{
    public bool RangeExceeded { get; set; }
    public double RateFtPerMinute { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        RangeExceeded = (raw & 0x8000) != 0;
        RateFtPerMinute = AsterixI021Binary.SignExtend(raw & 0x7FFF, 15) * 6.25;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var value = (int)Math.Round(RateFtPerMinute / 6.25) & 0x7FFF;
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)((RangeExceeded ? 0x8000 : 0) | value));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}