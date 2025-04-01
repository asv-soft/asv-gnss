using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests;

[TestSubject(typeof(NmeaTalkerId))]
public class NmeaTalkerIdTest
{
    [Theory]
    [InlineData("AG", NmeaTalkerClass.AutopilotGeneral)]
    [InlineData("GP", NmeaTalkerClass.GlobalPositioningSystem)]
    [InlineData("HE", NmeaTalkerClass.HeadingNorthSeekingGyro)]
    [InlineData("P", NmeaTalkerClass.ProprietaryCode)]
    [InlineData("XX", NmeaTalkerClass.Unknown)]
    public void Constructor_Should_Set_Correct_Type(string input, NmeaTalkerClass expectedType)
    {
        var talkerId = new NmeaTalkerId(input);
        Assert.Equal(expectedType, talkerId.Type);
    }

    [Theory]
    [InlineData("AG", "AG")]
    [InlineData("GP", "GP")]
    [InlineData("HE", "HE")]
    [InlineData("XX", "XX")]
    public void Constructor_Should_Set_Correct_Id(string input, string expectedId)
    {
        var talkerId = new NmeaTalkerId(input);
        Assert.Equal(expectedId, talkerId.Id);
    }

    [Fact]
    public void Constructor_Should_Throw_On_Empty_Input()
    {
        Assert.Throws<ArgumentException>(() => new NmeaTalkerId(string.Empty));
    }

    [Fact]
    public void Constructor_Should_Throw_On_Short_Input()
    {
        Assert.Throws<ArgumentException>(() => new NmeaTalkerId("A"));
    }

    [Theory]
    [InlineData("GP", "gp", true)]
    [InlineData("GP", "GP", true)]
    [InlineData("GP", "He", false)]
    public void Equals_Should_Work_Correctly(string first, string second, bool expectedResult)
    {
        var id1 = new NmeaTalkerId(first);
        var id2 = new NmeaTalkerId(second);
        Assert.Equal(expectedResult, id1.Equals(id2));
    }

    [Theory]
    [InlineData("AG")]
    [InlineData("GP")]
    [InlineData("HE")]
    public void GetHashCode_Should_Be_Case_Insensitive(string input)
    {
        var idLower = new NmeaTalkerId(input.ToLower());
        var idUpper = new NmeaTalkerId(input.ToUpper());
        Assert.Equal(idLower.GetHashCode(), idUpper.GetHashCode());
    }
}