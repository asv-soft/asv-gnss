using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Asv.Gnss;

/// <summary>A broadcast navigation record. Values retain the order and units of the RINEX specification.</summary>
public partial class RinexNavigationMessage : RinexMessageBase
{
    /// <inheritdoc />
    public override RinexMessageId Id => RinexMessageId.Navigation;
    /// <summary>Native satellite identifier, such as G01 or R24; system-only identifiers are retained for applicable system records.</summary>
    public string Satellite { get; set; } = "G01";
    /// <summary>Calendar epoch in the file time system; no UTC conversion is implied.</summary>
    public RinexEpoch Epoch { get; set; }
    /// <summary>Native navigation record family: EPH, STO, EOP or ION.</summary>
    public string RecordType { get; set; } = "EPH";
    /// <summary>RINEX 4 navigation-message designation, such as LNAV, CNAV or FDMA.</summary>
    public string NavigationMessageType { get; set; } = string.Empty;
    /// <summary>Optional RINEX 4.02 navigation subtype, such as KLOB or NEQN.</summary>
    public string NavigationMessageSubtype { get; set; } = string.Empty;
    /// <summary>STO time-system correction identifier retained from its text field.</summary>
    public string TimeSystemCorrectionCode { get; set; } = string.Empty;
    /// <summary>Optional SBAS identifier in an STO record.</summary>
    public string SbasIdentifier { get; set; } = string.Empty;
    /// <summary>Optional UTC realization identifier in an STO record.</summary>
    public string UtcIdentifier { get; set; } = string.Empty;
    /// <summary>Numeric navigation fields in specification order; null preserves a blank field.</summary>
    public List<double?> Values { get; } = [];
    /// <summary>Number of numeric fields in each data row, including blank fields.</summary>
    public List<int> ValueFieldCounts { get; } = [];

    internal static int? GetRecordLineCount(IReadOnlyList<string> pending, RinexHeader header)
    {
        if (pending.Count == 0) return null;
        if (header.Version >= 4)
        {
            for (var i = 1; i < pending.Count; i++)
                if (pending[i].StartsWith('>')) return i;
            return null;
        }
        var system = header.Version < 3 ? LegacySystem(header) : pending[0][0];
        return system == 'S' ? 4 : system == 'R' ? header.Version >= 3.05m ? 5 : 4 : 8;
    }

    /// <inheritdoc />
    protected override void ReadLines(IReadOnlyList<string> lines)
    {
        if (lines.Count == 0) throw new FormatException("A navigation record is empty.");
        Values.Clear();
        ValueFieldCounts.Clear();
        TimeSystemCorrectionCode = SbasIdentifier = UtcIdentifier = string.Empty;
        var start = 0;
        RecordType = "EPH";
        NavigationMessageType = NavigationMessageSubtype = string.Empty;
        if (Header.Version >= 4)
        {
            var descriptor = lines[0].Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (descriptor.Length < 4 || descriptor[0] != ">") throw new FormatException("Invalid RINEX 4 navigation descriptor.");
            RecordType = descriptor[1];
            Satellite = descriptor[2];
            NavigationMessageType = descriptor[3];
            NavigationMessageSubtype = descriptor.Length > 4 ? descriptor[4] : string.Empty;
            ValidateRecordType();
            start = 1;
        }
        if (lines.Count <= start) throw new FormatException("The navigation record has no data.");
        var first = lines[start];
        var legacy = Header.Version < 3;
        if (RecordType == "EPH")
        {
            var satellite = RinexNavigationFields.Slice(first, 0, legacy ? 2 : 3).Trim();
            if (Header.Version >= 4 && Satellite != satellite) throw new FormatException("The navigation descriptor and ephemeris satellite differ.");
            Satellite = legacy ? $"{LegacySystem(Header)}{int.Parse(satellite, CultureInfo.InvariantCulture):00}" : satellite;
            if (Satellite.Length != 3 || !"GRECJIS".Contains(Satellite[0])) throw new NotSupportedException($"Unsupported navigation satellite '{Satellite}'.");
        }
        ValidateRecordType();
        Epoch = legacy ? RinexNavigationFields.ReadLegacyEpoch(first, 3) : RinexNavigationFields.ReadEpoch(first, 4);
        Epoch.Validate();
        if (RecordType == "STO")
        {
            TimeSystemCorrectionCode = RinexNavigationFields.Slice(first, 24, 18).Trim();
            SbasIdentifier = RinexNavigationFields.Slice(first, 43, 18).Trim();
            UtcIdentifier = RinexNavigationFields.Slice(first, 62, 18).Trim();
        }
        else
        {
            var count = RecordType == "ION" ? Math.Clamp((Math.Max(0, first.Length - 23) + 18) / 19, 1, 3) : 3;
            var end = (legacy ? 22 : 23) + count * 19;
            if (first.Length > end && !string.IsNullOrWhiteSpace(first[end..])) throw new FormatException("Navigation epoch row exceeds its numeric field width.");
            ReadValues(first, legacy ? 22 : 23, count);
        }
        for (var i = start + 1; i < lines.Count; i++)
        {
            var offset = legacy ? 3 : 4;
            var count = Math.Clamp((Math.Max(0, lines[i].Length - offset) + 18) / 19, 1, 4);
            // RINEX 2/3 continuation rows contain four fields, even if trailing blanks are omitted.
            if (Header.Version < 4) count = 4;
            if (lines[i].Length > offset + 76 && !string.IsNullOrWhiteSpace(lines[i][(offset + 76)..]))
                throw new FormatException("Navigation data exceeds four numeric fields per continuation row.");
            ReadValues(lines[i], offset, count);
        }
        if (Header.Version < 4 && lines.Count != GetRecordLineCount(lines, Header))
            throw new FormatException("Incomplete navigation record.");
        if (Header.Version >= 4 && lines.Count - 1 != DefaultCounts().Length + (RecordType == "STO" ? 1 : 0))
            throw new FormatException("Incomplete or oversized RINEX 4 navigation record.");
    }

