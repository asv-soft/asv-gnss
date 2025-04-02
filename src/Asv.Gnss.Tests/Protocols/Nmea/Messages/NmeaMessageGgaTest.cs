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
    [InlineData("GNGGA,001043.00,4404.14036,N,12118.85961,W,1,12,0.98,1113.0,M,-21.3,M*47\r\n")]
    [InlineData("GNGGA,001043.00,4404.14036,N,12118.85961,W,1,12,0.98,1113.0,M,-21.3,M*47\r")]
    [InlineData("GNGGA,001043.00,4404.14036,N,12118.85961,W,1,12,0.98,1113.0,M,-21.3,M*47")]
    [InlineData("GNGGA,001043.00,4404.14036,N,12118.85961,W,1,12,0.98,1113.0,M,-21.3,M")]
    public void Deserialize_ShouldParseCorrectly_WithCompleteData(string dataString)
    {
        ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
        var msg = new NmeaMessageGga();
        msg.Deserialize(ref data);
        Assert.Equal(0, data.Length);
        Assert.Equal(new TimeSpan(0, 00, 10, 43, 00), msg.Time);
        Assert.Equal(44.069006, msg.Latitude, 6);
        Assert.Equal(-121.314327, msg.Longitude, 6);
        Assert.Equal(NmeaGpsQuality.GpsFix, msg.GpsQuality);
        Assert.Equal(12, msg.NumberOfSatellites);
        Assert.Equal(0.98, msg.HorizontalDilutionPrecision);
        Assert.Equal(1113.0, msg.AntennaAltitudeMsl);
        Assert.Equal("M", msg.AntennaAltitudeUnits);
        Assert.Equal(-21.3, msg.GeoidalSeparation);
        Assert.Equal("M", msg.GeoidalSeparationUnits);
    }
    
}