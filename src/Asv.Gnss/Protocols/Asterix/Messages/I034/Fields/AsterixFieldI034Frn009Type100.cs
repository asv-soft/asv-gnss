using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I034/100 Generic Polar Window.
/// </summary>
public sealed class AsterixFieldI034Frn009Type100 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 9;

    /// <inheritdoc />
    public override string Name => "Generic Polar Window";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Start range in nautical miles.
    /// </summary>
    public double RhoStartNm { get; set; }

    /// <summary>
    /// End range in nautical miles.
    /// </summary>
    public double RhoEndNm { get; set; }

    /// <summary>
    /// Start azimuth in degrees.
    /// </summary>
    public double ThetaStartDeg { get; set; }

    /// <summary>
    /// End azimuth in degrees.
    /// </summary>
    public double ThetaEndDeg { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        RhoStartNm = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 128.0;
        buffer = buffer[2..];
        RhoEndNm = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 128.0;
        buffer = buffer[2..];
        ThetaStartDeg = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
        ThetaEndDeg = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(RhoStartNm * 128.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(RhoEndNm * 128.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(ThetaStartDeg * 65536.0 / 360.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(ThetaEndDeg * 65536.0 / 360.0));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 8;
}