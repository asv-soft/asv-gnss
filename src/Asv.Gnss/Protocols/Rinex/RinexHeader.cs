using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Asv.Gnss;

/// <summary>File context. Observation values retain stored units; header corrections are not applied automatically.</summary>
public sealed partial class RinexHeader
{
    /// <summary>RINEX format revision; supported revisions are 2.10/2.11, 3.00–3.05 and 4.00–4.02.</summary>
    public decimal Version { get; set; } = 3.04m;
    /// <summary>Native file type: O for observations, N for navigation, M for meteorology, or a legacy navigation selector.</summary>
    public char FileType { get; set; } = 'O';
    /// <summary>Native satellite-system character; M denotes a mixed-system file.</summary>
    public char SatelliteSystem { get; set; } = 'G';
    /// <summary>Time-system identifier from TIME OF FIRST OBS; no conversion is performed.</summary>
    public string TimeSystem { get; set; } = string.Empty;
    /// <summary>Ordered observation codes for each RINEX 3/4 satellite system.</summary>
    public Dictionary<char, List<string>> ObservationTypes { get; } = new();
    /// <summary>Ordered observation or meteorological codes from the legacy header descriptor.</summary>
    public List<string> LegacyObservationTypes { get; } = new();
    /// <summary>Original records, including unknown metadata and END OF HEADER, in source order.</summary>
    public List<string> HeaderLines { get; } = new();
    /// <summary>Station marker name from retained header metadata, or an empty string when absent.</summary>
    public string MarkerName => GetValue("MARKER NAME");

    /// <summary>Returns the first matching header payload with surrounding spaces removed.</summary>
    public string GetValue(string label) => HeaderLines
        .Where(line => Label(line) == label)
        .Select(line => Field(line, 0, 60).Trim()).FirstOrDefault() ?? string.Empty;

    /// <summary>Validates a complete native header and constructs its record-decoding context.</summary>
    public static RinexHeader Parse(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        var header = new RinexHeader();
        header.HeaderLines.AddRange(lines);
        if (header.HeaderLines.Count < 2 || Label(header.HeaderLines[0]) != "RINEX VERSION / TYPE" ||
            Label(header.HeaderLines[^1]) != "END OF HEADER")
            throw new FormatException("A RINEX header requires VERSION / TYPE and END OF HEADER records.");
        var first = header.HeaderLines[0];
        header.Version = decimal.Parse(Field(first, 0, 9), NumberStyles.Float, CultureInfo.InvariantCulture);
        ValidateVersion(header.Version);
        header.FileType = Field(first, 20, 1)[0];
        header.SatelliteSystem = Field(first, 40, 1)[0];
        if (header.Version < 3)
            header.SatelliteSystem = header.FileType switch { 'G' => 'R', 'H' => 'S', _ => header.SatelliteSystem };
        if (header.SatelliteSystem == ' ') header.SatelliteSystem = 'G';
        if (header.FileType is not ('O' or 'N' or 'M' or 'G' or 'H' or 'L'))
            throw new NotSupportedException($"RINEX file type '{header.FileType}' is not supported; expected OBS, NAV or MET.");
        if (header.Version >= 3 && header.FileType is 'G' or 'H' or 'L')
            throw new FormatException("RINEX 3/4 navigation files use file type N.");
        char currentSystem = '\0';
        var declared = new Dictionary<char, int>();
        int? legacyCount = null;
        foreach (var line in header.HeaderLines.Skip(1))
        {
            switch (Label(line))
            {
                case "SYS / # / OBS TYPES":
                    var system = Field(line, 0, 1)[0];
                    var countText = Field(line, 3, 3).Trim();
                    if (system != ' ')
                    {
                        currentSystem = system;
                        if (countText.Length == 0) throw new FormatException("Missing RINEX system observation count.");
                        declared[system] = ParseCount(countText);
                        header.ObservationTypes[system] = new List<string>();
                    }
                    if (currentSystem == '\0') throw new FormatException("RINEX observation continuation has no system.");
                    for (var offset = 7; offset + 3 <= 60; offset += 4)
                    {
                        var code = Field(line, offset, 3).Trim();
                        if (code.Length != 0) header.ObservationTypes[currentSystem].Add(code);
                    }
                    break;
                case "# / TYPES OF OBSERV":
                    var legacyText = Field(line, 0, 6).Trim();
                    if (legacyText.Length != 0)
                    {
                        legacyCount = ParseCount(legacyText);
                        header.LegacyObservationTypes.Clear();
                    }
                    for (var offset = 6; offset < 60; offset += 6)
                    {
                        var code = Field(line, offset, 6).Trim();
                        if (code.Length != 0) header.LegacyObservationTypes.Add(code);
                    }
                    break;
                case "TIME OF FIRST OBS":
                    header.TimeSystem = Field(line, 48, 3).Trim();
                    break;
            }
        }
        foreach (var pair in declared)
            if (header.ObservationTypes[pair.Key].Count != pair.Value)
                throw new FormatException($"RINEX observation count for system {pair.Key} does not match its descriptors.");
        if (legacyCount.HasValue && header.LegacyObservationTypes.Count != legacyCount)
            throw new FormatException("RINEX observation count does not match its descriptors.");
        if (header.FileType == 'O' && ((header.Version < 3 && !legacyCount.HasValue) ||
            (header.Version >= 3 && header.ObservationTypes.Count == 0)))
            throw new FormatException("A RINEX observation header must declare observation types.");
        if (header.FileType == 'M' && header.LegacyObservationTypes.Count == 0)
            throw new FormatException("A RINEX meteorological header must declare observation types.");
        return header;
    }

