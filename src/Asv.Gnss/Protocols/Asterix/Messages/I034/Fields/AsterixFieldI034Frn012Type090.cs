using System;

namespace Asv.Gnss;

/// <summary>
/// I034/090 Collimation Error.
/// </summary>
public sealed class AsterixFieldI034Frn012Type090 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 12;

    /// <inheritdoc />
    public override string Name => "Collimation Error";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Range error in nautical miles.
    /// </summary>
    public double RangeErrorNm { get; set; }

    /// <summary>
    /// Azimuth error in degrees.
    /// </summary>
    public double AzimuthErrorDeg { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        RangeErrorNm = unchecked((sbyte)buffer[0]) / 128.0;
        AzimuthErrorDeg = unchecked((sbyte)buffer[1]) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = unchecked((byte)(sbyte)Math.Round(RangeErrorNm * 128.0));
        buffer[1] = unchecked((byte)(sbyte)Math.Round(AzimuthErrorDeg / (360.0 / 65536.0)));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}