    /// <inheritdoc />
    protected override IEnumerable<string> WriteLines()
    {
        ValidateRecordType();
        Epoch.Validate();
        var legacy = Header.Version < 3;
        if (legacy && Epoch.Year is < 1980 or > 2079) throw new FormatException("RINEX 2 two-digit years represent 1980 through 2079.");
        if (Header.Version < 4 && RecordType != "EPH") throw new NotSupportedException("STO, EOP and ION records require RINEX 4.");
        if (Header.Version >= 4)
        {
            if (string.IsNullOrWhiteSpace(NavigationMessageType)) throw new FormatException("RINEX 4 requires a navigation message type.");
            yield return $"> {RecordType} {Satellite} {NavigationMessageType}{(NavigationMessageSubtype.Length == 0 ? string.Empty : " " + NavigationMessageSubtype)}";
        }
        var counts = ValueFieldCounts.Count == 0 ? DefaultCounts() : ValueFieldCounts.ToArray();
        if (counts.Any(x => x is < 1 or > 4) || counts.Sum() != Values.Count)
            throw new FormatException("Navigation values do not match their row field counts.");
        if (counts.Length != DefaultCounts().Length) throw new FormatException("Navigation rows do not match the selected message layout.");
        if (Header.Version < 4 && !counts.SequenceEqual(DefaultCounts())) throw new FormatException("RINEX 2/3 navigation continuation rows require four fields.");
        var first = RecordType == "EPH" ? legacy
            ? $"{int.Parse(Satellite[1..], CultureInfo.InvariantCulture),2} {RinexNavigationFields.WriteLegacyEpoch(Epoch)}"
            : $"{Satellite} {RinexNavigationFields.WriteEpoch(Epoch)}"
            : $"    {RinexNavigationFields.WriteEpoch(Epoch)}";
        var index = 0;
        if (RecordType == "STO")
        {
            if (TimeSystemCorrectionCode.Length > 18 || SbasIdentifier.Length > 18 || UtcIdentifier.Length > 18)
                throw new FormatException("STO identifiers exceed their fixed width.");
            yield return first + $" {TimeSystemCorrectionCode,-18} {SbasIdentifier,-18} {UtcIdentifier,-18}";
        }
        else
        {
            if (counts.Length == 0 || (RecordType != "ION" && counts[0] != 3) || (RecordType == "ION" && counts[0] > 3)) throw new FormatException("Invalid navigation epoch row field count.");
            yield return first + FormatValues(ref index, counts[0]);
        }
        for (var row = RecordType == "STO" ? 0 : 1; row < counts.Length; row++)
            yield return new string(' ', legacy ? 3 : 4) + FormatValues(ref index, counts[row]);
    }

    private int[] DefaultCounts()
    {
        if (RecordType == "STO") return [4];
        if (RecordType == "EOP") return [3, 4, 4];
        if (RecordType == "ION") return NavigationMessageType switch
        {
            "IFNV" => [3, 1],
            "LNAV" or "D1D2" or "CNVX" => [3, 4, 1],
            "CNV1" or "CNV2" or "CNV3" => [3, 4, 2],
            "LXOC" => [3],
            "L1NV" when NavigationMessageSubtype == "KLOB" => [1, 4, 4, 4],
            "L1NV" when NavigationMessageSubtype == "NEQN" => [1, 4, 4, 4, 4, 4, 4],
            _ => throw new NotSupportedException("Unknown ionosphere model; set a supported navigation message type and subtype.")
        };
        var rows = Header.Version >= 4 ? (Satellite[0], NavigationMessageType) switch
        {
            ('G' or 'J', "LNAV") => 8,
            ('G' or 'J', "CNAV") => 9,
            ('G' or 'J', "CNV2") => 10,
            ('E', "INAV" or "FNAV") => 8,
            ('R', "FDMA") => 5,
            ('R', "L1OC" or "L3OC") => 9,
            ('C', "D1" or "D2" or "D1D2") => 8,
            ('C', "CNV1" or "CNV2") => 10,
            ('C', "CNV3") => 9,
            ('I', "LNAV") => 8,
            ('I', "L1NV") => 9,
            ('S', "SBAS") => 4,
            _ => throw new NotSupportedException($"Unsupported ephemeris layout '{Satellite} {NavigationMessageType}'.")
        } : Satellite.StartsWith('S') ? 4 : Satellite.StartsWith('R') ? Header.Version >= 3.05m ? 5 : 4 : 8;
        return Enumerable.Range(0, rows).Select(x => x == 0 ? 3 : 4).ToArray();
    }

