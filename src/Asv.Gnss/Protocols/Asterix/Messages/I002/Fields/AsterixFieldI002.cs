using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Asv.Gnss;

internal static class AsterixI002Binary
{
    public static uint ReadUInt24(ref ReadOnlySpan<byte> buffer)
    {
        var value = (uint)((buffer[0] << 16) | (buffer[1] << 8) | buffer[2]);
        buffer = buffer[3..];
        return value;
    }

    public static void WriteUInt24(ref Span<byte> buffer, uint value)
    {
        buffer[0] = (byte)(value >> 16);
        buffer[1] = (byte)(value >> 8);
        buffer[2] = (byte)value;
        buffer = buffer[3..];
    }
}

public abstract class AsterixFieldI002Fixed : AsterixField
{
    public override int Category => AsterixMessageI002.Category;

    public override void Accept(Asv.IO.IVisitor visitor)
    {
    }
}

public sealed class AsterixFieldI002Frn001Type010 : AsterixFieldI002Fixed
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

public sealed class AsterixFieldI002Frn002Type000 : AsterixFieldI002Fixed
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

public sealed class AsterixFieldI002Frn003Type020 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 3;
    public override string Name => "Sector Number";
    public override byte FieldReferenceNumber => StaticFrn;
    public double SectorNumberDeg { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        SectorNumberDeg = buffer[0] * 360.0 / 256.0;
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Math.Round(SectorNumberDeg / (360.0 / 256.0));
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}

public sealed class AsterixFieldI002Frn004Type030 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 4;
    public override string Name => "Time of Day";
    public override byte FieldReferenceNumber => StaticFrn;
    public double Seconds { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Seconds = AsterixI002Binary.ReadUInt24(ref buffer) / 128.0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI002Binary.WriteUInt24(ref buffer, (uint)Math.Round(Seconds * 128.0));
    }

    public override int GetByteSize() => 3;
}

public sealed class AsterixFieldI002Frn005Type041 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 5;
    public override string Name => "Antenna Rotation Speed";
    public override byte FieldReferenceNumber => StaticFrn;
    public double AntennaRotationPeriod { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AntennaRotationPeriod = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 128.0;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(AntennaRotationPeriod * 128.0));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}

public abstract class AsterixFieldI002ExtendableValues : AsterixFieldI002Fixed
{
    public List<byte> Values { get; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Values.Clear();
        byte raw;
        do
        {
            raw = buffer[0];
            Values.Add((byte)(raw >> 1));
            buffer = buffer[1..];
        }
        while ((raw & 0x01) != 0);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        for (var i = 0; i < Values.Count; i++)
        {
            buffer[0] = (byte)((Values[i] << 1) | (i < Values.Count - 1 ? 0x01 : 0x00));
            buffer = buffer[1..];
        }
    }

    public override int GetByteSize() => Values.Count;
}

public sealed class AsterixFieldI002Frn006Type050 : AsterixFieldI002ExtendableValues
{
    public const byte StaticFrn = 6;
    public override string Name => "Station Configuration Status";
    public override byte FieldReferenceNumber => StaticFrn;
}

public sealed class AsterixFieldI002Frn007Type060 : AsterixFieldI002ExtendableValues
{
    public const byte StaticFrn = 7;
    public override string Name => "Station Processing Mode";
    public override byte FieldReferenceNumber => StaticFrn;
}

public sealed class AsterixFieldI002Frn008Type070 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 8;
    public override string Name => "Plot Count Values";
    public override byte FieldReferenceNumber => StaticFrn;
    public List<AsterixI002PlotCountValue> Items { get; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
            buffer = buffer[2..];
            Items.Add(new AsterixI002PlotCountValue
            {
                Aerial = (raw & 0x8000) != 0,
                Ident = (byte)((raw >> 10) & 0x1F),
                Counter = (ushort)(raw & 0x03FF)
            });
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)Items.Count);
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            var raw = (ushort)((item.Aerial ? 0x8000 : 0) | ((item.Ident & 0x1F) << 10) | (item.Counter & 0x03FF));
            BinaryPrimitives.WriteUInt16BigEndian(buffer, raw);
            buffer = buffer[2..];
        }
    }

    public override int GetByteSize() => 1 + Items.Count * 2;
}

public sealed class AsterixI002PlotCountValue
{
    public bool Aerial { get; set; }
    public byte Ident { get; set; }
    public ushort Counter { get; set; }
}

public sealed class AsterixFieldI002Frn009Type100 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 9;
    public override string Name => "Dynamic Window - Type 1";
    public override byte FieldReferenceNumber => StaticFrn;
    public double RhoStartNm { get; set; }
    public double RhoEndNm { get; set; }
    public double ThetaStartDeg { get; set; }
    public double ThetaEndDeg { get; set; }

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

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(RhoStartNm * 128.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(RhoEndNm * 128.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(ThetaStartDeg / (360.0 / 65536.0)));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(ThetaEndDeg / (360.0 / 65536.0)));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 8;
}

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

public sealed class AsterixFieldI002Frn011Type080 : AsterixFieldI002ExtendableValues
{
    public const byte StaticFrn = 11;
    public override string Name => "Warning/Error Conditions";
    public override byte FieldReferenceNumber => StaticFrn;
}

public sealed class AsterixFieldI002Frn013TypeSp : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 13;
    public override string Name => "Special Purpose Field";
    public override byte FieldReferenceNumber => StaticFrn;
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
