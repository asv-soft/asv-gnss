using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(NmeaMessageGll))]
public class NmeaMessageGllTest(ITestOutputHelper output) 
    : NmeaMessageTestBase<NmeaMessageGll>(output, TestMessages)
{
    private static readonly Dictionary<string, string> TestMessages = new()
    {
        {
            // 0
            "$GPGLL,5109.0262317,N,11401.8407304,W,202725.00,A,D*79",
            "$GPGLL,5109.0262317,N,11401.8407304,W,202725.000,A,D"
        },
        {
            // 0
            "$GPGLL,3751.65,S,14507.36,E*77",
            "$GPGLL,3751.6500000,S,14507.3600000,E,,,"
        },
        {
            // 0
            "$GPGLL,5133.81,N,00042.25,W*75",
            "$GPGLL,5133.8100000,N,0042.2500000,W,,,"
        },
        {
            // v1.0.0 NmeaTests.TestGGLL
            "$GPGLL,5508.7020098,N,06124.3378698,E,130521.00,A,M*64",
            "$GPGLL,5508.7020098,N,6124.3378698,E,130521.000,A,M"
        },
    };

}
