using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

internal static class AsterixI010Binary
{
    public static int ReadInt24(ref ReadOnlySpan<byte> buffer)
    {
        var value = (buffer[0] << 16) | (buffer[1] << 8) | buffer[2];
        buffer = buffer[3..];
        return SignExtend(value, 24);
    }

    public static int ReadUInt24(ref ReadOnlySpan<byte> buffer)
    {
        var value = (buffer[0] << 16) | (buffer[1] << 8) | buffer[2];
        buffer = buffer[3..];
        return value;
    }

    public static void WriteUInt24(ref Span<byte> buffer, int value)
    {
        buffer[0] = (byte)(value >> 16);
        buffer[1] = (byte)(value >> 8);
        buffer[2] = (byte)value;
        buffer = buffer[3..];
    }

    public static int SignExtend(int value, int bitCount)
    {
        var shift = 32 - bitCount;
        return (value << shift) >> shift;
    }
}

/// <summary>
/// Base class for ASTERIX CAT010 fields.
/// </summary>
public abstract class AsterixFieldI010 : AsterixField
{
    /// <inheritdoc />
    public override int Category => AsterixMessageI010.Category;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}

/// <summary>
/// I010/010 Data Source Identifier.
/// </summary>
public class AsterixFieldI010Frn001Type010 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 1;

    /// <inheritdoc />
    public override string Name => "Data Source Identifier";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// System Area Code.
    /// </summary>
    public SystemAreaCode Sac { get; set; }

    /// <summary>
    /// System Identification Code.
    /// </summary>
    public byte Sic { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Sac = (SystemAreaCode)buffer[0];
        Sic = buffer[1];
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Sac;
        buffer[1] = Sic;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}

/// <summary>
/// I010/000 Message Type.
/// </summary>
public class AsterixFieldI010Frn002Type000 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 2;

    /// <inheritdoc />
    public override string Name => "Message Type";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// CAT010 message type value.
    /// </summary>
    public byte MessageType { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        MessageType = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = MessageType;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}

/// <summary>
/// I010/020 Target Report Descriptor.
/// </summary>
public class AsterixFieldI010Frn003Type020 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 3;

    /// <inheritdoc />
    public override string Name => "Target Report Descriptor";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Type of detection.
    /// </summary>
    public byte Typ { get; set; }

    /// <summary>
    /// Differential correction indicator.
    /// </summary>
    public bool Dcr { get; set; }

    /// <summary>
    /// Chain indicator.
    /// </summary>
    public bool Chn { get; set; }

    /// <summary>
    /// Ground bit set indicator.
    /// </summary>
    public bool Gbs { get; set; }

    /// <summary>
    /// Corrupted reply in multilateration indicator.
    /// </summary>
    public bool Crt { get; set; }

    /// <summary>
    /// Simulation report indicator.
    /// </summary>
    public bool Sim { get; set; }

    /// <summary>
    /// Test target indicator.
    /// </summary>
    public bool Tst { get; set; }

    /// <summary>
    /// Report from field monitor indicator.
    /// </summary>
    public bool Rab { get; set; }

    /// <summary>
    /// Loop status.
    /// </summary>
    public byte Lop { get; set; }

    /// <summary>
    /// Type of target.
    /// </summary>
    public byte Tot { get; set; }

    /// <summary>
    /// Special position identification indicator.
    /// </summary>
    public bool Spi { get; set; }

    /// <summary>
    /// Indicates that the first extension octet was present.
    /// </summary>
    public bool HasFirstExtension { get; set; }

    /// <summary>
    /// Indicates that the second extension octet was present.
    /// </summary>
    public bool HasSecondExtension { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var first = buffer[0];
        buffer = buffer[1..];

        Typ = (byte)(first >> 5);
        Dcr = (first & 0x10) != 0;
        Chn = (first & 0x08) != 0;
        Gbs = (first & 0x04) != 0;
        Crt = (first & 0x02) != 0;
        HasFirstExtension = (first & 0x01) != 0;

        if (!HasFirstExtension) return;

        var second = buffer[0];
        buffer = buffer[1..];
        Sim = (second & 0x80) != 0;
        Tst = (second & 0x40) != 0;
        Rab = (second & 0x20) != 0;
        Lop = (byte)((second >> 3) & 0x03);
        Tot = (byte)((second >> 1) & 0x03);
        HasSecondExtension = (second & 0x01) != 0;

        if (!HasSecondExtension) return;

        var third = buffer[0];
        buffer = buffer[1..];
        Spi = (third & 0x80) != 0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var hasSecond = HasSecondExtension || Spi;
        var hasFirst = HasFirstExtension || hasSecond || Sim || Tst || Rab || Lop != 0 || Tot != 0;
        buffer[0] = (byte)((Typ << 5) |
                           (Dcr ? 0x10 : 0) |
                           (Chn ? 0x08 : 0) |
                           (Gbs ? 0x04 : 0) |
                           (Crt ? 0x02 : 0) |
                           (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];

        if (!hasFirst) return;

        buffer[0] = (byte)((Sim ? 0x80 : 0) |
                           (Tst ? 0x40 : 0) |
                           (Rab ? 0x20 : 0) |
                           ((Lop & 0x03) << 3) |
                           ((Tot & 0x03) << 1) |
                           (hasSecond ? 0x01 : 0));
        buffer = buffer[1..];

        if (!hasSecond) return;

        buffer[0] = (byte)(Spi ? 0x80 : 0);
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (HasFirstExtension || HasSecondExtension || Sim || Tst || Rab || Lop != 0 || Tot != 0 || Spi ? 1 : 0) + (HasSecondExtension || Spi ? 1 : 0);
}

/// <summary>
/// I010/140 Time of Day.
/// </summary>
public class AsterixFieldI010Frn004Type140 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 4;

    /// <inheritdoc />
    public override string Name => "Time of Day";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Time of day in seconds since midnight UTC.
    /// </summary>
    public double Seconds { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Seconds = AsterixI010Binary.ReadUInt24(ref buffer) / 128.0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI010Binary.WriteUInt24(ref buffer, (int)Math.Round(Seconds * 128.0));
    }

    /// <inheritdoc />
    public override int GetByteSize() => 3;
}

