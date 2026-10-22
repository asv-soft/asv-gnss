using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Asv.Gnss;

/// <summary>A GREIS value, including omitted fields, nested or prefixed lists and options.</summary>
public sealed record JavadValue
{
    private JavadValue(
        string? scalar,
        bool quoted,
        ImmutableArray<JavadValue> items,
        string prefix = "",
        string? name = null,
        JavadValue? options = null,
        bool isReceiverText = false
    )
    {
        Scalar = scalar;
        IsQuoted = quoted;
        Items = items;
        Prefix = prefix;
        Name = name;
        Options = options;
        IsReceiverText = isReceiverText;
    }

    public string? Scalar { get; }
    public bool IsQuoted { get; }
    public ImmutableArray<JavadValue> Items { get; }
    public bool IsList => !Items.IsDefault;
    public bool IsOmitted => !IsList && Scalar is null;
    public string Prefix { get; }
    public string? Name { get; }
    public JavadValue? Options { get; }
    public bool IsReceiverText { get; }
    public static JavadValue Omitted { get; } = new(null, false, default);

    public static JavadValue Atom(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        ValidateAscii(value);
        if (
            value.AsSpan().Trim().Length != value.Length
            || value.IndexOfAny(['{', '}', ',', ':', '"', '\\', ';', '#', '%', '@', '=', '|']) >= 0
            || value.Contains("&&", StringComparison.Ordinal)
        )
        {
            throw new ArgumentException(
                "A GREIS atom cannot contain unquoted syntax characters or surrounding whitespace.",
                nameof(value)
            );
        }
        return new(value, false, default);
    }

    public static JavadValue String(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        ValidateAscii(value);
        return new(value, true, default);
    }

    /// <summary>Creates an opaque field of a receiver text message; it cannot be used in commands.</summary>
    public static JavadValue TextAtom(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        ValidateAscii(value);
        return new(value, false, default, isReceiverText: true);
    }

    public static JavadValue Integer(long value) =>
        Atom(value.ToString(CultureInfo.InvariantCulture));

    public static JavadValue UnsignedInteger(ulong value) =>
        Atom(value.ToString(CultureInfo.InvariantCulture));

    public static JavadValue Hexadecimal(ulong value) =>
        Atom("0x" + value.ToString("X", CultureInfo.InvariantCulture));

    public static JavadValue Boolean(bool value) => Atom(value ? "on" : "off");

