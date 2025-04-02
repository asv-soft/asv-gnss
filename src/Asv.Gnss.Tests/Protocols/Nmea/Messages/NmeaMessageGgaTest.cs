using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests.Messages;

[TestSubject(typeof(NmeaMessageGga))]
public class NmeaMessageGgaTest
{

    [Theory]
    [InlineData("$GNGGA,001043.00,4404.14036,N,12118.85961,W,1,12,0.98,1113.0,M,-21.3,M*47\r\n")]
    public void Deserialize_ShouldParseCorrectly_WithCompleteData(string dataString)
    {
        ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
        var msg = new NmeaMessageGga();
        msg.Deserialize(ref data);
        Assert.Equal(0, data.Length);
        
        
        
    }
    
}