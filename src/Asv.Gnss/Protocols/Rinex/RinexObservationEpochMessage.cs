using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Asv.Gnss;

/// <summary>A RINEX observation epoch, event, or cycle-slip record.</summary>
public sealed class RinexObservationEpochMessage : RinexMessageBase
{
    /// <inheritdoc />
    public override RinexMessageId Id => RinexMessageId.ObservationEpoch;
    /// <summary>Calendar epoch in the file time system; no UTC conversion is implied.</summary>
    public RinexEpoch Epoch { get; set; }
    /// <summary>Native epoch flag: 0/1 for observations, 2–5 for events, and 6 for cycle slips.</summary>
    public int EpochFlag { get; set; }
    /// <summary>Optional receiver clock offset in seconds, as stored in the record.</summary>
    public double? ReceiverClockOffset { get; set; }
    /// <summary>Satellite records belonging to this observation epoch.</summary>
    public List<RinexSatelliteObservation> Satellites { get; } = [];
    /// <summary>Native event or header-update lines for epoch flags 2–5.</summary>
    public List<string> SpecialRecords { get; } = [];

    internal static int? GetRecordLineCount(IReadOnlyList<string> pending, RinexHeader header, bool endOfFile = false)
    {
        if (pending.Count == 0) return null;
        var legacy = header.Version < 3m;
        var flag = Integer(pending[0], legacy ? 28 : 31, 1);
        var count = Integer(pending[0], legacy ? 29 : 32, 3);
        if (flag is < 0 or > 6 || count < 0)
            throw new FormatException("Invalid RINEX observation record count or epoch flag.");
        if (flag is >= 2 and <= 5) return pending.Count >= count + 1 ? count + 1 : null;
        if (legacy)
        {
            var length = Math.Max(1, (count + 11) / 12) + count * ((header.LegacyObservationTypes.Count + 4) / 5);
            return pending.Count >= length ? length : null;
        }
        // One epoch of lookahead also accepts wrapped records without treating omitted trailing values as missing lines.
        for (var i = 1; i < pending.Count; i++)
            if (pending[i].StartsWith('>')) return i;
        return endOfFile ? pending.Count : null;
    }

