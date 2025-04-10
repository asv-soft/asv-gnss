using System.Collections.Generic;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Test;

[TestSubject(typeof(NmeaMessageZda))]
public class NmeaMessageZdaTest(ITestOutputHelper output)
    : NmeaMessageTestBase<NmeaMessageZda>(output, TestMessages)
{
    private static readonly Dictionary<string, string> TestMessages = new()
    {
        {
            // 0
            "$GPZDA,204007.00,13,05,2022,,*62",
            "$GPZDA,204007.000,13,05,2022,,"
        },
       
    };
}