using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Xml;
using ConsoleAppFramework;
using Spectre.Console;

namespace Asv.Gnss.Shell;

public class AsterixStreamCommand
{
    /// <summary>
    /// Parse an ASTERIX stream container and print frame statistics.
    /// </summary>
    /// <param name="file">-f, input .stream file path</param>
    /// <param name="kml">-k, optional output KML file for CAT021 aircraft tracks</param>
    /// <param name="minTrackPoints">minimum number of points required to export a track</param>
    /// <returns>Zero if the file was parsed successfully.</returns>
    [Command("asterix-stream")]
    public int Run(string file, string? kml = null, int minTrackPoints = 2)
    {
        try
        {
            var result = Parse(file, collectTracks: string.IsNullOrWhiteSpace(kml) == false);
            Print(result);
            if (string.IsNullOrWhiteSpace(kml) == false)
            {
                var trackCount = WriteKml(result, kml, minTrackPoints);
                AnsiConsole.MarkupLineInterpolated(
                    $"[green]KML written:[/] {Path.GetFullPath(kml)} [grey]({trackCount:N0} tracks)[/]");
            }

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Failed to parse stream:[/] {ex.Message}");
            return 1;
        }
    }

    private static AsterixStreamStatistics Parse(string file, bool collectTracks)
    {
        if (string.IsNullOrWhiteSpace(file))
        {
            throw new ArgumentException("Input file path is required.", nameof(file));
        }

        var path = Path.GetFullPath(file);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Input file was not found.", path);
        }

        using var stream = File.OpenRead(path);
        if (stream.Length < AsterixStreamHeader.Size)
        {
            throw new InvalidDataException("File is too short to contain an ASTERIX stream header.");
        }

        var headerBuffer = new byte[AsterixStreamHeader.Size];
        stream.ReadExactly(headerBuffer);
        var header = AsterixStreamHeader.Parse(headerBuffer);
        var result = new AsterixStreamStatistics(path, stream.Length, header);
        var prefix = new byte[8];

        while (stream.Position < stream.Length)
        {
            var frameOffset = stream.Position;
            stream.ReadExactly(prefix);
            var payloadLength = BinaryPrimitives.ReadUInt32BigEndian(prefix.AsSpan(0, 4));
            var timeOffsetMs = BinaryPrimitives.ReadUInt32BigEndian(prefix.AsSpan(4, 4));

            if (payloadLength < 3)
            {
                throw new InvalidDataException($"Invalid payload length {payloadLength} at offset 0x{frameOffset:X}.");
            }

            if (payloadLength > int.MaxValue)
            {
                throw new InvalidDataException($"Payload length {payloadLength} at offset 0x{frameOffset:X} is too large.");
            }

            if (stream.Position + payloadLength > stream.Length)
            {
                throw new InvalidDataException($"Frame at offset 0x{frameOffset:X} exceeds file length.");
            }

            var payload = new byte[payloadLength];
            stream.ReadExactly(payload);

            var category = payload[0];
            var asterixLength = BinaryPrimitives.ReadUInt16BigEndian(payload.AsSpan(1, 2));
            if (asterixLength != payloadLength)
            {
                throw new InvalidDataException(
                    $"ASTERIX length mismatch at offset 0x{frameOffset:X}: frame={payloadLength}, block={asterixLength}, CAT={category}.");
            }

            result.AddFrame(category, (int)payloadLength, timeOffsetMs);
            if (category == AsterixMessageI247.Category)
            {
                TryAddCategoryVersions(result, payload);
            }

            if (collectTracks && category == AsterixMessageI021.Category)
            {
                TryAddTrackPoints(result, payload, result.Header.StartTime.AddMilliseconds(timeOffsetMs));
            }
        }

