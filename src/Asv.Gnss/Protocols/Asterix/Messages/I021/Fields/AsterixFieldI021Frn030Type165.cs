using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn030Type165 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 27;
    public override string Name => "Track Angle Rate";
    public override byte FieldReferenceNumber => StaticFrn;
    public double TrackAngleRateDegPerSecond { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TrackAngleRateDegPerSecond = BinaryPrimitives.ReadInt16BigEndian(buffer) * (1.0 / 32.0);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(TrackAngleRateDegPerSecond * 32.0));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}