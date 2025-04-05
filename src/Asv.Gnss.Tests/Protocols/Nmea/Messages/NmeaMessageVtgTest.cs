using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Asv.Gnss.Tests;

[TestSubject(typeof(NmeaMessageVtg))]
public class NmeaMessageVtgTest(ITestOutputHelper output)
    : NmeaMessageTestBase<NmeaMessageVtg>(output, TestMessages)
{
    private static readonly Dictionary<string, string> TestMessages = new()
    {
        {
            // 0
            "$GPVTG,224.592,T,224.592,M,0.003,N,0.005,K,D*20",
            "$GPVTG,224.592,T,224.592,M,0.0030000,N,0.0050000,K,D"
        },
        {
            // 1
            "$GNVTG,139.969,T,139.969,M,0.007,N,0.013,K,D*3D",
            "$GNVTG,139.969,T,139.969,M,0.0070000,N,0.0130000,K,D"
        }
    };
}
