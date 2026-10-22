using System;
using System.IO;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Xunit;

namespace Asv.Gnss;

public class RinexFileFixtureTest
{
    [Theory]
    [InlineData("demo.10o", 2.11, -0.12345)]
    [InlineData("obs3.01gage.10o", 3.01, -0.123456789012)]
    public void Read_PublicObservationFiles_DecodesEpochsSatellitesValuesAndIndicators(
        string file, double version, double clockOffset)
    {
        var messages = RinexFile.Read(PathFor(file)).ToArray();
        Assert.Equal((decimal)version, messages[0].Header.Version);
        var epochs = messages.OfType<RinexObservationEpochMessage>().ToArray();
        Assert.Equal(2, epochs.Length);
        Assert.Equal(new RinexEpoch(2010, 3, 5, 0, 0, 0m), epochs[0].Epoch);
        Assert.Equal(new RinexEpoch(2010, 3, 5, 0, 0, 30m), epochs[1].Epoch);
        Assert.Equal(14, epochs[0].Satellites.Count);
        Assert.Equal(8, epochs[1].Satellites.Count);
        Assert.Equal(clockOffset, epochs[0].ReceiverClockOffset);
        Assert.Equal("G13", epochs[0].Satellites[0].Satellite);
        var first = epochs[0].Satellites[0].Observations[0];
        Assert.Equal(121367582.205, first.Value);
        Assert.Equal((byte)0, first.LossOfLockIndicator);
        Assert.Equal((byte)8, first.SignalStrengthIndicator);
        Assert.Equal(94572134.492, epochs[0].Satellites[0].Observations[1].Value);
        Assert.Equal("G07", epochs[1].Satellites[3].Satellite);
        Assert.Equal("S24", epochs[0].Satellites[^1].Satellite);
        Assert.Equal(130321269.801, epochs[1].Satellites[0].Observations[0].Value);
    }

    [Theory]
    [InlineData("demo.10n", "G06", 1999, 9, 2, 19, 0, 0, -0.000839701388031, 31)]
    [InlineData("demo_nav3.10n", "S22", 2010, 10, 18, 0, 1, 4, -9.872019290924e-8, 15)]
    public void Read_PublicNavigationFiles_DecodesClockAndCompleteFieldLayout(
        string file, string satellite, int year, int month, int day, int hour, int minute,
        int seconds, double clock, int valueCount)
    {
        var messages = RinexFile.Read(PathFor(file)).OfType<RinexNavigationMessage>().ToArray();
        Assert.Equal(2, messages.Length);
        Assert.Equal(satellite, messages[0].Satellite);
        Assert.Equal(new RinexEpoch(year, month, day, hour, minute, seconds), messages[0].Epoch);
        Assert.Equal(clock, messages[0].Values[0]);
        Assert.All(messages, message => Assert.Equal(valueCount, message.Values.Count));
        Assert.Equal("EPH", messages[0].RecordType);
        Assert.NotEqual(messages[0].Values[0], messages[1].Values[0]);
    }

    [Fact]
    public void Read_PublicMixedNavigation_DecodesDifferentConstellationRecordLengths()
    {
        var records = RinexFile.Read(PathFor("BRDM00DLR_R_20130010000_01D_MN.rnx"))
            .OfType<RinexNavigationMessage>().ToArray();
        Assert.Equal(10, records.Length);
        Assert.Equal(new[] { "G01", "G01", "G02", "G02", "R01", "R01", "R02", "R02", "J01", "J01" },
            records.Select(x => x.Satellite));
        Assert.Equal(2.907030284405e-4, records[0].Values[0]);
        Assert.Equal(-1.750094816089e-4, records[4].Values[0]);
        Assert.Equal(31, records[0].Values.Count);
        Assert.Equal(15, records[4].Values.Count);
        Assert.Equal(31, records[8].Values.Count);
        Assert.Equal(new RinexEpoch(2013, 1, 1, 1, 15, 12m), records[^1].Epoch);
    }

    [Fact]
    public void Read_PublicGalileoNavigation_DecodesLowercaseExponentAndAdjacentNegativeFields()
    {
        var records = RinexFile.Read(PathFor("brdc0320.16l.txt")).OfType<RinexNavigationMessage>().ToArray();
        Assert.Equal(10, records.Length);
        Assert.Equal(new[] { "E02", "E03", "E04", "E10", "E11", "E12", "E18", "E19", "E26", "E27" },
            records.Select(x => x.Satellite));
        Assert.Equal(new RinexEpoch(2016, 1, 31, 23, 20, 0m), records[0].Epoch);
        Assert.Equal(-9.313225746155e-9, records[0].Values[7]);
        Assert.Equal(8.999999961816e-3, records[0].Values[8]);
        Assert.Equal(-5.227360597694e-9, records[0].Values[18]);
    }

    [Fact]
    public void Read_PublicMalformedGalileoNumber_RejectsEmbeddedBlankInExponentField()
    {
        var error = Assert.Throws<FormatException>(() => RinexFile.Read(PathFor("galileo3.15n")));
        Assert.Contains(". 000000000000E+00", error.ToString());
    }

