using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

public readonly struct NmeaIntFormat(string format, int minSize)
{
    public string Format { get; } = format;
    
    public int GetByteSize(int? value)
    {
        return value == null ? 0 : Math.Max(minSize, NmeaProtocol.CountDigits(value.Value));
    }
    
    public static NmeaIntFormat IntD1 = new("0", 1);
    public static NmeaIntFormat IntD2 = new("00", 2);
    public static NmeaIntFormat IntD3 = new("000", 3);
    public static NmeaIntFormat IntD4 = new("0000", 4);
    public static NmeaIntFormat IntD5 = new("00000", 5);
    public static NmeaIntFormat IntD6 = new("000000", 6);
    public static NmeaIntFormat IntD7 = new("0000000", 7);
    public static NmeaIntFormat IntD8 = new("00000000", 8);
}

public readonly struct NmeaDoubleFormat(string format, int minSizeBeforeDot, int minSizeAfterDot)
{
    public string Format { get; } = format;
    public int GetByteSize(double value)
    {
        return double.IsFinite(value) ? 0 : Math.Max(minSizeBeforeDot + 1 /*Dot (.)*/ + minSizeAfterDot, NmeaProtocol.CountDigits((int)value) + 1 /*Dot (.)*/ + minSizeAfterDot);
    }
    
    public static readonly NmeaDoubleFormat Double1X1 = new("0.0", 1, 1);
    public static readonly NmeaDoubleFormat Double1X2 = new("0.00", 1, 2);
    public static readonly NmeaDoubleFormat Double1X3 = new("0.000", 1, 3);
    public static readonly NmeaDoubleFormat Double1X4 = new("0.0000", 1, 4);
    public static readonly NmeaDoubleFormat Double1X5 = new("0.00000", 1, 5);
    public static readonly NmeaDoubleFormat Double1X6 = new("0.000000", 1, 6);
    public static readonly NmeaDoubleFormat Double2X1 = new("00.0", 2, 1);
    public static readonly NmeaDoubleFormat Double2X2 = new("00.00", 2, 2);
    public static readonly NmeaDoubleFormat Double2X3 = new("00.000", 2, 3);
    public static readonly NmeaDoubleFormat Double2X4 = new("00.0000", 2, 4);
    public static readonly NmeaDoubleFormat Double2X5 = new("00.00000", 2, 5);
    public static readonly NmeaDoubleFormat Double2X6 = new("00.000000", 2, 6);
    public static readonly NmeaDoubleFormat Double3X1 = new("000.0", 3, 1);
    public static readonly NmeaDoubleFormat Double3X2 = new("000.00", 3, 2);
    public static readonly NmeaDoubleFormat Double3X3 = new("000.000", 3, 3);
    public static readonly NmeaDoubleFormat Double3X4 = new("000.0000", 3, 4);
    public static readonly NmeaDoubleFormat Double3X5 = new("000.00000", 3, 5);
    public static readonly NmeaDoubleFormat Double3X6 = new("000.000000", 3, 6);
    public static readonly NmeaDoubleFormat Double4X1 = new("0000.0", 4, 1);
    public static readonly NmeaDoubleFormat Double4X2 = new("0000.00", 4, 2);
    public static readonly NmeaDoubleFormat Double4X3 = new("0000.000", 4, 3);
    public static readonly NmeaDoubleFormat Double4X4 = new("0000.0000", 4, 4);
    public static readonly NmeaDoubleFormat Double4X5 = new("0000.00000", 4, 5);
    public static readonly NmeaDoubleFormat Double4X6 = new("0000.000000", 4, 6);
    public static readonly NmeaDoubleFormat Double5X1 = new("00000.0", 5, 1);
    public static readonly NmeaDoubleFormat Double5X2 = new("00000.00", 5, 2);
    public static readonly NmeaDoubleFormat Double5X3 = new("00000.000", 5, 3);
    public static readonly NmeaDoubleFormat Double5X4 = new("00000.0000", 5, 4);
    public static readonly NmeaDoubleFormat Double5X5 = new("00000.00000", 5, 5);
    public static readonly NmeaDoubleFormat Double5X6 = new("00000.000000", 5, 6);
    public static readonly NmeaDoubleFormat Double6X1 = new("000000.0", 6, 1);
    public static readonly NmeaDoubleFormat Double6X2 = new("000000.00", 6, 2);
    public static readonly NmeaDoubleFormat Double6X3 = new("000000.000", 6, 3);
    public static readonly NmeaDoubleFormat Double6X4 = new("000000.0000", 6, 4);
    public static readonly NmeaDoubleFormat Double6X5 = new("000000.00000", 6, 5);
    public static readonly NmeaDoubleFormat Double6X6 = new("000000.000000", 6, 6);
        
}


