using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Asv.Gnss;

internal static class AsterixI004Binary
{
    public static uint ReadUInt24(ref ReadOnlySpan<byte> buffer)
    {
        var value = (uint)((buffer[0] << 16) | (buffer[1] << 8) | buffer[2]);
        buffer = buffer[3..];
        return value;
    }

    public static int ReadInt24(ref ReadOnlySpan<byte> buffer)
    {
        var value = (int)ReadUInt24(ref buffer);
        if ((value & 0x800000) != 0)
        {
            value |= unchecked((int)0xFF000000);
        }

        return value;
    }

    public static void WriteUInt24(ref Span<byte> buffer, uint value)
    {
        buffer[0] = (byte)(value >> 16);
        buffer[1] = (byte)(value >> 8);
        buffer[2] = (byte)value;
        buffer = buffer[3..];
    }

    public static void WriteInt24(ref Span<byte> buffer, int value)
    {
        WriteUInt24(ref buffer, (uint)(value & 0x00FFFFFF));
    }
}

public abstract class AsterixFieldI004Fixed : AsterixField
{
    public override int Category => AsterixMessageI004.Category;

    public override void Accept(Asv.IO.IVisitor visitor)
    {
    }
}

public sealed class AsterixFieldI004Frn001Type010 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 1;
    public override string Name => "Data Source Identifier";
    public override byte FieldReferenceNumber => StaticFrn;
    public SystemAreaCode Sac { get; set; }
    public byte Sic { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Sac = (SystemAreaCode)buffer[0];
        Sic = buffer[1];
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Sac;
        buffer[1] = Sic;
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}

public sealed class AsterixFieldI004Frn002Type000 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 2;
    public override string Name => "Message Type";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte MessageType { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        MessageType = buffer[0];
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = MessageType;
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}

public sealed class AsterixFieldI004Frn003Type015 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 3;
    public override string Name => "SDPS Identifier";
    public override byte FieldReferenceNumber => StaticFrn;
    public List<AsterixI004DataSourceIdentifier> Items { get; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            Items.Add(new AsterixI004DataSourceIdentifier
            {
                Sac = (SystemAreaCode)buffer[0],
                Sic = buffer[1]
            });
            buffer = buffer[2..];
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)Items.Count);
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            buffer[0] = (byte)item.Sac;
            buffer[1] = item.Sic;
            buffer = buffer[2..];
        }
    }

    public override int GetByteSize() => 1 + Items.Count * 2;
}

public sealed class AsterixI004DataSourceIdentifier
{
    public SystemAreaCode Sac { get; set; }
    public byte Sic { get; set; }
}

public sealed class AsterixFieldI004Frn004Type020 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 4;
    public override string Name => "Time of Message";
    public override byte FieldReferenceNumber => StaticFrn;
    public double Seconds { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Seconds = AsterixI004Binary.ReadUInt24(ref buffer) / 128.0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI004Binary.WriteUInt24(ref buffer, (uint)Math.Round(Seconds * 128.0));
    }

    public override int GetByteSize() => 3;
}

public sealed class AsterixFieldI004Frn005Type040 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 5;
    public override string Name => "Alert Identifier";
    public override byte FieldReferenceNumber => StaticFrn;
    public ushort AlertIdentifier { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AlertIdentifier = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, AlertIdentifier);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}

public sealed class AsterixFieldI004Frn006Type045 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 6;
    public override string Name => "Alert Status";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte AlertStatus { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AlertStatus = (byte)((buffer[0] >> 1) & 0x07);
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((AlertStatus & 0x07) << 1);
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}

