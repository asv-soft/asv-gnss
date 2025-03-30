using System;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests;

[TestSubject(typeof(NmeaMessageId))]
public class NmeaMessageIdTest
{

    [Fact]
    public void Constructor_WithEmptyMessage_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new NmeaMessageId(ReadOnlySpan<char>.Empty));
    }

    [Fact]
    public void Constructor_WithShortMessage_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new NmeaMessageId("ABC"));
    }

    [Fact]
    public void Constructor_WithProprietaryPrefix_SetsMessageIdCorrectly()
    {
        var messageId = new NmeaMessageId("PTEST");
        Assert.Equal("PTEST", messageId.MessageId);
        Assert.True(messageId.IsProprietary);
    }

    [Fact]
    public void Constructor_WithTalkerIgnoreAndProprietaryPrefix_SetsMessageIdCorrectly()
    {
        var messageId = new NmeaMessageId("-PXYZ");
        Assert.Equal("-PXYZ", messageId.MessageId);
        Assert.False(messageId.IsProprietary);
    }

    [Fact]
    public void Constructor_WithStandardPrefix_ReformatsMessageIdCorrectly()
    {
        var messageId = new NmeaMessageId("GPABC");
        Assert.Equal("--ABC", messageId.MessageId);
        Assert.False(messageId.IsProprietary);
    }

    [Fact]
    public void Equals_WithDifferentCaseMessageIds_ReturnsTrue()
    {
        var id1 = new NmeaMessageId("GPABC");
        var id2 = new NmeaMessageId("gpabc");
        Assert.True(id1.Equals(id2));
        Assert.True(id1 == id2);
        Assert.False(id1 != id2);
    }

    [Fact]
    public void Equals_WithDifferentMessageIds_ReturnsFalse()
    {
        var id1 = new NmeaMessageId("GPABC");
        var id2 = new NmeaMessageId("GPABD");
        Assert.False(id1.Equals(id2));
        Assert.False(id1 == id2);
        Assert.True(id1 != id2);
    }

    [Fact]
    public void GetHashCode_SameMessageIdDifferentCase_HashCodesEqual()
    {
        var id1 = new NmeaMessageId("GPABC");
        var id2 = new NmeaMessageId("gpabc");
        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsMessageId()
    {
        var messageId = new NmeaMessageId("GPABC");
        Assert.Equal("--ABC", messageId.ToString());
    }
}