public static class NmeaProtocol
{
    public const byte ComaByte = (byte)TokenSeparator;
    public const char TokenSeparator = ',';
    public const char StartCrcChar = '*';
    public const byte StartCrcByte = (byte)StartCrcChar;
    
    public const byte StartMessageByte1 = (byte)'$';
    public const byte StartMessageByte2 = (byte)'!';
    public const byte EndMessageByte1 = 0x0D;
    public const byte EndMessageByte2 = 0x0A;
    private const byte DigitSeparator = (byte)'.';
    public const char ProprietaryPrefix = 'P';
    public static ProtocolInfo ProtocolInfo { get; } = new("NAME", "NMEA 0183");
    public static Encoding Encoding => Encoding.ASCII;
    
    /// <summary>
    /// Calculates the CRC checksum for the given buffer of bytes.
    /// </summary>
    /// <param name="buffer">The buffer of bytes to calculate the CRC checksum for.</param>
    /// <returns>The calculated CRC checksum as a hexadecimal string.</returns>
    public static string CalcCrc(ReadOnlySpan<byte> buffer)
    {
        var crc = 0;
        foreach (var c in buffer)
        {
            if (crc == 0)
            {
                crc = c;
            }
            else
            {
                crc ^= c;
            }
        }

        return crc.ToString("X2");
    }

    private static bool TryBiteMessageId(ref ReadOnlySpan<byte> rawMessage, out ReadOnlySpan<byte> messageId)
    {
        if (rawMessage.Length == 0)
        {
            messageId = ReadOnlySpan<byte>.Empty;
            return false;
        }
        if (rawMessage[0] is StartMessageByte1 or StartMessageByte2)
        {
            rawMessage = rawMessage[1..]; // skip the start byte
        }
        
        var idEndIndex = rawMessage.IndexOf(ComaByte);
        if (idEndIndex <= 2) // Message ID must be at least 2 characters long: Talker Identifiers(1-2) + Message Type(>=1) + Coma
        {
            messageId = ReadOnlySpan<byte>.Empty;
            return false;
        }

        messageId = rawMessage[..idEndIndex];
        rawMessage = rawMessage[(idEndIndex+1)..];
        return true;
    }
    
    public static bool TryGetMessageId(ReadOnlySpan<byte> rawMessage, out NmeaMessageId msgId, out NmeaTalkerId talkerClass) => TryGetMessageId(ref rawMessage, out msgId, out talkerClass);

    public static bool TryGetMessageId(ref ReadOnlySpan<byte> rawMessage, out NmeaMessageId msgId, out NmeaTalkerId talkerClass)
    {
        if (TryBiteMessageId(ref rawMessage, out var messageIdBuff) == false)
        {
            msgId = default;
            talkerClass = default;
            return false;
        }
        var idBuff = ArrayPool<char>.Shared.Rent(messageIdBuff.Length);
        try
        {
            var idCharSpan = idBuff.AsSpan(0,messageIdBuff.Length);
            Encoding.GetChars(messageIdBuff, idCharSpan);
            talkerClass = new NmeaTalkerId(idCharSpan);
            idCharSpan = idCharSpan[talkerClass.GetByteSize()..];
            msgId = new NmeaMessageId(idCharSpan);
            return true;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(idBuff);
        }
    }
    
    public static bool TryReadNextToken(ref ReadOnlySpan<char> charBufferSpan, out ReadOnlySpan<char> token)
    {
        var idEndIndex = charBufferSpan.IndexOf(TokenSeparator);
        if (idEndIndex == -1)
        {
            token = ReadOnlySpan<char>.Empty;
            return false;
        }
        token = charBufferSpan[..idEndIndex];
        charBufferSpan = charBufferSpan[(idEndIndex+1)..];
        return true;
    }
    public static ReadOnlySpan<char> ReadNextRequiredToken(ref ReadOnlySpan<char> charBufferSpan)
    {
        if (TryReadNextToken(ref charBufferSpan, out var token) == false)
        {
            throw new FormatException("Token not found");
        }
        return token;
    }
    
