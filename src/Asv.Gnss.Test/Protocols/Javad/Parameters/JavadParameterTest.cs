using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Xunit;

namespace Asv.Gnss;

public class JavadParameterTest
{
    [Theory]
    [InlineData("{e1,{ee21,ee22},e3}")]
    [InlineData("jps/{SI:{1,10,50,0x2},EL,AZ}:2")]
    [InlineData("{e1,{e2:{o1,,o3},e3}}:{o4,o5}")]
    [InlineData("{main=\"2.5 Sep,13,2006 p2\",boot=0,hw=71,pow={fw=none,hw=none}}")]
    [InlineData("{WGS84,N83d42m47.556000s,E083d42m47.556000s,1.5}")]
    [InlineData("{1,,\"\",{}}")]
    public void Values_ParseManualNestedAndCompoundForms(string text) =>
        Assert.Equal(text, JavadValue.Parse(text).ToString());

    [Fact]
    public void Values_DistinguishOmittedEmptyStringsAndEmptyLists()
    {
        var value = JavadValue.Parse("{,\"\",{}}");
        Assert.True(value.Items[0].IsOmitted);
        Assert.False(value.Items[1].IsOmitted);
        Assert.Equal(string.Empty, value.Items[1].Scalar);
        Assert.True(value.Items[2].IsList);
        Assert.Empty(value.Items[2].Items);
    }

    [Theory]
    [InlineData("493", 493)]
    [InlineData("0371", 249)]
    [InlineData("0x03f0", 1008)]
    [InlineData("-0xCAF", -3247)]
    [InlineData("-9223372036854775808", long.MinValue)]
    public void IntegerValues_AcceptEveryDocumentedRadix(string text, long expected) =>
        Assert.Equal(expected, JavadValue.Parse(text).AsInteger());

    [Theory]
    [InlineData("y", true)]
    [InlineData("yes", true)]
    [InlineData("on", true)]
    [InlineData("n", false)]
    [InlineData("no", false)]
    [InlineData("off", false)]
    public void BooleanValues_AcceptEveryDocumentedSpelling(string text, bool expected) =>
        Assert.Equal(expected, JavadValue.Parse(text).AsBoolean());

    [Theory]
    [InlineData("{a,b")]
    [InlineData("\"bad\\n\"")]
    [InlineData("\"not closed")]
    [InlineData("{a}extra")]
    [InlineData("a,b")]
    public void Values_RejectMalformedStructures(string text) =>
        Assert.Throws<FormatException>(() => JavadValue.Parse(text));

