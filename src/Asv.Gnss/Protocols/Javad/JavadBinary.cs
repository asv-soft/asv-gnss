using System;
using System.Buffers.Binary;
using System.Text;

namespace Asv.Gnss;

internal static class JavadBinary
{
    public static byte ReadU1(ref ReadOnlySpan<byte> buffer)
    {
        Require(buffer, 1);
        var value = buffer[0];
        buffer = buffer[1..];
        return value;
    }

    public static sbyte ReadI1(ref ReadOnlySpan<byte> buffer) =>
        unchecked((sbyte)ReadU1(ref buffer));

    public static void WriteU1(ref Span<byte> buffer, byte value)
    {
        buffer[0] = value;
        buffer = buffer[1..];
    }

    public static void WriteI1(ref Span<byte> buffer, sbyte value) =>
        WriteU1(ref buffer, unchecked((byte)value));

    public static ushort ReadU2(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)
    {
        Require(buffer, 2);
        var value =
            order == JavadByteOrder.LittleEndian
                ? BinaryPrimitives.ReadUInt16LittleEndian(buffer)
                : BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        return value;
    }

    public static void WriteU2(ref Span<byte> buffer, ushort value, JavadByteOrder order)
    {
        if (order == JavadByteOrder.LittleEndian)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        }
        buffer = buffer[2..];
    }

    public static short ReadI2(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)
    {
        Require(buffer, 2);
        var value =
            order == JavadByteOrder.LittleEndian
                ? BinaryPrimitives.ReadInt16LittleEndian(buffer)
                : BinaryPrimitives.ReadInt16BigEndian(buffer);
        buffer = buffer[2..];
        return value;
    }

    public static void WriteI2(ref Span<byte> buffer, short value, JavadByteOrder order)
    {
        if (order == JavadByteOrder.LittleEndian)
        {
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        }
        buffer = buffer[2..];
    }

    public static uint ReadU4(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)
    {
        Require(buffer, 4);
        var value =
            order == JavadByteOrder.LittleEndian
                ? BinaryPrimitives.ReadUInt32LittleEndian(buffer)
                : BinaryPrimitives.ReadUInt32BigEndian(buffer);
        buffer = buffer[4..];
        return value;
    }

    public static void WriteU4(ref Span<byte> buffer, uint value, JavadByteOrder order)
    {
        if (order == JavadByteOrder.LittleEndian)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        }
        buffer = buffer[4..];
    }

    public static int ReadI4(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)
    {
        Require(buffer, 4);
        var value =
            order == JavadByteOrder.LittleEndian
                ? BinaryPrimitives.ReadInt32LittleEndian(buffer)
                : BinaryPrimitives.ReadInt32BigEndian(buffer);
        buffer = buffer[4..];
        return value;
    }

    public static void WriteI4(ref Span<byte> buffer, int value, JavadByteOrder order)
    {
        if (order == JavadByteOrder.LittleEndian)
        {
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt32BigEndian(buffer, value);
        }
        buffer = buffer[4..];
    }

    public static float ReadF4(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)
    {
        Require(buffer, 4);
        var value =
            order == JavadByteOrder.LittleEndian
                ? BinaryPrimitives.ReadSingleLittleEndian(buffer)
                : BinaryPrimitives.ReadSingleBigEndian(buffer);
        buffer = buffer[4..];
        return value;
    }

    public static void WriteF4(ref Span<byte> buffer, float value, JavadByteOrder order)
    {
        if (order == JavadByteOrder.LittleEndian)
        {
            BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteSingleBigEndian(buffer, value);
        }
        buffer = buffer[4..];
    }

    public static double ReadF8(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)
    {
        Require(buffer, 8);
        var value =
            order == JavadByteOrder.LittleEndian
                ? BinaryPrimitives.ReadDoubleLittleEndian(buffer)
                : BinaryPrimitives.ReadDoubleBigEndian(buffer);
        buffer = buffer[8..];
        return value;
    }

    public static void WriteF8(ref Span<byte> buffer, double value, JavadByteOrder order)
    {
        if (order == JavadByteOrder.LittleEndian)
        {
            BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteDoubleBigEndian(buffer, value);
        }
        buffer = buffer[8..];
    }

    public static byte[] ReadBytes(ref ReadOnlySpan<byte> buffer, int count)
    {
        Require(buffer, count);
        var result = buffer[..count].ToArray();
        buffer = buffer[count..];
        return result;
    }

    public static void WriteBytes(ref Span<byte> buffer, ReadOnlySpan<byte> value)
    {
        value.CopyTo(buffer);
        buffer = buffer[value.Length..];
    }

    public static string ReadAscii(ref ReadOnlySpan<byte> buffer, int count)
    {
        Require(buffer, count);
        var data = buffer[..count];
        foreach (var value in data)
        {
            if (value > 127)
            {
                throw new FormatException("A GREIS ASCII field contains a non-ASCII byte.");
            }
        }
        var result = Encoding.ASCII.GetString(data);
        buffer = buffer[count..];
        return result;
    }

    public static void WriteAscii(ref Span<byte> buffer, string value, int count)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (count < value.Length)
        {
            throw new ArgumentException(
                "The string exceeds its GREIS field length.",
                nameof(value)
            );
        }
        ValidateAscii(value);
        var destination = buffer[..count];
        destination.Fill((byte)' ');
        Encoding.ASCII.GetBytes(value, destination);
        buffer = buffer[count..];
    }

    public static string ReadNullTerminatedAscii(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer.IndexOf((byte)0);
        if (count < 0)
        {
            throw new FormatException("Missing terminator in a GREIS string.");
        }
        var result = ReadAscii(ref buffer, count);
        buffer = buffer[1..];
        return result;
    }

    public static void WriteNullTerminatedAscii(ref Span<byte> buffer, string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Contains('\0'))
        {
            throw new ArgumentException(
                "A GREIS string cannot contain its terminator.",
                nameof(value)
            );
        }
        WriteAscii(ref buffer, value, value.Length);
        WriteU1(ref buffer, 0);
    }

    public static void ValidateAscii(string value)
    {
        foreach (var character in value)
        {
            if (character > 127)
            {
                throw new ArgumentException(
                    "GREIS strings must contain only ASCII characters.",
                    nameof(value)
                );
            }
        }
    }

    private static void Require(ReadOnlySpan<byte> buffer, int count)
    {
        if (count < 0 || buffer.Length < count)
        {
            throw new FormatException("Truncated GREIS payload field.");
        }
    }
}
