using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/500 Standard Deviation of Position.
/// </summary>
public class AsterixFieldI010Frn022Type500 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 22;

    /// <inheritdoc />
    public override string Name => "Standard Deviation of Position";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Standard deviation of X component in metres.
    /// </summary>
    public double SdpX { get; set; }

    /// <summary>
    /// Standard deviation of Y component in metres.
    /// </summary>
    public double SdpY { get; set; }

    /// <summary>
    /// Covariance component in square metres.
    /// </summary>
    public double SdpXy { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        SdpX = buffer[0] * 0.25;
        SdpY = buffer[1] * 0.25;
        buffer = buffer[2..];
        SdpXy = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Math.Round(SdpX / 0.25);
        buffer[1] = (byte)Math.Round(SdpY / 0.25);
        buffer = buffer[2..];
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(SdpXy / 0.25));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}