using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/202 Calculated Track Velocity in Cartesian Coordinates.
/// </summary>
public class AsterixFieldI010Frn009Type202 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 9;

    /// <inheritdoc />
    public override string Name => "Calculated Track Velocity in Cartesian Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Enables the three-octet 12-bit component encoding used by old sensis CAT010 samples.
    /// </summary>
    public bool IsSensisEncoding { get; set; }

    /// <summary>
    /// X velocity component.
    /// </summary>
    public double Vx { get; set; }

    /// <summary>
    /// Y velocity component.
    /// </summary>
    public double Vy { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        if (IsSensisEncoding)
        {
            var first = (buffer[0] << 4) | (buffer[1] >> 4);
            var second = ((buffer[1] & 0x0F) << 8) | buffer[2];
            Vx = AsterixI010Binary.SignExtend(first, 12);
            Vy = AsterixI010Binary.SignExtend(second, 12);
            buffer = buffer[3..];
            return;
        }

        Vx = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
        Vy = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        if (IsSensisEncoding)
        {
            var vx = ((int)Math.Round(Vx)) & 0x0FFF;
            var vy = ((int)Math.Round(Vy)) & 0x0FFF;
            buffer[0] = (byte)(vx >> 4);
            buffer[1] = (byte)((vx << 4) | (vy >> 8));
            buffer[2] = (byte)vy;
            buffer = buffer[3..];
            return;
        }

        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(Vx / 0.25));
        buffer = buffer[2..];
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(Vy / 0.25));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => IsSensisEncoding ? 3 : 4;
}