/// <summary>
/// I010/041 Position in WGS-84 Coordinates.
/// </summary>
public class AsterixFieldI010Frn005Type041 : AsterixFieldI010
{
    private const double Scale = 180.0 / 2147483648.0;

    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 5;

    /// <inheritdoc />
    public override string Name => "Position in WGS-84 Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Latitude in degrees.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in degrees.
    /// </summary>
    public double Longitude { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Latitude = BinaryPrimitives.ReadInt32BigEndian(buffer) * Scale;
        buffer = buffer[4..];
        Longitude = BinaryPrimitives.ReadInt32BigEndian(buffer) * Scale;
        buffer = buffer[4..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)Math.Round(Latitude / Scale));
        buffer = buffer[4..];
        BinaryPrimitives.WriteInt32BigEndian(buffer, (int)Math.Round(Longitude / Scale));
        buffer = buffer[4..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 8;
}

/// <summary>
/// I010/040 Measured Position in Polar Coordinates.
/// </summary>
public class AsterixFieldI010Frn006Type040 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 6;

    /// <inheritdoc />
    public override string Name => "Measured Position in Polar Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Range in metres.
    /// </summary>
    public double Rho { get; set; }

    /// <summary>
    /// Azimuth in degrees.
    /// </summary>
    public double Theta { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Rho = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        Theta = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(Rho));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(Theta * 65536.0 / 360.0));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}

/// <summary>
/// I010/042 Position in Cartesian Coordinates.
/// </summary>
public class AsterixFieldI010Frn007Type042 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 7;

    /// <inheritdoc />
    public override string Name => "Position in Cartesian Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// X position in metres.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Y position in metres.
    /// </summary>
    public double Y { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        X = BinaryPrimitives.ReadInt16BigEndian(buffer);
        buffer = buffer[2..];
        Y = BinaryPrimitives.ReadInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(X));
        buffer = buffer[2..];
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(Y));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}

/// <summary>
/// I010/200 Calculated Track Velocity in Polar Coordinates.
/// </summary>
public class AsterixFieldI010Frn008Type200 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 8;

    /// <inheritdoc />
    public override string Name => "Calculated Track Velocity in Polar Coordinates";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Ground speed in nautical miles per second.
    /// </summary>
    public double GroundSpeed { get; set; }

    /// <summary>
    /// Track angle in degrees.
    /// </summary>
    public double TrackAngle { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        GroundSpeed = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 16384.0;
        buffer = buffer[2..];
        TrackAngle = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(GroundSpeed * 16384.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(TrackAngle * 65536.0 / 360.0));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 4;
}

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

/// <summary>
/// I010/161 Track Number.
/// </summary>
public class AsterixFieldI010Frn010Type161 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 10;

    /// <inheritdoc />
    public override string Name => "Track Number";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Track number.
    /// </summary>
    public ushort TrackNumber { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TrackNumber = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer) & 0x0FFF);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(TrackNumber & 0x0FFF));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}