        return result;
    }

    private static void TryAddTrackPoints(AsterixStreamStatistics result, byte[] payload, DateTime timestamp)
    {
        try
        {
            var message = new AsterixMessageI021();
            var buffer = new ReadOnlySpan<byte>(payload);
            message.Deserialize(ref buffer);
            if (buffer.Length != 0)
            {
                return;
            }

            foreach (var record in message)
            {
                var targetAddress = record.TargetAddress?.TargetAddress;
                if (targetAddress == null)
                {
                    continue;
                }

                var position = GetPosition(record);
                if (position == null)
                {
                    continue;
                }

                var (latitude, longitude) = position.Value;
                if (latitude is < -90.0 or > 90.0 || longitude is < -180.0 or > 180.0)
                {
                    continue;
                }

                var altitudeMeters = GetAltitudeMeters(record);
                var targetIdentification = record.TargetIdentification?.TargetIdentification?.Trim();
                result.AddTrackPoint(
                    targetAddress.Value,
                    string.IsNullOrWhiteSpace(targetIdentification) ? null : targetIdentification,
                    new AsterixTrackPoint(timestamp, latitude, longitude, altitudeMeters));
            }
        }
        catch
        {
            // Track generation is best-effort. Frame validation and statistics are handled by the stream parser.
        }
    }

    private static (double Latitude, double Longitude)? GetPosition(AsterixRecordI021 record)
    {
        if (record.HighResolutionPositionWgs84 != null)
        {
            return (record.HighResolutionPositionWgs84.Latitude, record.HighResolutionPositionWgs84.Longitude);
        }

        if (record.PositionWgs84 != null)
        {
            return (record.PositionWgs84.Latitude, record.PositionWgs84.Longitude);
        }

        return null;
    }

    private static double GetAltitudeMeters(AsterixRecordI021 record)
    {
        if (record.GeometricHeight != null)
        {
            return record.GeometricHeight.GeometricHeightFt * 0.3048;
        }

        if (record.FlightLevel != null)
        {
            return record.FlightLevel.FlightLevel * 100.0 * 0.3048;
        }

        return 0.0;
    }

    private static void TryAddCategoryVersions(AsterixStreamStatistics result, byte[] payload)
    {
        try
        {
            var message = new AsterixMessageI247();
            var buffer = new ReadOnlySpan<byte>(payload);
            message.Deserialize(ref buffer);
            if (buffer.Length != 0)
            {
                return;
            }

            foreach (var record in message)
            {
                var report = record.CategoryVersionNumberReport;
                if (report == null)
                {
                    continue;
                }

                foreach (var version in report.Versions)
                {
                    result.AddVersion(version.Category, version.Major, version.Minor);
                }
            }
        }
        catch
        {
            // CAT247 is auxiliary for this command. The validated frame statistics are still useful.
        }
    }

    private static void Print(AsterixStreamStatistics result)
    {
        var culture = CultureInfo.InvariantCulture;

        AnsiConsole.Write(new Rule("[bold yellow]ASTERIX Stream[/]").RuleStyle("grey").LeftJustified());

        var summary = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[grey]Property[/]")
            .AddColumn("[grey]Value[/]");

        summary.AddRow("File", Markup.Escape(result.FilePath));
        summary.AddRow("File size", result.FileSize.ToString("N0", culture) + " bytes");
        summary.AddRow("Header time", result.Header.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff", culture));
        summary.AddRow("Duration", FormatDuration(TimeSpan.FromMilliseconds(result.Header.DurationMs)));
        summary.AddRow("Operator", Markup.Escape(result.Header.Operator));
        summary.AddRow("Frames", result.FrameCount.ToString("N0", culture));
        summary.AddRow("Time offset", $"{FormatDuration(TimeSpan.FromMilliseconds(result.MinTimeOffsetMs))} .. {FormatDuration(TimeSpan.FromMilliseconds(result.MaxTimeOffsetMs))}");
        summary.AddRow("Reserved", result.Header.ReservedHex);
        AnsiConsole.Write(summary);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Categories[/]")
            .AddColumn(new TableColumn("CAT").Centered())
            .AddColumn(new TableColumn("Frames").RightAligned())
            .AddColumn(new TableColumn("%").RightAligned())
            .AddColumn(new TableColumn("Bytes").RightAligned())
            .AddColumn(new TableColumn("Len min").RightAligned())
            .AddColumn(new TableColumn("Len avg").RightAligned())
            .AddColumn(new TableColumn("Len max").RightAligned())
            .AddColumn("First")
            .AddColumn("Last");

        foreach (var category in result.Categories.Values.OrderBy(x => x.Category))
        {
            var percent = result.FrameCount == 0 ? 0.0 : category.FrameCount * 100.0 / result.FrameCount;
            table.AddRow(
                category.Category.ToString(culture),
                category.FrameCount.ToString("N0", culture),
                percent.ToString("N2", culture),
                category.PayloadBytes.ToString("N0", culture),
                category.MinPayloadLength.ToString(culture),
                category.AveragePayloadLength.ToString("N1", culture),
                category.MaxPayloadLength.ToString(culture),
                FormatDuration(TimeSpan.FromMilliseconds(category.FirstTimeOffsetMs)),
                FormatDuration(TimeSpan.FromMilliseconds(category.LastTimeOffsetMs)));
        }

        AnsiConsole.Write(table);

        if (result.CategoryVersions.Count == 0)
        {
            return;
        }

        var versions = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]CAT247 Version Report[/]")
            .AddColumn(new TableColumn("CAT").Centered())
            .AddColumn("Version")
            .AddColumn(new TableColumn("Reports").RightAligned());

        foreach (var version in result.CategoryVersions.Values.OrderBy(x => x.Category).ThenBy(x => x.Major).ThenBy(x => x.Minor))
        {
            versions.AddRow(
                version.Category.ToString(culture),
                $"{version.Major}.{version.Minor}",
                version.Count.ToString("N0", culture));
        }

        AnsiConsole.Write(versions);

        if (result.Tracks.Count > 0)
        {
            var tracks = result.Tracks.Values;
            var exportedPoints = tracks.Sum(x => x.Points.Count);
            AnsiConsole.MarkupLineInterpolated(
                $"[grey]Collected {tracks.Count:N0} CAT021 tracks with {exportedPoints:N0} points for optional KML export.[/]");
        }
    }

    private static int WriteKml(AsterixStreamStatistics result, string file, int minTrackPoints)
    {
        var path = Path.GetFullPath(file);
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }

        const string kmlNs = "http://www.opengis.net/kml/2.2";
        const string gxNs = "http://www.google.com/kml/ext/2.2";
        var exportedTracks = result.Tracks.Values
            .Where(x => x.Points.Count >= minTrackPoints)
            .OrderByDescending(x => x.Points.Count)
            .ThenBy(x => x.Name, StringComparer.Ordinal)
            .ToArray();

        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true
        };

        using var writer = XmlWriter.Create(path, settings);
        writer.WriteStartDocument();
        writer.WriteStartElement("kml", kmlNs);
        writer.WriteAttributeString("xmlns", "gx", null, gxNs);
        writer.WriteStartElement("Document", kmlNs);
        writer.WriteElementString("name", kmlNs, $"ASTERIX tracks {result.Header.StartTime:yyyy-MM-dd HH:mm:ss}");

        writer.WriteStartElement("Style", kmlNs);
        writer.WriteAttributeString("id", "aircraft-track");
        writer.WriteStartElement("LineStyle", kmlNs);
        writer.WriteElementString("color", kmlNs, "ff00ffff");
        writer.WriteElementString("width", kmlNs, "2");
        writer.WriteEndElement();
        writer.WriteStartElement("IconStyle", kmlNs);
        writer.WriteElementString("scale", kmlNs, "0.7");
        writer.WriteStartElement("Icon", kmlNs);
        writer.WriteElementString("href", kmlNs, "http://maps.google.com/mapfiles/kml/shapes/airports.png");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();

        foreach (var track in exportedTracks)
        {
            var orderedPoints = track.Points.OrderBy(x => x.Timestamp).ToArray();
            writer.WriteStartElement("Placemark", kmlNs);
            writer.WriteElementString("name", kmlNs, track.Name);
            writer.WriteElementString("styleUrl", kmlNs, "#aircraft-track");
            writer.WriteStartElement("description", kmlNs);
            writer.WriteCData($"Target address: {track.TargetAddress:X6}\nPoints: {orderedPoints.Length:N0}");
            writer.WriteEndElement();

            writer.WriteStartElement("gx", "Track", gxNs);
            writer.WriteElementString("altitudeMode", kmlNs, "absolute");
            foreach (var point in orderedPoints)
            {
                writer.WriteElementString("when", kmlNs, point.Timestamp.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
            }

            foreach (var point in orderedPoints)
            {
                writer.WriteElementString(
                    "gx",
                    "coord",
                    gxNs,
                    string.Create(
                        CultureInfo.InvariantCulture,
                        $"{point.Longitude:F8} {point.Latitude:F8} {point.AltitudeMeters:F1}"));
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
        return exportedTracks.Length;
    }

    private static string FormatDuration(TimeSpan value)
    {
        return value.ToString(value.TotalHours >= 1 ? @"h\:mm\:ss\.fff" : @"m\:ss\.fff", CultureInfo.InvariantCulture);
    }

    private sealed class AsterixStreamStatistics(string filePath, long fileSize, AsterixStreamHeader header)
    {
        public string FilePath { get; } = filePath;
        public long FileSize { get; } = fileSize;
        public AsterixStreamHeader Header { get; } = header;
        public long FrameCount { get; private set; }
        public uint MinTimeOffsetMs { get; private set; } = uint.MaxValue;
        public uint MaxTimeOffsetMs { get; private set; }
        public Dictionary<byte, CategoryStatistics> Categories { get; } = [];
        public Dictionary<string, CategoryVersionStatistics> CategoryVersions { get; } = [];
        public Dictionary<uint, AsterixTrack> Tracks { get; } = [];

        public void AddFrame(byte category, int payloadLength, uint timeOffsetMs)
        {
            FrameCount++;
            MinTimeOffsetMs = Math.Min(MinTimeOffsetMs, timeOffsetMs);
            MaxTimeOffsetMs = Math.Max(MaxTimeOffsetMs, timeOffsetMs);

            if (!Categories.TryGetValue(category, out var statistics))
            {
                statistics = new CategoryStatistics(category);
                Categories.Add(category, statistics);
            }

            statistics.Add(payloadLength, timeOffsetMs);
        }

        public void AddVersion(byte category, byte major, byte minor)
        {
            var key = $"{category}:{major}:{minor}";
            if (!CategoryVersions.TryGetValue(key, out var statistics))
            {
                statistics = new CategoryVersionStatistics(category, major, minor);
                CategoryVersions.Add(key, statistics);
            }

            statistics.Count++;
        }

        public void AddTrackPoint(uint targetAddress, string? targetIdentification, AsterixTrackPoint point)
        {
            if (!Tracks.TryGetValue(targetAddress, out var track))
            {
                track = new AsterixTrack(targetAddress);
                Tracks.Add(targetAddress, track);
            }

            if (string.IsNullOrWhiteSpace(targetIdentification) == false)
            {
                track.TargetIdentification = targetIdentification;
            }

            track.Points.Add(point);
        }
    }

    private sealed class CategoryStatistics(byte category)
    {
        public byte Category { get; } = category;
        public long FrameCount { get; private set; }
        public long PayloadBytes { get; private set; }
        public int MinPayloadLength { get; private set; } = int.MaxValue;
        public int MaxPayloadLength { get; private set; }
        public uint FirstTimeOffsetMs { get; private set; }
        public uint LastTimeOffsetMs { get; private set; }
        public double AveragePayloadLength => FrameCount == 0 ? 0.0 : PayloadBytes / (double)FrameCount;

        public void Add(int payloadLength, uint timeOffsetMs)
        {
            if (FrameCount == 0)
            {
                FirstTimeOffsetMs = timeOffsetMs;
            }

            FrameCount++;
            PayloadBytes += payloadLength;
            MinPayloadLength = Math.Min(MinPayloadLength, payloadLength);
            MaxPayloadLength = Math.Max(MaxPayloadLength, payloadLength);
            LastTimeOffsetMs = timeOffsetMs;
        }
    }

    private sealed class CategoryVersionStatistics(byte category, byte major, byte minor)
    {
        public byte Category { get; } = category;
        public byte Major { get; } = major;
        public byte Minor { get; } = minor;
        public long Count { get; set; }
    }

    private sealed class AsterixTrack(uint targetAddress)
    {
        public uint TargetAddress { get; } = targetAddress;
        public string? TargetIdentification { get; set; }
        public List<AsterixTrackPoint> Points { get; } = [];
        public string Name => string.IsNullOrWhiteSpace(TargetIdentification)
            ? TargetAddress.ToString("X6", CultureInfo.InvariantCulture)
            : $"{TargetIdentification} ({TargetAddress:X6})";
    }

    private readonly record struct AsterixTrackPoint(
        DateTime Timestamp,
        double Latitude,
        double Longitude,
        double AltitudeMeters);

    private sealed class AsterixStreamHeader
    {
        public const int Size = 64;
        private const string Magic = "stream1\0";

        public DateTime StartTime { get; private init; }
        public uint DurationMs { get; private init; }
        public string Operator { get; private init; } = string.Empty;
        public string ReservedHex { get; private init; } = string.Empty;

        public static AsterixStreamHeader Parse(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length != Size)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), "ASTERIX stream header must be exactly 64 bytes.");
            }

            var magic = Encoding.ASCII.GetString(buffer[..8]);
            if (magic != Magic)
            {
                throw new InvalidDataException("Invalid ASTERIX stream magic. Expected 'stream1'.");
            }

            var year = BinaryPrimitives.ReadUInt16BigEndian(buffer[8..10]);
            var month = buffer[10];
            var day = buffer[11];
            var hour = buffer[12];
            var minute = buffer[13];
            var second = buffer[14];
            var millisecond = BinaryPrimitives.ReadUInt16BigEndian(buffer[15..17]);
            var durationMs = BinaryPrimitives.ReadUInt32BigEndian(buffer[17..21]);
            var operatorName = Encoding.ASCII.GetString(buffer[21..43]).TrimEnd('\0');
            var reserved = string.Join(" ", buffer[56..64].ToArray().Select(x => x.ToString("X2", CultureInfo.InvariantCulture)));

            return new AsterixStreamHeader
            {
                StartTime = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc),
                DurationMs = durationMs,
                Operator = operatorName,
                ReservedHex = reserved
            };
        }
    }
}
