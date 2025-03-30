using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

public static class NmeaProtocol
{
    public const byte ComaByte = (byte)',';
    public const byte StartCrcByte = (byte)'*';
    public const byte StartMessageByte1 = (byte)'$';
    public const byte StartMessageByte2 = (byte)'!';
    public const byte EndMessageByte = 0x0D;
    public const char ProprietaryPrefix = 'P';
    public const char TalkerIgnoreSymbol = '-';
    public const string UnknownMessageId = "--???";

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

    private static bool TrySliceMessageId(ref ReadOnlySpan<byte> rawMessage)
    {
        if (rawMessage.Length == 0)
        {
            return false;
        }
        if (rawMessage[0] is StartMessageByte1 or StartMessageByte2)
        {
            rawMessage = rawMessage[1..]; // skip the start byte
        }
        
        var idEndIndex = rawMessage.IndexOf(ComaByte);
        if (idEndIndex <= 2) // Message ID must be at least 2 characters long: Talker Identifiers(1-2) + Message Type(>=1) + Coma
        {
            return false;
        }
        rawMessage = rawMessage[..(idEndIndex)];
        return true;
    }
    
    
    public static bool TryGetMessageId(ReadOnlySpan<byte> rawMessage, out NmeaMessageId msgId)
    {
        if (TrySliceMessageId(ref rawMessage) == false)
        {
            msgId = default;
            return false;
        }
        Span<char> msgIdChars = stackalloc char[rawMessage.Length]; // ASCII encoding is 1 byte per character
        Encoding.GetChars(rawMessage,msgIdChars);
        msgId = new NmeaMessageId(msgIdChars);
        return true;
    }
    
    public static void GetMessageIdAndTalkerId(ref ReadOnlySpan<byte> message, out NmeaMessageId msgId, out NmeaTalkerId talkerClass)
    {
        if (TrySliceMessageId(ref message) == false)
        {
            throw new Exception("Failed to slice message ID from the message.");
        }
        Span<char> msgIdChars = stackalloc char[message.Length]; // ASCII encoding is 1 byte per character
        Encoding.GetChars(message,msgIdChars);
        msgId = new NmeaMessageId(msgIdChars);
        talkerClass = new NmeaTalkerId(msgIdChars);
        return true;
    }
}