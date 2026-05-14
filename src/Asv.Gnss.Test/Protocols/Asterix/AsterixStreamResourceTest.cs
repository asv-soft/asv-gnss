using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Asv.Gnss;
using Xunit;

namespace Asv.Gnss.Test;

public class AsterixStreamResourceTest
{
    private const string AsterixStream0700FileName = "2025-07-09-07.stream";
    private const int StreamHeaderSize = 64;

    [Fact]
    public void Parse_AsterixStream0700Resource_ShouldReadAllFramesAndSupportedMessages()
    {
        var data = ReadAsterixStream0700Data();

        Assert.Equal(6_544_528, data.Length);

        var header = ParseHeader(data.AsSpan(0, StreamHeaderSize));
        Assert.Equal(new DateTime(2025, 7, 9, 7, 0, 0, DateTimeKind.Utc), header.StartTime);
        Assert.Equal(3_600_000u, header.DurationMs);
        Assert.Equal("--FileStreamOperator--", header.Operator);
        Assert.Equal([0xA2, 0xAA, 0xEE, 0xAA, 0x4C, 0x7F, 0x00, 0x00], header.Reserved);

        var result = ParseStream(data);

        Assert.Equal(data.Length, result.EndPosition);
        Assert.Equal(0, result.LengthErrors);
        Assert.Equal(43_441, result.FrameCount);
        Assert.Equal(55u, result.MinTimeOffsetMs);
        Assert.Equal(1_759_656u, result.MaxTimeOffsetMs);

        AssertCategory(result, 19, 3_344, 297_616, 89, 89, 257, 1_759_656);
        AssertCategory(result, 20, 17_156, 3_384_667, 84, 713, 57, 1_759_549);
        AssertCategory(result, 21, 22_597, 2_505_709, 47, 449, 55, 1_759_549);
        AssertCategory(result, 247, 344, 8_944, 26, 26, 5_057, 1_758_055);

        Assert.Equal(22_597, result.TypedParseSuccess[AsterixMessageI021.Category]);
        Assert.Equal(344, result.TypedParseSuccess[AsterixMessageI247.Category]);
        Assert.Empty(result.TypedParseFailures);

        Assert.Equal(344, result.VersionReports[(19, 1, 3)]);
        Assert.Equal(344, result.VersionReports[(20, 1, 8)]);
        Assert.Equal(344, result.VersionReports[(21, 2, 4)]);
        Assert.Equal(344, result.VersionReports[(23, 1, 2)]);
        Assert.Equal(344, result.VersionReports[(25, 1, 1)]);
    }

