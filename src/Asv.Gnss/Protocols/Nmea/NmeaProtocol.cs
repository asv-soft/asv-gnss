using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Asv.IO;
using DotNext;

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
    
    public static NmeaDoubleFormat LatitudeFormat = NmeaDoubleFormat.Double4X7;
    private const string LatitudeSouthChars = "Ss";
    private const string LatitudeNorthChars = "Nn";
    public static NmeaDoubleFormat LongitudeFormat = NmeaDoubleFormat.Double4X7;
    private const string LongitudeEastChars = "Ee";
    private const string LongitudeWestChars = "Ww";
    
    public static ProtocolInfo Info { get; } = new("NAME", "NMEA 0183");
    public static Encoding Encoding => Encoding.ASCII;
    
    
    public static void RegisterNmeaProtocol(this IProtocolParserBuilder builder, Action<IProtocolMessageFactoryBuilder<NmeaMessageBase, NmeaMessageId>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<NmeaMessageBase, NmeaMessageId>(Info);
        // register default messages
        factory
            .Add<NmeaMessageGbs>()
            .Add<NmeaMessageGga>()
            .Add<NmeaMessageGll>();
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(Info, (core,stat) => new NmeaMessageParser(messageFactory, core,stat));
    }
    
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

    #region Time
    
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

    #endregion

    #region Double

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

    #endregion

    #region Int

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

    #endregion
    
    #region Latitude 

    public static void ReadLatitude(ReadOnlySpan<char> digit,ReadOnlySpan<char> northOrSouth, out double field)
    {
        if (northOrSouth.IsEmpty)
        {
            field = double.NaN;
            return;
        }

        ReadDouble(digit, out var lat);
        field = ConvertToDecimalDegrees(in lat);

        if (northOrSouth[0].IsOneOf(LatitudeSouthChars))
        {
            field *= -1;
        }
        else if (northOrSouth[0].IsOneOf(LatitudeNorthChars) == false)
        {
            throw new Exception($"Latitude char must be {LatitudeSouthChars} or {LatitudeNorthChars}");
        }
    }

    public static void WriteLatitude(ref Span<byte> buffer, in double field)
    {
        WriteDouble(ref buffer, ConvertToDdmmMmmmmm(field), LatitudeFormat);
        WriteSeparator(ref buffer);
        buffer[0] = field < 0 ? (byte)LatitudeSouthChars[0] : (byte)LatitudeNorthChars[0];
        buffer = buffer[1..];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfLatitude(in double latitude) => LatitudeFormat.GetByteSize(ConvertToDdmmMmmmmm(latitude)) + 1 /*COMA*/ + 1 /*N/S*/;

    /// <summary>
    /// Converts NMEA coordinate format (ddmm.mmmmmmm) to decimal degrees (dd.dddddd).
    /// </summary>
    /// <param name="value">Coordinate in NMEA format (e.g. "4916.45" => 49° 16.45')</param>
    /// <returns>Decimal degrees or NaN if invalid input</returns>
    private static double ConvertToDecimalDegrees(in double value)
    {
        var degrees = (int)(value / 100);
        var minutes = value - degrees * 100;
        return degrees + (minutes / 60.0);
    }
    
    /// <summary>
    /// Converts decimal degrees (dd.dddddd) to NMEA coordinate format (ddmm.mmmmmmm).
    /// </summary>
    /// <param name="decimalDegrees">Input in decimal degrees (e.g. 49.274167)</param>
    /// <returns>Coordinate in ddmm.mmmmmmm format</returns>
    private static double ConvertToDdmmMmmmmm(double decimalDegrees)
    {
        if (!double.IsFinite(decimalDegrees))
            return double.NaN;

        decimalDegrees = Math.Abs(decimalDegrees);
        var degrees = (int)decimalDegrees;
        var minutes = (decimalDegrees - degrees) * 60.0;

        return degrees * 100 + minutes;
    }
    
    #endregion

    #region Longitude

    public static void ReadLongitude(ReadOnlySpan<char> digit, ReadOnlySpan<char> eastOrWest, out double field)
    {
        if (eastOrWest.IsEmpty)
        {
            field = double.NaN;
            return;
        }

        ReadDouble(digit, out var lat);
        field = ConvertToDecimalDegrees(in lat);

        if (eastOrWest[0].IsOneOf(LongitudeWestChars))
        {
            field *= -1;
        }
        else if (eastOrWest[0].IsOneOf(LongitudeEastChars) == false)
        {
            throw new Exception($"Longitude char must be {LongitudeEastChars} or {LongitudeWestChars}");
        }
    }
    
    public static void WriteLongitude(ref Span<byte> buffer, in double longitude)
    {
        WriteDouble(ref buffer, ConvertToDdmmMmmmmm(longitude), LongitudeFormat);
        WriteSeparator(ref buffer);
        buffer[0] = longitude < 0 ? (byte)LongitudeWestChars[0] : (byte)LongitudeEastChars[0];
        buffer = buffer[1..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfLongitude(in double longitude) => LongitudeFormat.GetByteSize(ConvertToDdmmMmmmmm(longitude)) + 1 /*COMA*/ + 1 /*E/W*/;
    
    #endregion

    #region String

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadString(ReadOnlySpan<char> token) => token.IsEmpty ? string.Empty : token.ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteString(ref Span<byte> buffer, string? value)
    {
        if (value == null) return;
        var count = Encoding.GetBytes(value, buffer);
        buffer = buffer[count..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfString(string? value) => value == null ? 0 : Encoding.GetByteCount(value);

    #endregion

    #region GpsQuality

    public static void ReadGpsQuality(ReadOnlySpan<char> buffer, out NmeaGpsQuality? value)
    {
        ReadInt(buffer, out var temp);
        if (temp.HasValue)
        {
            value = (NmeaGpsQuality)temp.Value;    
        }
        else
        {
            value = null;
        }
    }
    
    public static void WriteGpsQuality(ref Span<byte> buffer, in NmeaGpsQuality? value)
    {
        WriteInt(ref buffer, (int?)value, NmeaIntFormat.IntD1);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfGpsQuality(in NmeaGpsQuality? gpsQuality) => SizeOfInt((int?)gpsQuality, NmeaIntFormat.IntD1);

    #endregion
    
    #region Status

    public static void ReadDataStatus(ref ReadOnlySpan<char> buffer, out NmeaDataStatus? value)
    {
        if (buffer.IsEmpty)
        {
            value = null;
            return;
        }
        value = (NmeaDataStatus)buffer[0];
        buffer = buffer[1..];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDataStatus(ref Span<byte> buffer, in NmeaDataStatus? value)
    {
        if (value != null)
        {
            buffer[0] = (byte)value;    
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfStatus(NmeaDataStatus? status) => status == null ? 0 : 1;

    #endregion

    #region PositioningSystemMode
    
    public static void ReadPositioningSystemMode(ref ReadOnlySpan<char> buffer, out NmeaPositioningSystemMode? value)
    {
        if (buffer.IsEmpty)
        {
            value = null;
            return;
        }
        value = (NmeaPositioningSystemMode)buffer[0];
        buffer = buffer[1..];
    }

    public static void WritePositioningSystemMode(ref Span<byte> buffer, in NmeaPositioningSystemMode? value)
    {
        if (value == null)
        {
            return;
        }
        buffer[0] = (byte)value.Value;
        buffer = buffer[1..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfPositioningSystemMode(NmeaPositioningSystemMode? status) => status == null ? 0 : 1;

    #endregion

    #region FixMode

    public static void ReadFixMode(ref ReadOnlySpan<char> buffer, out NmeaFixQuality? value)
    {
        if (buffer.IsEmpty)
        {
            value = null;
            return;
        }
        value = (NmeaFixQuality)buffer[0];
        buffer = buffer[1..];
    }

    public static void WriteFixMode(ref Span<byte> buffer, in NmeaFixQuality? value)
    {
        if (value == null)
        {
            return;
        }
        buffer[0] = (byte)value.Value;
        buffer = buffer[1..];
    }

    public static int SizeOfFixMode(in NmeaFixQuality? value) => value == null ? 0 : 1;

    #endregion

    #region DopMode

    public static void ReadDopMode(ref ReadOnlySpan<char> buffer, out NmeaDopMode? value)
    {
        if (buffer.IsEmpty)
        {
            value = null;
            return;
        }
        value = (NmeaDopMode)buffer[0];
        buffer = buffer[1..];
    }

    public static void WriteDopMode(ref Span<byte> buffer, in NmeaDopMode? value)
    {
        if (value == null)
        {
            return;
        }
        buffer[0] = (byte)value.Value;
        buffer = buffer[1..];
    }

    public static int SizeOfDopMode(in NmeaDopMode? value) => value == null ? 0 : 1;

    #endregion
}