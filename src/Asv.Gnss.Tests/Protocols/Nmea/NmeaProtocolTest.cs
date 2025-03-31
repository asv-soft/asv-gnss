using System;
using System.Text;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests;

[TestSubject(typeof(NmeaProtocol))]
public class NmeaProtocolTest
{

    [Fact]
    public void CalcCrc_WithKnownBuffer_ReturnsExpectedChecksum()
    {
        var buffer = "GPGLL,4916.45,N,12311.12,W,225444,A"u8.ToArray();
        var checksum = NmeaProtocol.CalcCrc(buffer);
        Assert.Equal("31", checksum);
    }

    [Fact]
    public void CalcCrc_WithEmptyBuffer_ReturnsZeroChecksum()
    {
        var checksum = NmeaProtocol.CalcCrc(Array.Empty<byte>());
        Assert.Equal("00", checksum);
    }

    [Fact]
    public void TryGetMessageId_WithValidMessage_ReturnsTrueAndCorrectMessageId()
    {
        var message = "$GPGGA,123456.00,1234.5678,N,12345.6789,W,1,12,0.8,545.4,M,46.9,M,,*47"u8.ToArray().AsSpan();
        
        var result = NmeaProtocol.TryGetMessageId(message, out var msgId, out _);
        Assert.True(result);
        Assert.Equal("GGA", msgId.MessageId);
    }

    [Fact]
    public void TryGetMessageId_WithShortMessage_ReturnsFalse()
    {
        var message = "$G,"u8.ToArray();
        var result = NmeaProtocol.TryGetMessageId(message.AsSpan(1), out var msgId, out _);
        Assert.False(result);
    }

    [Fact]
    public void TryGetMessageId_WithNoComma_ReturnsFalse()
    {
        var message = "$GPGGA123456"u8.ToArray();
        var result = NmeaProtocol.TryGetMessageId(message.AsSpan(1), out var msgId, out _);
        Assert.False(result);
    }

    [Fact]
    public void TryGetMessageId_WithProprietaryMessageId_ReturnsCorrectMessageId()
    {
        var message = "$PTEST,DATA,123*4F"u8.ToArray();
        var result = NmeaProtocol.TryGetMessageId(message.AsSpan(1), out var msgId, out _);
        Assert.True(result);
        Assert.Equal("TEST", msgId.MessageId);
    }
    
    [Theory]
    [InlineData("$GPGGA,123519,4807.038,N,01131.000,E*65\r", true, "GGA", NmeaTalkerClass.GlobalPositioningSystem)]
    [InlineData("!AIVDM,1,1,,B,15N:GPP000Or@`vKf@jWh0?N0<06,0*4F\r", true, "VDM", NmeaTalkerClass.Unknown)]
    [InlineData("$PGRME,22.0,M,52.9,M,51.0,M*14\r", true, "GRME", NmeaTalkerClass.ProprietaryCode)]
    [InlineData("$GPTXT,01,01,02,u-blox ag - www.u-blox.com*50\r", true, "TXT", NmeaTalkerClass.GlobalPositioningSystem)]
    [InlineData("GPTXT,01,01,02,u-blox ag - www.u-blox.com*50\r", true, "TXT", NmeaTalkerClass.GlobalPositioningSystem)]
    [InlineData("$XXBADMESSAGE*00\r", false, "", NmeaTalkerClass.Unknown)]
    [InlineData("", false, "", NmeaTalkerClass.Unknown)]
    public void TryGetMessageId_ShouldParseCorrectly(
        string rawMessage, bool expectedResult, string expectedMessageId, NmeaTalkerClass expectedTalkerClass)
    {
        var buffer = Encoding.ASCII.GetBytes(rawMessage);
        var result = NmeaProtocol.TryGetMessageId(buffer, out var messageId, out var talkerClass);

        Assert.Equal(expectedResult, result);
        if (expectedResult)
        {
            Assert.Equal(expectedMessageId, messageId.MessageId);
            Assert.Equal(expectedTalkerClass, talkerClass.Type);
        }
    }