    private static byte[] ReadAsterixStream0700Data()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", AsterixStream0700FileName));
    }

    private static AsterixStreamHeader ParseHeader(ReadOnlySpan<byte> buffer)
    {
        Assert.Equal(StreamHeaderSize, buffer.Length);
        Assert.Equal("stream1\0", Encoding.ASCII.GetString(buffer[..8]));

        var year = BinaryPrimitives.ReadUInt16BigEndian(buffer[8..10]);
        var month = buffer[10];
        var day = buffer[11];
        var hour = buffer[12];
        var minute = buffer[13];
        var second = buffer[14];
        var millisecond = BinaryPrimitives.ReadUInt16BigEndian(buffer[15..17]);
        var durationMs = BinaryPrimitives.ReadUInt32BigEndian(buffer[17..21]);
        var operatorName = Encoding.ASCII.GetString(buffer[21..43]).TrimEnd('\0');

        return new AsterixStreamHeader(
            new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc),
            durationMs,
            operatorName,
            buffer[56..64].ToArray());
    }

    private static AsterixStreamParseResult ParseStream(byte[] data)
    {
        var result = new AsterixStreamParseResult();
        var position = StreamHeaderSize;

        while (position < data.Length)
        {
            var frameOffset = position;
            var payloadLength = BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(position, 4));
            position += 4;
            var timeOffsetMs = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(position, 4));
            position += 4;

            if (payloadLength < 3 || position + payloadLength > data.Length)
            {
                result.LengthErrors++;
                break;
            }

            var payload = data.AsSpan(position, payloadLength);
            position += payloadLength;

            var category = payload[0];
            var asterixLength = BinaryPrimitives.ReadUInt16BigEndian(payload[1..3]);
            if (asterixLength != payloadLength)
            {
                result.LengthErrors++;
                continue;
            }

            result.AddFrame(category, payloadLength, timeOffsetMs);
            if (category == AsterixMessageI021.Category)
            {
                ParseCat021(result, payload, frameOffset);
            }
            else if (category == AsterixMessageI247.Category)
            {
                ParseCat247(result, payload, frameOffset);
            }
        }

        result.EndPosition = position;
        return result;
    }

    private static void ParseCat021(AsterixStreamParseResult result, ReadOnlySpan<byte> payload, int frameOffset)
    {
        try
        {
            var message = new AsterixMessageI021();
            var buffer = payload;
            message.Deserialize(ref buffer);
            if (buffer.Length != 0)
            {
                result.AddFailure(AsterixMessageI021.Category, frameOffset, $"remaining={buffer.Length}");
                return;
            }

            result.AddTypedSuccess(AsterixMessageI021.Category);
        }
        catch (Exception ex)
        {
            result.AddFailure(AsterixMessageI021.Category, frameOffset, ex.Message);
        }
    }

    private static void ParseCat247(AsterixStreamParseResult result, ReadOnlySpan<byte> payload, int frameOffset)
    {
        try
        {
            var message = new AsterixMessageI247();
            var buffer = payload;
            message.Deserialize(ref buffer);
            if (buffer.Length != 0)
            {
                result.AddFailure(AsterixMessageI247.Category, frameOffset, $"remaining={buffer.Length}");
                return;
            }

            result.AddTypedSuccess(AsterixMessageI247.Category);
            foreach (var record in message)
            {
                var report = record.CategoryVersionNumberReport;
                if (report == null)
                {
                    continue;
                }

                foreach (var version in report.Versions)
                {
                    var key = (version.Category, version.Major, version.Minor);
                    result.VersionReports[key] = result.VersionReports.GetValueOrDefault(key) + 1;
                }
            }
        }
        catch (Exception ex)
        {
            result.AddFailure(AsterixMessageI247.Category, frameOffset, ex.Message);
        }
    }

    private static void AssertCategory(
        AsterixStreamParseResult result,
        byte category,
        int frameCount,
        long bytes,
        int minLength,
        int maxLength,
        uint firstTimeOffsetMs,
        uint lastTimeOffsetMs)
    {
        var stats = result.Categories[category];
        Assert.Equal(frameCount, stats.FrameCount);
        Assert.Equal(bytes, stats.PayloadBytes);
        Assert.Equal(minLength, stats.MinPayloadLength);
        Assert.Equal(maxLength, stats.MaxPayloadLength);
        Assert.Equal(firstTimeOffsetMs, stats.FirstTimeOffsetMs);
        Assert.Equal(lastTimeOffsetMs, stats.LastTimeOffsetMs);
    }

    private readonly record struct AsterixStreamHeader(
        DateTime StartTime,
        uint DurationMs,
        string Operator,
        byte[] Reserved);

    private sealed class AsterixStreamParseResult
    {
        public int FrameCount { get; private set; }
        public int LengthErrors { get; set; }
        public int EndPosition { get; set; }
        public uint MinTimeOffsetMs { get; private set; } = uint.MaxValue;
        public uint MaxTimeOffsetMs { get; private set; }
        public Dictionary<byte, AsterixCategoryStats> Categories { get; } = [];
        public Dictionary<byte, int> TypedParseSuccess { get; } = [];
        public List<string> TypedParseFailures { get; } = [];
        public Dictionary<(byte Category, byte Major, byte Minor), int> VersionReports { get; } = [];

        public void AddFrame(byte category, int payloadLength, uint timeOffsetMs)
        {
            FrameCount++;
            MinTimeOffsetMs = Math.Min(MinTimeOffsetMs, timeOffsetMs);
            MaxTimeOffsetMs = Math.Max(MaxTimeOffsetMs, timeOffsetMs);

            if (!Categories.TryGetValue(category, out var stats))
            {
                stats = new AsterixCategoryStats(category, timeOffsetMs);
                Categories.Add(category, stats);
            }

            stats.AddFrame(payloadLength, timeOffsetMs);
        }

        public void AddTypedSuccess(byte category)
        {
            TypedParseSuccess[category] = TypedParseSuccess.GetValueOrDefault(category) + 1;
        }

        public void AddFailure(byte category, int frameOffset, string reason)
        {
            TypedParseFailures.Add($"CAT{category:D3} offset=0x{frameOffset:X}: {reason}");
        }
    }

    private sealed class AsterixCategoryStats(byte category, uint firstTimeOffsetMs)
    {
        public byte Category { get; } = category;
        public int FrameCount { get; private set; }
        public long PayloadBytes { get; private set; }
        public int MinPayloadLength { get; private set; } = int.MaxValue;
        public int MaxPayloadLength { get; private set; }
        public uint FirstTimeOffsetMs { get; } = firstTimeOffsetMs;
        public uint LastTimeOffsetMs { get; private set; }

        public void AddFrame(int payloadLength, uint timeOffsetMs)
        {
            FrameCount++;
            PayloadBytes += payloadLength;
            MinPayloadLength = Math.Min(MinPayloadLength, payloadLength);
            MaxPayloadLength = Math.Max(MaxPayloadLength, payloadLength);
            LastTimeOffsetMs = timeOffsetMs;
        }
    }
}
