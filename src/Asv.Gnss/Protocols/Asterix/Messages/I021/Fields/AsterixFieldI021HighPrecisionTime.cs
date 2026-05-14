using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public abstract class AsterixFieldI021HighPrecisionTime : AsterixFieldI021Fixed
{
    private const double Lsb = 1.0 / 1073741824.0;
    public byte FullSecondIndication { get; set; }
    public double FractionalSeconds { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt32BigEndian(buffer);
        buffer = buffer[4..];
        FullSecondIndication = (byte)((raw >> 30) & 0x03);
        FractionalSeconds = (raw & 0x3FFFFFFF) * Lsb;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var raw = ((uint)(FullSecondIndication & 0x03) << 30) | ((uint)Math.Round(FractionalSeconds / Lsb) & 0x3FFFFFFF);
        BinaryPrimitives.WriteUInt32BigEndian(buffer, raw);
        buffer = buffer[4..];
    }

    public override int GetByteSize() => 4;
}