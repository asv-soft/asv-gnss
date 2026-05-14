using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn029Type160 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 26;
    public override string Name => "Airborne Ground Vector";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool RangeExceeded { get; set; }
    public double GroundSpeed { get; set; }
    public double TrackAngleDeg { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        RangeExceeded = (raw & 0x8000) != 0;
        GroundSpeed = (raw & 0x7FFF) / 16384.0;
        TrackAngleDeg = BinaryPrimitives.ReadUInt16BigEndian(buffer) * (360.0 / 65536.0);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var speed = (ushort)Math.Round(GroundSpeed * 16384.0);
        if (RangeExceeded) speed |= 0x8000;
        BinaryPrimitives.WriteUInt16BigEndian(buffer, speed);
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(TrackAngleDeg / (360.0 / 65536.0)));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 4;
}