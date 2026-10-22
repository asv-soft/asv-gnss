using System;
using System.Collections.Immutable;
using System.Globalization;
using Asv.IO;

namespace Asv.Gnss;

internal readonly record struct JavadTextVisitField(int Index, Field Field);

/// <summary>Visits typed text values without normalizing unchanged receiver spelling.</summary>
internal static class JavadTextVisitor
{
    public static readonly StringOptionalType TextType = new(EncodingId.Ascii, 0, 4095);
    public static readonly CharOptionalType CharacterType = new(EncodingId.Ascii);

    public static void VisitFields(
        IVisitor visitor,
        ImmutableArray<JavadTextVisitField> schema,
        ref ImmutableArray<JavadValue> fields
    )
    {
        foreach (var item in schema)
        {
            var original = JavadTextFields.At(fields, item.Index);
            var value = original;
            VisitValue(visitor, item.Field, item.Field.DataType, ref value);
            if (!ReferenceEquals(original, value))
                Set(ref fields, item.Index, value);
        }
    }

    public static void VisitRecord(IVisitor visitor, StructType type, ref JavadValue value)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var fields = value.IsOmitted
            ? ImmutableArray<JavadValue>.Empty
            : JavadTextFields.List(value);
        var original = fields;
        for (var index = 0; index < type.Fields.Length; index++)
        {
            var before = JavadTextFields.At(fields, index);
            var item = before;
            var field = type.Fields[index];
            VisitValue(visitor, field, field.DataType, ref item);
            if (!ReferenceEquals(before, item))
                Set(ref fields, index, item);
        }
        if (fields != original)
            value = WithItems(value, fields);
    }

    public static void VisitRepeated(
        IVisitor visitor,
        Field countField,
        Field listField,
        int countIndex,
        int firstIndex,
        ref ImmutableArray<JavadValue> fields
    )
    {
        var originalCount =
            JavadTextFields.Int(fields, countIndex)
            ?? throw new FormatException("A repeated text group requires its count.");
        if (originalCount < 0 || originalCount > fields.Length - firstIndex)
            throw new FormatException("The text group count exceeds available fields.");
        var count = originalCount;
        Int32Type.Accept(visitor, countField, countField.DataType, ref count);
        var type = (ListType)listField.DataType;
        if (count < type.MinSize || count > type.MaxSize)
            throw new ArgumentOutOfRangeException(nameof(count));
        var size = (uint)count;
        var listVisitor = visitor as ListType.IVisitor;
        if (listVisitor is null)
            visitor.VisitUnknown(listField, type);
        else
            listVisitor.BeginList(listField, type, ref size);
        if (size < type.MinSize || size > type.MaxSize)
            throw new ArgumentOutOfRangeException(nameof(size));
        ImmutableArray<JavadValue>.Builder? modified = null;
        if (size != originalCount)
        {
            modified = fields.ToBuilder();
            if (size < originalCount)
                modified.RemoveRange(firstIndex + (int)size, originalCount - (int)size);
            else
                for (var index = originalCount; index < size; index++)
                    modified.Insert(firstIndex + index, DefaultValue(type.ElementType));
            modified[countIndex] = JavadValue.Integer(size);
        }
        if (listVisitor is not null)
        {
            for (var index = 0; index < size; index++)
            {
                var offset = firstIndex + index;
                var original = modified is null ? fields[offset] : modified[offset];
                var value = original;
                VisitValue(visitor, listField, type.ElementType, ref value);
                if (!ReferenceEquals(original, value))
                {
                    modified ??= fields.ToBuilder();
                    modified[offset] = value;
                }
            }
            listVisitor.EndList();
        }
        if (modified is not null)
            fields = modified.ToImmutable();
    }

    public static void VisitValue(
        IVisitor visitor,
        Field field,
        IFieldType type,
        ref JavadValue value
    )
    {
        switch (type)
        {
            case Int32OptionalType:
                var integer = JavadTextFields.Integer(value);
                var originalInteger = integer;
                Int32OptionalType.Accept(visitor, field, type, ref integer);
                if (integer != originalInteger)
                    value = integer is null
                        ? JavadValue.Omitted
                        : JavadValue.Integer(integer.Value);
                return;
            case DoubleOptionalType:
                var number = JavadTextFields.Number(value);
                var originalNumber = number;
                DoubleOptionalType.Accept(visitor, field, type, ref number);
                if (!Nullable.Equals(number, originalNumber))
                    value = number is null ? JavadValue.Omitted : JavadValue.Float(number.Value);
                return;
            case StringOptionalType:
                var text = JavadTextFields.Scalar(value);
                var originalText = text;
                StringOptionalType.Accept(visitor, field, type, ref text);
                if (!string.Equals(text, originalText, StringComparison.Ordinal))
                    value = TextValue(text);
                return;
            case CharOptionalType:
                var character = JavadTextFields.Character(value);
                var originalCharacter = character;
                CharOptionalType.Accept(visitor, field, type, ref character);
                if (character != originalCharacter)
                    value = character is null
                        ? JavadValue.Omitted
                        : TextValue(character.Value.ToString());
                return;
            case TimeSpanOptionalType:
                var time = JavadTextFields.Time(value);
                var originalTime = time;
                TimeSpanOptionalType.Accept(visitor, field, type, ref time);
                if (time != originalTime)
                    value = TimeValue(time);
                return;
            case ListType listType:
                VisitList(visitor, field, listType, ref value);
                return;
            case StructType structType:
                if (visitor is StructType.IVisitor structVisitor)
                {
                    structVisitor.BeginStruct(field, structType);
                    VisitRecord(visitor, structType, ref value);
                    structVisitor.EndStruct();
                }
                else
                    visitor.VisitUnknown(field, type);
                return;
            case OptionalStructType optionalType:
                if (visitor is OptionalStructType.IVisitor optionalVisitor)
                {
                    optionalVisitor.BeginOptionalStruct(
                        field,
                        optionalType,
                        !value.IsOmitted,
                        out var create
                    );
                    if (create)
                        value = DefaultValue(optionalType.BaseType);
                    if (!value.IsOmitted)
                        VisitRecord(visitor, optionalType.BaseType, ref value);
                    optionalVisitor.EndOptionalStruct(!value.IsOmitted);
                }
                else
                    visitor.VisitUnknown(field, type);
                return;
            default:
                visitor.VisitUnknown(field, type);
                return;
        }
    }

    private static void VisitList(
        IVisitor visitor,
        Field field,
        ListType type,
        ref JavadValue value
    )
    {
        if (visitor is not ListType.IVisitor listVisitor)
        {
            visitor.VisitUnknown(field, type);
            return;
        }
        var fields = value.IsOmitted
            ? ImmutableArray<JavadValue>.Empty
            : JavadTextFields.List(value);
        var original = fields;
        var size = (uint)fields.Length;
        listVisitor.BeginList(field, type, ref size);
        if (size < type.MinSize || size > type.MaxSize)
            throw new ArgumentOutOfRangeException(nameof(size));
        if (size != fields.Length)
        {
            var resized = fields.ToBuilder();
            while (resized.Count < size)
                resized.Add(DefaultValue(type.ElementType));
            while (resized.Count > size)
                resized.RemoveAt(resized.Count - 1);
            fields = resized.ToImmutable();
        }
        for (var index = 0; index < fields.Length; index++)
        {
            var item = fields[index];
            VisitValue(visitor, field, type.ElementType, ref item);
            if (!ReferenceEquals(item, fields[index]))
                Set(ref fields, index, item);
        }
        listVisitor.EndList();
        if (fields != original)
            value = WithItems(value, fields);
    }

    private static JavadValue DefaultValue(IFieldType type) =>
        type switch
        {
            StructType structure => JavadValue.List([
                .. System.Linq.Enumerable.Select(structure.Fields, x => DefaultValue(x.DataType)),
            ]),
            ListType => JavadValue.List(),
            _ => JavadValue.Omitted,
        };

    public static void Set(ref ImmutableArray<JavadValue> fields, int index, JavadValue value)
    {
        var builder = fields.ToBuilder();
        while (builder.Count <= index)
            builder.Add(JavadValue.Omitted);
        builder[index] = value;
        fields = builder.ToImmutable();
    }

    private static JavadValue WithItems(JavadValue original, ImmutableArray<JavadValue> fields)
    {
        var result = JavadValue.List([.. fields]);
        if (original.Prefix.Length != 0)
            result = result.WithPrefix(original.Prefix);
        if (original.Name is { } name)
            result = result.WithName(name);
        if (original.Options is { } options)
            result = result.WithOptions(options);
        return result;
    }

    private static JavadValue TextValue(string? value) =>
        value is null ? JavadValue.Omitted
        : value.Length == 0
        || value.IndexOfAny([',', '{', '}']) >= 0
        || value.StartsWith('"')
        || value.AsSpan().Trim().Length != value.Length
            ? JavadValue.String(value)
        : JavadValue.TextAtom(value);

    private static JavadValue TimeValue(TimeSpan? value)
    {
        if (value is null)
            return JavadValue.Omitted;
        if (value < TimeSpan.Zero || value >= TimeSpan.FromDays(1))
            throw new ArgumentOutOfRangeException(nameof(value));
        var time = value.Value;
        return JavadValue.TextAtom(
            (
                time.Hours * 10000 + time.Minutes * 100 + time.Seconds + time.Milliseconds / 1000d
            ).ToString("000000.###", CultureInfo.InvariantCulture)
        );
    }
}