    [Fact]
    public void Read_PublicRinex402Navigation_DecodesAllRecordFamiliesAndMessageSubtypes()
    {
        var records = RinexFile.Read(PathFor("rinex402_examples_MN.rnx"))
            .OfType<RinexNavigationMessage>().ToArray();
        Assert.Equal(34, records.Length);
        Assert.Equal(9, records.Count(x => x.RecordType == "EPH"));
        Assert.Equal(13, records.Count(x => x.RecordType == "STO"));
        Assert.Equal(5, records.Count(x => x.RecordType == "EOP"));
        Assert.Equal(7, records.Count(x => x.RecordType == "ION"));
        Assert.Equal("G04", records[0].Satellite);
        Assert.Equal("LNAV", records[0].NavigationMessageType);
        Assert.Equal(new RinexEpoch(2019, 3, 14, 4, 0, 0m), records[0].Epoch);
        Assert.Equal(1.330170780420e-4, records[0].Values[0]);
        Assert.Contains(records, x => x.NavigationMessageType == "L1OC" && x.Satellite == "R26");
        Assert.Contains(records, x => x.RecordType == "ION" && x.NavigationMessageSubtype == "KLOB");
        Assert.Contains(records, x => x.RecordType == "ION" && x.NavigationMessageSubtype == "NEQN");
    }

    [Theory]
    [InlineData("cari0010.07m", 3, 1996, 4, 1, 15, 987.1, 10.6, 89.5)]
    [InlineData("met4-example.txt", 5, 2021, 1, 7, 0, 993.3, 23.0, 90.0)]
    public void Read_PublicMeteorologicalFiles_DecodesHeaderCodesAndMeasurements(
        string file, int count, int year, int month, int day, int seconds,
        double pressure, double temperature, double humidity)
    {
        var records = RinexFile.Read(PathFor(file)).OfType<RinexMeteorologicalMessage>().ToArray();
        Assert.Equal(count, records.Length);
        Assert.Equal(new RinexEpoch(year, month, day, 0, 0, seconds), records[0].Epoch);
        Assert.Equal(new[] { "PR", "TD", "HR" }, records[0].Observations.Select(x => x.Code));
        Assert.Equal(new double?[] { pressure, temperature, humidity }, records[0].Observations.Select(x => x.Value));
        Assert.NotEqual(records[0].Epoch, records[^1].Epoch);
    }

    [Theory]
    [InlineData("demo.10o")]
    [InlineData("obs3.01gage.10o")]
    [InlineData("demo.10n")]
    [InlineData("demo_nav3.10n")]
    [InlineData("rinex402_examples_MN.rnx")]
    [InlineData("cari0010.07m")]
    [InlineData("met4-example.txt")]
    [InlineData("demo_nav3.17n")]
    [InlineData("BRDM00DLR_R_20130010000_01D_MN.rnx")]
    [InlineData("brdc0320.16l.txt")]
    public void Write_PublicFileAndReadAgain_PreservesTypedRecordsAndCanonicalBytes(string file)
    {
        var original = RinexFile.Read(PathFor(file)).ToArray();
        using var output = new MemoryStream();
        RinexFile.Write(output, original);
        output.Position = 0;
        var decoded = RinexFile.Read(output).ToArray();
        Assert.Equal(original.Select(x => x.Id), decoded.Select(x => x.Id));
        Assert.Equal(original[0].Header.Version, decoded[0].Header.Version);
        for (var i = 0; i < original.Length; i++)
            Assert.Equal(Serialize(original[i]), Serialize(decoded[i]));
        using var second = new MemoryStream();
        RinexFile.Write(second, decoded);
        Assert.Equal(output.ToArray(), second.ToArray());
    }