    internal IEnumerable<string> WriteLines()
    {
        ValidateVersion(Version);
        if (FileType is not ('O' or 'N' or 'M' or 'G' or 'H' or 'L') ||
            (Version >= 3 && FileType is 'G' or 'H' or 'L'))
            throw new NotSupportedException($"RINEX file type '{FileType}' is not supported for version {Version}.");
        if (FileType == 'O' && (Version < 3 ? LegacyObservationTypes.Count == 0 : ObservationTypes.Count == 0) ||
            FileType == 'M' && LegacyObservationTypes.Count == 0)
            throw new FormatException("A RINEX observation or meteorological header requires observation types.");
        if (TimeSystem.Length > 3) throw new ArgumentException("RINEX time-system codes are at most three characters.");
        if (TimeSystem.Length != 0 && !HeaderLines.Any(line => Label(line) == "TIME OF FIRST OBS"))
            throw new FormatException("A time-system identifier requires a TIME OF FIRST OBS metadata record.");
        yield return FormatLine($"{Version.ToString("0.00", CultureInfo.InvariantCulture),9}".PadRight(20) +
            FileType + new string(' ', 19) + SatelliteSystem, "RINEX VERSION / TYPE");
        if (FileType == 'O' && Version >= 3)
        {
            foreach (var pair in ObservationTypes)
            {
                if (pair.Value.Count == 0) throw new InvalidOperationException("An observation system requires at least one code.");
                for (var start = 0; start < pair.Value.Count; start += 13)
                {
                    var prefix = start == 0 ? $"{pair.Key}  {pair.Value.Count,3} " : "       ";
                    yield return FormatLine(prefix + string.Concat(pair.Value.Skip(start).Take(13).Select(code =>
                    {
                        if (code.Length != 3 && !(code.Length == 2 && code[0] == 'X'))
                            throw new ArgumentException("RINEX 3/4 observation codes have three columns.");
                        return code.PadRight(4);
                    })), "SYS / # / OBS TYPES");
                }
            }
        }
        else if (FileType is 'O' or 'M')
        {
            for (var start = 0; start < LegacyObservationTypes.Count; start += 9)
                yield return FormatLine((start == 0 ? $"{LegacyObservationTypes.Count,6}" : "      ") +
                    string.Concat(LegacyObservationTypes.Skip(start).Take(9).Select(code =>
                    {
                        if (code.Length != 2) throw new ArgumentException("Legacy and meteorological observation codes have two characters.");
                        return code.PadLeft(6);
                    })), "# / TYPES OF OBSERV");
        }
        foreach (var line in HeaderLines)
        {
            var label = Label(line);
            if (label is "RINEX VERSION / TYPE" or "END OF HEADER" or "SYS / # / OBS TYPES" or "# / TYPES OF OBSERV") continue;
            if (label == "TIME OF FIRST OBS")
            {
                if (TimeSystem.Length > 3) throw new ArgumentException("RINEX time-system codes are at most three characters.");
                yield return FormatLine(Field(line, 0, 48) + TimeSystem.PadLeft(3) + Field(line, 51, 9), label);
            }
            else yield return line;
        }
        yield return FormatLine(string.Empty, "END OF HEADER");
    }

