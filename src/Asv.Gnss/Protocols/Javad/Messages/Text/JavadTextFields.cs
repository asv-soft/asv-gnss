using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Asv.Gnss;

/// <summary>GREIS receiver text fields. Unlike commands, colons, equals signs and angle quotes are literal.</summary>
public static class JavadTextFields
{
    public static ImmutableArray<JavadValue> Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var reader = new Reader(text);
        var result = reader.ReadFields(false, 0);
        if (!reader.AtEnd)
            throw new FormatException("Unexpected closing brace in GREIS text.");
        return result;
    }

    public static string Format(ImmutableArray<JavadValue> fields)
    {
        if (fields.IsDefault)
            throw new ArgumentException("Fields must be initialized.", nameof(fields));
        return string.Join(
            ",",
            fields.Select(x =>
                x?.ToString()
                ?? throw new ArgumentException("A text field cannot be null.", nameof(fields))
            )
        );
    }

    public static string? Scalar(JavadValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.IsList)
            throw new FormatException("Expected a scalar GREIS text field.");
        return value.Scalar;
    }

    public static int? Integer(JavadValue value)
    {
        var text = Scalar(value);
        return string.IsNullOrEmpty(text)
            ? null
            : int.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    public static double? Number(JavadValue value)
    {
        var text = Scalar(value);
        if (string.IsNullOrEmpty(text))
            return null;
        var result = double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        return double.IsFinite(result)
            ? result
            : throw new FormatException("GREIS text numbers must be finite.");
    }

    public static char? Character(JavadValue value)
    {
        var text = Scalar(value);
        return string.IsNullOrEmpty(text) ? null
            : text.Length == 1 ? text[0]
            : throw new FormatException("Expected one ASCII character.");
    }

    public static ImmutableArray<JavadValue> List(JavadValue value, int minimum = 0)
    {
        if (!value.IsList || value.Items.Length < minimum)
            throw new FormatException("GREIS text list is missing required fields.");
        return value.Items;
    }

    public static JavadValue At(ImmutableArray<JavadValue> fields, int index) =>
        index < fields.Length ? fields[index] : JavadValue.Omitted;

    public static string? Text(ImmutableArray<JavadValue> fields, int index) =>
        Scalar(At(fields, index));

    public static int? Int(ImmutableArray<JavadValue> fields, int index) =>
        Integer(At(fields, index));

    public static double? Double(ImmutableArray<JavadValue> fields, int index) =>
        Number(At(fields, index));

    public static char? Char(ImmutableArray<JavadValue> fields, int index) =>
        Character(At(fields, index));

    /// <summary>Decodes the decimal HHMMSS representation; a leap second can produce a value of one day.</summary>
    public static TimeSpan? Time(JavadValue value)
    {
        var number = Number(value);
        if (number is null)
            return null;
        var hours = (int)(number.Value / 10000);
        var minutes = (int)(number.Value / 100) % 100;
        var seconds = number.Value % 100;
        if (hours is < 0 or > 23 || minutes is < 0 or > 59 || seconds is < 0 or >= 61)
            throw new FormatException("Invalid GREIS UTC time.");
        return TimeSpan.FromHours(hours)
            + TimeSpan.FromMinutes(minutes)
            + TimeSpan.FromSeconds(seconds);
    }

    public static double? Degrees(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return null;
        if (text[0] is not ('N' or 'S' or 'E' or 'W'))
            throw new FormatException("An angle must start with its hemisphere.");
        var degreeEnd = text.IndexOfAny(['o', 'd']);
        var minuteEnd = text.IndexOfAny(['\'', 'm']);
        if (degreeEnd <= 1 || minuteEnd <= degreeEnd || text[^1] is not ('"' or 's'))
            throw new FormatException("Invalid GREIS degrees/minutes/seconds angle.");
        var degrees = double.Parse(text.AsSpan(1, degreeEnd - 1), CultureInfo.InvariantCulture);
        var minutes = double.Parse(
            text.AsSpan(degreeEnd + 1, minuteEnd - degreeEnd - 1),
            CultureInfo.InvariantCulture
        );
        var seconds = double.Parse(
            text.AsSpan(minuteEnd + 1, text.Length - minuteEnd - 2),
            CultureInfo.InvariantCulture
        );
        if (
            !double.IsFinite(degrees)
            || !double.IsFinite(minutes)
            || !double.IsFinite(seconds)
            || degrees < 0
            || degrees > (text[0] is 'N' or 'S' ? 90 : 180)
            || minutes is < 0 or >= 60
            || seconds is < 0 or >= 60
        )
            throw new FormatException("Invalid GREIS angle components.");
        var result = degrees + minutes / 60 + seconds / 3600;
        if (result > (text[0] is 'N' or 'S' ? 90 : 180))
            throw new FormatException("A GREIS angle exceeds its hemisphere range.");
        return text[0] is 'S' or 'W' ? -result : result;
    }

    private sealed class Reader(string text)
    {
        private int _position;
        public bool AtEnd => _position == text.Length;

        private void SkipSpaces()
        {
            while (!AtEnd && text[_position] == ' ')
                _position++;
        }

        public ImmutableArray<JavadValue> ReadFields(bool nested, int depth)
        {
            SkipSpaces();
            if (depth > 256)
                throw new FormatException("GREIS text nesting exceeds 256 levels.");
            var values = ImmutableArray.CreateBuilder<JavadValue>();
            if (AtEnd || nested && text[_position] == '}')
                return values.ToImmutable();
            while (true)
            {
                values.Add(ReadValue(depth));
                SkipSpaces();
                if (AtEnd || text[_position] == '}')
                    return values.ToImmutable();
                if (text[_position++] != ',')
                    throw new FormatException("Missing GREIS text field separator.");
                SkipSpaces();
                if (AtEnd || text[_position] == '}')
                {
                    values.Add(JavadValue.Omitted);
                    return values.ToImmutable();
                }
            }
        }

        private JavadValue ReadValue(int depth)
        {
            SkipSpaces();
            if (AtEnd || text[_position] is ',' or '}')
                return JavadValue.Omitted;
            if (text[_position] == '{')
            {
                _position++;
                var items = ReadFields(true, depth + 1);
                if (AtEnd || text[_position++] != '}')
                    throw new FormatException("Unclosed GREIS text list.");
                return JavadValue.List(items.ToArray());
            }
            if (text[_position] == '"')
            {
                _position++;
                var builder = new StringBuilder();
                while (!AtEnd)
                {
                    var c = text[_position++];
                    if (c == '"')
                        return JavadValue.String(builder.ToString());
                    if (c == '\\' && !AtEnd && text[_position] is '"' or '\\')
                        c = text[_position++];
                    builder.Append(c);
                }
                throw new FormatException("Unclosed GREIS text string.");
            }
            var begin = _position;
            while (!AtEnd && text[_position] is not (',' or '}' or '{'))
                _position++;
            if (!AtEnd && text[_position] == '{')
                throw new FormatException("Unexpected opening brace in a GREIS text scalar.");
            return _position == begin
                ? JavadValue.Omitted
                : JavadValue.TextAtom(text[begin.._position].Trim());
        }
    }
}