    [Fact]
    public void TryGetMessageId_ShouldReturnFalse_OnIncompleteMessage()
    {
        var incompleteMessage = Encoding.ASCII.GetBytes("$GP\r");
        var result = NmeaProtocol.TryGetMessageId(incompleteMessage, out _, out _);
        
        Assert.False(result);
    }

    [Fact]
    public void TryGetMessageId_ShouldHandleProprietaryMessage()
    {
        var message = "$PASHR,123.4,T,1.2,M,3.4,M,5.6,M*1A\r";
        var buffer = Encoding.ASCII.GetBytes(message);
        
        var result = NmeaProtocol.TryGetMessageId(buffer, out var messageId, out var talkerClass);

        Assert.True(result);
        Assert.Equal(NmeaTalkerClass.ProprietaryCode, talkerClass.Type);
        Assert.Equal("ASHR", messageId.MessageId);
    }
    
    
    
    [Theory]
    [InlineData("123.456", 123.456)]
    [InlineData("-78.9", -78.9)]
    [InlineData("0", 0)]
    [InlineData("", double.NaN)]
    public void ReadDouble_ShouldParseCorrectly(string input, double expected)
    {
        var result = NmeaProtocol.ReadDouble(input.AsSpan());
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123.456, "F3", "123.456")]
    [InlineData(-78.9, "F1", "-78.9")]
    [InlineData(0, "G", "0")]
    public void WriteDouble_ShouldWriteCorrectly(double input, string format, string expected)
    {
        Span<byte> buffer = stackalloc byte[50];
        NmeaProtocol.WriteDouble(ref buffer, input, format.AsSpan());
        var result = Encoding.ASCII.GetString(buffer.Slice(0, expected.Length));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123.456, "F3", 7)]
    [InlineData(-78.9, "F1", 5)]
    [InlineData(0, "G", 1)]
    public void SizeOfDouble_ShouldCalculateCorrectSize(double input, string format, int expectedSize)
    {
        var size = NmeaProtocol.SizeOfDouble(input, format.AsSpan());
        Assert.Equal(expectedSize, size);
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData("-789", -789)]
    [InlineData("", null)]
    public void ReadInt_ShouldParseCorrectly(string input, int? expected)
    {
        var result = NmeaProtocol.ReadInt(input.AsSpan());
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123, "D", "123")]
    [InlineData(-78, "D", "-78")]
    [InlineData(null, "D", "")]
    public void WriteInt_ShouldWriteCorrectly(int? input, string format, string expected)
    {
        Span<byte> buffer = stackalloc byte[50];
        var originalBuffer = buffer;
        NmeaProtocol.WriteInt(ref buffer, input, format.AsSpan());

        if (input == null)
        {
            Assert.Equal(originalBuffer.Length, buffer.Length); // Ничего не записалось
        }
        else
        {
            var result = Encoding.ASCII.GetString(originalBuffer[..^buffer.Length]);
            Assert.Equal(expected, result);
        }
    }

    [Theory]
    [InlineData(12345, "D3", 5)]
    [InlineData(-12345, "D6", 6)]
    [InlineData(null, "D", 0)]
    public void SizeOfInt_ShouldCalculateCorrectSize(int? input, string format, int expectedSize)
    {
        var size = NmeaProtocol.SizeOfInt(input, format.AsSpan());
        Assert.Equal(expectedSize, size);
    }


    [Theory]
    [InlineData(0, 1)]
    [InlineData(9, 1)]
    [InlineData(10, 2)]
    [InlineData(12345, 5)]
    [InlineData(123456789, 9)] 
    public void CountDigits_ShouldReturnCorrectDigitCount(uint input, int expectedDigits)
    {
        var digits = NmeaProtocol.CountDigits(input);
        Assert.Equal(expectedDigits, digits);
    }
}