    internal RinexHeader WithUpdates(IEnumerable<string> records)
    {
        var combined = HeaderLines.Count > 0 ? HeaderLines.ToList() : WriteLines().ToList();
        foreach (var group in records.GroupBy(Label))
        {
            var label = group.Key;
            if (label is "RINEX VERSION / TYPE" or "END OF HEADER")
                throw new FormatException("An event cannot replace the file version or header boundary.");
            if (label == "SYS / # / OBS TYPES")
            {
                // Rebuild all system descriptors so continuation lines cannot retain stale codes.
                var updates = group.ToArray();
                var replacement = Parse(combined.Take(combined.Count - 1).Where(line => Label(line) != label)
                    .Concat(updates).Append(FormatLine(string.Empty, "END OF HEADER")));
                foreach (var pair in ObservationTypes)
                    if (!replacement.ObservationTypes.ContainsKey(pair.Key)) replacement.ObservationTypes[pair.Key] = pair.Value.ToList();
                combined = replacement.WriteLines().ToList();
            }
            else
            {
                combined.RemoveAll(line => Label(line) == label);
                combined.InsertRange(combined.Count - 1, group);
            }
        }
        return Parse(combined);
    }

    internal static string Label(string line) => line.Length > 60 ? line[60..].Trim() : string.Empty;
    internal static string Field(string line, int start, int width) => start >= line.Length
        ? new string(' ', width) : line.Substring(start, Math.Min(width, line.Length - start)).PadRight(width);
    /// <summary>Formats a native 60-column header payload followed by its 20-column label.</summary>
    public static string FormatLine(string value, string label)
    {
        if (value.Length > 60 || label.Length > 20) throw new ArgumentException("A RINEX header record exceeds its field width.");
        return value.PadRight(60) + label.PadRight(20);
    }
    private static int ParseCount(string value)
    {
        var count = int.Parse(value, CultureInfo.InvariantCulture);
        if (count is < 0 or > 999) throw new FormatException("Invalid RINEX observation type count.");
        return count;
    }
    internal static void ValidateVersion(decimal version)
    {
        if (version * 100 != decimal.Truncate(version * 100) ||
            (version is not (2.10m or 2.11m) && !(version >= 3.00m && version <= 3.05m) &&
            !(version >= 4.00m && version <= 4.02m)))
            throw new NotSupportedException($"RINEX version {version} is not supported.");
    }
}

/// <summary>A complete file header, including metadata needed to decode later records.</summary>
public sealed partial class RinexHeaderMessage : RinexMessageBase
{
    /// <inheritdoc />
    public override RinexMessageId Id => RinexMessageId.Header;
    /// <inheritdoc />
    protected override void ReadLines(IReadOnlyList<string> lines) => Header = RinexHeader.Parse(lines);
    /// <inheritdoc />
    protected override IEnumerable<string> WriteLines() => Header.WriteLines();
}
