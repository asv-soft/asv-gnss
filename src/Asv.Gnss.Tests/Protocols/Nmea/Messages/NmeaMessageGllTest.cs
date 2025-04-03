using System;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests;

[TestSubject(typeof(NmeaMessageGll))]
public class NmeaMessageGllTest
{

    [Theory]
    [InlineData("$GNGLL,5109.0262321,N,11401.8407167,W,174738.00,A,D*6B\r\n")]
    [InlineData("$GNGLL,5109.0262321,N,11401.8407167,W,174738.00,A,D*6B\r")]
    [InlineData("$GNGLL,5109.0262321,N,11401.8407167,W,174738.00,A,D*6B")]
    [InlineData("$GNGLL,5109.0262321,N,11401.8407167,W,174738.00,A,D")]
    [InlineData("GNGLL,5109.0262321,N,11401.8407167,W,174738.00,A,D*6B")]
    public void Deserialize_ShouldParseCorrectly_WithCompleteData(string dataString)
    {
        ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
        var msg = new NmeaMessageGll();
        msg.Deserialize(ref data);
        Assert.Equal(0, data.Length);
        Assert.Equal(51.150437195, msg.Latitude, 6);
        Assert.Equal(-114.03067884, msg.Longitude, 6);
        Assert.Equal(new TimeSpan(0, 17, 47, 38, 0), msg.Time);
        Assert.Equal(NmeaDataStatus.Valid, msg.Status);
        Assert.Equal(NmeaPositioningSystemMode.Differential, msg.PositioningMode);
    }
    
    [Theory]
    [InlineData("$GPGLL,5109.0262317,N,11401.8407304,W,202725.00,A,D*79\r\n")]
    [InlineData("$GPGLL,3751.65,S,14507.36,E*77\r\n")]
    [InlineData("$GPGLL,4916.45,N,12311.12,W,225444,A\r\n")]
    [InlineData("$GPGLL,5133.81,N,00042.25,W*75\r\n")]
    public void Deserialize_ShouldParseRuntimeCorrectly_WithCompleteData(string dataString)
    {
        ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
        var msg = new NmeaMessageGll();
        msg.Deserialize(ref data);
    }
}