public sealed class AsterixFieldI004Frn007Type060 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 7;
    public override string Name => "Safety Net Function & System Status";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool Mrva { get; set; }
    public bool Ramld { get; set; }
    public bool Ramhd { get; set; }
    public bool Msaw { get; set; }
    public bool Apw { get; set; }
    public bool Clam { get; set; }
    public bool Stca { get; set; }
    public bool Afda { get; set; }
    public bool Rimca { get; set; }
    public bool Overflow { get; set; }
    public bool Overload { get; set; }
    public bool HasFirstExtension { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        buffer = buffer[1..];
        Mrva = (b & 0x80) != 0;
        Ramld = (b & 0x40) != 0;
        Ramhd = (b & 0x20) != 0;
        Msaw = (b & 0x10) != 0;
        Apw = (b & 0x08) != 0;
        Clam = (b & 0x04) != 0;
        Stca = (b & 0x02) != 0;
        HasFirstExtension = (b & 0x01) != 0;
        if (!HasFirstExtension)
        {
            return;
        }

        b = buffer[0];
        buffer = buffer[1..];
        Afda = (b & 0x80) != 0;
        Rimca = (b & 0x40) != 0;
        Overflow = (b & 0x04) != 0;
        Overload = (b & 0x02) != 0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var hasFirst = HasFirstExtension || Afda || Rimca || Overflow || Overload;
        buffer[0] = (byte)((Mrva ? 0x80 : 0) | (Ramld ? 0x40 : 0) | (Ramhd ? 0x20 : 0) |
                           (Msaw ? 0x10 : 0) | (Apw ? 0x08 : 0) | (Clam ? 0x04 : 0) |
                           (Stca ? 0x02 : 0) | (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];
        if (!hasFirst)
        {
            return;
        }

        buffer[0] = (byte)((Afda ? 0x80 : 0) | (Rimca ? 0x40 : 0) | (Overflow ? 0x04 : 0) |
                           (Overload ? 0x02 : 0));
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1 + (HasFirstExtension || Afda || Rimca || Overflow || Overload ? 1 : 0);
}

public sealed class AsterixFieldI004Frn008Type030 : AsterixI004TrackNumberField
{
    public const byte StaticFrn = 8;
    public override string Name => "Track Number 1";
    public override byte FieldReferenceNumber => StaticFrn;
}

public sealed class AsterixFieldI004Frn016Type035 : AsterixI004TrackNumberField
{
    public const byte StaticFrn = 16;
    public override string Name => "Track Number 2";
    public override byte FieldReferenceNumber => StaticFrn;
}

public abstract class AsterixI004TrackNumberField : AsterixFieldI004Fixed
{
    public ushort TrackNumber { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TrackNumber = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, TrackNumber);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}

public sealed class AsterixFieldI004Frn009Type170 : AsterixI004CompoundField
{
    public const byte StaticFrn = 9;
    public override string Name => "Aircraft Identification & Characteristics 1";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => 7,
        1 => 2,
        2 => 10,
        3 => 8,
        4 => 3,
        5 => 2,
        6 => (buffer[0] & 0x01) != 0 ? 2 : 1,
        8 => 6,
        9 => 4,
        10 => 2,
        _ => 0
    };
}

public sealed class AsterixFieldI004Frn010Type120 : AsterixI004CompoundField
{
    public const byte StaticFrn = 10;
    public override string Name => "Conflict Characteristics";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => (buffer[0] & 0x01) != 0 ? 2 : 1,
        1 => 1,
        2 => 1,
        3 => 3,
        _ => 0
    };
}

public sealed class AsterixFieldI004Frn011Type070 : AsterixI004CompoundField
{
    public const byte StaticFrn = 11;
    public override string Name => "Conflict Timing and Separation";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => 3,
        1 => 3,
        2 => 3,
        3 => 2,
        4 => 2,
        5 => 2,
        _ => 0
    };
}

public sealed class AsterixFieldI004Frn012Type076 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 12;
    public override string Name => "Vertical Deviation";
    public override byte FieldReferenceNumber => StaticFrn;
    public double VerticalDeviationFt { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        VerticalDeviationFt = BinaryPrimitives.ReadInt16BigEndian(buffer) * 25.0;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(VerticalDeviationFt / 25.0));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}

public sealed class AsterixFieldI004Frn013Type074 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 13;
    public override string Name => "Longitudinal Deviation";
    public override byte FieldReferenceNumber => StaticFrn;
    public double LongitudinalDeviationM { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        LongitudinalDeviationM = BinaryPrimitives.ReadInt16BigEndian(buffer) * 32.0;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(LongitudinalDeviationM / 32.0));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}

