using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn039Type016 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 35;
    public override string Name => "Service Management";
    public override byte FieldReferenceNumber => StaticFrn;
    public double ReportPeriod { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        ReportPeriod = buffer[0] * 0.5;
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Math.Round(ReportPeriod / 0.5);
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}