    private void ValidateRecordType()
    {
        if (RecordType is not ("EPH" or "STO" or "EOP" or "ION")) throw new NotSupportedException($"Unsupported RINEX navigation record '{RecordType}'.");
        if (Satellite.Length is not (1 or 3) || !"GRECJIS".Contains(Satellite[0]) ||
            Satellite.Length == 3 && (!char.IsAsciiDigit(Satellite[1]) || !char.IsAsciiDigit(Satellite[2])) ||
            RecordType == "EPH" && Satellite.Length != 3)
            throw new FormatException("Navigation satellite identifiers require a system code and a two-digit PRN, or a system code for system records.");
        if (NavigationMessageType.Length > 4 || NavigationMessageSubtype.Length > 4 ||
            NavigationMessageType.Any(char.IsWhiteSpace) || NavigationMessageSubtype.Any(char.IsWhiteSpace))
            throw new FormatException("Navigation type and subtype identifiers contain at most four non-whitespace characters.");
    }

    private void ReadValues(string line, int offset, int count)
    {
        ValueFieldCounts.Add(count);
        for (var i = 0; i < count; i++) Values.Add(RinexNavigationFields.ReadNumber(line, offset + i * 19, 19));
    }

    private string FormatValues(ref int index, int count)
    {
        var result = string.Empty;
        for (var i = 0; i < count; i++) result += RinexNavigationFields.WriteNumber(Values[index++], Header.Version < 3);
        return result;
    }

    private static char LegacySystem(RinexHeader header) => header.FileType switch { 'G' => 'R', 'H' => 'S', 'L' => 'E', _ => header.SatelliteSystem is ' ' or '\0' ? 'G' : header.SatelliteSystem };
}

internal static class RinexNavigationFields
{
    public static string Slice(string line, int start, int length) => start >= line.Length ? string.Empty : line.Substring(start, Math.Min(length, line.Length - start));
    public static double? ReadNumber(string line, int start, int length)
    {
        var value = Slice(line, start, length).Trim();
        if (value.Length == 0) return null;
        var number = double.Parse(value.Replace('D', 'E').Replace('d', 'E'), NumberStyles.Float, CultureInfo.InvariantCulture);
        if (!double.IsFinite(number)) throw new FormatException("RINEX numeric fields must be finite.");
        return number;
    }
    public static string WriteNumber(double? value, bool legacy)
    {
        if (value == null) return new string(' ', 19);
        if (!double.IsFinite(value.Value)) throw new FormatException("RINEX numeric fields must be finite.");
        var text = value.Value.ToString("0.000000000000E+00", CultureInfo.InvariantCulture);
        if (text.Length > 19) throw new FormatException("Navigation value exceeds its fixed width.");
        return (legacy ? text.Replace('E', 'D') : text).PadLeft(19);
    }
    public static RinexEpoch ReadEpoch(string line, int start)
    {
        var fields = Slice(line, start, 19).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length != 6) throw new FormatException("Invalid RINEX epoch.");
        return new RinexEpoch(int.Parse(fields[0], CultureInfo.InvariantCulture), int.Parse(fields[1], CultureInfo.InvariantCulture), int.Parse(fields[2], CultureInfo.InvariantCulture), int.Parse(fields[3], CultureInfo.InvariantCulture), int.Parse(fields[4], CultureInfo.InvariantCulture), decimal.Parse(fields[5], CultureInfo.InvariantCulture));
    }
    public static RinexEpoch ReadLegacyEpoch(string line, int start)
    {
        var fields = Slice(line, start, 19).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length != 6) throw new FormatException("Invalid RINEX 2 epoch.");
        var year = int.Parse(fields[0], CultureInfo.InvariantCulture);
        return new RinexEpoch(year >= 80 ? 1900 + year : 2000 + year, int.Parse(fields[1], CultureInfo.InvariantCulture), int.Parse(fields[2], CultureInfo.InvariantCulture), int.Parse(fields[3], CultureInfo.InvariantCulture), int.Parse(fields[4], CultureInfo.InvariantCulture), decimal.Parse(fields[5], CultureInfo.InvariantCulture));
    }
    public static string WriteEpoch(RinexEpoch epoch)
    {
        if (epoch.Seconds != decimal.Truncate(epoch.Seconds)) throw new FormatException("Navigation and meteorological epochs require integer seconds.");
        return FormattableString.Invariant($"{epoch.Year:0000} {epoch.Month:00} {epoch.Day:00} {epoch.Hour:00} {epoch.Minute:00} {epoch.Seconds:00}");
    }
    public static string WriteLegacyEpoch(RinexEpoch epoch) => FormattableString.Invariant($"{epoch.Year % 100:00} {epoch.Month,2} {epoch.Day,2} {epoch.Hour,2} {epoch.Minute,2}{epoch.Seconds,5:0.0}");
}