public sealed class AsterixFieldI004Frn014Type075 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 14;
    public override string Name => "Transversal Distance Deviation";
    public override byte FieldReferenceNumber => StaticFrn;
    public double TransversalDistanceDeviationM { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TransversalDistanceDeviationM = AsterixI004Binary.ReadInt24(ref buffer) * 0.5;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI004Binary.WriteInt24(ref buffer, (int)Math.Round(TransversalDistanceDeviationM / 0.5));
    }

    public override int GetByteSize() => 3;
}

public sealed class AsterixFieldI004Frn015Type100 : AsterixI004CompoundField
{
    public const byte StaticFrn = 15;
    public override string Name => "Area Definition";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => 6,
        1 => 7,
        2 => 7,
        3 => 7,
        4 => 7,
        5 => 7,
        _ => 0
    };
}

public sealed class AsterixFieldI004Frn017Type171 : AsterixI004CompoundField
{
    public const byte StaticFrn = 17;
    public override string Name => "Aircraft Identification & Characteristics 2";
    public override byte FieldReferenceNumber => StaticFrn;
    protected override int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer) => dataBitIndex switch
    {
        0 => 7,
        1 => 2,
        2 => 10,
        3 => 8,
        4 => 3,
        5 => 2,
        6 => (buffer[0] & 0x01) != 0 ? 2 : 1,
        8 => 6,
        9 => 4,
        10 => 2,
        _ => 0
    };
}

public sealed class AsterixFieldI004Frn018Type110 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 18;
    public override string Name => "FDPS Sector Control Identification";
    public override byte FieldReferenceNumber => StaticFrn;
    public List<AsterixI004FdpsSectorControl> Items { get; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            Items.Add(new AsterixI004FdpsSectorControl
            {
                Centre = buffer[0],
                Position = buffer[1]
            });
            buffer = buffer[2..];
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)Items.Count);
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            buffer[0] = item.Centre;
            buffer[1] = item.Position;
            buffer = buffer[2..];
        }
    }

    public override int GetByteSize() => 1 + Items.Count * 2;
}

public sealed class AsterixI004FdpsSectorControl
{
    public byte Centre { get; set; }
    public byte Position { get; set; }
}

public sealed class AsterixFieldI004Frn020TypeRe : AsterixI004LengthPrefixedRawField
{
    public const byte StaticFrn = 20;
    public override string Name => "Reserved Expansion Field";
    public override byte FieldReferenceNumber => StaticFrn;
}

public sealed class AsterixFieldI004Frn021TypeSp : AsterixI004LengthPrefixedRawField
{
    public const byte StaticFrn = 21;
    public override string Name => "Special Purpose Field";
    public override byte FieldReferenceNumber => StaticFrn;
}

public abstract class AsterixI004CompoundField : AsterixFieldI004Fixed
{
    public byte[] Data { get; set; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var start = buffer;
        var fspec = new VariableLengthValue();
        fspec.Deserialize(ref buffer);
        for (var i = 0; i < fspec.DataBitsCount; i++)
        {
            if (fspec[i] != true)
            {
                continue;
            }

            var size = GetItemByteSize(i, buffer);
            buffer = buffer[size..];
        }

        Data = start[..(start.Length - buffer.Length)].ToArray();
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        Data.CopyTo(buffer);
        buffer = buffer[Data.Length..];
    }

    public override int GetByteSize() => Data.Length;
    protected abstract int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer);
}

public abstract class AsterixI004LengthPrefixedRawField : AsterixFieldI004Fixed
{
    public byte[] Data { get; set; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var length = buffer[0];
        buffer = buffer[1..];
        Data = length == 0 ? [] : buffer[..(length - 1)].ToArray();
        buffer = buffer[Data.Length..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)(Data.Length + 1));
        Data.CopyTo(buffer[1..]);
        buffer = buffer[(Data.Length + 1)..];
    }

    public override int GetByteSize() => Data.Length + 1;
}
