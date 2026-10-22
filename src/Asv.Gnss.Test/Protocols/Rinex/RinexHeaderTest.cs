using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Asv.Gnss.Test;

public class RinexHeaderTest
{
    [Fact]
    public void Serialize_MetadataDependencies_DescriptorsPrecedeScaleFactorsAndUnknownMetadataSurvives()
    {
        var header = new RinexHeader { Version = 3.04m, FileType = 'O', SatelliteSystem = 'G' };
        header.ObservationTypes.Add('G', new List<string> { "C1C", "L1C" });
        var unknown = RinexHeader.FormatLine("vendor-specific metadata", "VENDOR EXTENSION");
        header.HeaderLines.Add(RinexHeader.FormatLine("G   10   0", "SYS / SCALE FACTOR"));
        header.HeaderLines.Add(unknown);
        var message = new RinexHeaderMessage { Header = header };
        var bytes = new byte[message.GetByteSize()];
        var output = bytes.AsSpan();
        message.Serialize(ref output);
        Assert.Empty(output.ToArray());
        var text = Encoding.ASCII.GetString(bytes);
        Assert.True(text.IndexOf("SYS / # / OBS TYPES", StringComparison.Ordinal) <
            text.IndexOf("SYS / SCALE FACTOR", StringComparison.Ordinal));
        var copy = RinexHeader.Parse(text.TrimEnd('\n').Split('\n'));
        Assert.Equal(new[] { "C1C", "L1C" }, copy.ObservationTypes['G']);
        Assert.Contains(unknown, copy.HeaderLines);
        Assert.Equal("G   10   0", copy.GetValue("SYS / SCALE FACTOR"));
    }

    [Theory]
    [InlineData("BDT")]
    [InlineData("")]
    public void Serialize_ChangedTimeSystem_RewritesKnownMetadataIncludingBlank(string timeSystem)
    {
        var header = new RinexHeader { TimeSystem = timeSystem };
        header.ObservationTypes.Add('G', new List<string> { "C1C" });
        header.HeaderLines.Add(RinexHeader.FormatLine(
            FormattableString.Invariant($"{2025,6}{1,6}{1,6}{0,6}{0,6}{0m,13:0.0000000}     GPS"), "TIME OF FIRST OBS"));
        var message = new RinexHeaderMessage { Header = header };
        var bytes = new byte[message.GetByteSize()];
        var output = bytes.AsSpan();
        message.Serialize(ref output);
        ReadOnlySpan<byte> input = bytes;
        var decoded = new RinexHeaderMessage();
        decoded.Deserialize(ref input);
        Assert.Equal(timeSystem, decoded.Header.TimeSystem);
        Assert.Empty(input.ToArray());
    }

    [Fact]
    public void Serialize_TimeSystemWithoutEpochMetadata_RejectsSilentMetadataLoss()
    {
        var header = new RinexHeader { TimeSystem = "GPS" };
        header.ObservationTypes.Add('G', new List<string> { "C1C" });
        Assert.Throws<FormatException>(() => new RinexHeaderMessage { Header = header }.GetByteSize());
    }

    [Fact]
    public void Epoch_LeapSecondAndPicoseconds_DoNotSilentlyConvertTimeSystems()
    {
        var epoch = new RinexEpoch(2025, 1, 1, 0, 0, 12.123456789012m);
        var date = epoch.ToDateTime();
        Assert.Equal(DateTimeKind.Unspecified, date.Kind);
        Assert.Equal(121234567, date.TimeOfDay.Ticks);
        Assert.Throws<InvalidOperationException>(() => (epoch with { Seconds = 60m }).ToDateTime());
    }
}
