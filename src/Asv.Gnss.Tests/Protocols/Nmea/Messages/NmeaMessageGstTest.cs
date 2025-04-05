using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Tests;


[TestSubject(typeof(NmeaMessageGst))]
public class NmeaMessageGstTest(ITestOutputHelper output) 
    : NmeaMessageTestBase<NmeaMessageGst>(output, TestMessages)
{
    private static readonly Dictionary<string, string> TestMessages = new()
    {
        {
            // 0
            "$GPGST,203017.00,1.25,0.02,0.01,-16.7566,0.02,0.01,0.03*7D",
            "$GPGST,203017.000,1.250,0.020,0.010,-16.7566,0.020,0.010,0.030"
        },
        {
            // 1
            "$GNGST,205246.00,1.19,0.02,0.01,-2.4501,0.02,0.01,0.03*5B",
            "$GNGST,205246.000,1.190,0.020,0.010,-2.4501,0.020,0.010,0.030"
        },
        {
            // 2
            "$GPGST,172814.0,0.006,0.023,0.020,273.6,0.023,0.020,0.031*6A",
            "$GPGST,172814.000,0.006,0.023,0.020,273.6000,0.023,0.020,0.031"
        },
        
    };

}