/// <summary>
/// I010/170 Track Status.
/// </summary>
public class AsterixFieldI010Frn011Type170 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 11;

    /// <inheritdoc />
    public override string Name => "Track Status";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Confirmed track indicator.
    /// </summary>
    public bool Cnf { get; set; }

    /// <summary>
    /// Last report for a track indicator.
    /// </summary>
    public bool Tre { get; set; }

    /// <summary>
    /// Track type or correlation status.
    /// </summary>
    public bool Cst { get; set; }

    /// <summary>
    /// Manoeuvre horizontal sense indicator.
    /// </summary>
    public bool Mah { get; set; }

    /// <summary>
    /// Tracking coordination status.
    /// </summary>
    public bool Tcc { get; set; }

    /// <summary>
    /// Smoothed track indicator.
    /// </summary>
    public bool Sth { get; set; }

    /// <summary>
    /// Type of movement.
    /// </summary>
    public byte Tom { get; set; }

    /// <summary>
    /// Doubtful track indicator.
    /// </summary>
    public bool Dou { get; set; }

    /// <summary>
    /// Merge or split status.
    /// </summary>
    public byte Mrs { get; set; }

    /// <summary>
    /// Ghost target indicator.
    /// </summary>
    public bool Gho { get; set; }

    /// <summary>
    /// Indicates that the first extension octet was present.
    /// </summary>
    public bool HasFirstExtension { get; set; }

    /// <summary>
    /// Indicates that the second extension octet was present.
    /// </summary>
    public bool HasSecondExtension { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var first = buffer[0];
        buffer = buffer[1..];
        Cnf = (first & 0x80) != 0;
        Tre = (first & 0x40) != 0;
        Cst = (first & 0x20) != 0;
        Mah = (first & 0x10) != 0;
        Tcc = (first & 0x08) != 0;
        Sth = (first & 0x02) != 0;
        HasFirstExtension = (first & 0x01) != 0;

        if (!HasFirstExtension) return;

        var second = buffer[0];
        buffer = buffer[1..];
        Tom = (byte)(second >> 6);
        Dou = (second & 0x20) != 0;
        Mrs = (byte)((second >> 1) & 0x0F);
        HasSecondExtension = (second & 0x01) != 0;

        if (!HasSecondExtension) return;

        var third = buffer[0];
        buffer = buffer[1..];
        Gho = (third & 0x80) != 0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var hasSecond = HasSecondExtension || Gho;
        var hasFirst = HasFirstExtension || hasSecond || Tom != 0 || Dou || Mrs != 0;
        buffer[0] = (byte)((Cnf ? 0x80 : 0) |
                           (Tre ? 0x40 : 0) |
                           (Cst ? 0x20 : 0) |
                           (Mah ? 0x10 : 0) |
                           (Tcc ? 0x08 : 0) |
                           (Sth ? 0x02 : 0) |
                           (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];

        if (!hasFirst) return;

        buffer[0] = (byte)(((Tom & 0x03) << 6) |
                           (Dou ? 0x20 : 0) |
                           ((Mrs & 0x0F) << 1) |
                           (hasSecond ? 0x01 : 0));
        buffer = buffer[1..];

        if (!hasSecond) return;

        buffer[0] = (byte)(Gho ? 0x80 : 0);
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (HasFirstExtension || HasSecondExtension || Tom != 0 || Dou || Mrs != 0 || Gho ? 1 : 0) + (HasSecondExtension || Gho ? 1 : 0);
}

/// <summary>
/// I010/060 Mode-3/A Code in Octal Representation.
/// </summary>
public class AsterixFieldI010Frn012Type060 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 12;

    /// <inheritdoc />
    public override string Name => "Mode-3/A Code in Octal Representation";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Mode-3/A code validation flag.
    /// </summary>
    public bool V { get; set; }

    /// <summary>
    /// Garbled code flag.
    /// </summary>
    public bool G { get; set; }

    /// <summary>
    /// Last reply flag.
    /// </summary>
    public bool L { get; set; }

    /// <summary>
    /// Squawk code in octal representation.
    /// </summary>
    public ushort Squawk { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var value = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        V = (value & 0x8000) != 0;
        G = (value & 0x4000) != 0;
        L = (value & 0x2000) != 0;
        Squawk = AsterixProtocol.GetSquawk((ushort)(value & 0x1FFF));
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var value = AsterixProtocol.SetSquawk(Squawk);
        if (V) value |= 0x8000;
        if (G) value |= 0x4000;
        if (L) value |= 0x2000;
        BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}

/// <summary>
/// I010/220 Target Address.
/// </summary>
public class AsterixFieldI010Frn013Type220 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 13;

    /// <inheritdoc />
    public override string Name => "Target Address";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// 24-bit ICAO target address.
    /// </summary>
    public int TargetAddress { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TargetAddress = AsterixI010Binary.ReadUInt24(ref buffer);
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI010Binary.WriteUInt24(ref buffer, TargetAddress);
    }

    /// <inheritdoc />
    public override int GetByteSize() => 3;
}

