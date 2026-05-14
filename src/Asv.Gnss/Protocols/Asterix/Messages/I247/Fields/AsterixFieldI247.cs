using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

internal static class AsterixI247Binary
{
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
}

/// <summary>
/// Base class for ASTERIX CAT247 fields.
/// </summary>
public abstract class AsterixFieldI247 : AsterixField
{
    /// <inheritdoc />
    public override int Category => AsterixMessageI247.Category;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}

/// <summary>
/// I247/010 Data Source Identifier.
/// </summary>
public class AsterixFieldI247Frn001Type010 : AsterixFieldI247
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
/// I247/015 Service Identification.
/// </summary>
public class AsterixFieldI247Frn002Type015 : AsterixFieldI247
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 2;

    /// <inheritdoc />
    public override string Name => "Service Identification";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Identification of the service provided to one or more users.
    /// </summary>
    public byte ServiceIdentification { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        ServiceIdentification = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = ServiceIdentification;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}

/// <summary>
/// I247/140 Time of Day.
/// </summary>
public class AsterixFieldI247Frn003Type140 : AsterixFieldI247
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 3;

    /// <inheritdoc />
    public override string Name => "Time of Day";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Absolute UTC time in seconds since midnight.
    /// </summary>
    public double Seconds { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Seconds = AsterixI247Binary.ReadUInt24(ref buffer) / 128.0;
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI247Binary.WriteUInt24(ref buffer, (int)Math.Round(Seconds * 128.0));
    }

    /// <inheritdoc />
    public override int GetByteSize() => 3;
}

/// <summary>
/// Version information for a single ASTERIX category.
/// </summary>
public class AsterixCategoryVersion
{
    /// <summary>
    /// ASTERIX category number.
    /// </summary>
    public byte Category { get; set; }

    /// <summary>
    /// Main version number.
    /// </summary>
    public byte Major { get; set; }

    /// <summary>
    /// Sub-version number.
    /// </summary>
    public byte Minor { get; set; }
}

/// <summary>
/// I247/550 Category Version Number Report.
/// </summary>
public class AsterixFieldI247Frn004Type550 : AsterixFieldI247
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 4;

    /// <inheritdoc />
    public override string Name => "Category Version Number Report";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Reported ASTERIX category versions.
    /// </summary>
    public List<AsterixCategoryVersion> Versions { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Versions.Clear();
        for (var i = 0; i < count; i++)
        {
            Versions.Add(new AsterixCategoryVersion
            {
                Category = buffer[0],
                Major = buffer[1],
                Minor = buffer[2]
            });
            buffer = buffer[3..];
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Versions.Count;
        buffer = buffer[1..];
        foreach (var version in Versions)
        {
            buffer[0] = version.Category;
            buffer[1] = version.Major;
            buffer[2] = version.Minor;
            buffer = buffer[3..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Versions.Count * 3;
}

/// <summary>
/// I247/SP Special Purpose Field.
/// </summary>
public class AsterixFieldI247Frn006TypeSp : AsterixFieldI247
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 6;

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
/// I247/RE Reserved Expansion Field.
/// </summary>
public class AsterixFieldI247Frn007TypeRe : AsterixFieldI247
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 7;

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
