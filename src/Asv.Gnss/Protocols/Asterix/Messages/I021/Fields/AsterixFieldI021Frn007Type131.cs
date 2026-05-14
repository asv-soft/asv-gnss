using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn007Type131 : AsterixFieldI021Fixed
{
    private const double Lsb = 180.0 / 1073741824.0;
    public const byte StaticFrn = 7;
    public override string Name => "High-Resolution Position in WGS-84 Co-ordinates";
    public override byte FieldReferenceNumber => StaticFrn;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Latitude = BinaryPrimitives.ReadInt32BigEndian(buffer) * Lsb;
        buffer = buffer[4..];
        Longitude = BinaryPrimitives.ReadInt32BigEndian(buffer) * Lsb;
        buffer = buffer[4..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)Math.Round(Latitude / Lsb));
        buffer = buffer[4..];
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)Math.Round(Longitude / Lsb));
        buffer = buffer[4..];
    }

    public override int GetByteSize() => 8;
}