/// <summary>
/// I010/245 Target Identification.
/// </summary>
public class AsterixFieldI010Frn014Type245 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 14;

    /// <inheritdoc />
    public override string Name => "Target Identification";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Source and type identifier.
    /// </summary>
    public StiEnum Sti { get; set; }

    /// <summary>
    /// Aircraft identification or registration.
    /// </summary>
    public string? TargetIdentification { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Sti = (StiEnum)(buffer[0] >> 6);
        TargetIdentification = AsterixProtocol.GetAircraftId(buffer.Slice(1, 6));
        buffer = buffer[7..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((byte)Sti << 6);
        buffer = buffer[1..];
        AsterixProtocol.SetAircraftId(TargetIdentification, ref buffer);
        buffer = buffer[6..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 7;
}

/// <summary>
/// Mode S MB data block used by I010/250.
/// </summary>
public readonly record struct AsterixI010ModeSMbData(byte[] Data);

/// <summary>
/// I010/250 Mode S MB Data.
/// </summary>
public class AsterixFieldI010Frn015Type250 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 15;

    /// <inheritdoc />
    public override string Name => "Mode S MB Data";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Repetitive Mode S MB blocks.
    /// </summary>
    public List<AsterixI010ModeSMbData> Items { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            Items.Add(new AsterixI010ModeSMbData(buffer[..8].ToArray()));
            buffer = buffer[8..];
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Items.Count;
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            item.Data.CopyTo(buffer);
            buffer = buffer[8..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Items.Count * 8;
}

/// <summary>
/// I010/300 Vehicle Fleet Identification.
/// </summary>
public class AsterixFieldI010Frn016Type300 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 16;

    /// <inheritdoc />
    public override string Name => "Vehicle Fleet Identification";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Vehicle fleet identification code.
    /// </summary>
    public byte VehicleFleetIdentification { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        VehicleFleetIdentification = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = VehicleFleetIdentification;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}

/// <summary>
/// I010/090 Flight Level in Binary Representation.
/// </summary>
public class AsterixFieldI010Frn017Type090 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 17;

    /// <inheritdoc />
    public override string Name => "Flight Level in Binary Representation";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Validation flag.
    /// </summary>
    public bool V { get; set; }

    /// <summary>
    /// Garbled flag.
    /// </summary>
    public bool G { get; set; }

    /// <summary>
    /// Flight level in FL units.
    /// </summary>
    public double FlightLevel { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var value = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        V = (value & 0x8000) != 0;
        G = (value & 0x4000) != 0;
        FlightLevel = AsterixI010Binary.SignExtend(value & 0x3FFF, 14) * 0.25;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var raw = ((int)Math.Round(FlightLevel / 0.25)) & 0x3FFF;
        if (V) raw |= 0x8000;
        if (G) raw |= 0x4000;
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)raw);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}

/// <summary>
/// I010/091 Measured Height.
/// </summary>
public class AsterixFieldI010Frn018Type091 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 18;

    /// <inheritdoc />
    public override string Name => "Measured Height";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Measured height in flight level units.
    /// </summary>
    public double MeasuredHeight { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        MeasuredHeight = BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(MeasuredHeight / 0.25));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}

/// <summary>
/// I010/270 Target Size and Orientation.
/// </summary>
public class AsterixFieldI010Frn019Type270 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 19;

    /// <inheritdoc />
    public override string Name => "Target Size and Orientation";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Target length in metres.
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Target orientation in degrees.
    /// </summary>
    public double Orientation { get; set; }

    /// <summary>
    /// Target width in metres.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Indicates that orientation octet is present.
    /// </summary>
    public bool HasOrientation { get; set; }

    /// <summary>
    /// Indicates that width octet is present.
    /// </summary>
    public bool HasWidth { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var first = buffer[0];
        buffer = buffer[1..];
        Length = first >> 1;
        HasOrientation = (first & 0x01) != 0;
        if (!HasOrientation) return;

        var second = buffer[0];
        buffer = buffer[1..];
        Orientation = (second >> 1) * 360.0 / 128.0;
        HasWidth = (second & 0x01) != 0;
        if (!HasWidth) return;

        var third = buffer[0];
        buffer = buffer[1..];
        Width = third >> 1;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(((int)Math.Round(Length) << 1) | (HasOrientation ? 1 : 0));
        buffer = buffer[1..];
        if (!HasOrientation) return;

        buffer[0] = (byte)(((int)Math.Round(Orientation * 128.0 / 360.0) << 1) | (HasWidth ? 1 : 0));
        buffer = buffer[1..];
        if (!HasWidth) return;

        buffer[0] = (byte)((int)Math.Round(Width) << 1);
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + (HasOrientation ? 1 : 0) + (HasWidth ? 1 : 0);
}

