using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxMixedStreamTest
{
    private const string FileName = "ubx_20080526.ubx";
    private const int IncompleteFrameOffset = 262126;

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(257)]
    [InlineData(4096)]
    public void Push_RecordedMixedStream_ShouldPreserveSupportedSentenceOrderAcrossTransportChunks(int chunkSize)
    {
        var data = ReadRecording();
        var expected = ReadIndependentFrameOracle(data);
        using var stream = new RecordedGnssStream();

        stream.Push(data, chunkSize);

        Assert.Equal(2321, stream.Messages.Count);
        Assert.All(stream.Messages, message => Assert.IsAssignableFrom<NmeaMessageBase>(message));
        Assert.Equal(expected, stream.Messages.Select(message => message.Name));
        var counts = stream.Messages.GroupBy(message => message.Name).ToDictionary(group => group.Key, group => group.Count());
        Assert.Equal(7, counts.Count);
        Assert.Equal(242, counts["GGA"]);
        Assert.Equal(242, counts["GLL"]);
        Assert.Equal(242, counts["GSA"]);
        Assert.Equal(869, counts["GSV"]);
        Assert.Equal(242, counts["RMC"]);
        Assert.Equal(242, counts["VTG"]);
        Assert.Equal(242, counts["ZDA"]);

        Assert.Equal(1326, stream.Errors.Count);
        var unknown = stream.Errors.Select(error => Assert.IsType<ProtocolParserUnknownMessageException>(error)).ToArray();
        Assert.Equal(1084, unknown.Count(error => error.Parser.Id == UbxProtocol.Info.Id));
        Assert.Equal(242, unknown.Count(error => error.Parser.Id == NmeaProtocol.Info.Id));
        AssertFirstPositionAndLastTime(stream.Messages);
    }

    [Fact]
    public void Push_RecordedIncompleteTail_ShouldNotPublishAnAdditionalMessageOrError()
    {
        var data = ReadRecording();
        using var stream = new RecordedGnssStream();
        stream.Push(data.AsSpan(0, IncompleteFrameOffset), 257);
        Assert.Equal(2321, stream.Messages.Count);
        Assert.Equal(1326, stream.Errors.Count);
        Assert.Equal("B56202112A0004810C800E9A00FC1F00DFFF", Convert.ToHexString(data.AsSpan(IncompleteFrameOffset)));

        stream.Push(data.AsSpan(IncompleteFrameOffset), 1);

        Assert.Equal(2321, stream.Messages.Count);
        Assert.Equal(1326, stream.Errors.Count);
        var last = Assert.IsType<NmeaMessageZda>(stream.Messages[^1]);
        Assert.Equal(new TimeOnly(6, 3, 12), last.Time);
        Assert.Equal(new DateOnly(2008, 5, 26), last.Date);
    }

    [Theory]
    [InlineData(3256, 5, 59, 14, 0x21)]
    [InlineData(41658, 5, 59, 50, 0x24)]
    [InlineData(203303, 6, 2, 19, 0x21)]
    public void Push_RecordedRmcAfterStartMarkerInBinaryChecksum_ShouldRestartSentenceIdentifier(
        int offset, int hour, int minute, int second, byte precedingChecksum)
    {
        var data = ReadRecording();
        Assert.Equal(precedingChecksum, data[offset - 1]);
        var remaining = data.AsSpan(offset);
        var sentence = remaining[..(remaining.IndexOf("\r\n"u8) + 2)];
        using var stream = new RecordedGnssStream();
        stream.Push(data.AsSpan(offset - 1, 1), 1);

        stream.Push(sentence, 7);

        var message = Assert.IsType<NmeaMessageRmc>(Assert.Single(stream.Messages));
        Assert.Equal(new TimeOnly(hour, minute, second), message.Time);
        Assert.Equal(new DateTime(2008, 5, 26, 0, 0, 0, DateTimeKind.Utc), message.Date);
        Assert.True(message.HasCrc);
        Assert.Empty(stream.Errors);
    }

    private static byte[] ReadRecording()
    {
        var data = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Ubx", FileName));
        Assert.Equal(262144, data.Length);
        Assert.Equal("8986134FBAC86B910C9A79F82C53C05DBC7B9442DD5CBA669AE7BE6A05B75870", Convert.ToHexString(SHA256.HashData(data)));
        return data;
    }

    private static List<string> ReadIndependentFrameOracle(byte[] data)
    {
        var supported = new List<string>();
        var offset = 0;
        var rawFrames = 0;
        var subframes = 0;
        var nmeaFrames = 0;
        var unsupportedNmea = 0;
        while (offset < data.Length)
        {
            var remaining = data.AsSpan(offset);
            if (remaining.Length >= 2 && remaining[0] == 0xB5 && remaining[1] == 0x62)
            {
                Assert.True(remaining.Length >= 6);
                var payloadSize = BinaryPrimitives.ReadUInt16LittleEndian(remaining[4..]);
                var packetSize = payloadSize + 8;
                if (remaining.Length < packetSize)
                {
                    Assert.Equal(IncompleteFrameOffset, offset);
                    Assert.Equal(42, payloadSize);
                    Assert.Equal(18, remaining.Length);
                    break;
                }

                Assert.Equal((byte)0x02, remaining[2]);
                if (remaining[3] == 0x10)
                {
                    rawFrames++;
                }
                else
                {
                    Assert.Equal((byte)0x11, remaining[3]);
                    subframes++;
                }
                byte checksumA = 0;
                byte checksumB = 0;
                foreach (var value in remaining.Slice(2, payloadSize + 4))
                {
                    checksumA = unchecked((byte)(checksumA + value));
                    checksumB = unchecked((byte)(checksumB + checksumA));
                }
                Assert.Equal(checksumA, remaining[packetSize - 2]);
                Assert.Equal(checksumB, remaining[packetSize - 1]);
                offset += packetSize;
                continue;
            }

            Assert.Equal((byte)'$', remaining[0]);
            var end = remaining.IndexOf("\r\n"u8);
            Assert.True(end > 0);
            var sentence = remaining[..end];
            var star = sentence.IndexOf((byte)'*');
            Assert.Equal(sentence.Length - 3, star);
            byte crc = 0;
            foreach (var value in sentence[1..star])
            {
                crc ^= value;
            }
            var wireCrc = byte.Parse(Encoding.ASCII.GetString(sentence[(star + 1)..]), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.Equal(wireCrc, crc);
            Assert.Equal("GP", Encoding.ASCII.GetString(sentence.Slice(1, 2)));
            var name = Encoding.ASCII.GetString(sentence.Slice(3, 3));
            if (name == "GRS")
            {
                unsupportedNmea++;
            }
            else
            {
                Assert.Contains(name, new[] { "GGA", "GLL", "GSA", "GSV", "RMC", "VTG", "ZDA" });
                supported.Add(name);
            }
            nmeaFrames++;
            offset += end + 2;
        }

        Assert.Equal(IncompleteFrameOffset, offset);
        Assert.Equal(242, rawFrames);
        Assert.Equal(842, subframes);
        Assert.Equal(2563, nmeaFrames);
        Assert.Equal(242, unsupportedNmea);
        Assert.Equal(2321, supported.Count);
        return supported;
    }

    private static void AssertFirstPositionAndLastTime(IReadOnlyList<IProtocolMessage> messages)
    {
        var first = Assert.IsType<NmeaMessageRmc>(messages[0]);
        Assert.Equal(new TimeOnly(5, 59, 11), first.Time);
        Assert.Equal(new DateTime(2008, 5, 26, 0, 0, 0, DateTimeKind.Utc), first.Date);
        Assert.Equal(NmeaPositionFixStatus.Valid, first.Status);
        Assert.Equal(35.87290816666667, first.Latitude, 10);
        Assert.Equal(138.38978683333335, first.Longitude, 10);
        Assert.Equal(0.059, first.SpeedOverGroundKnots);
        Assert.Equal(42.46, first.TrackMadeGoodDegreesTrue);
        Assert.Equal(NmeaPositioningSystemMode.Differential, first.PositioningSystemMode);
        var firstGga = messages.OfType<NmeaMessageGga>().First();
        Assert.Equal(956.1, firstGga.AntennaAltitudeMsl);
        Assert.Equal(38.3, firstGga.GeoidalSeparation);
        Assert.Equal(NmeaGpsQuality.Dgnss, firstGga.GpsQuality);
        Assert.Equal(8, firstGga.NumberOfSatellites);
        Assert.Equal(1.17, firstGga.HorizontalDilutionPrecision);
        var last = Assert.IsType<NmeaMessageZda>(messages[^1]);
        Assert.Equal(new TimeOnly(6, 3, 12), last.Time);
        Assert.Equal(new DateOnly(2008, 5, 26), last.Date);
    }
}
