using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Asv.Gnss.Test;

public class RtcmV2MessageTest
{
    [Fact]
    public void DefaultMessages_ShouldMatchLegacyV163Set()
    {
        var ids = RtcmV2Factory.DefaultMessages.Select(_ => _().MessageId).OrderBy(_ => _).ToArray();

        Assert.Equal([1, 9, 14, 15, 17, 21, 31], ids);
    }

    [Fact]
    public void Message1_WithLegacyV163Frame_ShouldDeserialize()
    {
        byte[] source =
        [
            0x66, 0x41, 0x42, 0x40, 0x4E, 0x4C, 0x5A, 0x4C, 0x42, 0x68,
            0x67, 0x43, 0x68, 0x4B, 0x52, 0x40, 0x50, 0x55, 0x55, 0x5B
        ];

        var frames = DecodeRtcm2Frames(source).ToArray();

        var frame = Assert.Single(frames);
        Assert.Equal(RtcmV2Message1.RtcmMessageId, frame.MessageId);

        var message = new RtcmV2Message1();
        ReadOnlySpan<byte> payload = frame.Payload;
        message.Deserialize(ref payload);

        Assert.InRange(payload.Length, 0, 1);
        Assert.Equal(RtcmV2Message1.RtcmMessageId, message.MessageId);
        Assert.NotEmpty(message.ObservationItems);
    }

    [Fact]
    public void RealLegacyV163TestGloData_ShouldDeserializeKnownMessages()
    {
        var data = File.ReadAllBytes(GetResourcePath("testglo.rtcm2"));
        var knownMessages = RtcmV2Factory.DefaultMessages.ToDictionary(_ => _().MessageId, _ => _);
        var parsed = new List<RtcmV2MessageBase>();

        foreach (var frame in DecodeRtcm2Frames(data))
        {
            if (!knownMessages.TryGetValue(frame.MessageId, out var create))
            {
                continue;
            }

            var message = create();
            ReadOnlySpan<byte> payload = frame.Payload;
            message.Deserialize(ref payload);
            Assert.InRange(payload.Length, 0, 2);
            parsed.Add(message);
        }

        Assert.NotEmpty(parsed);
        Assert.Contains(parsed, _ => _.MessageId == RtcmV2Message1.RtcmMessageId);
        Assert.All(parsed, _ => Assert.InRange(_.ZCount, 0.0, 3600.0));

        var gpsCorrections = parsed.OfType<RtcmV2Message1>().ToArray();
        Assert.NotEmpty(gpsCorrections);
        Assert.Contains(gpsCorrections, _ => _.ObservationItems.Length > 0);
    }

    [Fact]
    public void GpsdSampleRtcm2_ShouldMatchGpsdGoldenResult()
    {
        var data = File.ReadAllBytes(GetResourcePath("gpsd-sample.rtcm2"));
        var expected = File.ReadAllLines(GetResourcePath("gpsd-sample.rtcm2.chk"))
            .Where(_ => !string.IsNullOrWhiteSpace(_))
            .Select(_ => JsonSerializer.Deserialize<GpsdRtcm2Message>(_))
            .Select(_ => Assert.IsType<GpsdRtcm2Message>(_))
            .ToArray();

        var frames = DecodeRtcm2Frames(data).ToArray();

        Assert.Equal(expected.Length, frames.Length);

        for (var i = 0; i < expected.Length; i++)
        {
            var actual = new RtcmV2Message9();
            ReadOnlySpan<byte> payload = frames[i].Payload;
            actual.Deserialize(ref payload);

            Assert.InRange(payload.Length, 0, 2);
            AssertGpsdMessage(expected[i], frames[i], actual);
        }
    }

    private static string GetResourcePath(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "Resources", fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Resource '{fileName}' was not found");
    }

