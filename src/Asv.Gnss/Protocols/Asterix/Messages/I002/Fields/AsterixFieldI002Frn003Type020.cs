using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI002Frn003Type020 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 3;
    public override string Name => "Sector Number";
    public override byte FieldReferenceNumber => StaticFrn;
    public double SectorNumberDeg { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        SectorNumberDeg = buffer[0] * 360.0 / 256.0;
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Math.Round(SectorNumberDeg / (360.0 / 256.0));
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}