    [Fact]
    public void Read_ZeroBlankValuesAndBlankIndicators_RemainDistinct()
    {
        var source = File.ReadAllText(PathFor("obs3.01gage.10o"));
        var original = "G13 121367582.20508  94572134.49208";
        Assert.Contains(original, source);
        var changed = source.Replace(original, "G13" + "0.000".PadLeft(14) + "  " + new string(' ', 16), StringComparison.Ordinal);
        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(changed));
        var epoch = RinexFile.Read(stream).OfType<RinexObservationEpochMessage>().First();
        var values = epoch.Satellites[0].Observations;
        Assert.Equal(0d, values[0].Value);
        Assert.Null(values[0].LossOfLockIndicator);
        Assert.Null(values[0].SignalStrengthIndicator);
        Assert.Null(values[1].Value);
        Assert.Null(values[1].LossOfLockIndicator);
        Assert.Null(values[1].SignalStrengthIndicator);
        Assert.Equal(23095489.677, values[2].Value);
        Assert.Equal((byte)9, values[2].SignalStrengthIndicator);
    }

    [Fact]
    public void Read_NonEnglishCulture_UsesInvariantDecimalAndExponentSyntax()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            var record = RinexFile.Read(PathFor("demo.10n")).OfType<RinexNavigationMessage>().First();
            Assert.Equal(-0.000839701388031, record.Values[0]);
            var bytes = Serialize(record);
            Assert.Contains("D-04", Encoding.ASCII.GetString(bytes));
            Assert.DoesNotContain(",", Encoding.ASCII.GetString(bytes));
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    [Theory]
    [InlineData("demo.10o")]
    [InlineData("rinex402_examples_MN.rnx")]
    public void Read_GzipStream_DecodesSameRecordsAsPlainPublicFixture(string file)
    {
        using var compressed = new MemoryStream();
        using (var gzip = new GZipStream(compressed, CompressionMode.Compress, leaveOpen: true))
            gzip.Write(File.ReadAllBytes(PathFor(file)));
        compressed.Position = 0;
        using var decoded = new GZipStream(compressed, CompressionMode.Decompress);
        var expected = RinexFile.Read(PathFor(file)).ToArray();
        var actual = RinexFile.Read(decoded).ToArray();
        Assert.Equal(expected.Length, actual.Length);
        for (var i = 0; i < expected.Length; i++)
            Assert.Equal(Serialize(expected[i]), Serialize(actual[i]));
    }

    [Fact]
    public async Task ReadWriteAsync_GzipPublicFixture_PreservesRecordsAndLeavesCallerStreamsOpen()
    {
        using var compressed = new MemoryStream();
        using (var gzip = new GZipStream(compressed, CompressionMode.Compress, leaveOpen: true))
            gzip.Write(File.ReadAllBytes(PathFor("demo.10o")));
        compressed.Position = 0;
        var records = await RinexFile.ReadAsync(compressed, TestContext.Current.CancellationToken);
        Assert.True(compressed.CanRead);
        Assert.Equal(3, records.Count);
        Assert.Equal(14, Assert.IsType<RinexObservationEpochMessage>(records[1]).Satellites.Count);
        using var output = new MemoryStream();
        await RinexFile.WriteAsync(output, records, TestContext.Current.CancellationToken);
        Assert.True(output.CanWrite);
        output.Position = 0;
        var decoded = await RinexFile.ReadAsync(output, TestContext.Current.CancellationToken);
        Assert.Equal(records.Select(x => x.Id), decoded.Select(x => x.Id));
        Assert.Equal(Serialize(records[2]), Serialize(decoded[2]));
        Assert.True(output.CanRead);
    }

    [Fact]
    public async Task ReadWriteAsync_CanceledToken_DoesNotConsumeOrWriteCallerStream()
    {
        using var input = new MemoryStream(File.ReadAllBytes(PathFor("demo.10n")));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => RinexFile.ReadAsync(input, cancellation.Token));
        Assert.Equal(0, input.Position);
        Assert.True(input.CanRead);
        using var output = new MemoryStream();
        var records = RinexFile.Read(PathFor("demo.10n"));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => RinexFile.WriteAsync(output, records, cancellation.Token));
        Assert.Equal(0, output.Length);
        Assert.True(output.CanWrite);
    }

    [Fact]
    public void Write_NoHeader_RejectsBeforeWritingBodyBytes()
    {
        using var output = new MemoryStream();
        Assert.Throws<ArgumentException>(() => RinexFile.Write(output, Array.Empty<RinexMessageBase>()));
        var body = RinexFile.Read(PathFor("demo.10n"))[1];
        Assert.Throws<ArgumentException>(() => RinexFile.Write(output, new[] { body }));
        Assert.Equal(0, output.Length);
    }

    [Fact]
    public void Read_HeaderWithoutEndOfHeader_RejectsTruncatedPublicFile()
    {
        var bytes = File.ReadAllBytes(PathFor("demo.10o"));
        using var stream = new MemoryStream(bytes[..100]);
        Assert.Throws<FormatException>(() => RinexFile.Read(stream).ToArray());
    }

    [Theory]
    [InlineData("demo.10o")]
    [InlineData("demo.10n")]
    [InlineData("rinex402_examples_MN.rnx")]
    public void Read_TruncatedFinalBody_RejectsIncompleteRecord(string file)
    {
        var text = File.ReadAllText(PathFor(file));
        var lines = text.TrimEnd('\r', '\n').Split('\n');
        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(string.Join("\n", lines[..^1]) + "\n"));
        Assert.Throws<FormatException>(() => RinexFile.Read(stream).ToArray());
    }

    [Fact]
    public void Parse_UnsupportedClockAndCompactHeaders_ExplainsUnsupportedFileType()
    {
        foreach (var type in new[] { 'C', 'D' })
        {
            var header = new[]
            {
                RinexHeader.FormatLine("     3.04           " + type, "RINEX VERSION / TYPE"),
                RinexHeader.FormatLine("", "END OF HEADER")
            };
            Assert.Throws<NotSupportedException>(() => RinexHeader.Parse(header));
        }
    }

    internal static string PathFor(string file) => Path.Combine(AppContext.BaseDirectory, "Resources", "Rinex", file);
    internal static byte[] Serialize(RinexMessageBase message)
    {
        var bytes = new byte[message.GetByteSize()];
        var span = bytes.AsSpan();
        message.Serialize(ref span);
        Assert.Empty(span.ToArray());
        return bytes;
    }
}
