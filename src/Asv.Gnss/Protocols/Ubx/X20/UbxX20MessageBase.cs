using System;
using System.Buffers.Binary;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>A typed UBX payload from the pinned u-blox X20 HPG 2.11 interface.</summary>
/// <remarks>All numeric properties retain their native wire values. XML documentation and visitor metadata describe scales and units.</remarks>
public abstract class UbxX20MessageBase : UbxMessageBase
{
    /// <summary>Manufacturer variant name; distinct from any payload version field.</summary>
    public abstract string Variant { get; }

    /// <summary>Reads the complete native payload.</summary>
    protected abstract void ReadPayload(ref ReadOnlySpan<byte> buffer);

    /// <summary>Writes the complete native payload.</summary>
    protected abstract void WritePayload(ref Span<byte> buffer);

    /// <summary>Validates storage and returns the native payload size.</summary>
    protected abstract int GetPayloadByteSize();

    /// <inheritdoc />
    protected sealed override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        ReadPayload(ref buffer);
        if (!buffer.IsEmpty)
            throw new FormatException("The X20 payload has trailing bytes or an inconsistent repeated-group count.");
    }

    /// <inheritdoc />
    protected sealed override void SerializeContent(ref Span<byte> buffer)
    {
        _ = GetPayloadByteSize();
        WritePayload(ref buffer);
        if (!buffer.IsEmpty)
            throw new InvalidOperationException("The X20 payload serializer did not consume its declared size.");
    }

    /// <inheritdoc />
    protected sealed override int GetContentByteSize() => UbxX20Payload.CheckedSize(GetPayloadByteSize());

    /// <inheritdoc />
    public override void Randomize(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);
        // Defaults deliberately retain discriminator values and valid empty repeated groups.
    }
}

internal static class UbxX20Payload
{
    internal static Field ReadOnlyField(string name, IFieldType type, string description, string? units = null)
    {
        var builder = new Field.Builder().Name(name).Title(name).Description(description)
            .DataType(type).Metadata("readOnly", true);
        if (units is not null)
            builder.Units(units);
        return builder.Build();
    }

    internal static int CheckedSize(int size)
    {
        if (size is < 0 or > ushort.MaxValue)
            throw new InvalidOperationException("A UBX payload cannot exceed 65535 bytes.");
        return size;
    }

    internal static void RequireAvailable(int available, int count, int elementSize)
    {
        if (count < 0 || count > ushort.MaxValue / elementSize || count > available / elementSize)
            throw new FormatException("The X20 payload is truncated or declares too many repeated records.");
    }

    internal static int RemainingCount(int remaining, int elementSize)
    {
        if (remaining % elementSize != 0)
            throw new FormatException("The X20 payload ends with an incomplete repeated record.");
        return remaining / elementSize;
    }

    internal static string Ascii(byte[] value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var end = Array.IndexOf(value, (byte)0);
        return Encoding.ASCII.GetString(value.AsSpan(0, end < 0 ? value.Length : end));
    }

    internal static byte ReadUInt8(ref ReadOnlySpan<byte> buffer)
    {
        var value = buffer[0];
        buffer = buffer[1..];
        return value;
    }
    internal static sbyte ReadInt8(ref ReadOnlySpan<byte> buffer) => unchecked((sbyte)ReadUInt8(ref buffer));
    internal static ushort ReadUInt16(ref ReadOnlySpan<byte> buffer)
    {
        var value = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        buffer = buffer[2..];
        return value;
    }
    internal static short ReadInt16(ref ReadOnlySpan<byte> buffer) => unchecked((short)ReadUInt16(ref buffer));
    internal static uint ReadUInt32(ref ReadOnlySpan<byte> buffer)
    {
        var value = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
        buffer = buffer[4..];
        return value;
    }
    internal static int ReadInt32(ref ReadOnlySpan<byte> buffer) => unchecked((int)ReadUInt32(ref buffer));
    internal static float ReadFloat(ref ReadOnlySpan<byte> buffer) => BitConverter.Int32BitsToSingle(ReadInt32(ref buffer));
    internal static double ReadDouble(ref ReadOnlySpan<byte> buffer)
    {
        var value = BinaryPrimitives.ReadDoubleLittleEndian(buffer);
        buffer = buffer[8..];
        return value;
    }
    internal static void WriteUInt8(ref Span<byte> buffer, byte value)
    {
        buffer[0] = value;
        buffer = buffer[1..];
    }
    internal static void WriteInt8(ref Span<byte> buffer, sbyte value) => WriteUInt8(ref buffer, unchecked((byte)value));
    internal static void WriteUInt16(ref Span<byte> buffer, ushort value)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        buffer = buffer[2..];
    }
    internal static void WriteInt16(ref Span<byte> buffer, short value) => WriteUInt16(ref buffer, unchecked((ushort)value));
    internal static void WriteUInt32(ref Span<byte> buffer, uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        buffer = buffer[4..];
    }
    internal static void WriteInt32(ref Span<byte> buffer, int value) => WriteUInt32(ref buffer, unchecked((uint)value));
    internal static void WriteFloat(ref Span<byte> buffer, float value) => WriteInt32(ref buffer, BitConverter.SingleToInt32Bits(value));
    internal static void WriteDouble(ref Span<byte> buffer, double value)
    {
        BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
        buffer = buffer[8..];
    }
}
