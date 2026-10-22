using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Visits an immutable GREIS value, replacing only edited nodes. Metadata describes the exact
/// finite tree and is weakly cached for each value. Use the value factories to change tree shape.
/// </summary>
public static class JavadValueVisitor
{
    private static readonly ConditionalWeakTable<JavadValue, Metadata> Cache = new();
    private static readonly StructType EmptyType = new([]);
    private static readonly StringOptionalType OptionalText = new(
        EncodingId.Ascii,
        0,
        int.MaxValue
    );
    private static readonly StringType Text = new(EncodingId.Ascii, 0, int.MaxValue);
    private static readonly Field IsListField = JavadVisitor.ReadOnlyField(
        nameof(JavadValue.IsList),
        BoolType.Default,
        "Whether the value is a list."
    );
    private static readonly Field NameField = JavadVisitor.Field(
        nameof(JavadValue.Name),
        OptionalText,
        "Optional GREIS field name."
    );
    private static readonly Field ScalarField = JavadVisitor.Field(
        nameof(JavadValue.Scalar),
        OptionalText,
        "Scalar text; null represents an omitted value."
    );
    private static readonly Field IsQuotedField = JavadVisitor.Field(
        nameof(JavadValue.IsQuoted),
        BoolType.Default,
        "Whether scalar text is quoted."
    );
    private static readonly Field PrefixField = JavadVisitor.Field(
        nameof(JavadValue.Prefix),
        Text,
        "Shared prefix of a GREIS list."
    );
    private static readonly Field IsReceiverTextField = JavadVisitor.ReadOnlyField(
        nameof(JavadValue.IsReceiverText),
        BoolType.Default,
        "Whether an unquoted scalar belongs to the receiver text grammar."
    );

    /// <summary>Returns metadata for this value's current immutable shape.</summary>
    public static StructType GetType(JavadValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return GetMetadata(value).Type;
    }

    /// <summary>Visits a structured field and publishes a validated replacement after all edits succeed.</summary>
    public static void Visit(IVisitor visitor, Field field, ref JavadValue value)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(value);
        var type = GetType(value);
        if (!ReferenceEquals(field.DataType, type))
            throw new ArgumentException(
                "The field must use this value's current visitor metadata.",
                nameof(field)
            );
        if (visitor is not StructType.IVisitor nested)
        {
            visitor.VisitUnknown(field, type);
            return;
        }
        nested.BeginStruct(field, type);
        var edited = value;
        VisitFields(visitor, ref edited);
        nested.EndStruct();
        value = edited;
    }

    /// <summary>Visits the fields without an enclosing structure callback. A no-op retains identity.</summary>
    public static void VisitFields(IVisitor visitor, ref JavadValue value)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        ArgumentNullException.ThrowIfNull(value);
        var metadata = GetMetadata(value);
        var isList = value.IsList;
        BoolType.Accept(visitor, IsListField, ref isList);
        if (isList != value.IsList)
            throw new InvalidOperationException(
                "The GREIS value shape is read-only; use the value factories to replace it."
            );
        var name = value.Name;
        StringOptionalType.Accept(visitor, NameField, ref name);
        var scalar = value.Scalar;
        var quoted = value.IsQuoted;
        var prefix = value.Prefix;
        JavadValue[]? items = null;
        if (value.IsList)
        {
            StringType.Accept(visitor, PrefixField, ref prefix);
            if (visitor is StructType.IVisitor nested)
            {
                nested.BeginStruct(metadata.Items!, metadata.ItemsType!);
                for (var index = 0; index < value.Items.Length; index++)
                {
                    var item = value.Items[index];
                    Visit(visitor, metadata.ItemsType![index], ref item);
                    if (ReferenceEquals(item, value.Items[index]))
                        continue;
                    items ??= value.Items.ToArray();
                    items[index] = item;
                }
                nested.EndStruct();
            }
            else
                visitor.VisitUnknown(metadata.Items!, metadata.ItemsType!);
        }
        else
        {
            StringOptionalType.Accept(visitor, ScalarField, ref scalar);
            BoolType.Accept(visitor, IsQuotedField, ref quoted);
        }
        var receiverText = value.IsReceiverText;
        BoolType.Accept(visitor, IsReceiverTextField, ref receiverText);
        if (receiverText != value.IsReceiverText)
            throw new InvalidOperationException(
                "The GREIS scalar grammar is read-only; use Atom or TextAtom to replace it."
            );
        var options = value.Options;
        var optionsType = (OptionalStructType)metadata.Options.DataType;
        if (visitor is OptionalStructType.IVisitor optional)
        {
            optional.BeginOptionalStruct(
                metadata.Options,
                optionsType,
                options is not null,
                out var createNew
            );
            if (createNew)
                throw new NotSupportedException(
                    "Use WithOptions to create or replace an option tree before visiting it."
                );
            if (options is not null)
                VisitFields(visitor, ref options);
            optional.EndOptionalStruct(options is not null);
        }
        else
            visitor.VisitUnknown(metadata.Options, optionsType);
        if (
            name == value.Name
            && scalar == value.Scalar
            && quoted == value.IsQuoted
            && prefix == value.Prefix
            && items is null
            && ReferenceEquals(options, value.Options)
        )
            return;
        if (!value.IsList && scalar is null && quoted)
            throw new InvalidOperationException("An omitted GREIS scalar cannot be quoted.");
        ArgumentNullException.ThrowIfNull(prefix);
        var replacement =
            value.IsList ? JavadValue.List(items ?? value.Items.ToArray())
            : scalar is null ? JavadValue.Omitted
            : quoted ? JavadValue.String(scalar)
            : value.IsReceiverText ? JavadValue.TextAtom(scalar)
            : JavadValue.Atom(scalar);
        if (value.IsList && prefix.Length != 0)
            replacement = replacement.WithPrefix(prefix);
        if (name is not null)
            replacement = replacement.WithName(name);
        if (options is not null)
            replacement = replacement.WithOptions(options);
        value = replacement;
    }

    private static Metadata GetMetadata(JavadValue value) =>
        Cache.GetValue(value, static node => new Metadata(node));

    private sealed class Metadata
    {
        public Metadata(JavadValue value)
        {
            var fields = ImmutableArray.CreateBuilder<Field>();
            fields.Add(IsListField);
            fields.Add(NameField);
            if (value.IsList)
            {
                fields.Add(PrefixField);
                var children = ImmutableArray.CreateBuilder<Field>(value.Items.Length);
                for (var index = 0; index < value.Items.Length; index++)
                    children.Add(
                        JavadVisitor.Field(
                            index.ToString(CultureInfo.InvariantCulture),
                            JavadValueVisitor.GetType(value.Items[index]),
                            "GREIS list element."
                        )
                    );
                ItemsType = new StructType(children.MoveToImmutable());
                Items = JavadVisitor.Field(
                    nameof(JavadValue.Items),
                    ItemsType,
                    "Ordered heterogeneous GREIS list elements."
                );
                fields.Add(Items);
            }
            else
            {
                fields.Add(ScalarField);
                fields.Add(IsQuotedField);
            }
            fields.Add(IsReceiverTextField);
            Options = JavadVisitor.Field(
                nameof(JavadValue.Options),
                new OptionalStructType(
                    value.Options is null ? EmptyType : JavadValueVisitor.GetType(value.Options)
                ),
                "Optional GREIS options; use WithOptions to change their shape."
            );
            fields.Add(Options);
            Type = new StructType(fields.ToImmutable());
        }

        public StructType Type { get; }
        public Field? Items { get; }
        public StructType? ItemsType { get; }
        public Field Options { get; }
    }
}
