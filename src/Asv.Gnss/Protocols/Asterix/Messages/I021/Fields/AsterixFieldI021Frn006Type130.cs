using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn006Type130 : AsterixFieldI021Fixed
{
    private const double Lsb = 180.0 / 8388608.0;
    public const byte StaticFrn = 6;
    public override string Name => "Position in WGS-84 Co-ordinates";
    public override byte FieldReferenceNumber => StaticFrn;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Latitude = AsterixI021Binary.ReadInt24(ref buffer) * Lsb;
        Longitude = AsterixI021Binary.ReadInt24(ref buffer) * Lsb;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI021Binary.WriteInt24(ref buffer, (int)Math.Round(Latitude / Lsb));
        AsterixI021Binary.WriteInt24(ref buffer, (int)Math.Round(Longitude / Lsb));
    }

    public override int GetByteSize() => 6;
}