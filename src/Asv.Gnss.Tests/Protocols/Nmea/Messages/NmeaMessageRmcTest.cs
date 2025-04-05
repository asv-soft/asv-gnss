using System.Collections.Generic;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Tests;


[TestSubject(typeof(NmeaMessageRmc))]
public class NmeaMessageRmcTest(ITestOutputHelper output) : NmeaMessageTestBase<NmeaMessageRmc>(output, TestMessages)
{
    private static readonly Dictionary<string, string> TestMessages = new()
    {
        {
            // 0
            "$GPRMC,203522.00,A,5109.0262308,N,11401.8407342,W,0.004,133.4,130522,0.0,E,D*2B",
            "$GPRMC,203522.000,A,5109.0262308,N,11401.8407342,W,0.004,133.4,130522,0.0,E,D,"
        },
        {
            // 1
            "$GNRMC,204520.00,A,5109.0262239,N,11401.8407338,W,0.004,102.3,130522,0.0,E,D*3B",
            "$GNRMC,204520.000,A,5109.0262239,N,11401.8407338,W,0.004,102.3,130522,0.0,E,D,"
        },
    };
}