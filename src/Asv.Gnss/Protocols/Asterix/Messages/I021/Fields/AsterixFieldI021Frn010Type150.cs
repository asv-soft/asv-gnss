using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn010Type150 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 9;
    public override string Name => "Air Speed";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool IsMach { get; set; }
    public double Speed { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        IsMach = (raw & 0x8000) != 0;
        Speed = (raw & 0x7FFF) * (IsMach ? 0.001 : 1.0 / 16384.0);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var raw = (ushort)Math.Round(Speed / (IsMach ? 0.001 : 1.0 / 16384.0));
        if (IsMach) raw |= 0x8000;
        BinaryPrimitives.WriteUInt16BigEndian(buffer, raw);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}