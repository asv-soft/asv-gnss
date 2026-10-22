using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Asv.Gnss;

/// <summary>A header-defined meteorological observable with its nullable value in the native unit.</summary>
/// <param name="Code">Two-character descriptor, such as PR, TD or HR.</param>
/// <param name="Value">Stored measurement; null denotes a blank field.</param>
public sealed partial record RinexMeteorologicalValue(string Code, double? Value);

/// <summary>Meteorological observations in the order declared by the file header.</summary>
public partial class RinexMeteorologicalMessage : RinexMessageBase
{
    /// <inheritdoc />
    public override RinexMessageId Id => RinexMessageId.Meteorological;
    /// <summary>Calendar epoch in the file time system; no UTC conversion is implied.</summary>
    public RinexEpoch Epoch { get; set; }
    /// <summary>Observable values in header-defined order, including explicit missing values.</summary>
    public List<RinexMeteorologicalValue> Observations { get; } = [];

    internal static int? GetRecordLineCount(IReadOnlyList<string> pending, RinexHeader header)
    {
        if (header.LegacyObservationTypes.Count == 0) throw new FormatException("Meteorological observation types are missing from the header.");
        return 1 + Math.Max(0, (header.LegacyObservationTypes.Count - 8 + 9) / 10);
    }

    /// <inheritdoc />
    protected override void ReadLines(IReadOnlyList<string> lines)
    {
        if (lines.Count != GetRecordLineCount(lines, Header)) throw new FormatException("Incomplete meteorological record.");
        var legacy = Header.Version < 3;
        var offset = legacy ? 18 : 20;
        var fields = RinexNavigationFields.Slice(lines[0], 0, offset).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length != 6) throw new FormatException("Invalid meteorological epoch.");
        var year = int.Parse(fields[0], CultureInfo.InvariantCulture);
        if (legacy) year += year >= 80 ? 1900 : 2000;
        Epoch = new RinexEpoch(year, int.Parse(fields[1], CultureInfo.InvariantCulture), int.Parse(fields[2], CultureInfo.InvariantCulture), int.Parse(fields[3], CultureInfo.InvariantCulture), int.Parse(fields[4], CultureInfo.InvariantCulture), decimal.Parse(fields[5], CultureInfo.InvariantCulture));
        Epoch.Validate();
        Observations.Clear();
        for (var i = 0; i < Header.LegacyObservationTypes.Count; i++)
        {
            var row = i < 8 ? 0 : 1 + (i - 8) / 10;
            var column = i < 8 ? offset + i * 7 : 4 + (i - 8) % 10 * 7;
            Observations.Add(new RinexMeteorologicalValue(Header.LegacyObservationTypes[i], RinexNavigationFields.ReadNumber(lines[row], column, 7)));
        }
        for (var row = 0; row < lines.Count; row++)
        {
            var count = row == 0 ? Math.Min(8, Observations.Count) : Math.Min(10, Observations.Count - 8 - (row - 1) * 10);
            var end = (row == 0 ? offset : 4) + count * 7;
            if (lines[row].Length > end && !string.IsNullOrWhiteSpace(lines[row][end..])) throw new FormatException("Meteorological values exceed the header observation count.");
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<string> WriteLines()
    {
        Epoch.Validate();
        if (Header.Version < 3 && Epoch.Year is < 1980 or > 2079) throw new FormatException("RINEX 2 two-digit years represent 1980 through 2079.");
        if (Observations.Count != Header.LegacyObservationTypes.Count || !Observations.Select(x => x.Code).SequenceEqual(Header.LegacyObservationTypes))
            throw new FormatException("Meteorological observations must match the header types and order.");
        if (Observations.Count == 0) throw new FormatException("A meteorological record requires observation types.");
        if (Epoch.Seconds != decimal.Truncate(Epoch.Seconds)) throw new FormatException("Meteorological epochs require integer seconds.");
        var line = Header.Version < 3
            ? FormattableString.Invariant($" {Epoch.Year % 100:00} {Epoch.Month,2} {Epoch.Day,2} {Epoch.Hour,2} {Epoch.Minute,2} {Epoch.Seconds,2:0}")
            : FormattableString.Invariant($" {Epoch.Year:0000} {Epoch.Month,2} {Epoch.Day,2} {Epoch.Hour,2} {Epoch.Minute,2} {Epoch.Seconds,2:0}");
        for (var i = 0; i < Observations.Count; i++)
        {
            if (i == 8 || i > 8 && (i - 8) % 10 == 0)
            {
                yield return line;
                line = "    ";
            }
            var value = Observations[i].Value;
            if (value is null) line += "       ";
            else
            {
                if (!double.IsFinite(value.Value)) throw new FormatException("Meteorological values must be finite.");
                var text = value.Value.ToString("0.0", CultureInfo.InvariantCulture);
                if (text.Length > 7) throw new FormatException("Meteorological value exceeds F7.1 width.");
                line += text.PadLeft(7);
            }
        }
        yield return line;
    }
}
