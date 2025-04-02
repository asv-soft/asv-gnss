using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests.Protocols.Nmea.Messages;

[TestSubject(typeof(NmeaMessageGsa))]
public class NmeaMessageGsaTest
{

    [Theory]
    [InlineData("$GPGSA,A,3,,,,,,16,18,,22,24,,,3.6,2.1,2.2*3C\r\n")]
    public void Deserialize_ShouldParseCorrectly_WithCompleteData(string dataString)
    {
        ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
        var msg = new NmeaMessageGsa();
        msg.Deserialize(ref data);
    }
    
    [Theory]
    [InlineData("$GPGSA,M,3,05,02,31,06,19,29,20,12,24,25,,,0.9,0.5,0.7*35\r\n")]
    [InlineData("$GNGSA,M,3,03,14,17,06,12,19,02,01,24,32,,,0.8,0.5,0.6*22\r\n")]
    [InlineData("$GNGSA,M,3,66,85,75,67,73,84,83,,,,,,0.8,0.5,0.6*26\r\n")]
    [InlineData("$GNGSA,M,3,12,11,33,31,03,24,25,08,,,,,0.8,0.5,0.6*20\r\n")]
    [InlineData("$GNGSA,M,3,28,46,36,27,39,23,43,37,,,,,0.8,0.5,0.6*2B\r\n")]
    public void Deserialize_ShouldParseRuntimeCorrectly_WithCompleteData(string dataString)
    {
        ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
        var msg = new NmeaMessageGsa();
        msg.Deserialize(ref data);
    }
}