    [Fact]
    public void PrimaryTypes_UseInvariantFormattingAndValidUnitRanges()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            Assert.Equal(
                "{WGS84,55.75,-37.5,100.25}",
                JavadParameterValues.Geodetic("WGS84", 55.75, -37.5, 100.25).ToString()
            );
            Assert.Equal(
                "{WGS84,-1.5,0,10000000}",
                JavadParameterValues.Cartesian("WGS84", -1.5, 0, 10000000).ToString()
            );
            Assert.Equal(
                "{1.5,,50,0x2}",
                JavadParameterValues.Scheduling(1.5, count: 50, flags: 2).ToString()
            );
            Assert.Equal(
                "192.168.1.2",
                JavadParameterValues.IpAddress(IPAddress.Parse("192.168.1.2")).Scalar
            );
            Assert.Equal(3.24e-10, JavadValue.Parse("+3.24e-10").AsFloat());
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            JavadParameterValues.Geodetic("WGS84", 90.1, 0, 0)
        );
        Assert.Throws<ArgumentOutOfRangeException>(() => JavadParameterValues.Scheduling(86400));
        Assert.Equal("{,,-256,}", JavadParameterValues.Scheduling(count: -256).ToString());
        Assert.Equal("{,,32766,}", JavadParameterValues.Scheduling(count: 32766).ToString());
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            JavadParameterValues.Scheduling(count: 32767)
        );
        Assert.Throws<ArgumentException>(() =>
            JavadParameterValues.IpAddress(IPAddress.IPv6Loopback)
        );
        Assert.Throws<ArgumentOutOfRangeException>(() => JavadValue.Float(double.NaN));
    }

    [Theory]
    [InlineData("4d17h40m18s", "4d17h40m18s")]
    [InlineData("__d__h00m__s", "__d__h00m__s")]
    [InlineData("8h2s", "__d08h__m02s")]
    [InlineData("", "__d__h__m__s")]
    [InlineData("_d_ h", null)]
    public void TimeSpecifications_NormalizeWildcardsAndAbbreviations(string text, string? expected)
    {
        if (expected is null)
            Assert.Throws<FormatException>(() => JavadTimeSpecification.Parse(text));
        else
            Assert.Equal(expected, JavadTimeSpecification.Parse(text).ToString());
    }

    [Fact]
    public void Paths_PreserveCaseRelativeNamesAttributesAndAllRootNamespaces()
    {
        foreach (var root in new[] { "/", "/par", "/dev", "/msg", "/log", "/cur", "/eph" })
            Assert.Equal(root, new JavadObjectPath(root).ToString());
        Assert.Equal(
            "/par/dev/ser/a/rate&def",
            new JavadObjectPath("/par/dev/ser/a").Child("rate").Attribute("def").ToString()
        );
        Assert.Equal("/msg/jps/RT", new JavadObjectPath("/msg/jps").Child("RT").ToString());
        Assert.False(new JavadObjectPath("rcv/id").IsAbsolute);
        Assert.Throws<ArgumentException>(() => new JavadObjectPath("/par/power;set"));
        Assert.Throws<InvalidOperationException>(() => default(JavadObjectPath).ToValue());
    }

    [Fact]
    public void Catalog_CoversEveryExtractedDeclarationAndAppendixWithSourceMetadata()
    {
        Assert.Equal(1227, JavadParameterCatalog.All.Length);
        Assert.All(
            JavadParameterCatalog.All,
            x =>
            {
                Assert.StartsWith("/", x.PathTemplate);
                Assert.InRange(x.PdfPage, 190, 566);
            }
        );
        var reset = Assert.Single(JavadParameterCatalog.Find("/par/reset"));
        Assert.False(reset.CanRead);
        Assert.True(reset.CanWrite);
        Assert.Equal("boolean", reset.Type);
        Assert.Equal("yes,no", reset.Values);
        Assert.Equal(190, reset.PdfPage);
        var appendix = Assert.Single(JavadParameterCatalog.Find("/par/raw/rtm/geoid"));
        Assert.Equal(566, appendix.PdfPage);
        Assert.Contains(
            "term",
            Assert.Single(JavadParameterCatalog.Find("/par/[port]/imode")).Values
        );
        Assert.True(
            Assert.Single(JavadParameterCatalog.Find("/par/[cport]/sid/out/first")).CanWrite
        );
        Assert.Contains(
            JavadParameterCatalog.Under("/par/net/tcpcl/"),
            x => x.PathTemplate.EndsWith("/ntrip/passwd", StringComparison.Ordinal)
        );
    }

    [Fact]
    public void Catalog_BindsIndexedAndPortTemplatesWithoutRestrictingDynamicObjectAccess()
    {
        var descriptor = Assert.Single(JavadParameterCatalog.Find("/par/dev/tcpo/X/port"));
        Assert.Equal(
            "/par/dev/tcpo/c/port",
            descriptor.Bind(new Dictionary<string, string> { ["X"] = "c" }).ToString()
        );
        var port = Assert.Single(JavadParameterCatalog.Find("/par/[port]/imode"));
        Assert.Equal(
            "/par/dev/ser/a/imode",
            port.Bind(new Dictionary<string, string> { ["[port]"] = "dev/ser/a" }).ToString()
        );
        Assert.Throws<ArgumentException>(() => descriptor.Bind(new Dictionary<string, string>()));
        var file = Assert.Single(JavadParameterCatalog.Find("/log/NAME&content"));
        Assert.Equal(
            "/log/SiteA&content",
            file.Bind(new Dictionary<string, string> { ["NAME"] = "SiteA" }).ToString()
        );
        Assert.Equal(
            "print,/par/future/object",
            JavadCommand.Print(new("/par/future/object")).ToString()
        );
    }
}
