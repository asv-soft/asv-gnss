using System;

namespace Asv.Gnss;

/// <summary>Receiver transport encoding of a native Mode S downlink.</summary>
public enum AdsbFrameFormat { Avr, Beast }

/// <summary>Which parity validation is possible without an aircraft-address cache.</summary>
public enum AdsbParityStatus { Valid, Invalid, AddressParityUnverified, InterrogatorParity }

internal readonly record struct AdsbFrameHeader(AdsbFrameFormat Format, char Marker, ulong? Timestamp, byte? Signal, int RawSize, int Size);

internal static class AdsbFrameCodec
{
    internal const byte Escape = 0x1A;
    internal const int MaximumFrameSize = 64;
    internal const ulong MaximumTimestamp = 0xFFFFFFFFFFFF;

    internal static bool IsAvrStart(byte value) => value is (byte)'*' or (byte)':' or (byte)'@' or (byte)'%' or (byte)'<';

    internal static AdsbFrameHeader Read(ReadOnlySpan<byte> source, Span<byte> raw)
    {
        if (source.IsEmpty) throw new FormatException("Empty ADS-B receiver frame.");
        if (source[0] == Escape)
        {
            if (source.Length < 2 || source[1] is not (0x32 or 0x33))
                throw new FormatException("Expected a Beast Mode S frame (type 2 or 3).");
            var rawSize = source[1] == 0x32 ? 7 : 14;
            var index = 2;
            ulong timestamp = 0;
            for (var i = 0; i < 6; i++) timestamp = (timestamp << 8) | ReadEscaped(source, ref index);
            var signal = ReadEscaped(source, ref index);
            for (var i = 0; i < rawSize; i++) raw[i] = ReadEscaped(source, ref index);
            return new(AdsbFrameFormat.Beast, '*', timestamp, signal, rawSize, index);
        }
        if (!IsAvrStart(source[0])) throw new FormatException("Unknown ADS-B receiver framing.");
        var end = source.IndexOf((byte)';');
        if (end < 0 || end + 1 > MaximumFrameSize) throw new FormatException("Incomplete or oversized AVR frame.");
        var marker = (char)source[0];
        var offset = 1;
        ulong? time = null;
        byte? level = null;
        if (marker is '@' or '%' or '<')
        {
            time = 0;
            for (var i = 0; i < 6; i++) time = (time.Value << 8) | ReadHex(source[..end], ref offset);
            if (marker == '<') level = ReadHex(source[..end], ref offset);
        }
        var count = end - offset;
        if (count is not (14 or 28)) throw new FormatException("An AVR Mode S frame needs 14 or 28 hexadecimal digits.");
        for (var i = 0; i < count / 2; i++) raw[i] = ReadHex(source[..end], ref offset);
        return new(AdsbFrameFormat.Avr, marker, time, level, count / 2, end + 1);
    }

    internal static int Size(AdsbDfMessage message)
    {
        ValidateTransport(message);
        if (message.FrameFormat == AdsbFrameFormat.Avr)
            return 2 + message.RawData.Length * 2 + (message.AvrMarker is '@' or '%' or '<' ? 12 : 0) + (message.AvrMarker == '<' ? 2 : 0);
        var size = 2 + 7 + message.RawData.Length;
        var timestamp = message.ReceiverTimestamp!.Value;
        for (var i = 0; i < 6; i++) if ((byte)(timestamp >> (i * 8)) == Escape) size++;
        if (message.SignalLevel == Escape) size++;
        foreach (var value in message.RawData.Span) if (value == Escape) size++;
        return size;
    }

    internal static void Write(AdsbDfMessage message, ref Span<byte> destination)
    {
        var size = Size(message);
        if (destination.Length < size) throw new ArgumentException("Insufficient ADS-B output buffer.", nameof(destination));
        var output = destination[..size];
        var index = 0;
        if (message.FrameFormat == AdsbFrameFormat.Avr)
        {
            output[index++] = (byte)message.AvrMarker;
            if (message.AvrMarker is '@' or '%' or '<')
            {
                for (var i = 5; i >= 0; i--) WriteHex(output, ref index, (byte)(message.ReceiverTimestamp!.Value >> (i * 8)));
                if (message.AvrMarker == '<') WriteHex(output, ref index, message.SignalLevel!.Value);
            }
            foreach (var value in message.RawData.Span) WriteHex(output, ref index, value);
            output[index++] = (byte)';';
        }
        else
        {
            output[index++] = Escape;
            output[index++] = message.RawData.Length == 7 ? (byte)0x32 : (byte)0x33;
            for (var i = 5; i >= 0; i--) WriteEscaped(output, ref index, (byte)(message.ReceiverTimestamp!.Value >> (i * 8)));
            WriteEscaped(output, ref index, message.SignalLevel!.Value);
            foreach (var value in message.RawData.Span) WriteEscaped(output, ref index, value);
        }
        destination = destination[size..];
    }

    private static void ValidateTransport(AdsbDfMessage message)
    {
        if (message.FrameFormat is not (AdsbFrameFormat.Avr or AdsbFrameFormat.Beast))
            throw new ArgumentOutOfRangeException(nameof(message.FrameFormat));
        if (message.ReceiverTimestamp > MaximumTimestamp) throw new ArgumentOutOfRangeException(nameof(message.ReceiverTimestamp));
        if (message.FrameFormat == AdsbFrameFormat.Avr && message.AvrMarker is not ('*' or ':' or '@' or '%' or '<'))
            throw new ArgumentOutOfRangeException(nameof(message.AvrMarker));
        if ((message.FrameFormat == AdsbFrameFormat.Beast || message.AvrMarker is '@' or '%' or '<') && message.ReceiverTimestamp == null)
            throw new InvalidOperationException("This ADS-B transport requires a 48-bit receiver timestamp.");
        if ((message.FrameFormat == AdsbFrameFormat.Beast || message.AvrMarker == '<') && message.SignalLevel == null)
            throw new InvalidOperationException("This ADS-B transport requires a signal level.");
    }

    private static byte ReadEscaped(ReadOnlySpan<byte> source, ref int index)
    {
        if (index >= source.Length) throw new FormatException("Truncated Beast frame.");
        var value = source[index++];
        if (value == Escape && (index >= source.Length || source[index++] != Escape))
            throw new FormatException("Invalid Beast escape sequence.");
        return value;
    }

    private static byte ReadHex(ReadOnlySpan<byte> source, ref int index)
    {
        if (index + 2 > source.Length) throw new FormatException("Truncated AVR hexadecimal field.");
        var high = Hex(source[index++]);
        var low = Hex(source[index++]);
        if (high < 0 || low < 0) throw new FormatException("Invalid AVR hexadecimal digit.");
        return (byte)(high * 16 + low);
    }

    internal static int Hex(byte value) => value switch
    {
        >= (byte)'0' and <= (byte)'9' => value - '0',
        >= (byte)'A' and <= (byte)'F' => value - 'A' + 10,
        >= (byte)'a' and <= (byte)'f' => value - 'a' + 10,
        _ => -1,
    };

    private static void WriteHex(Span<byte> output, ref int index, byte value)
    {
        const string digits = "0123456789ABCDEF";
        output[index++] = (byte)digits[value >> 4];
        output[index++] = (byte)digits[value & 15];
    }

    private static void WriteEscaped(Span<byte> output, ref int index, byte value)
    {
        output[index++] = value;
        if (value == Escape) output[index++] = Escape;
    }
}