    public static void WriteSeparator(ref Span<byte> buffer) => BinSerialize.WriteByte(ref buffer, ComaByte);
    public static int SizeOfSeparator() => 1 /*COMA*/;

    /// <summary>
    /// hhmmss.ss | hhmmss | 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TimeSpan? ReadTime(ReadOnlySpan<char> value)
    {
        if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp)) return null;
        var sss = (int)((temp - (int)temp) * 1000.0);
        var hh = (int)((int)temp / 10000.0);
        var mm = (int)(((int)temp - hh * 10000.0) / 100.0);
        var ss = (int)((int)temp - hh * 10000.0 - mm * 100.0);
        return new TimeSpan(0, hh, mm, ss, sss);
    }
    
    public static void WriteTime(ref Span<byte> buffer, TimeSpan? value)
    {
        if (value == null) return;
        var time = value.Value;
        time.Hours.TryFormat(buffer, out var written, NmeaIntFormat.IntD2.Format);
        Debug.Assert(written == 2, "hh == 2 char" );
        buffer = buffer[written..];
        time.Minutes.TryFormat(buffer, out written, NmeaIntFormat.IntD2.Format);
        Debug.Assert(written == 2, "mm == 2 char" );
        buffer = buffer[written..];
        time.Seconds.TryFormat(buffer, out written, NmeaIntFormat.IntD2.Format);
        Debug.Assert(written == 2, "ss == 2 char" );
        buffer = buffer[written..];
        buffer[0] = DigitSeparator;
        buffer = buffer[1..];
        time.Milliseconds.TryFormat(buffer, out written, NmeaIntFormat.IntD3.Format);
        Debug.Assert(written == 3, "sss == 3 char" );
        buffer = buffer[written..];
    }
    
    public static int SizeOfTime(TimeSpan? value)
    {
        return value == null ? 0 : 10 /* hhmmss.sss */;
    }

    /// <summary>
    /// x.x
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double ReadDouble(ReadOnlySpan<char> value)
    {
        return value.IsEmpty ? double.NaN : double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
    }
    public static void WriteDouble(ref Span<byte> buffer, double value, NmeaDoubleFormat format)
    {
        if (double.IsFinite(value))
        {
            value.TryFormat(buffer, out var written, format.Format, NumberFormatInfo.InvariantInfo);
            buffer = buffer[written..];
        }
    }
    
    public static int SizeOfDouble(double value, NmeaDoubleFormat format) => format.GetByteSize(value);

    /// <summary>
    /// x | xx | xxx | xxxx | xxxxx | xxxxxx
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int? ReadInt(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty) return null;
        return int.Parse(value, CultureInfo.InvariantCulture);
    }
    
    public static void WriteInt(ref Span<byte> buffer, int? value, NmeaIntFormat format)
    {
        if (value == null) return;
        value.Value.TryFormat(buffer, out var written,format.Format, NumberFormatInfo.InvariantInfo);
        buffer = buffer[written..];
    }

    public static int SizeOfInt(int? value, NmeaIntFormat format) => format.GetByteSize(value);
    
    public static int CountDigits(int value)
    {
        return value switch
        {
            0 => 1,
            < 0 => CountDigits((uint)-value) + 1,
            _ => CountDigits((uint)value)
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountDigits(uint value)
    {
        // Algorithm based on https://lemire.me/blog/2021/06/03/computing-the-number-of-digits-of-an-integer-even-faster.
        ReadOnlySpan<long> table =
        [
            4294967296,
            8589934582,
            8589934582,
            8589934582,
            12884901788,
            12884901788,
            12884901788,
            17179868184,
            17179868184,
            17179868184,
            21474826480,
            21474826480,
            21474826480,
            21474826480,
            25769703776,
            25769703776,
            25769703776,
            30063771072,
            30063771072,
            30063771072,
            34349738368,
            34349738368,
            34349738368,
            34349738368,
            38554705664,
            38554705664,
            38554705664,
            41949672960,
            41949672960,
            41949672960,
            42949672960,
            42949672960
        ];
        Debug.Assert(table.Length == 32, "Every result of uint.Log2(value) needs a long entry in the table.");

        // TODO: Replace with table[uint.Log2(value)] once https://github.com/dotnet/runtime/issues/79257 is fixed
        var tableValue = Unsafe.Add(ref MemoryMarshal.GetReference(table), uint.Log2(value));
        return (int)((value + tableValue) >> 32);
    }


    
}