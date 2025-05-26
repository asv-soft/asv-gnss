using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Asv.Gnss.Test;

[TestSubject(typeof(NmeaMessageGbs))]
public class NmeaMessageGbsTest(ITestOutputHelper output) 
    : NmeaMessageTestBase<NmeaMessageGbs>(output, TestMessages)
{
    private static readonly Dictionary<string, string> TestMessages = new()
    {
        {
            // 0
            "$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D",
            "$GPGBS,015509.000,-0.031,-0.186,0.219,19,0.000,-0.354,6.972,"
        },
    };
   
    
}   