    /// <inheritdoc />
    protected override void ReadLines(IReadOnlyList<string> lines)
    {
        if (lines.Count == 0) throw new FormatException("Missing RINEX observation epoch.");
        Satellites.Clear();
        SpecialRecords.Clear();
        var legacy = Header.Version < 3m;
        var first = lines[0];
        EpochFlag = Integer(first, legacy ? 28 : 31, 1);
        if (EpochFlag is < 0 or > 6) throw new FormatException("Invalid RINEX epoch flag.");
        var count = Integer(first, legacy ? 29 : 32, 3);
        if (count < 0) throw new FormatException("Invalid RINEX satellite or event record count.");
        var yearText = Field(first, legacy ? 1 : 2, legacy ? 2 : 4).Trim();
        if (yearText.Length == 0)
        {
            if (EpochFlag is not (>= 2 and <= 5)) throw new FormatException("Missing observation epoch time.");
            Epoch = default;
        }
        else
        {
            var year = int.Parse(yearText, CultureInfo.InvariantCulture);
            if (legacy) year += year >= 80 ? 1900 : 2000;
            var seconds = decimal.Parse(Field(first, legacy ? 15 : 18, 11), CultureInfo.InvariantCulture);
            if (Header.Version >= 4.02m && Field(first, 57, 5).Trim().Length > 0)
                seconds += Integer(first, 57, 5) / 1_000_000_000_000m;
            Epoch = new RinexEpoch(year, Integer(first, legacy ? 4 : 7, 2),
                Integer(first, legacy ? 7 : 10, 2), Integer(first, legacy ? 10 : 13, 2),
                Integer(first, legacy ? 13 : 16, 2), seconds);
            Epoch.Validate();
        }
        ReceiverClockOffset = Number(Field(first, legacy ? 68 : 41, legacy ? 12 : 15));
        if (EpochFlag is >= 2 and <= 5)
        {
            if (lines.Count != count + 1) throw new FormatException("Incorrect RINEX event record count.");
            SpecialRecords.AddRange(lines.Skip(1));
            return;
        }

        var index = 1;
        if (legacy)
        {
            var satelliteLines = Math.Max(1, (count + 11) / 12);
            if (lines.Count < satelliteLines) throw new FormatException("Incomplete RINEX satellite list.");
            for (var i = 0; i < count; i++)
            {
                var satellite = Field(lines[i / 12], 32 + i % 12 * 3, 3);
                if (string.IsNullOrWhiteSpace(satellite)) throw new FormatException("Missing RINEX satellite identifier.");
                Satellites.Add(new RinexSatelliteObservation { Satellite = NormalizeSatellite(satellite) });
            }
            index = satelliteLines;
            foreach (var satellite in Satellites)
            {
                var codes = Header.LegacyObservationTypes;
                if (codes.Count == 0) throw new FormatException("Missing RINEX observation types.");
                for (var i = 0; i < codes.Count; i++)
                {
                    if (index + i / 5 >= lines.Count) throw new FormatException("Incomplete RINEX observations.");
                    satellite.Observations.Add(ReadObservation(codes[i], Field(lines[index + i / 5], i % 5 * 16, 16)));
                }
                index += (codes.Count + 4) / 5;
            }
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                if (index >= lines.Count) throw new FormatException("Incomplete RINEX observations.");
                var line = lines[index++];
                var identifier = Field(line, 0, 3);
                if (identifier.Length != 3 || !char.IsLetter(identifier[0]) ||
                    !int.TryParse(identifier[1..], NumberStyles.Integer, CultureInfo.InvariantCulture, out var satelliteNumber) ||
                    satelliteNumber is < 1 or > 99)
                    throw new FormatException("Invalid RINEX satellite identifier.");
                identifier = NormalizeSatellite(identifier);
                if (!Header.ObservationTypes.TryGetValue(identifier[0], out var codes) || codes.Count == 0)
                    throw new FormatException($"Missing RINEX observation types for {identifier[0]}.");
                var data = new StringBuilder(line.Length > 3 ? line[3..] : string.Empty);
                while (index < lines.Count && lines[index].StartsWith("   ", StringComparison.Ordinal))
                {
                    // Nonstandard wrapped RINEX 3 records retain complete 16-character fields.
                    var remainder = data.Length % 16;
                    if (remainder != 0 && string.IsNullOrWhiteSpace(data.ToString(data.Length - remainder, remainder)))
                        data.Length -= remainder;
                    else if (remainder != 0)
                        data.Append(' ', 16 - remainder);
                    data.Append(lines[index].Length > 3 ? lines[index][3..] : string.Empty);
                    index++;
                }
                var satellite = new RinexSatelliteObservation { Satellite = identifier };
                var text = data.ToString();
                if (text.Length > codes.Count * 16 && !string.IsNullOrWhiteSpace(text[(codes.Count * 16)..]))
                    throw new FormatException("RINEX observation fields exceed the header-defined count.");
                for (var j = 0; j < codes.Count; j++)
                    satellite.Observations.Add(ReadObservation(codes[j], Field(text, j * 16, 16)));
                Satellites.Add(satellite);
            }
        }
        if (index != lines.Count) throw new FormatException("Unexpected trailing RINEX observation records.");
    }

    /// <inheritdoc />
    protected override IEnumerable<string> WriteLines()
    {
        if (EpochFlag is < 0 or > 6) throw new FormatException("Invalid RINEX epoch flag.");
        var legacy = Header.Version < 3m;
        var special = EpochFlag is >= 2 and <= 5;
        if (Epoch != default || !special) Epoch.Validate();
        if (legacy && Epoch != default && Epoch.Year is < 1980 or > 2079)
            throw new FormatException("RINEX 2 epochs must be in the unambiguous range 1980-2079.");
        if (ReceiverClockOffset.HasValue && !double.IsFinite(ReceiverClockOffset.Value))
            throw new FormatException("Receiver clock offset must be finite.");
        var count = special ? SpecialRecords.Count : Satellites.Count;
        if (count > 999) throw new FormatException("Too many RINEX satellites or event records.");
        var prefix = legacy ? " " : "> ";
        if (Epoch == default && special)
            prefix += new string(' ', legacy ? 25 : 27);
        else
        {
            var seconds = Header.Version >= 4.02m ? decimal.Floor(Epoch.Seconds * 10_000_000m) / 10_000_000m : Epoch.Seconds;
            prefix += legacy ? (Epoch.Year % 100).ToString("00", CultureInfo.InvariantCulture) : Epoch.Year.ToString("0000", CultureInfo.InvariantCulture);
            prefix += legacy
                ? FormattableString.Invariant($" {Epoch.Month,2} {Epoch.Day,2} {Epoch.Hour,2} {Epoch.Minute,2}")
                : FormattableString.Invariant($" {Epoch.Month:00} {Epoch.Day:00} {Epoch.Hour:00} {Epoch.Minute:00}");
            var secondsText = seconds.ToString("0.0000000", CultureInfo.InvariantCulture);
            if (decimal.Parse(secondsText, CultureInfo.InvariantCulture) >= 61m)
                throw new FormatException("RINEX seconds round outside the representable interval [0, 61).");
            prefix += Fixed(secondsText, 11);
        }
        prefix += "  " + EpochFlag.ToString(CultureInfo.InvariantCulture) + Fixed(count.ToString(CultureInfo.InvariantCulture), 3);
        if (!legacy)
        {
            prefix += "      " + (ReceiverClockOffset.HasValue ? Fixed(ReceiverClockOffset.Value.ToString("0.000000000000", CultureInfo.InvariantCulture), 15) : new string(' ', 15));
            if (Header.Version >= 4.02m && Epoch != default)
            {
                var remainder = (int)decimal.Truncate((Epoch.Seconds - decimal.Floor(Epoch.Seconds * 10_000_000m) / 10_000_000m) * 1_000_000_000_000m);
                if (remainder != 0) prefix += " " + remainder.ToString("00000", CultureInfo.InvariantCulture);
            }
            yield return prefix.TrimEnd();
        }
        else
        {
            var firstSatellites = special ? string.Empty : string.Concat(Satellites.Take(12).Select(s => SatelliteField(s.Satellite)));
            var line = prefix + firstSatellites;
            if (ReceiverClockOffset.HasValue)
                line = line.PadRight(68) + Fixed(ReceiverClockOffset.Value.ToString("0.000000000", CultureInfo.InvariantCulture), 12);
            yield return line;
            if (!special)
                for (var i = 12; i < count; i += 12)
                    yield return new string(' ', 32) + string.Concat(Satellites.Skip(i).Take(12).Select(s => SatelliteField(s.Satellite)));
        }
        if (special)
        {
            foreach (var record in SpecialRecords) yield return record;
            yield break;
        }
        foreach (var satellite in Satellites)
        {
            var identifier = SatelliteField(satellite.Satellite);
            if (!legacy && (!Header.ObservationTypes.TryGetValue(identifier[0], out var types) || types.Count == 0))
                throw new FormatException($"Missing RINEX observation types for {identifier[0]}.");
            IReadOnlyList<string> codes = legacy ? Header.LegacyObservationTypes : Header.ObservationTypes[identifier[0]];
            if (codes.Count == 0) throw new FormatException("Missing RINEX observation types.");
            if (satellite.Observations.Any(o => !codes.Contains(o.Code)))
                throw new FormatException("Observation code is not declared in the RINEX header.");
            var fields = codes.Select(code =>
            {
                var matches = satellite.Observations.Where(o => o.Code == code).ToList();
                if (matches.Count > 1) throw new FormatException("Duplicate RINEX observation code.");
                return WriteObservation(matches.SingleOrDefault());
            }).ToList();
            if (legacy)
            {
                for (var i = 0; i < fields.Count; i += 5)
                    yield return string.Concat(fields.Skip(i).Take(5));
            }
            else yield return identifier + string.Concat(fields);
        }
    }

    private string NormalizeSatellite(string value)
    {
        var system = char.IsLetter(value[0]) ? value[0] : Header.SatelliteSystem;
        if (system is ' ' or 'M' or '\0') system = 'G';
        var number = int.Parse(value[1..], CultureInfo.InvariantCulture);
        if (number is < 1 or > 99) throw new FormatException("RINEX satellite number must be in the range 1-99.");
        return system + number.ToString("00", CultureInfo.InvariantCulture);
    }

    private static string SatelliteField(string value)
    {
        if (value.Length != 3 || !char.IsLetter(value[0]) || !char.IsDigit(value[1]) || !char.IsDigit(value[2]))
            throw new FormatException("RINEX satellite identifier must contain a system and two digits.");
        if (value[1..] == "00") throw new FormatException("RINEX satellite number must be in the range 1-99.");
        return value;
    }

    private static RinexObservationValue ReadObservation(string code, string field) =>
        new(code, Number(Field(field, 0, 14)), Indicator(field, 14, 7), Indicator(field, 15, 9));

    private static string WriteObservation(RinexObservationValue? observation)
    {
        if (observation == null) return new string(' ', 16);
        if (observation.Value.HasValue && !double.IsFinite(observation.Value.Value))
            throw new FormatException("RINEX observation must be finite.");
        return (observation.Value.HasValue ? Fixed(observation.Value.Value.ToString("0.000", CultureInfo.InvariantCulture), 14) : new string(' ', 14)) +
               WriteIndicator(observation.LossOfLockIndicator, 7) + WriteIndicator(observation.SignalStrengthIndicator, 9);
    }

    private static string WriteIndicator(byte? value, byte maximum)
    {
        if (value > maximum) throw new FormatException("Invalid RINEX observation indicator.");
        return value?.ToString(CultureInfo.InvariantCulture) ?? " ";
    }

    private static byte? Indicator(string field, int offset, byte maximum)
    {
        var text = Field(field, offset, 1);
        if (string.IsNullOrWhiteSpace(text)) return null;
        if (!byte.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value) || value > maximum)
            throw new FormatException("Invalid RINEX observation indicator.");
        return value;
    }

    private static double? Number(string field)
    {
        if (string.IsNullOrWhiteSpace(field)) return null;
        var value = double.Parse(field.Replace('D', 'E').Replace('d', 'E'), NumberStyles.Float, CultureInfo.InvariantCulture);
        if (!double.IsFinite(value)) throw new FormatException("RINEX numeric value must be finite.");
        return value;
    }

    private static int Integer(string line, int offset, int count) => int.Parse(Field(line, offset, count), CultureInfo.InvariantCulture);

    private static string Field(string line, int offset, int count) =>
        offset >= line.Length ? string.Empty : line.Substring(offset, Math.Min(count, line.Length - offset));

    private static string Fixed(string value, int width) => value.Length <= width
        ? value.PadLeft(width)
        : throw new FormatException("RINEX value exceeds the fixed field width.");
}

/// <summary>Observations for one satellite in header-defined code order.</summary>
public sealed class RinexSatelliteObservation
{
    /// <summary>Native satellite identifier, such as G01 or R24; system-only identifiers are retained for applicable system records.</summary>
    public string Satellite { get; set; } = "G01";
    /// <summary>Observable values in header-defined order, including explicit missing values.</summary>
    public List<RinexObservationValue> Observations { get; } = [];
}

/// <summary>A stored observation; blank values and blank indicators remain distinct from zero.</summary>
public sealed record RinexObservationValue(string Code, double? Value, byte? LossOfLockIndicator = null, byte? SignalStrengthIndicator = null);
