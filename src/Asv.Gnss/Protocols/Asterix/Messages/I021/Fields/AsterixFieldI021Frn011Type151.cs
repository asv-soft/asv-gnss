using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn011Type151 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 10;
    public override string Name => "True Airspeed";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool RangeExceeded { get; set; }
    public ushort TrueAirSpeed { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        RangeExceeded = (raw & 0x8000) != 0;
        TrueAirSpeed = (ushort)(raw & 0x7FFF);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)((RangeExceeded ? 0x8000 : 0) | (TrueAirSpeed & 0x7FFF)));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}