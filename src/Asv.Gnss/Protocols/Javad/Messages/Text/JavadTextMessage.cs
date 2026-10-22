using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Asv.Gnss;

/// <summary>A native GREIS text message with typed field access and lossless unknown fields.</summary>
public abstract class JavadTextMessage : JavadMessageBase
{
    private readonly string _expectedTitle;
    private string _title;
    private ImmutableArray<JavadValue> _fields;
    private string? _receivedPayload;

    protected JavadTextMessage(string title, string defaultFields)
    {
        _expectedTitle = _title = title;
        _fields = JavadTextFields.Parse(defaultFields);
    }

    public override JavadChecksumKind ChecksumKind => JavadChecksumKind.AsciiHex8;
    public string Title => _title;

    /// <summary>Fields following the title, including future additions and omitted values.</summary>
    public ImmutableArray<JavadValue> Fields
    {
        get => _fields;
        set
        {
            if (value.IsDefault || value.Any(x => x is null))
                throw new ArgumentException(
                    "Fields must be initialized and cannot contain null.",
                    nameof(value)
                );
            _fields = value;
            _receivedPayload = null;
        }
    }

    protected virtual bool IsValidTitle(string title) => title == _expectedTitle;

    protected void SetTitle(string title)
    {
        if (string.IsNullOrEmpty(title) || title.IndexOfAny([',', '{', '}', '@']) >= 0)
            throw new ArgumentException("Invalid GREIS text title.", nameof(title));
        _ = JavadValue.TextAtom(title);
        _title = title;
        _receivedPayload = null;
    }

    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        foreach (var value in buffer)
            if (value is < 32 or > 126)
                throw new FormatException("GREIS text messages contain printable ASCII.");
        var payload = Encoding.ASCII.GetString(buffer);
        if (!payload.StartsWith(',') || !payload.EndsWith(",@", StringComparison.Ordinal))
            throw new FormatException(
                "A GREIS text message requires its leading comma and final ,@ marker."
            );
        var values = JavadTextFields.Parse(payload[1..^2]);
        if (
            values.Length == 0
            || values[0].IsList
            || values[0].Scalar is not { Length: > 0 } title
            || !IsValidTitle(title)
        )
            throw new FormatException("Unexpected GREIS text message title.");
        _title = title;
        _fields = values.RemoveAt(0);
        ValidateFields();
        _receivedPayload = payload;
        buffer = buffer[buffer.Length..];
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        var payload = GetPayload();
        Encoding.ASCII.GetBytes(payload, buffer);
        buffer = buffer[payload.Length..];
    }

    protected override int GetPayloadByteSize() => GetPayload().Length;

    private string GetPayload()
    {
        ValidateFields();
        return _receivedPayload
            ?? ","
                + _title
                + (_fields.Length == 0 ? "" : "," + JavadTextFields.Format(_fields))
                + ",@";
    }

    protected virtual void ValidateFields() { }

    protected void Require(int count)
    {
        if (_fields.Length < count)
            throw new FormatException($"{Id} requires at least {count} fields after its title.");
    }

    protected void Validate(string types)
    {
        Require(types.Length);
        for (var i = 0; i < types.Length; i++)
        {
            switch (types[i])
            {
                case 'i':
                    _ = Integer(i);
                    break;
                case 't':
                    _ = Time(i);
                    break;
                case 'f':
                    _ = Number(i);
                    break;
                case 'c':
                    _ = Character(i);
                    break;
                case 's':
                    _ = Text(i);
                    break;
                case 'l':
                    _ = JavadTextFields.List(_fields[i]);
                    break;
            }
        }
    }

    protected JavadValue Field(int index) => JavadTextFields.At(_fields, index);

    protected string? Text(int index) => JavadTextFields.Scalar(Field(index));

    protected int? Integer(int index) => JavadTextFields.Integer(Field(index));

    protected double? Number(int index) => JavadTextFields.Number(Field(index));

    protected char? Character(int index) => JavadTextFields.Character(Field(index));

    protected TimeSpan? Time(int index) => JavadTextFields.Time(Field(index));

    protected int Count(int index)
    {
        var value = Integer(index);
        return value is >= 0
            ? value.Value
            : throw new FormatException(
                "A repeated GREIS text field requires a nonnegative count."
            );
    }

    protected void Set(int index, JavadValue value)
    {
        var values = _fields.ToBuilder();
        while (values.Count <= index)
            values.Add(JavadValue.Omitted);
        values[index] = value;
        Fields = values.ToImmutable();
    }

    protected void Set(int index, string? value) =>
        Set(
            index,
            value is null ? JavadValue.Omitted
                : value.IndexOfAny([',', '{', '}']) >= 0
                || value.StartsWith('"')
                || value.AsSpan().Trim().Length != value.Length
                    ? JavadValue.String(value)
                : JavadValue.TextAtom(value)
        );

    protected void Set(int index, double? value) =>
        Set(index, value is null ? JavadValue.Omitted : JavadValue.Float(value.Value));

    protected void Set(int index, int? value) =>
        Set(index, value is null ? JavadValue.Omitted : JavadValue.Integer(value.Value));

    protected void Set(int index, char? value) =>
        Set(
            index,
            value is null ? JavadValue.Omitted : JavadValue.TextAtom(value.Value.ToString())
        );

    protected void SetTime(int index, TimeSpan? value)
    {
        if (value is null)
        {
            Set(index, JavadValue.Omitted);
            return;
        }
        if (value < TimeSpan.Zero || value >= TimeSpan.FromDays(1))
            throw new ArgumentOutOfRangeException(nameof(value));
        Set(
            index,
            JavadValue.TextAtom(
                (
                    value.Value.Hours * 10000
                    + value.Value.Minutes * 100
                    + value.Value.Seconds
                    + value.Value.Milliseconds / 1000d
                ).ToString("000000.###", CultureInfo.InvariantCulture)
            )
        );
    }

    protected ImmutableArray<JavadValue> Repeated(int countIndex, int firstIndex, int minimumFields)
    {
        var count = Count(countIndex);
        if (firstIndex > _fields.Length || count > _fields.Length - firstIndex)
            throw new FormatException("GREIS repeated field count exceeds the available fields.");
        var result = _fields.Slice(firstIndex, count);
        foreach (var value in result)
            _ = JavadTextFields.List(value, minimumFields);
        return result;
    }

    protected void SetRepeated(int countIndex, int firstIndex, ImmutableArray<JavadValue> values)
    {
        if (values.IsDefault)
            throw new ArgumentException("Repeated fields must be initialized.", nameof(values));
        var oldCount = Count(countIndex);
        var builder = _fields.ToBuilder();
        while (builder.Count < firstIndex)
            builder.Add(JavadValue.Omitted);
        builder.RemoveRange(firstIndex, Math.Min(oldCount, builder.Count - firstIndex));
        builder.InsertRange(firstIndex, values);
        builder[countIndex] = JavadValue.Integer(values.Length);
        Fields = builder.ToImmutable();
    }
}
