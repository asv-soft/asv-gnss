using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public abstract class AsterixFieldI021SelectedAltitude : AsterixFieldI021Fixed
{
    public bool SourceAvailability { get; set; }
    public byte Source { get; set; }
    public double AltitudeFt { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        SourceAvailability = (raw & 0x8000) != 0;
        Source = (byte)((raw >> 13) & 0x03);
        AltitudeFt = AsterixI021Binary.SignExtend(raw & 0x1FFF, 13) * 25.0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var value = ((SourceAvailability ? 0x8000 : 0) | ((Source & 0x03) << 13) |
                     ((int)Math.Round(AltitudeFt / 25.0) & 0x1FFF));
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)value);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}