/// <summary>
/// I010/550 System Status.
/// </summary>
public class AsterixFieldI010Frn020Type550 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 20;

    /// <inheritdoc />
    public override string Name => "System Status";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Raw system status octet.
    /// </summary>
    public byte RawStatus { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        RawStatus = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = RawStatus;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}

/// <summary>
/// I010/310 Pre-programmed Message.
/// </summary>
public class AsterixFieldI010Frn021Type310 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 21;

    /// <inheritdoc />
    public override string Name => "Pre-programmed Message";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Trouble or emergency indicator.
    /// </summary>
    public bool Trb { get; set; }

    /// <summary>
    /// Message identifier.
    /// </summary>
    public byte Message { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Trb = (buffer[0] & 0x80) != 0;
        Message = (byte)(buffer[0] & 0x7F);
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((Trb ? 0x80 : 0) | (Message & 0x7F));
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}

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

/// <summary>
/// Presence item used by I010/280.
/// </summary>
public readonly record struct AsterixI010PresenceItem(sbyte Drho, sbyte Dtheta);

/// <summary>
/// I010/280 Presence.
/// </summary>
public class AsterixFieldI010Frn023Type280 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 23;

    /// <inheritdoc />
    public override string Name => "Presence";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Presence cells.
    /// </summary>
    public List<AsterixI010PresenceItem> Items { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            Items.Add(new AsterixI010PresenceItem((sbyte)buffer[0], (sbyte)buffer[1]));
            buffer = buffer[2..];
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Items.Count;
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            buffer[0] = (byte)item.Drho;
            buffer[1] = (byte)item.Dtheta;
            buffer = buffer[2..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Items.Count * 2;
}

/// <summary>
/// I010/131 Amplitude of Primary Plot.
/// </summary>
public class AsterixFieldI010Frn024Type131 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 24;

    /// <inheritdoc />
    public override string Name => "Amplitude of Primary Plot";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Primary plot amplitude.
    /// </summary>
    public sbyte Amplitude { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Amplitude = (sbyte)buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Amplitude;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}

/// <summary>
/// I010/210 Calculated Acceleration.
/// </summary>
public class AsterixFieldI010Frn025Type210 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 25;

    /// <inheritdoc />
    public override string Name => "Calculated Acceleration";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// X acceleration component.
    /// </summary>
    public double Ax { get; set; }

    /// <summary>
    /// Y acceleration component.
    /// </summary>
    public double Ay { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Ax = (sbyte)buffer[0] * 0.25;
        Ay = (sbyte)buffer[1] * 0.25;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(sbyte)Math.Round(Ax / 0.25);
        buffer[1] = (byte)(sbyte)Math.Round(Ay / 0.25);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}

/// <summary>
/// I010/SP Special Purpose Field.
/// </summary>
public class AsterixFieldI010Frn026TypeSp : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 26;

    /// <inheritdoc />
    public override string Name => "Special Purpose Field";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Raw special purpose payload without the length octet.
    /// </summary>
    public byte[] Data { get; set; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var length = buffer[0];
        Data = buffer.Slice(1, length - 1).ToArray();
        buffer = buffer[length..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(Data.Length + 1);
        Data.CopyTo(buffer[1..]);
        buffer = buffer[(Data.Length + 1)..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Data.Length;
}

/// <summary>
/// I010/RE Reserved Expansion Field.
/// </summary>
public class AsterixFieldI010Frn027TypeRe : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 27;

    /// <inheritdoc />
    public override string Name => "Reserved Expansion Field";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Raw reserved expansion payload without the length octet.
    /// </summary>
    public byte[] Data { get; set; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var length = buffer[0];
        Data = buffer.Slice(1, length - 1).ToArray();
        buffer = buffer[length..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(Data.Length + 1);
        Data.CopyTo(buffer[1..]);
        buffer = buffer[(Data.Length + 1)..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Data.Length;
}
