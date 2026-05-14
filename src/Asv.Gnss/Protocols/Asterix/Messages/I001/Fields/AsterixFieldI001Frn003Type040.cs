using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT001 field I001/040: Measured Position in Polar Coordinates.
/// Contains range and azimuth measured by the radar sensor.
/// </summary>
public class AsterixFieldI001Frn003Type040 : AsterixField
{
    /// <summary>
    /// Field reference number for I001/040.
    /// </summary>
    public const byte StaticFrn = 3;

    /// <summary>
    /// Human-readable field name.
    /// </summary>
    public const string StaticName = "Measured Position in Polar Coordinates";

    /// <inheritdoc />
    public override string Name => StaticName;

    /// <inheritdoc />
    public override int Category => AsterixMessageI001.Category;

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    private ushort _rhoRaw;
    private ushort _thetaRaw;

    /// <summary>
    /// Gets or sets the measured range in nautical miles.
    /// </summary>
    public double Rho
    {
        get => _rhoRaw / 128.0;
        set => _rhoRaw = checked((ushort)Math.Round(value * 128.0));
    }

    /// <summary>
    /// Gets or sets the measured azimuth in degrees.
    /// </summary>
    public double Theta
    {
        get => _thetaRaw * 360.0 / 65536.0;
        set => _thetaRaw = checked((ushort)Math.Round(value * 65536.0 / 360.0));
    }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rhoRaw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        _thetaRaw = BinaryPrimitives.ReadUInt16BigEndian(buffer[2..]);
        buffer = buffer[4..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, _rhoRaw);
        BinaryPrimitives.WriteUInt16BigEndian(buffer[2..], _thetaRaw);
        buffer = buffer[4..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}
