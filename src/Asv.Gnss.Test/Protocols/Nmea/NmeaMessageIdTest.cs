using System;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(NmeaMessageId))]
public class NmeaMessageIdTest
{

    [Fact]
    public void Constructor_ShouldSetMessageId()
    {
        var id = new NmeaMessageId("GGA");
        
        Assert.Equal("GGA", id.MessageId);
    }

    [Fact]
    public void Constructor_FromSpan_ShouldSetMessageId()
    {
        ReadOnlySpan<char> msgSpan = "RMC";
        var id = new NmeaMessageId(msgSpan);
        
        Assert.Equal("RMC", id.MessageId);
    }


    [Fact]
    public void Equals_ShouldReturnTrue_WhenIdsMatch_IgnoringCase()
    {
        var id1 = new NmeaMessageId("gga");
        var id2 = new NmeaMessageId("GGA");
        
        Assert.True(id1.Equals(id2));
        Assert.True(id1 == id2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenIdsDiffer()
    {
        var id1 = new NmeaMessageId("GGA");
        var id2 = new NmeaMessageId("RMC");
        
        Assert.False(id1.Equals(id2));
        Assert.True(id1 != id2);
    }

    [Fact]
    public void GetHashCode_ShouldBeEqual_ForEqualIds_IgnoringCase()
    {
        var id1 = new NmeaMessageId("GGA");
        var id2 = new NmeaMessageId("gga");
        
        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnMessageId()
    {
        var id = new NmeaMessageId("GSA");
        
        Assert.Equal("GSA", id.ToString());
    }
}