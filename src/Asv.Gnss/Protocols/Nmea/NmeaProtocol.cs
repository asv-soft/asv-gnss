using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

public static class NmeaProtocol
{
    public const byte ComaByte = (byte)TokenSeparator;
    public const char TokenSeparator = ',';
    public const char StartCrcChar = '*';
    public const byte StartCrcByte = (byte)StartCrcChar;
    
    public const byte StartMessageByte1 = (byte)'$';
    public const byte StartMessageByte2 = (byte)'!';
    public const char EndMessage1 = '\r'; //0x0D;
    public const byte EndMessageByte1 = (byte)EndMessage1; 
    public const char EndMessage2 = '\n'; // 0x0A;
    public const byte EndMessageByte2 = (byte)EndMessage2;

    public const char DigitSeparator = '.';
    public const byte DigitSeparatorByte = (byte)DigitSeparator;
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
    
    public static bool TryReadNextToken(ref ReadOnlySpan<char> buffer, out ReadOnlySpan<char> token)
    {
        if (buffer.IsEmpty)
        {
            token = ReadOnlySpan<char>.Empty;
            return false;
        }
        var idEndIndex = buffer.IndexOfAny(TokenSeparator, StartCrcChar, EndMessage1);
        if (idEndIndex == -1)
        {
            token = buffer;
            buffer = ReadOnlySpan<char>.Empty;
            return true;
        }
        token = buffer[..idEndIndex];
        buffer = buffer[(idEndIndex+1)..];
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSeparator(ref Span<byte> buffer) => BinSerialize.WriteByte(ref buffer, ComaByte);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfSeparator() => 1 /*COMA*/;

    /// <summary>
    /// Parses UTC time in the format hhmmss[.sss] into a nullable TimeSpan.
    /// </summary>
    /// <param name="value">Input time as a span of characters (e.g., "123519.123").</param>
    /// <param name="field">Output nullable TimeSpan, or null if empty or invalid.</param>
    public static void ReadTime(ReadOnlySpan<char> value, out TimeSpan? field)
    {
        field = null;
        if (value.IsEmpty)
        {
            return;
        }

        var dotIndex = value.IndexOf(DigitSeparator);
        ReadOnlySpan<char> hmsSpan;
        var milliseconds = 0;

        if (dotIndex >= 0)
        {
            hmsSpan = value[..dotIndex];
            var fracSpan = value[(dotIndex + 1)..];
            if (fracSpan.Length > 3) fracSpan = fracSpan[..3]; // cap to milliseconds
            // pad right if needed (e.g. ".1" -> "100")
            Span<char> padded = stackalloc char[3];
            fracSpan.CopyTo(padded);
            for (var i = fracSpan.Length; i < 3; i++)
            {
                padded[i] = '0';
            }

            if (!int.TryParse(padded, out milliseconds))
                return;
        }
        else
        {
            hmsSpan = value;
        }

        if (!int.TryParse(hmsSpan, out var hms) || hms < 0 || hms > 235959)
            return;

        var hours = hms / 10000;
        var minutes = (hms % 10000) / 100;
        var seconds = hms % 100;

        // Basic validation
        if ((hours is < 0 or > 23) || minutes is < 0 or > 59 || seconds is < 0 or > 59)
            return;

        field = new TimeSpan(0, hours, minutes, seconds, milliseconds);
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
        buffer[0] = DigitSeparatorByte;
        buffer = buffer[1..];
        time.Milliseconds.TryFormat(buffer, out written, NmeaIntFormat.IntD3.Format);
        Debug.Assert(written == 3, "sss == 3 char" );
        buffer = buffer[written..];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfTime(in TimeSpan? value)
    {
        return value == null ? 0 : 10 /* hhmmss.sss */;
    }

    /// <summary>
    /// Parses a double value from a character span.
    /// Returns NaN if the input is empty or invalid.
    /// </summary>
    /// <param name="value">Input span containing the number.</param>
    /// <param name="field">Parsed double value or NaN.</param>
    public static void ReadDouble(ReadOnlySpan<char> value, out double field)
    {
        if (value.IsEmpty || !double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out field))
        {
            field = double.NaN;
        }
    }
    public static void WriteDouble(ref Span<byte> buffer,in double value,in NmeaDoubleFormat format)
    {
        if (double.IsFinite(value))
        {
            value.TryFormat(buffer, out var written, format.Format, NumberFormatInfo.InvariantInfo);
            buffer = buffer[written..];
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfDouble(in double value,in NmeaDoubleFormat format) => format.GetByteSize(in value);

    /// <summary>
    /// Parses an integer value from a character span.
    /// Returns null if the input is empty or invalid.
    /// </summary>
    /// <param name="value">Input span containing the integer.</param>
    /// <param name="field">Parsed integer value or null.</param>
    public static void ReadInt(ReadOnlySpan<char> value, out int? field)
    {
        if (value.IsEmpty || !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            field = null;
            return;
        }

        field = result;
    }
    
    public static void WriteInt(ref Span<byte> buffer,in int? value,in NmeaIntFormat format)
    {
        if (value == null) return;
        value.Value.TryFormat(buffer, out var written,format.Format, NumberFormatInfo.InvariantInfo);
        buffer = buffer[written..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfInt(in int? value,in NmeaIntFormat format) => format.GetByteSize(value);
    
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