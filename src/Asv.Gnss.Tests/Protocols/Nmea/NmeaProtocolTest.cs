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
        var message = "$GPGGA,123456.00,1234.5678,N,12345.6789,W,1,12,0.8,545.4,M,46.9,M,,*47"u8.ToArray();
        var result = NmeaProtocol.TryGetMessageId(message.AsSpan(1), out var msgId);
        Assert.True(result);
        Assert.Equal("--GGA", msgId.MessageId);
    }

    [Fact]
    public void TryGetMessageId_WithShortMessage_ReturnsFalse()
    {
        var message = "$G,"u8.ToArray();
        var result = NmeaProtocol.TryGetMessageId(message.AsSpan(1), out var msgId);
        Assert.False(result);
    }

    [Fact]
    public void TryGetMessageId_WithNoComma_ReturnsFalse()
    {
        var message = "$GPGGA123456"u8.ToArray();
        var result = NmeaProtocol.TryGetMessageId(message.AsSpan(1), out var msgId);
        Assert.False(result);
    }

    [Fact]
    public void TryGetMessageId_WithProprietaryMessageId_ReturnsCorrectMessageId()
    {
        var message = "$PTEST,DATA,123*4F"u8.ToArray();
        var result = NmeaProtocol.TryGetMessageId(message.AsSpan(1), out var msgId);
        Assert.True(result);
        Assert.Equal("PTEST", msgId.MessageId);
        Assert.True(msgId.IsProprietary);
    }
}