    private static IEnumerable<RtcmV2Frame> DecodeRtcm2Frames(byte[] data)
    {
        var buffer = new byte[RtcmV2Protocol.MaxMessageSize];
        uint word = 0;
        var readBytes = 0;
        var readBits = 0;
        var messageLength = 0;

        foreach (var source in data)
        {
            var dataByte = source;
            if ((dataByte & 0xC0) != 0x40)
            {
                continue;
            }

            for (var i = 0; i < 6; i++, dataByte >>= 1)
            {
                word = (word << 1) + (uint)(dataByte & 1);

                if (readBytes == 0)
                {
                    var preamble = (byte)(word >> 22);
                    if ((word & 0x40000000) != 0)
                    {
                        preamble ^= 0xFF;
                    }

                    if (preamble != RtcmV2Protocol.SyncByte || !DecodeWord(word, buffer, 0))
                    {
                        continue;
                    }

                    readBytes = 3;
                    readBits = 0;
                    continue;
                }

                if (++readBits < 30)
                {
                    continue;
                }

                readBits = 0;
                if (!DecodeWord(word, buffer, readBytes))
                {
                    readBytes = 0;
                    word &= 0x3;
                    continue;
                }

                readBytes += 3;
                if (readBytes == 6)
                {
                    messageLength = (buffer[5] >> 3) * 3 + 6;
                }

                if (readBytes < messageLength)
                {
                    continue;
                }

                var payload = buffer.AsSpan(0, messageLength).ToArray();
                yield return new RtcmV2Frame(GetMessageId(payload), payload);
                readBytes = 0;
                word &= 0x3;
            }
        }
    }

    private static ushort GetMessageId(byte[] buffer)
    {
        var bitIndex = 8;
        return (ushort)Asv.IO.SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
    }

    private static bool DecodeWord(uint word, byte[] data, int offset)
    {
        uint[] hamming = [0xBB1F3480, 0x5D8F9A40, 0xAEC7CD00, 0x5763E680, 0x6BB1F340, 0x8B7A89C0];
        uint parity = 0;

        if ((word & 0x40000000) != 0)
        {
            word ^= 0x3FFFFFC0;
        }

        for (var i = 0; i < 6; i++)
        {
            parity <<= 1;
            for (var w = (word & hamming[i]) >> 6; w != 0; w >>= 1)
            {
                parity ^= w & 0x1;
            }
        }

        if (parity != (word & 0x3F))
        {
            return false;
        }

        for (var i = 0; i < 3; i++)
        {
            data[i + offset] = (byte)(word >> (22 - i * 8));
        }

        return true;
    }

    private static void AssertGpsdMessage(GpsdRtcm2Message expected, RtcmV2Frame frame, RtcmV2Message9 actual)
    {
        Assert.Equal("RTCM2", expected.Class);
        Assert.Equal(RtcmV2Message9.RtcmMessageId, expected.Type);
        Assert.Equal(expected.Type, actual.MessageId);
        Assert.Equal(expected.Type, frame.MessageId);
        Assert.Equal(expected.StationId, actual.ReferenceStationId);
        Assert.Equal(expected.ZCount, actual.ZCount, 1);
        Assert.Equal(expected.SequenceNumber, actual.SequenceNumber);
        Assert.Equal(expected.Length, frame.PayloadWordLength);
        Assert.Equal(1.0, actual.Udre, 1);

        Assert.Equal(expected.Satellites.Length, actual.ObservationItems.Length);
        for (var i = 0; i < expected.Satellites.Length; i++)
        {
            var expectedSatellite = expected.Satellites[i];
            var actualSatellite = actual.ObservationItems[i];

            Assert.Equal(expectedSatellite.Ident, actualSatellite.Prn);
            Assert.Equal((SatUdreEnum)expectedSatellite.Udre, actualSatellite.Udre);
            Assert.Equal(expectedSatellite.Iod, actualSatellite.Iod);
            Assert.Equal(expectedSatellite.Prc, actualSatellite.Prc, 3);
            Assert.Equal(expectedSatellite.Rrc, actualSatellite.Rrc, 3);
        }
    }

    private readonly record struct RtcmV2Frame(ushort MessageId, byte[] Payload)
    {
        public int PayloadWordLength => (Payload.Length - 6) / 3;
    }

    private sealed class GpsdRtcm2Message
    {
        [JsonPropertyName("class")]
        public string Class { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public ushort Type { get; set; }

        [JsonPropertyName("station_id")]
        public ushort StationId { get; set; }

        [JsonPropertyName("zcount")]
        public double ZCount { get; set; }

        [JsonPropertyName("seqnum")]
        public byte SequenceNumber { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("satellites")]
        public GpsdRtcm2Satellite[] Satellites { get; set; } = [];
    }

    private sealed class GpsdRtcm2Satellite
    {
        [JsonPropertyName("ident")]
        public byte Ident { get; set; }

        [JsonPropertyName("udre")]
        public byte Udre { get; set; }

        [JsonPropertyName("iod")]
        public byte Iod { get; set; }

        [JsonPropertyName("prc")]
        public double Prc { get; set; }

        [JsonPropertyName("rrc")]
        public double Rrc { get; set; }
    }
}
