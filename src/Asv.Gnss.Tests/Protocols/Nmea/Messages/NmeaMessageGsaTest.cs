using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Tests;

// https://receiverhelp.trimble.com/alloy-gnss/en-us/nmea0183-messages-gsa.html?TocPath=Output%20Messages%7CNMEA-0183%20messages%7C_____8
// https://docs.novatel.com/OEM7/Content/Logs/GPGSA.htm?tocpath=Commands%20%2526%20Logs%7CLogs%7CGNSS%20Logs%7C_____63
// https://www.trimble.com/Support/NMEA0183/Trimble%20NMEA%20Reference%20Guide.pdf

[TestSubject(typeof(NmeaMessageGsa))]
public class NmeaMessageGsaTest(ITestOutputHelper output) 
    : NmeaMessageTestBase<NmeaMessageGsa>(output, TestMessages)
{
    private static readonly Dictionary<string, string> TestMessages = new()
    {
        {
            // 0
            "$GPGSA,M,3,05,02,31,06,19,29,20,12,24,25,,,0.9,0.5,0.7*35",
            "$GPGSA,M,3,05,02,31,06,19,29,20,12,24,25,,,0.9,0.5,0.7,"
        },
        
        {
            // 1
            "$GNGSA,M,3,03,14,17,06,12,19,02,01,24,32,,,0.8,0.5,0.6*22",
            "$GNGSA,M,3,03,14,17,06,12,19,02,01,24,32,,,0.8,0.5,0.6,"
        },
        {
            // 2
            "$GNGSA,M,3,66,85,75,67,73,84,83,,,,,,0.8,0.5,0.6*26",
            "$GNGSA,M,3,66,85,75,67,73,84,83,,,,,,0.8,0.5,0.6,"
        },
        {
            // 3
            "$GNGSA,M,3,12,11,33,31,03,24,25,08,,,,,0.8,0.5,0.6*20",
            "$GNGSA,M,3,12,11,33,31,03,24,25,08,,,,,0.8,0.5,0.6,"
        },
        {
            // 4
            "$GNGSA,M,3,28,46,36,27,39,23,43,37,,,,,0.8,0.5,0.6*2B",
            "$GNGSA,M,3,28,46,36,27,39,23,43,37,,,,,0.8,0.5,0.6,"
        },
        {
            // 5
            "$GNGSA,A,3,21,5,29,25,12,10,26,2,,,,,1.2,0.7,1.0*27",
            "$GNGSA,A,3,21,05,29,25,12,10,26,02,,,,,1.2,0.7,1.0,"
        },
        {
            // 6
            "$GNGSA,A,3,65,67,80,81,82,88,66,,,,,,1.2,0.7,1.0*20",
            "$GNGSA,A,3,65,67,80,81,82,88,66,,,,,,1.2,0.7,1.0,"
        }
    };

    [Fact]
    public void Test1()
    {
        var data = "$GNGSA,A,3,31,06,11,20,05,12,28,29,09,,,,1.28,0.74,1.04,1*04\r\n";
        var origin = NmeaProtocol.Encoding.GetBytes(data);
        var originSpan = new ReadOnlySpan<byte>(origin);
        var message = new NmeaMessageGsa();
        message.Deserialize(ref originSpan);
        Assert.Equal(0,originSpan.Length);
        var serialized = new byte[data.Length];
        var serializedSpan = new Span<byte>(serialized);
        message.Serialize(ref serializedSpan);
        var str = NmeaProtocol.Encoding.GetString(serialized);
        Assert.Equal(data,str);
    }
}