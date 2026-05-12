using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Asv.Gnss.Test;

[TestSubject(typeof(NmeaMessageGga))]
public class NmeaMessageGgaTest(ITestOutputHelper output) 
    : NmeaMessageTestBase<NmeaMessageGga>(output, TestMessages)
{
    private static readonly Dictionary<string, string> TestMessages = new()
    {
        {
            // 0
            "$GNGGA,001043.00,4404.14036,N,12118.85961,W,1,12,0.98,1113.0,M,-21.3,M*47",
            "$GNGGA,001043.000,4404.1403600,N,12118.8596100,W,1,12,0.980,1113.000,M,-21.300,M,,"
        },
        {
            // v1.0.0 NmeaTests.TestGGA
            "$GPGGA,125319.00,5508.7020098,N,06124.3378698,E,7,08,2.4,259.0000,M,-12.794,M,,*76",
            "$GPGGA,125319.000,5508.7020098,N,6124.3378698,E,7,8,2.400,259.000,M,-12.794,M,,"
        },
    };
   
    
}
