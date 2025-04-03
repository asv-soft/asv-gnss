using System;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests;

[TestSubject(typeof(NmeaMessageGsa))]
public class NmeaMessageGsaTest
{

    [Theory]
    [InlineData("$GPGSA,A,3,,,,,,16,18,,22,24,,,3.6,2.1,2.2*3C\r\n")]
    [InlineData("GPGSA,A,3,,,,,,16,18,,22,24,,,3.6,2.1,2.2*3C\r\n")]
    [InlineData("GPGSA,A,3,,,,,,16,18,,22,24,,,3.6,2.1,2.2*3C\r")]
    [InlineData("GPGSA,A,3,,,,,,16,18,,22,24,,,3.6,2.1,2.2*3C")]
    [InlineData("GPGSA,A,3,,,,,,16,18,,22,24,,,3.6,2.1,2.2")]
    public void Deserialize_ShouldParseCorrectly_WithCompleteData(string dataString)
    {
        ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
        var msg = new NmeaMessageGsa();
        msg.Deserialize(ref data);
        Assert.Equal(NmeaDopMode.Automatic2D3D, msg.DopMode);
        Assert.Equal( NmeaFixQuality.Fix3D , msg.FixMode);
        Assert.Equal(16, msg.Satellites[0]);
        
    }
    
    [Theory]
    [InlineData("$GPGSA,M,3,05,02,31,06,19,29,20,12,24,25,,,0.9,0.5,0.7*35\r\n")]
    [InlineData("$GNGSA,M,3,03,14,17,06,12,19,02,01,24,32,,,0.8,0.5,0.6*22\r\n")]
    [InlineData("$GNGSA,M,3,66,85,75,67,73,84,83,,,,,,0.8,0.5,0.6*26\r\n")]
    [InlineData("$GNGSA,M,3,12,11,33,31,03,24,25,08,,,,,0.8,0.5,0.6*20\r\n")]
    [InlineData("$GNGSA,M,3,28,46,36,27,39,23,43,37,,,,,0.8,0.5,0.6*2B\r\n")]
    [InlineData("$GNGSA,A,3,21,5,29,25,12,10,26,2,,,,,1.2,0.7,1.0*27\r\n")]
    [InlineData("$GNGSA,A,3,65,67,80,81,82,88,66,,,,,,1.2,0.7,1.0*20\r\n")]
    public void Deserialize_ShouldParseRuntimeCorrectly_WithCompleteData(string dataString)
    {
        // https://receiverhelp.trimble.com/alloy-gnss/en-us/nmea0183-messages-gsa.html?TocPath=Output%20Messages%7CNMEA-0183%20messages%7C_____8
        // https://docs.novatel.com/OEM7/Content/Logs/GPGSA.htm?tocpath=Commands%20%2526%20Logs%7CLogs%7CGNSS%20Logs%7C_____63
        // https://www.trimble.com/Support/NMEA0183/Trimble%20NMEA%20Reference%20Guide.pdf
        ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
        var msg = new NmeaMessageGsa();
        msg.Deserialize(ref data);
    }
}