    public static JavadValue Float(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value));
        return Atom(value.ToString("R", CultureInfo.InvariantCulture));
    }

    public static JavadValue List(params JavadValue[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        foreach (var value in values)
            ArgumentNullException.ThrowIfNull(value);
        return new(null, false, [.. values]);
    }

    public JavadValue WithPrefix(string prefix)
    {
        if (!IsList)
            throw new InvalidOperationException("Only GREIS lists can have a shared prefix.");
        _ = Atom(prefix);
        return new(Scalar, IsQuoted, Items, prefix, Name, Options, IsReceiverText);
    }

    public JavadValue WithName(string name)
    {
        _ = Atom(name);
        return new(Scalar, IsQuoted, Items, Prefix, name, Options, IsReceiverText);
    }

    public JavadValue WithOptions(JavadValue options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new(Scalar, IsQuoted, Items, Prefix, Name, options, IsReceiverText);
    }

    /// <summary>Parses a complete value; incomplete multi-message replies must first be assembled.</summary>
    public static JavadValue Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        ValidateAscii(text);
        var reader = new ValueReader(text);
        var value = reader.Read(0);
        reader.SkipSpaces();
        if (!reader.AtEnd)
            throw new FormatException("Unexpected text after the GREIS value.");
        return value;
    }

    public long AsInteger()
    {
        var text = RequiredScalar();
        var negative = text.StartsWith('-');
        var unsigned = text.AsSpan(text.StartsWith('-') || text.StartsWith('+') ? 1 : 0);
        var radix =
            unsigned.StartsWith("0x", StringComparison.Ordinal) ? 16
            : unsigned.Length > 1 && unsigned[0] == '0' ? 8
            : 10;
        if (radix == 16)
            unsigned = unsigned[2..];
        if (unsigned.IsEmpty)
            throw new FormatException("An integer requires digits.");
        ulong result = 0;
        foreach (var c in unsigned)
        {
            var digit =
                c is >= '0' and <= '9' ? c - '0'
                : c is >= 'a' and <= 'f' ? c - 'a' + 10
                : c is >= 'A' and <= 'F' ? c - 'A' + 10
                : -1;
            if (digit < 0 || digit >= radix)
                throw new FormatException("Invalid GREIS integer digit.");
            result = checked(result * (ulong)radix + (ulong)digit);
        }
        return negative
            ? result == 0x8000000000000000
                ? long.MinValue
                : -checked((long)result)
            : checked((long)result);
    }

    public double AsFloat()
    {
        var value = double.Parse(
            RequiredScalar(),
            NumberStyles.Float,
            CultureInfo.InvariantCulture
        );
        return double.IsFinite(value)
            ? value
            : throw new FormatException("GREIS floating-point values must be finite.");
    }

    public bool AsBoolean() =>
        RequiredScalar() switch
        {
            "y" or "yes" or "on" => true,
            "n" or "no" or "off" => false,
            _ => throw new FormatException("Invalid GREIS boolean value."),
        };

    private string RequiredScalar() =>
        !IsList && Scalar is not null
            ? Scalar
            : throw new InvalidOperationException("The value is not a scalar.");

    public override string ToString()
    {
        var builder = new StringBuilder();
        AppendTo(builder);
        return builder.ToString();
    }

    internal void AppendTo(StringBuilder builder)
    {
        if (Name is not null)
            builder.Append(Name).Append('=');
        if (IsList)
        {
            builder.Append(Prefix).Append('{');
            for (var i = 0; i < Items.Length; i++)
            {
                if (i != 0)
                    builder.Append(',');
                Items[i].AppendTo(builder);
            }
            builder.Append('}');
        }
        else if (IsQuoted)
        {
            builder.Append('"');
            foreach (var c in Scalar!)
            {
                if (c is '"' or '\\')
                    builder.Append('\\');
                builder.Append(c);
            }
            builder.Append('"');
        }
        else
            builder.Append(Scalar);
        if (Options is not null)
        {
            builder.Append(':');
            Options.AppendTo(builder);
        }
    }

    internal static void ValidateAscii(string text)
    {
        foreach (var c in text)
        {
            if (c is < ' ' or > '~')
                throw new ArgumentException(
                    "GREIS text must contain printable ASCII characters only.",
                    nameof(text)
                );
        }
    }

    internal void ValidateForCommand()
    {
        if (IsReceiverText)
            throw new ArgumentException(
                "Opaque receiver text must be explicitly quoted before use in a GREIS command."
            );
        if (IsList)
            foreach (var item in Items)
                item.ValidateForCommand();
        Options?.ValidateForCommand();
    }

    private sealed class ValueReader(string text)
    {
        private int _position;
        public bool AtEnd => _position == text.Length;

        public void SkipSpaces()
        {
            while (!AtEnd && text[_position] == ' ')
                _position++;
        }

        public JavadValue Read(int depth)
        {
            if (depth > 256)
                throw new FormatException("GREIS value nesting exceeds 256 levels.");
            SkipSpaces();
            var begin = _position;
            while (!AtEnd && text[_position] is not ('{' or '}' or ',' or ':' or '"' or '='))
                _position++;
            var prefix = text[begin.._position].Trim();
            string? name = null;
            if (!AtEnd && text[_position] == '=')
            {
                _ = Atom(prefix);
                name = prefix;
                _position++;
                var named = Read(depth + 1);
                return named.WithName(name);
            }
            JavadValue value;
            if (!AtEnd && text[_position] == '{')
            {
                if (prefix.Length != 0)
                    _ = Atom(prefix);
                _position++;
                SkipSpaces();
                var items = new List<JavadValue>();
                if (!AtEnd && text[_position] != '}')
                {
                    while (true)
                    {
                        items.Add(Read(depth + 1));
                        SkipSpaces();
                        if (AtEnd || text[_position] != ',')
                            break;
                        _position++;
                    }
                }
                if (AtEnd || text[_position++] != '}')
                    throw new FormatException("Unclosed GREIS list.");
                value = new(null, false, [.. items], prefix, name);
            }
            else if (!AtEnd && text[_position] == '"')
            {
                if (prefix.Length != 0)
                    throw new FormatException("Unexpected text before a quoted value.");
                _position++;
                var builder = new StringBuilder();
                var closed = false;
                while (!AtEnd)
                {
                    var c = text[_position++];
                    if (c == '"')
                    {
                        closed = true;
                        break;
                    }
                    if (c == '\\')
                    {
                        if (AtEnd || text[_position] is not ('"' or '\\'))
                            throw new FormatException(
                                "Only quotes and backslashes can be escaped in GREIS strings."
                            );
                        c = text[_position++];
                    }
                    builder.Append(c);
                }
                if (!closed)
                    throw new FormatException("Unclosed GREIS quoted value.");
                value = String(builder.ToString());
            }
            else
                value = prefix.Length == 0 ? Omitted : Atom(prefix);
            SkipSpaces();
            if (!AtEnd && text[_position] == ':')
            {
                _position++;
                value = value.WithOptions(Read(depth + 1));
            }
            return value;
        }
    }
}
