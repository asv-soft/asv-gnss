using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI002Frn010Type090 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 10;
    public override string Name => "Collimation Error";
    public override byte FieldReferenceNumber => StaticFrn;
    public double RangeErrorNm { get; set; }
    public double AzimuthErrorDeg { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        RangeErrorNm = unchecked((sbyte)buffer[0]) / 128.0;
        AzimuthErrorDeg = unchecked((sbyte)buffer[1]) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = unchecked((byte)(sbyte)Math.Round(RangeErrorNm * 128.0));
        buffer[1] = unchecked((byte)(sbyte)Math.Round(AzimuthErrorDeg / (360.0 / 65536.0)));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}