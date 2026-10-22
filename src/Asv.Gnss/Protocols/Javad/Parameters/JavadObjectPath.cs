using System;

namespace Asv.Gnss;

/// <summary>A case-sensitive GREIS object identifier; relative names retain their command-specific meaning.</summary>
public readonly record struct JavadObjectPath
{
    public JavadObjectPath(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        _ = JavadValue.Atom(value);
        if (value.Contains(' '))
            throw new ArgumentException(
                "Object identifiers cannot contain whitespace.",
                nameof(value)
            );
        Value = value;
    }

    public string Value { get; }
    public bool IsAbsolute => Value?.StartsWith('/') == true;

    public JavadObjectPath Child(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        if (name.Contains('/') || name.Contains('&'))
            throw new ArgumentException("A child must be a single object name.", nameof(name));
        return new(RequiredValue().TrimEnd('/') + "/" + name);
    }

    public JavadObjectPath Attribute(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        if (name.Contains('/') || name.Contains('&') || RequiredValue().Contains('&'))
            throw new ArgumentException("Invalid object attribute.", nameof(name));
        return new(Value + "&" + name);
    }

    public JavadValue ToValue() => JavadValue.Atom(RequiredValue());

    public override string ToString() => RequiredValue();

    private string RequiredValue() =>
        Value
        ?? throw new InvalidOperationException("The GREIS object path has not been initialized.");
}
